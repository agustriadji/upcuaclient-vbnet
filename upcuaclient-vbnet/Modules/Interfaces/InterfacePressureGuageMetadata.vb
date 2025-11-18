Public Class InterfacePressureGaugeMetadata
    Property NodeId As String
    Property NodeText As String
    Property NodeType As String
    Property NodeParent As String
    Property DataType As String
    Property Unit As String ' e.g., "PSI", "Kpa"
    Property IsActive As Boolean ' Default true, indicates if the sensor is active and ready to use
    Property IsRunning As Boolean ' indicates if the sensor is currently operational, if false sensor ready to use
End Class
