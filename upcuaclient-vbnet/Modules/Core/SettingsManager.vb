Imports Newtonsoft.Json

Public Class SettingsManager

    ' === Initialize default values ===
    Public Shared Sub InitializeDefaults()
        If String.IsNullOrEmpty(My.Settings.hostOpc) Then
            My.Settings.hostOpc = "opc.tcp://localhost:4840"
        End If

        If String.IsNullOrEmpty(My.Settings.namespaceOpc) Then
            My.Settings.namespaceOpc = "ns=2;i=2"
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
            My.Settings.intervalRefreshTimer = 60000
        End If

        If String.IsNullOrEmpty(My.Settings.Units) Then
            My.Settings.Units = "PSI"
        End If

        If String.IsNullOrEmpty(My.Settings.hostDB) Then
            My.Settings.hostDB = "Server=localhost\SQLEXPRESS;Database=OpcUaClient"
        End If

        If My.Settings.intervalCheckConnections = 0 Then
            My.Settings.intervalCheckConnections = 120000
        End If
        
        If String.IsNullOrEmpty(My.Settings.selectedNodeSensor) Then
            My.Settings.selectedNodeSensor = "{}"
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
            LoggerDebug.LogInfo($"GetSelectedNodeIdOpc: Raw JSON = {My.Settings.selectedNodeIdOpc}")

            Dim result = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, Object)))(My.Settings.selectedNodeIdOpc)

            LoggerDebug.LogInfo($"GetSelectedNodeIdOpc: Deserialized {result.Count} objects")

            For i = 0 To result.Count - 1
                LoggerDebug.LogInfo($"  Object {i}: {result(i).Keys.Count} keys")
                For Each key In result(i).Keys
                    Dim value = result(i)(key)
                    If key = "ChildNodeId" Then
                        LoggerDebug.LogInfo($"    Key: {key} = Type: {value.GetType().Name}, Value: {value}")
                        If TypeOf value Is List(Of Object) Then
                            Dim childList = DirectCast(value, List(Of Object))
                            LoggerDebug.LogInfo($"      ChildNodeId has {childList.Count} items")
                            For j = 0 To Math.Min(childList.Count - 1, 2) ' Show first 3 items
                                LoggerDebug.LogInfo($"        Item {j}: Type: {childList(j).GetType().Name}, Value: {childList(j)}")
                            Next
                        End If
                    Else
                        LoggerDebug.LogInfo($"    Key: {key} = {value}")
                    End If
                Next
            Next

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
        LoggerDebug.LogInfo($"SetSelectedNodeSensor: Saving JSON data: {jsonData.Substring(0, Math.Min(200, jsonData.Length))}...")
        My.Settings.selectedNodeSensor = jsonData
        My.Settings.Save()
        LoggerDebug.LogInfo($"SetSelectedNodeSensor: Data saved to My.Settings")
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

    ' === Save all settings ===
    Public Shared Sub SaveAll()
        My.Settings.Save()
    End Sub

End Class