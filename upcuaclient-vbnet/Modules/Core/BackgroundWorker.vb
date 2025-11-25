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
            Console.WriteLine("🔄 BackgroundWorkerManager.Start() called")

            ' Check if worker is already running
            If worker.IsBusy Then
                Console.WriteLine("⚠️ BackgroundWorker is already running")
                Return
            End If

            ' Initialize settings defaults
            SettingsManager.InitializeDefaults()
            Console.WriteLine($"✅ Settings loaded: endpoint={My.Settings.hostOpc}, interval={My.Settings.intervalRefreshTimer}")
            worker.RunWorkerAsync()
        End Sub

        Private Async Sub ProcessSensorData()
            Try
                ' 1. Get running sensors dari selectedNodeSensor dengan NodeStatus = "running"
                Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()
                Dim runningSensors As New List(Of Dictionary(Of String, String))

                For Each kvp In selectedNodeSensor
                    Dim sensorList = kvp.Value
                    For Each sensor In sensorList
                        If sensor.ContainsKey("NodeStatus") AndAlso sensor("NodeStatus").ToLower() = "running" Then
                            runningSensors.Add(sensor)
                        End If
                    Next
                Next

                If runningSensors.Count = 0 Then
                    Console.WriteLine("ℹ️ No running sensors found in selectedNodeSensor")
                    Return ' Exit ProcessSensorData but continue main loop
                End If

                Console.WriteLine($"🔍 Processing {runningSensors.Count} running sensors...")

                ' 1.5. Check auto end recording for running sensors
                CheckAutoEndRecording(runningSensors)

                ' 2. Get selected root objects dari selectedNodeIdOpc
                Dim selectedRootObjects = SettingsManager.GetSelectedNodeIdOpc()
                If selectedRootObjects.Count = 0 Then
                    Console.WriteLine("⚠️ No selected root objects found in selectedNodeIdOpc")
                    Return
                End If

                ' 3. Initialize processors
                Dim dataProcessor As New InterfaceData()
                Dim analytics2 As New AnalyticsManager2()

                ' 4. Process each root object
                For Each rootObj In selectedRootObjects
                    Dim rootNodeId = rootObj("NodeId").ToString()
                    Dim rootNodeText = rootObj("NodeText").ToString()

                    Console.WriteLine($"📊 Reading all child sensors from {rootNodeText} ({rootNodeId})...")

                    ' 5. Read all child sensors dari OPC (single connection per root object)
                    Dim allChildSensorData = Await ReadChildSensors(rootNodeId)

                    ' 6. Filter hanya sensor yang running dari selectedNodeSensor
                    For Each runningSensor In runningSensors
                        Dim sensorNodeText = runningSensor("NodeText")
                        Dim sensorNodeId = runningSensor("NodeId")

                        ' Check if this running sensor data exists in OPC reading
                        If allChildSensorData.ContainsKey(sensorNodeText) Then
                            Dim pressure = allChildSensorData(sensorNodeText)

                            ' 7. Process data melalui AnalyticsManager dan SQLiteManager
                            Dim sensorData = analytics2.MapSensorData(sensorNodeId, sensorNodeText, pressure, "Good")
                            Dim alert = analytics2.GenerateAlert(sensorNodeId, pressure)

                            dataProcessor.ProcessSensorData(sensorData)
                            If alert IsNot Nothing Then
                                Console.WriteLine($"🚨 Alert will be processed: {alert.Message}")
                                dataProcessor.ProcessAlert(alert)
                            Else
                                Console.WriteLine($"ℹ️ No alert generated for {sensorNodeText}")
                            End If

                            Console.WriteLine($"💾 Processed running sensor {sensorNodeText}: {pressure} PSI")

                            Console.WriteLine($"💾 Processed running sensor {sensorNodeText}: {pressure} PSI")
                        End If
                    Next
                Next

            Catch ex As Exception
                Console.WriteLine($"⚠️ Process sensor data error: {ex.Message}")
            End Try
        End Sub

        Private Sub CheckAutoEndRecording(runningSensors As List(Of Dictionary(Of String, String)))
            Try
                ' Reload settings to detect manual file changes
                SettingsManager.ReloadSettings()
                
                ' Debug: Show current time and endRecording settings
                Dim currentTimeUtc = DateTime.UtcNow
                Dim endRecordingJson = My.Settings.endRecording
                Console.WriteLine($"🔍 CheckAutoEndRecording - Current UTC: {currentTimeUtc:yyyy-MM-dd HH:mm:ss}")
                Console.WriteLine($"🔍 EndRecording JSON: {endRecordingJson}")

                ' Get expired end recordings from settings
                Dim expiredRecordings = SettingsManager.GetExpiredEndRecordings()
                Console.WriteLine($"🔍 Found {expiredRecordings.Count} expired recordings")

                If expiredRecordings.Count > 0 Then
                    Console.WriteLine($"⏰ Found {expiredRecordings.Count} expired auto end recordings")

                    For Each expiredRecording In expiredRecordings
                        Console.WriteLine($"🔍 Checking expired recording: {JsonConvert.SerializeObject(expiredRecording)}")

                        ' Find matching running sensor
                        Dim matchingSensor = runningSensors.FirstOrDefault(Function(s) s("NodeId") = expiredRecording("pressureTire"))
                        If matchingSensor IsNot Nothing Then
                            Console.WriteLine($"⏰ Auto end recording triggered for {matchingSensor("NodeText")} at {expiredRecording("end_date")}")
                            ProcessAutoEndRecording(matchingSensor, expiredRecording)
                        Else
                            Console.WriteLine($"⚠️ No matching running sensor found for {expiredRecording("pressureTire")}")
                        End If
                    Next
                Else
                    Console.WriteLine($"ℹ️ No expired auto end recordings found")
                End If

            Catch ex As Exception
                Console.WriteLine($"⚠️ CheckAutoEndRecording Error: {ex.Message}")
            End Try
        End Sub

        Private Sub ProcessAutoEndRecording(sensor As Dictionary(Of String, String), expiredRecording As Dictionary(Of String, String))
            Try
                Dim nodeText = sensor("NodeText")
                Dim tireNodeId = sensor("NodeId")
                Dim gaugeNodeId = expiredRecording("pressureGauge")

                Console.WriteLine($"🔄 Processing auto end recording for {nodeText}")

                ' 1. Remove expired recording from endRecording array
                SettingsManager.RemoveEndRecording(tireNodeId)

                ' 2. Update sensor status to Idle
                Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

                ' Update PressureTire status
                If selectedNodeSensor.ContainsKey("PressureTire") Then
                    For Each tireSensor In selectedNodeSensor("PressureTire")
                        If tireSensor("NodeId") = tireNodeId Then
                            tireSensor("NodeStatus") = "Idle"
                        End If
                    Next
                End If

                ' Update PressureGauge status
                If selectedNodeSensor.ContainsKey("PressureGauge") Then
                    For Each gaugeSensor In selectedNodeSensor("PressureGauge")
                        If gaugeSensor("NodeId") = gaugeNodeId Then
                            gaugeSensor("NodeStatus") = "Idle"
                            Exit For
                        End If
                    Next
                End If

                SettingsManager.SetSelectedNodeSensor(selectedNodeSensor)

                ' 3. Call end recording function (similar to DetailRecord.vb)
                EndRecordingForSensor(tireNodeId)

                Console.WriteLine($"✅ Auto end recording completed for {nodeText}")

            Catch ex As Exception
                Console.WriteLine($"⚠️ ProcessAutoEndRecording Error: {ex.Message}")
            End Try
        End Sub

        Private Sub EndRecordingForSensor(tireNodeId As String)
            Try
                ' Find active batch for this sensor
                Dim sqlite As New SQLiteManager()
                Dim batches = sqlite.QueryBatchRange(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow)

                For Each batch In batches
                    If batch.PressureTireId = tireNodeId AndAlso batch.Status.ToLower() = "recording" Then
                        ' Update batch status to Finished
                        batch.Status = "Finished"
                        batch.SyncStatus = "Finished"
                        batch.EndDate = DateTime.UtcNow

                        Console.WriteLine($"🔄 Ending batch {batch.BatchId} - Status: {batch.Status}, EndDate: {batch.EndDate:yyyy-MM-dd HH:mm:ss} UTC")

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

                        Console.WriteLine($"✅ Batch {batch.BatchId} ended automatically")
                        Exit For
                    End If
                Next

            Catch ex As Exception
                Console.WriteLine($"⚠️ EndRecordingForSensor Error: {ex.Message}")
            End Try
        End Sub

        Private Async Function CheckHealthConnections() As Task
            Try
                Console.WriteLine("🔍 BackgroundWorker checking connections health...")

                ' 1. Check OPC Connection
                Dim opcHealthy As Boolean = False
                Try
                    ' Check if session is still valid first
                    If OpcConnection.Session Is Nothing Then
                        Console.WriteLine("❌ OPC Session is null, trying to reconnect...")
                        opcHealthy = Await OpcConnection.InitializeAsync()
                        Console.WriteLine($"🔄 OPC Reconnect result: {opcHealthy}")
                    ElseIf Not OpcConnection.Session.Connected Then
                        Console.WriteLine("❌ OPC Session not connected, trying to reconnect...")
                        opcHealthy = Await OpcConnection.InitializeAsync()
                        Console.WriteLine($"🔄 OPC Reconnect result: {opcHealthy}")
                    Else
                        Console.WriteLine("🔍 OPC Session appears connected, testing...")
                        opcHealthy = Await OpcConnection.CheckHealthServer()
                        Console.WriteLine($"📡 OPC Health result: {opcHealthy}")

                        ' If health check failed but session exists, try to reconnect
                        If Not opcHealthy Then
                            Console.WriteLine("❌ OPC Health check failed, trying to reconnect...")
                            Try
                                Await OpcConnection.Disconnect()
                                opcHealthy = Await OpcConnection.InitializeAsync()
                                Console.WriteLine($"🔄 OPC Force reconnect result: {opcHealthy}")
                            Catch reconnectEx As Exception
                                Console.WriteLine($"❌ OPC Force reconnect failed: {reconnectEx.Message}")
                                opcHealthy = False
                            End Try
                        End If
                    End If
                Catch ex As Exception
                    Console.WriteLine($"❌ OPC Health check exception: {ex.Message}")
                    opcHealthy = False
                End Try

                Console.WriteLine($"🔧 Setting OPC connection to: {opcHealthy}")
                If opcHealthy Then
                    LoggerDebug.LogSuccess("OPC connection OK")
                Else
                    LoggerDebug.LogWarning("OPC not connected")
                End If
                SettingsManager.SetConnectionOPC(opcHealthy)

                ' 2. Check SQL Server Database only
                Dim sqlServerHealthy As Boolean = False
                Try
                    Console.WriteLine("🔍 Testing SQL Server connection...")
                    sqlServerHealthy = Await SqlServerConnection.CheckHealth()

                    If Not sqlServerHealthy Then
                        Console.WriteLine("❌ SQL Server disconnected, trying to reconnect...")
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
                            Console.WriteLine($"🔄 SQL Server Reconnect result: {sqlServerHealthy}")
                        Catch reconnectEx As Exception
                            Console.WriteLine($"❌ SQL Server reconnect failed: {reconnectEx.Message}")
                            sqlServerHealthy = False
                        End Try
                    End If

                    Console.WriteLine($"📊 SQL Server Health: {If(sqlServerHealthy, "✅ Connected", "❌ Disconnected")}")
                Catch ex As Exception
                    Console.WriteLine($"📊 SQL Server Health: ❌ Error - {ex.GetType().Name}: {ex.Message}")
                    sqlServerHealthy = False
                End Try

                Console.WriteLine($"🔧 Setting DB connection to: {sqlServerHealthy}")
                If sqlServerHealthy Then
                    LoggerDebug.LogSuccess("DB connection OK")
                Else
                    LoggerDebug.LogWarning("DB not connected")
                End If
                SettingsManager.SetConnectionDB(sqlServerHealthy)

            Catch ex As Exception
                Console.WriteLine($"⚠️ Health check error: {ex.Message}")
                SettingsManager.SetConnectionOPC(False)
                SettingsManager.SetConnectionDB(False)
            End Try
        End Function

        Private Async Function ReadChildSensors(parentNodeId As String) As Task(Of Dictionary(Of String, Double))
            Dim sensors As New Dictionary(Of String, Double)
            Try
                If Not OpcConnection.IsConnected Then Return sensors

                Dim browser = New Browser(OpcConnection.Session) With {
                    .BrowseDirection = BrowseDirection.Forward,
                    .NodeClassMask = NodeClass.Variable
                }

                Dim nodeId = Opc.Ua.NodeId.Parse(parentNodeId)
                Dim childRefs = Await browser.BrowseAsync(nodeId)

                For Each childRef As ReferenceDescription In childRefs
                    If childRef.NodeClass = NodeClass.Variable Then
                        Try
                            Dim childNodeId = ExpandedNodeId.ToNodeId(childRef.NodeId, OpcConnection.Session.NamespaceUris)
                            Dim value = Await OpcConnection.Session.ReadValueAsync(childNodeId)
                            'Console.WriteLine($"NodeId: {childNodeId}")
                            'Console.WriteLine($"Raw Value: {}")
                            'Console.WriteLine($"Data Type: {value.Value?.GetType()}")
                            'Console.WriteLine($"Status Code: {value.StatusCode}")
                            'Console.WriteLine($"Server Timestamp: {value.ServerTimestamp}")
                            'Console.WriteLine($"Source Timestamp: {value.SourceTimestamp}")
                            'Console.WriteLine("===============================================")


                            If StatusCode.IsGood(value.StatusCode) AndAlso value.Value IsNot Nothing Then
                                Dim pressure = value.Value
                                sensors.Add(childRef.DisplayName.Text, pressure)
                            End If
                        Catch ex As Exception
                            Console.WriteLine($"⚠️ Read child sensor error: {ex.Message}")
                        End Try
                    End If
                Next
            Catch ex As Exception
                Console.WriteLine($"⚠️ Browse child sensors error: {ex.Message}")
            End Try
            Return sensors
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
                        Console.WriteLine($"⚠️ ProcessSensorData error: {sensorEx.Message}")
                    End Try

                    cycleCount += 1
                    Console.WriteLine($"✅ BackgroundWorker cycle {cycleCount} completed - OPC: {My.Settings.stateConnectionOPC}, DB: {My.Settings.stateConnectionDB}")
                    Console.WriteLine($"⏱️ Next cycle in {My.Settings.intervalRefreshTimer} ms")

                    ' 3. Sleep sesuai interval dari My.Settings
                    Threading.Thread.Sleep(My.Settings.intervalRefreshTimer)

                Catch ex As Exception
                    Console.WriteLine($"⚠️ BackgroundWorker cycle error: {ex.Message}")
                    Console.WriteLine($"🔍 Exception details: {ex.ToString()}")

                    ' Set connections to false on error
                    SettingsManager.SetConnectionOPC(False)
                    SettingsManager.SetConnectionDB(False)

                    Threading.Thread.Sleep(My.Settings.intervalRefreshTimer)
                End Try
            Loop

            Console.WriteLine("🛑 BackgroundWorker stopped")
        End Sub

        Public Sub [Stop]()
            Try
                If worker.IsBusy Then
                    Console.WriteLine("🛑 Stopping BackgroundWorker...")
                    worker.CancelAsync()

                    ' Wait for worker to finish (with timeout)
                    Dim timeout = 0
                    While worker.IsBusy AndAlso timeout < 50 ' 5 seconds max
                        Threading.Thread.Sleep(100)
                        timeout += 1
                    End While

                    If worker.IsBusy Then
                        Console.WriteLine("⚠️ BackgroundWorker did not stop gracefully")
                    Else
                        Console.WriteLine("✅ BackgroundWorker stopped successfully")
                    End If
                End If
            Catch ex As Exception
                Console.WriteLine($"⚠️ Stop BackgroundWorker error: {ex.Message}")
            End Try
        End Sub

        Public ReadOnly Property IsRunning As Boolean
            Get
                Return worker.IsBusy
            End Get
        End Property
    End Class
End Namespace
