Public Class Helper

    Public Shared Function GetNodeTextFromId(selectedNodeSensor As Dictionary(Of String, List(Of Dictionary(Of String, String))), sensorType As String, nodeId As String) As String
        Try
            If selectedNodeSensor.ContainsKey(sensorType) Then
                Dim sensors = selectedNodeSensor(sensorType)
                For Each sensor In sensors
                    If sensor.ContainsKey("NodeId") AndAlso sensor("NodeId") = nodeId Then
                        Return If(sensor.ContainsKey("NodeText"), sensor("NodeText"), nodeId)
                    End If
                Next
            End If
        Catch
            ' Return nodeId if lookup fails
        End Try
        Return nodeId
    End Function

End Class
