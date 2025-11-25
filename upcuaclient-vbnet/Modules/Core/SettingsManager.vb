Imports Newtonsoft.Json

Public Class SettingsManager

    ' === Initialize default values ===
    Public Shared Sub InitializeDefaults()
        If String.IsNullOrEmpty(My.Settings.hostOpc) Then
            My.Settings.hostOpc = "opc.tcp://localhost:4840"
        End If

        If String.IsNullOrEmpty(My.Settings.namespaceOpc) Then
            My.Settings.namespaceOpc = ""
        End If

        If String.IsNullOrEmpty(My.Settings.nodeIdOpc) Then
            My.Settings.nodeIdOpc = "[]"
        End If

        If String.IsNullOrEmpty(My.Settings.selectedNodeIdOpc) Then
            My.Settings.selectedNodeIdOpc = "[]"
        End If

        If My.Settings.defaultTimeout = 0 Then
            My.Settings.defaultTimeout = 30
        End If

        If My.Settings.intervalTime = 0 Then
            My.Settings.intervalTime = 120000
        End If

        If My.Settings.intervalRefreshTimer = 0 Then
            My.Settings.intervalRefreshTimer = 5000  ' 5 detik untuk UI dinamis
        End If

        If String.IsNullOrEmpty(My.Settings.Units) Then
            My.Settings.Units = "PSI"
        End If

        If String.IsNullOrEmpty(My.Settings.hostDB) Then
            My.Settings.hostDB = "Server=localhost\SQLEXPRESS;Database=OpcUaClient"
        End If

        If My.Settings.intervalCheckConnections = 0 Then
            My.Settings.intervalCheckConnections = 10000  ' 10 detik untuk indicator dinamis
        End If

        If String.IsNullOrEmpty(My.Settings.selectedNodeSensor) Then
            My.Settings.selectedNodeSensor = "{}"
        End If

        If My.Settings.intervalRefreshMain = 0 Then
            My.Settings.intervalRefreshMain = 3000  ' 3 detik untuk main form refresh
        End If

        If String.IsNullOrEmpty(My.Settings.thresholdPressureGauge) Then
            My.Settings.thresholdPressureGauge = "0.2"
        End If

        If String.IsNullOrEmpty(My.Settings.endRecording) Then
            My.Settings.endRecording = "[]"
        End If

        My.Settings.Save()
    End Sub

    ' === JSON Helper Methods ===
    Public Shared Function GetNodeIdOpc() As List(Of Dictionary(Of String, String))
        Try
            Return JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(My.Settings.nodeIdOpc)
        Catch
            Return New List(Of Dictionary(Of String, String))
        End Try
    End Function

    Public Shared Sub SetNodeIdOpc(nodes As List(Of Dictionary(Of String, String)))
        My.Settings.nodeIdOpc = JsonConvert.SerializeObject(nodes)
        My.Settings.Save()
    End Sub

    Public Shared Function GetSelectedNodeIdOpc() As List(Of Dictionary(Of String, Object))
        Try
            'LoggerDebug.LogInfo($"GetSelectedNodeIdOpc: Raw JSON = {My.Settings.selectedNodeIdOpc}")

            Dim result = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, Object)))(My.Settings.selectedNodeIdOpc)

            Return result
        Catch ex As Exception
            LoggerDebug.LogError($"GetSelectedNodeIdOpc error: {ex.Message}")
            Return New List(Of Dictionary(Of String, Object))
        End Try
    End Function

    Public Shared Sub SetSelectedNodeIdOpc(selectedNodes As List(Of Dictionary(Of String, Object)))
        My.Settings.selectedNodeIdOpc = JsonConvert.SerializeObject(selectedNodes)
        My.Settings.Save()
    End Sub

    ' === Add node to nodeIdOpc ===
    Public Shared Sub AddNodeIdOpc(nodeText As String, nodeId As String, nodeType As String)
        Dim nodes = GetNodeIdOpc()

        ' Check if already exists
        If Not nodes.Any(Function(n) n("NodeId") = nodeId) Then
            nodes.Add(New Dictionary(Of String, String) From {
                {"NodeText", nodeText},
                {"NodeId", nodeId},
                {"NodeType", nodeType}
            })
            SetNodeIdOpc(nodes)
        End If
    End Sub

    ' === Add selected node with children ===
    Public Shared Sub AddSelectedNodeIdOpc(nodeText As String, nodeId As String, nodeType As String, childNodes As List(Of Dictionary(Of String, String)))
        Dim selectedNodes = GetSelectedNodeIdOpc()

        ' Remove existing if any
        selectedNodes.RemoveAll(Function(n) n("NodeId").ToString() = nodeId)

        ' Add new - ensure childNodes is stored as object array, not JSON string
        selectedNodes.Add(New Dictionary(Of String, Object) From {
            {"NodeText", nodeText},
            {"NodeId", nodeId},
            {"NodeType", nodeType},
            {"ChildNodeId", childNodes.Cast(Of Object).ToList()}
        })

        SetSelectedNodeIdOpc(selectedNodes)
    End Sub

    ' === Remove selected node ===
    Public Shared Sub RemoveSelectedNodeIdOpc(nodeId As String)
        Dim selectedNodes = GetSelectedNodeIdOpc()
        selectedNodes.RemoveAll(Function(n) n("NodeId").ToString() = nodeId)
        SetSelectedNodeIdOpc(selectedNodes)
    End Sub

    ' === Selected Node Sensor Methods ===
    Public Shared Function GetSelectedNodeSensor() As Dictionary(Of String, List(Of Dictionary(Of String, String)))
        Try
            If String.IsNullOrEmpty(My.Settings.selectedNodeSensor) Then
                My.Settings.selectedNodeSensor = "{}"
            End If
            Return JsonConvert.DeserializeObject(Of Dictionary(Of String, List(Of Dictionary(Of String, String))))(My.Settings.selectedNodeSensor)
        Catch
            Return New Dictionary(Of String, List(Of Dictionary(Of String, String)))
        End Try
    End Function
    
    Public Shared Sub SetSelectedNodeSensor(sensors As Dictionary(Of String, List(Of Dictionary(Of String, String))))
        Dim jsonData = JsonConvert.SerializeObject(sensors)
        My.Settings.selectedNodeSensor = jsonData
        My.Settings.Save()
    End Sub

    ' === Get running sensors from selectedNodeSensor ===
    Public Shared Function GetRunningSensors() As Dictionary(Of String, Object)
        Try
            Dim selectedNodeSensor = GetSelectedNodeSensor()
            Dim runningSensors As New Dictionary(Of String, Object)

            ' Filter sensor dengan NodeStatus = "running"
            For Each kvp In selectedNodeSensor
                If TypeOf kvp.Value Is List(Of Dictionary(Of String, String)) Then
                    Dim sensorList = DirectCast(kvp.Value, List(Of Dictionary(Of String, String)))
                    For Each sensor In sensorList
                        If sensor.ContainsKey("NodeStatus") AndAlso sensor("NodeStatus").ToLower() = "running" Then
                            runningSensors.Add(sensor("NodeId"), sensor)
                        End If
                    Next
                End If
            Next

            Return runningSensors
        Catch ex As Exception
            LoggerDebug.LogError($"GetRunningSensors error: {ex.Message}")
            Return New Dictionary(Of String, Object)
        End Try
    End Function

    ' === Connection Status Methods ===
    Public Shared Function CheckHealth() As (opcConnected As Boolean, dbConnected As Boolean)
        Return (My.Settings.stateConnectionOPC, My.Settings.stateConnectionDB)
    End Function

    Public Shared Sub SetConnectionOPC(isConnected As Boolean)
        My.Settings.stateConnectionOPC = isConnected
        My.Settings.Save()
    End Sub

    Public Shared Sub SetConnectionDB(isConnected As Boolean)
        My.Settings.stateConnectionDB = isConnected
        My.Settings.Save()
    End Sub

    Public Shared Function IsOPCConnected() As Boolean
        Return My.Settings.stateConnectionOPC
    End Function

    Public Shared Function IsDBConnected() As Boolean
        Return My.Settings.stateConnectionDB
    End Function

    ' === End Recording Methods ===
    Public Shared Function GetEndRecording() As List(Of Dictionary(Of String, String))
        Try
            Return JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(My.Settings.endRecording)
        Catch
            Return New List(Of Dictionary(Of String, String))
        End Try
    End Function

    Public Shared Sub SetEndRecording(endRecordings As List(Of Dictionary(Of String, String)))
        My.Settings.endRecording = JsonConvert.SerializeObject(endRecordings)
        My.Settings.Save()
    End Sub

    Public Shared Sub AddEndRecording(pressureTireId As String, pressureGaugeId As String, endDate As DateTime)
        Dim endRecordings = GetEndRecording()

        ' Remove existing entry for same sensors if any
        endRecordings.RemoveAll(Function(e) e("pressureTire") = pressureTireId)

        ' Store as UTC, but convert to local for comparison
        Dim endDateUtc = If(endDate.Kind = DateTimeKind.Utc, endDate, endDate.ToUniversalTime())

        ' Add new entry
        endRecordings.Add(New Dictionary(Of String, String) From {
            {"pressureTire", pressureTireId},
            {"pressureGauge", pressureGaugeId},
            {"end_date", endDateUtc.ToString("yyyy-MM-dd HH:mm:ss")}
        })

        SetEndRecording(endRecordings)
    End Sub

    Public Shared Sub RemoveEndRecording(pressureTireId As String)
        Dim endRecordings = GetEndRecording()
        endRecordings.RemoveAll(Function(e) e("pressureTire") = pressureTireId)
        SetEndRecording(endRecordings)
    End Sub

    Public Shared Function GetExpiredEndRecordings() As List(Of Dictionary(Of String, String))
        Dim endRecordings = GetEndRecording()
        Dim expiredRecordings As New List(Of Dictionary(Of String, String))
        Dim currentTimeUtc = DateTime.UtcNow

        For Each recording In endRecordings
            If recording.ContainsKey("end_date") Then
                Dim endDate As DateTime
                If DateTime.TryParse(recording("end_date"), endDate) Then
                    ' Treat stored time as UTC for comparison
                    Dim endDateUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc)
                    If currentTimeUtc >= endDateUtc Then
                        expiredRecordings.Add(recording)
                    End If
                End If
            End If
        Next

        Return expiredRecordings
    End Function

    ' === Reload settings from file ===
    Public Shared Sub ReloadSettings()
        Try
            My.Settings.Reload()
            Console.WriteLine($"🔄 Settings reloaded from file")
        Catch ex As Exception
            Console.WriteLine($"⚠️ Settings reload error: {ex.Message}")
        End Try
    End Sub

    ' === Save all settings ===
    Public Shared Sub SaveAll()
        My.Settings.Save()
    End Sub

End Class