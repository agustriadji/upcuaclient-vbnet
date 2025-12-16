Imports System.IO
Imports Newtonsoft.Json

Namespace upcuaclient_vbnet
    Public Class TempManager
        Private Shared ReadOnly projectRoot As String = Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath))
        Private Shared ReadOnly tempPath As String = Path.Combine(projectRoot, "Temp")

        Public Shared Sub UpdateState(sensorId As Integer, syncAttempt As Integer)
            Try
                Dim sensorFolder = Path.Combine(tempPath, $"sensor{sensorId}")
                Dim stateFile = Path.Combine(sensorFolder, "state.json")

                ' Ensure directory exists
                If Not Directory.Exists(sensorFolder) Then
                    Directory.CreateDirectory(sensorFolder)
                End If

                Dim stateData = New StateData With {
                    .SensorId = $"sensor{sensorId}",
                    .LastUpdated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    .SyncAttempt = syncAttempt
                }

                Dim json = JsonConvert.SerializeObject(stateData, Formatting.Indented)
                File.WriteAllText(stateFile, json)


            Catch ex As Exception
                Console.WriteLine($"⚠️ State update error: {ex.Message}")
            End Try
        End Sub

        Public Shared Sub AddAlert(sensorId As Integer, level As String, message As String)
            Try
                Dim today = DateTime.Now.ToString("yyyy-MM-dd")
                Dim sensorFolder = Path.Combine(tempPath, $"sensor{sensorId}")
                Dim alertFile = Path.Combine(sensorFolder, $"alert.logs.{today}.json")

                ' Ensure directory exists
                If Not Directory.Exists(sensorFolder) Then
                    Directory.CreateDirectory(sensorFolder)
                End If

                Dim alertData As AlertData

                ' Load existing alerts or create new
                If File.Exists(alertFile) Then
                    Dim json = File.ReadAllText(alertFile)
                    alertData = JsonConvert.DeserializeObject(Of AlertData)(json)
                Else
                    alertData = New AlertData With {
                        .alert = New List(Of AlertEntry)
                    }
                End If

                ' Add new alert
                alertData.alert.Add(New AlertEntry With {
                    .timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    .level = level,
                    .message = message
                })

                ' Save back to file
                Dim updatedJson = JsonConvert.SerializeObject(alertData, Formatting.Indented)
                File.WriteAllText(alertFile, updatedJson)


            Catch ex As Exception
                Console.WriteLine($"⚠️ Alert save error: {ex.Message}")
            End Try
        End Sub

        Public Shared Function GetSyncAttempt(sensorId As Integer) As Integer
            Try
                Dim sensorFolder = Path.Combine(tempPath, $"sensor{sensorId}")
                Dim stateFile = Path.Combine(sensorFolder, "state.json")

                If File.Exists(stateFile) Then
                    Dim json = File.ReadAllText(stateFile)
                    Dim stateData = JsonConvert.DeserializeObject(Of StateData)(json)
                    Return stateData.SyncAttempt
                End If

                Return 0
            Catch ex As Exception
                Console.WriteLine($"⚠️ Get sync attempt error: {ex.Message}")
                Return 0
            End Try
        End Function
    End Class

    Public Class StateData
        Public Property SensorId As String
        Public Property LastUpdated As String
        Public Property SyncAttempt As Integer
    End Class

    Public Class AlertData
        Public Property alert As List(Of AlertEntry)
    End Class

    Public Class AlertEntry
        Public Property timestamp As String
        Public Property level As String
        Public Property message As String
    End Class
End Namespace