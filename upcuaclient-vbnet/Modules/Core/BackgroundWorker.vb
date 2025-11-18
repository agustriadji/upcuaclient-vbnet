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
                                dataProcessor.ProcessAlert(alert)
                            End If

                            Console.WriteLine($"💾 Processed running sensor {sensorNodeText}: {pressure} PSI")
                        End If
                    Next
                Next

            Catch ex As Exception
                Console.WriteLine($"⚠️ Process sensor data error: {ex.Message}")
            End Try
        End Sub

        Private Sub CheckHealthConnections()
            Try
                ' Hanya check health jika sudah lewat interval untuk menghindari double handler dengan FormMainNew
                Dim now = DateTime.Now
                If (now - lastHealthCheck) < healthCheckInterval Then
                    Return
                End If

                lastHealthCheck = now
                Console.WriteLine("🔍 BackgroundWorker checking connections health...")

                ' Check OPC connection health
                Task.Run(Async Function()
                             Try
                                 Dim opcHealthy = Await OpcConnection.CheckHealthServer()
                                 Console.WriteLine($"📡 OPC Health: {If(opcHealthy, "✅ Connected", "❌ Disconnected")}")
                             Catch ex As Exception
                                 Console.WriteLine($"⚠️ OPC health check error: {ex.Message}")
                             End Try
                         End Function)

                ' Check database connection health (SQLite)
                Try
                    Dim sqlite As New SQLiteManager()
                    Dim testCount = sqlite.GetTotalSensorDataCount()
                    Console.WriteLine($"🗄️ Database Health: ✅ Connected (Records: {testCount})")
                Catch ex As Exception
                    Console.WriteLine($"🗄️ Database Health: ❌ Error - {ex.Message}")
                End Try

            Catch ex As Exception
                Console.WriteLine($"⚠️ Health check error: {ex.Message}")
            End Try
        End Sub

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
                            If StatusCode.IsGood(value.StatusCode) AndAlso value.Value IsNot Nothing Then
                                Dim pressure = Double.Parse(value.Value.ToString().Replace(",", "."))
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
            Console.WriteLine("🔄 BackgroundWorker started...")

            ' Initialize OPC connection using My.Settings
            Dim connected = Await OpcConnection.InitializeAsync()
            If Not connected Then
                Console.WriteLine("❌ Failed to connect to OPC server")
                Return
            End If

            Console.WriteLine("✅ BackgroundWorker OPC connection established")

            ' Main processing loop
            Dim cycleCount = 0
            Do While Not worker.CancellationPending
                Try
                    ' 1. Check health connections (dengan interval untuk avoid double handler)
                    CheckHealthConnections()

                    ' 2. Process sensor data hanya jika ada running sensors
                    ProcessSensorData()

                    cycleCount += 1
                    Console.WriteLine($"🔄 BackgroundWorker cycle {cycleCount} completed")

                    ' 3. Sleep sesuai interval dari My.Settings
                    Console.WriteLine($"⏱️ Sleeping for {My.Settings.intervalRefreshTimer} ms...")
                    Threading.Thread.Sleep(My.Settings.intervalRefreshTimer)

                Catch ex As Exception
                    Console.WriteLine($"⚠️ BackgroundWorker cycle error: {ex.Message}")
                    Console.WriteLine($"🔍 Exception details: {ex.ToString()}")
                    Threading.Thread.Sleep(5000) ' Sleep 5 detik jika error
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
