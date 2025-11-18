Imports System
Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class InterfaceData
    Private sqlite As New SQLiteManager()
    Private analytics As New AnalyticsManager()

    Public Sub ProcessSensorData(sensorData As InterfaceSensorData)
        Console.WriteLine($"🔄 InterfaceData.ProcessSensorData: {sensorData.NodeId} = {sensorData.Value}")
        
        ' Simpan ke SQLite
        If sqlite.InsertSensorData(sensorData) Then
            Console.WriteLine($"✅ SensorData saved: {sensorData.NodeId}")
            sqlite.LogEvent("Info", "InterfaceData", $"SensorData saved: {sensorData.NodeId}")
        Else
            Console.WriteLine($"❌ Failed to save SensorData: {sensorData.NodeId}")
            sqlite.LogEvent("Error", "InterfaceData", $"Failed to save SensorData: {sensorData.NodeId}")
        End If

        ' Kirim ke AnalyticsManager
        'analytics.ReceiveSensorData(sensorData)
    End Sub

    Public Sub ProcessAlert(alert As InterfaceAlertData)
        If sqlite.InsertAlert(alert) Then
            sqlite.LogEvent("Warning", "InterfaceData", $"Alert saved: {alert.NodeId} - {alert.Message}")
        Else
            sqlite.LogEvent("Error", "InterfaceData", $"Failed to save Alert: {alert.NodeId}")
        End If

        'analytics.ReceiveAlert(alert)
    End Sub

    Public Sub ProcessBatchMetadata(entry As InterfaceRecordMetadata)
        If sqlite.InsertOrUpdateRecordMetadata(entry) Then
            sqlite.LogEvent("Info", "InterfaceData", $"Batch metadata saved: {entry.BatchId}")
        End If
    End Sub
End Class