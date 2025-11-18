Imports Microsoft.Extensions.Logging.Abstractions
Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class DataViewManager
    Private sqlite As New SQLiteManager()

    Public Function GetSensorData(sensorId As String, dates As String) As List(Of InterfaceSensorData)
        Return sqlite.QueryDaily(sensorId, dates)
    End Function

    Public Function GetAlerts(sensorId As String, dates As String) As List(Of AlertData)
        Return sqlite.QueryAlerts(sensorId, dates)
    End Function

    Public Function GetBatchInfo(batchId As String) As InterfaceRecordMetadata
        Return sqlite.QueryRecordMetadata(batchId)
    End Function

    Public Function GetLogs(eventType As String, dates As String) As List(Of InterfaceLogs)
        Return sqlite.QueryLogs(eventType, dates)
    End Function

    Public Sub CheckDatabaseData()
        Try
            ' Check total sensor data count
            Dim totalCount = sqlite.GetTotalSensorDataCount()
            Console.WriteLine($"📈 Total sensor data records: {totalCount}")
            
            ' Check recent data (last 10 records)
            Dim recentData = sqlite.GetRecentSensorData(10)
            Console.WriteLine($"🕰️ Recent {recentData.Count} records:")
            
            For Each sensorData In recentData
                Console.WriteLine($"  {sensorData.Timestamp:HH:mm:ss} | {sensorData.NodeId} | {sensorData.Value:F2} PSI | {sensorData.Status}")
            Next
            
        Catch ex As Exception
            Console.WriteLine($"⚠️ Check database error: {ex.Message}")
        End Try
    End Sub
End Class