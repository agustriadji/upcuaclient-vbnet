Imports BitFaster.Caching

Public Class InterfaceSensorData
    Property NodeId As String ' NodeId of pressureTire or pressureGauge sensor
    Property NodeText As String = String.Empty ' Optional NodeText - default empty string
    Property SensorType As String ' e.g., "SensorPressureTireMetadata" or "SensorPressureGaugeMetadata"
    Property Value As Double
    Property DataType As String
    Property Status As String ' e.g., "Good", "Bad", "Uncertain" value yang di sediakan oleh opc
    Property SyncStatus As String ' e.g., "Synchronized", "NotSynchronized" status sinkronisasi data ke database
    Property Timestamp As DateTime
End Class
