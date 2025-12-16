Imports System.ComponentModel
Imports System.IO
Imports Newtonsoft.Json
Imports Opc.Ua
Imports Opc.Ua.Client
Imports Opc.Ua.Configuration
Imports System.Threading.Tasks

Namespace upcuaclient_vbnet
    Public Class BackgroundWorkerManager
        Private Shared _instance As BackgroundWorkerManager
        Private Shared ReadOnly _lock As New Object()

        Private WithEvents worker As New BackgroundWorker With {
        .WorkerReportsProgress = False,
        .WorkerSupportsCancellation = True
    }

        ' Removed config - using My.Settings directly
        Private lastHealthCheck As DateTime = DateTime.MinValue
        Private healthCheckInterval As TimeSpan = TimeSpan.FromMinutes(2)

        Public Shared ReadOnly Property Instance As BackgroundWorkerManager
            Get
                SyncLock _lock
                    If _instance Is Nothing Then
                        _instance = New BackgroundWorkerManager()
                    End If
                    Return _instance
                End SyncLock
            End Get
        End Property

        Public Sub Start()
            ' Check if worker is already running
            If worker.IsBusy Then
                Return
            End If

            ' Initialize settings defaults
            SettingsManager.InitializeDefaults()
            worker.RunWorkerAsync()
        End Sub

        Private Async Sub ProcessSensorData()
            Try
                ' 1. Get active recording batches
                Dim sqlite As New SQLiteManager()
                Dim activeBatches = sqlite.QueryBatchRange(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow).Where(Function(b) b.Status.ToLower() = "recording").ToList()

                If activeBatches.Count = 0 Then
                    Return
                End If

                ' 2. Check auto end recording for active batches
                CheckAutoEndRecording(activeBatches)

                ' 3. Get selected root objects dari selectedNodeIdOpc
                Dim selectedRootObjects = SettingsManager.GetSelectedNodeIdOpc()
                If selectedRootObjects.Count = 0 Then
                    Return
                End If

                ' 4. Initialize processors
                Dim dataProcessor As New InterfaceData()
                Dim analytics2 As New AnalyticsManager2()

                ' 5. Process each active batch - ensure both sensors are saved
                For Each batch In activeBatches
                    Try

                        ' Read all sensor data from OPC - use NodeId as key
                        Dim allSensorData As New Dictionary(Of String, Double)
                        For Each rootObj In selectedRootObjects
                            Dim rootSensorData = Await ReadChildSensors(rootObj("NodeId").ToString())
                            For Each kvp In rootSensorData
                                allSensorData(kvp.Key) = kvp.Value
                            Next
                        Next

                        ' Get sensor info from selectedNodeSensor
                        Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

                        ' Process PressureTire sensor - use NodeId directly
                        If allSensorData.ContainsKey(batch.PressureTireId) Then
                            Dim tirePressure = allSensorData(batch.PressureTireId)
                            Dim TireNodeText = GetNodeTextFromId(selectedNodeSensor, "PressureTire", batch.PressureTireId)
                            Dim tireSensorData = analytics2.MapSensorData(batch.PressureTireId, "pressureTire", tirePressure, "Good", TireNodeText)
                            dataProcessor.ProcessSensorData(tireSensorData)
                        End If

                        ' Process PressureGauge sensor - use NodeId directly
                        If allSensorData.ContainsKey(batch.PressureGaugeId) Then
                            Dim gaugePressure = allSensorData(batch.PressureGaugeId)

                            ' Get NodeText for gauge sensor
                            Dim gaugeNodeText = GetNodeTextFromId(selectedNodeSensor, "PressureGauge", batch.PressureGaugeId)

                            Dim gaugeSensorData = analytics2.MapSensorData(batch.PressureGaugeId, "pressureGauge", gaugePressure, "Good", gaugeNodeText)
                            dataProcessor.ProcessSensorData(gaugeSensorData)

                            ' Generate alert for PressureGauge if value > threshold
                            Dim alert = analytics2.GenerateAlert(batch.PressureGaugeId, gaugePressure, gaugeNodeText)
                            If alert IsNot Nothing Then
                                dataProcessor.ProcessAlert(alert)
                            End If
                        End If

                    Catch batchEx As Exception
                        ' Log critical errors only
                        LoggerDebug.LogError($"Error processing batch {batch.BatchId}: {batchEx.Message}")
                    End Try
                Next

            Catch ex As Exception
                LoggerDebug.LogError($"Process sensor data error: {ex.Message}")
            End Try
        End Sub

        Private Function GetNodeTextFromId(selectedNodeSensor As Dictionary(Of String, List(Of Dictionary(Of String, String))), sensorType As String, nodeId As String) As String
            If selectedNodeSensor.ContainsKey(sensorType) Then
                For Each sensor In selectedNodeSensor(sensorType)
                    If sensor("NodeId") = nodeId Then
                        Return sensor("NodeText")
                    End If
                Next
            End If
            Return String.Empty
        End Function

        Private Sub CheckAutoEndRecording(activeBatches As List(Of InterfaceRecordMetadata))
            Try
                ' Reload settings to detect manual file changes
                SettingsManager.ReloadSettings()

                ' Debug: Show current time and endRecording settings
                Dim currentTimeUtc = DateTime.UtcNow
                Dim endRecordingJson = My.Settings.endRecording

                ' Get expired end recordings from settings
                Dim expiredRecordings = SettingsManager.GetExpiredEndRecordings()

                If expiredRecordings.Count > 0 Then
                    For Each expiredRecording In expiredRecordings

                        ' Find matching active batch
                        Dim matchingBatch = activeBatches.FirstOrDefault(Function(b) b.PressureTireId = expiredRecording("pressureTire"))
                        If matchingBatch IsNot Nothing Then
                            ProcessAutoEndRecording(matchingBatch, expiredRecording)
                        End If
                    Next
                End If

            Catch ex As Exception
                LoggerDebug.LogError($"CheckAutoEndRecording Error: {ex.Message}")
            End Try
        End Sub

        Private Sub ProcessAutoEndRecording(batch As InterfaceRecordMetadata, expiredRecording As Dictionary(Of String, String))
            Try

                ' 1. Remove expired recording from endRecording array
                SettingsManager.RemoveEndRecording(batch.PressureTireId)

                ' 2. Update sensor status to ready
                Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

                ' Update PressureTire status
                If selectedNodeSensor.ContainsKey("PressureTire") Then
                    For Each tireSensor In selectedNodeSensor("PressureTire")
                        If tireSensor("NodeId") = batch.PressureTireId Then
                            tireSensor("NodeStatus") = "ready"
                        End If
                    Next
                End If

                ' Update PressureGauge status
                If selectedNodeSensor.ContainsKey("PressureGauge") Then
                    For Each gaugeSensor In selectedNodeSensor("PressureGauge")
                        If gaugeSensor("NodeId") = batch.PressureGaugeId Then
                            gaugeSensor("NodeStatus") = "ready"
                            Exit For
                        End If
                    Next
                End If

                SettingsManager.SetSelectedNodeSensor(selectedNodeSensor)

                ' 3. Call end recording function
                EndRecordingForBatch(batch)

            Catch ex As Exception
                LoggerDebug.LogError($"ProcessAutoEndRecording Error: {ex.Message}")
            End Try
        End Sub

        Private Sub EndRecordingForBatch(batch As InterfaceRecordMetadata)
            Try
                ' Update batch status to Finished
                batch.Status = "Finished"
                batch.SyncStatus = "Finished"
                batch.EndDate = DateTime.UtcNow

                Dim sqlite As New SQLiteManager()
                sqlite.InsertOrUpdateRecordMetadata(batch)

                ' Export to SQL Server if connected
                If My.Settings.stateConnectionDB Then
                    Try
                        Dim sqlServerManager As New SQLServerManager()
                        sqlServerManager.ExportRecordData(batch.BatchId)
                    Catch
                        ' Ignore SQL Server export errors
                    End Try
                End If

            Catch ex As Exception
                LoggerDebug.LogError($"EndRecordingForBatch Error: {ex.Message}")
            End Try
        End Sub

        Private Async Function CheckHealthConnections() As Task
            Try

                ' 1. Check OPC Connection
                Dim opcHealthy As Boolean = False
                Try
                    ' Check if session is still valid first
                    If OpcConnection.Session Is Nothing Then
                        opcHealthy = Await OpcConnection.InitializeAsync()
                    ElseIf Not OpcConnection.Session.Connected Then
                        opcHealthy = Await OpcConnection.InitializeAsync()
                    Else
                        opcHealthy = Await OpcConnection.CheckHealthServer()

                        ' If health check failed but session exists, try to reconnect
                        If Not opcHealthy Then
                            Try
                                Await OpcConnection.Disconnect()
                                opcHealthy = Await OpcConnection.InitializeAsync()
                            Catch reconnectEx As Exception
                                opcHealthy = False
                            End Try
                        End If
                    End If
                Catch ex As Exception
                    opcHealthy = False
                End Try
                If opcHealthy Then
                    LoggerDebug.LogSuccess("OPC connection OK")
                Else
                    LoggerDebug.LogWarning("OPC not connected")
                End If
                SettingsManager.SetConnectionOPC(opcHealthy)

                ' 2. Check SQL Server Database only
                Dim sqlServerHealthy As Boolean = False
                Try
                    sqlServerHealthy = Await SqlServerConnection.CheckHealth()

                    If Not sqlServerHealthy Then
                        ' Try to reinitialize SQL Server connection
                        Try
                            Dim parts = My.Settings.hostDB.Split(";"c)
                            Dim server = "localhost\SQLEXPRESS"
                            Dim database = "OpcUaClient"

                            For Each part In parts
                                If part.ToLower().Contains("server") AndAlso part.Contains("=") Then
                                    server = part.Split("="c)(1).Trim()
                                ElseIf part.ToLower().Contains("database") AndAlso part.Contains("=") Then
                                    database = part.Split("="c)(1).Trim()
                                End If
                            Next

                            SqlServerConnection.SetConnectionString(server, database)
                            sqlServerHealthy = Await SqlServerConnection.CheckHealth()
                        Catch reconnectEx As Exception
                            sqlServerHealthy = False
                        End Try
                    End If
                Catch ex As Exception
                    sqlServerHealthy = False
                End Try
                If sqlServerHealthy Then
                    LoggerDebug.LogSuccess("DB connection OK")
                Else
                    LoggerDebug.LogWarning("DB not connected")
                End If
                SettingsManager.SetConnectionDB(sqlServerHealthy)

            Catch ex As Exception
                SettingsManager.SetConnectionOPC(False)
                SettingsManager.SetConnectionDB(False)
            End Try
        End Function

        Private Async Function ReadChildSensors(parentNodeId As String) As Task(Of Dictionary(Of String, Double))
            Dim sensors As New Dictionary(Of String, Double)
            Try
                If Not OpcConnection.IsConnected Then Return sensors

                ' Get all sensors from settings (both PressureTire and PressureGauge)
                Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()
                Dim allSensors As New List(Of Dictionary(Of String, String))

                ' Collect all sensors from both PressureTire and PressureGauge
                For Each kvp In selectedNodeSensor
                    For Each sensor In kvp.Value
                        allSensors.Add(sensor)
                    Next
                Next

                If allSensors.Count = 0 Then
                    Console.WriteLine("ℹ️ No sensors found in settings")
                    Return sensors
                End If

                ' Detect NodeId format from first sensor
                Dim firstSensorNodeId = allSensors(0)("NodeId")
                Dim isDirectFormat = IsDirectNodeIdFormat(firstSensorNodeId)

                Console.WriteLine($"🔍 Detected NodeId format: {If(isDirectFormat, "Direct", "Hierarchical")} (sample: {firstSensorNodeId})")

                If isDirectFormat Then
                    ' Direct format: read directly from NodeId - use NodeId as key
                    For Each sensor In allSensors
                        Try
                            Dim nodeId = sensor("NodeId")
                            Dim nodeText = sensor("NodeText")

                            Dim value = Await OpcConnection.Session.ReadValueAsync(Opc.Ua.NodeId.Parse(nodeId))

                            If StatusCode.IsGood(value.StatusCode) AndAlso value.Value IsNot Nothing Then
                                Dim pressure As Double = Convert.ToDouble(value.Value)
                                sensors.Add(nodeId, pressure) ' Use NodeId as key
                                Console.WriteLine($"✅ Direct read {nodeText} ({nodeId}): {pressure} PSI")
                            Else
                                Console.WriteLine($"⚠️ Failed to read {nodeText} from {nodeId}: {value.StatusCode}")
                            End If
                        Catch ex As Exception
                            Console.WriteLine($"⚠️ Direct read error for {sensor("NodeText")}: {ex.Message}")
                        End Try
                    Next
                Else
                    ' Hierarchical format: try direct read first, then browse if needed - use NodeId as key
                    For Each sensor In allSensors
                        Try
                            Dim nodeId = sensor("NodeId")
                            Dim nodeText = sensor("NodeText")

                            ' Try direct read from hierarchical NodeId first
                            Dim value = Await OpcConnection.Session.ReadValueAsync(Opc.Ua.NodeId.Parse(nodeId))

                            If StatusCode.IsGood(value.StatusCode) AndAlso value.Value IsNot Nothing Then
                                Dim pressure As Double = Convert.ToDouble(value.Value)
                                sensors.Add(nodeId, pressure) ' Use NodeId as key
                                Console.WriteLine($"✅ Hierarchical direct read {nodeText} ({nodeId}): {pressure} PSI")
                            Else
                                Console.WriteLine($"⚠️ Failed to read {nodeText} from {nodeId}: {value.StatusCode}")
                            End If
                        Catch ex As Exception
                            Console.WriteLine($"⚠️ Hierarchical read error for {sensor("NodeText")}: {ex.Message}")
                        End Try
                    Next
                End If

            Catch ex As Exception
                Console.WriteLine($"⚠️ ReadChildSensors error: {ex.Message}")
            End Try
            Return sensors
        End Function
        
        Private Function IsDirectNodeIdFormat(nodeId As String) As Boolean
            ' Direct format examples: ns=2;s=DB3.DBD120, ns=2;s=DB3.DBD60
            ' Hierarchical format examples: ns=2;s=PLC.S7-1200.PressureTire.Sensor 1
            
            If String.IsNullOrEmpty(nodeId) Then Return False
            
            ' Check if it contains typical direct patterns
            If nodeId.Contains("DB") AndAlso nodeId.Contains("DBD") Then
                Return True ' DB3.DBD120 pattern
            End If
            
            ' Check if it contains hierarchical patterns
            If nodeId.Contains("PLC.") AndAlso nodeId.Contains("Pressure") Then
                Return False ' PLC.S7-1200.PressureTire pattern
            End If
            
            ' Default: assume direct if short, hierarchical if long with multiple dots
            Dim dotCount = nodeId.Count(Function(c) c = "."c)
            Return dotCount <= 1 ' Direct if 1 or fewer dots, hierarchical if more
        End Function



        Private Async Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles worker.DoWork


            ' Try to initialize OPC connection, but continue even if it fails
            Dim connected = Await OpcConnection.InitializeAsync()
            If connected Then
                Console.WriteLine("✅ BackgroundWorker OPC connection established")
            Else
                Console.WriteLine("⚠️ OPC connection failed, but BackgroundWorker will continue for health checks")
            End If

            ' Main processing loop
            Dim cycleCount = 0
            Do While Not worker.CancellationPending
                Try
                    ' 1. Check health connections setiap cycle untuk UI dinamis
                    Await CheckHealthConnections()

                    ' 2. Process sensor data hanya jika ada running sensors
                    Try
                        ProcessSensorData()
                    Catch sensorEx As Exception
                        ' Log critical errors only
                    End Try

                    cycleCount += 1

                    ' 3. Sleep sesuai interval dari My.Settings
                    Threading.Thread.Sleep(My.Settings.intervalRefreshTimer)

                Catch ex As Exception

                    ' Set connections to false on error
                    SettingsManager.SetConnectionOPC(False)
                    SettingsManager.SetConnectionDB(False)

                    Threading.Thread.Sleep(My.Settings.intervalRefreshTimer)
                End Try
            Loop

        End Sub

        Public Sub [Stop]()
            Try
                If worker.IsBusy Then
                    worker.CancelAsync()

                    ' Wait for worker to finish (with timeout)
                    Dim timeout = 0
                    While worker.IsBusy AndAlso timeout < 50 ' 5 seconds max
                        Threading.Thread.Sleep(100)
                        timeout += 1
                    End While
                End If
            Catch ex As Exception
                ' Ignore stop errors
            End Try
        End Sub

        Public ReadOnly Property IsRunning As Boolean
            Get
                Return worker.IsBusy
            End Get
        End Property
    End Class
End Namespace
