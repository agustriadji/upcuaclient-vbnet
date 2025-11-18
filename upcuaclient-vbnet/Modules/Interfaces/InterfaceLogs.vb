Public Class InterfaceLogs
    Property EventType As String ' e.g., "Info", "Warning", "Error" ,"OK"
    Property Source As String ' e.g., "Database", "OPCServer"
    Property Details As String ' detailed message about the event
    Property Timestamp As DateTime
End Class
