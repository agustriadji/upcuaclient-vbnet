Public Class InterfaceAlertData
    Property NodeId As String ' NodeId of pressureTire or pressureGauge sensor
    Property SensorType As String ' e.g., "SensorPressureTireMetadata" or "SensorPressureGaugeMetadata"
    Property Message As String
    Property Threshold As Double   ' the threshold that triggered the alert
    Property CurrentValue As Double ' the current value that caused the alert
    Property Severity As String    ' e.g., "Low", "Medium", "High", "Critical"
    Property Timestamp As DateTime
End Class
