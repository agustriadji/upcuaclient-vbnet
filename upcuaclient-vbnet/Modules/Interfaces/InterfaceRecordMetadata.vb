Public Class InterfaceRecordMetadata
    Property BatchId As String
    Property PressureTireId As String ' ID of the pressure tire sensor
    Property PressureGaugeId As String ' ID of the pressure gauge sensor
    Property Size As Integer ' size of Tire
    Property CreatedBy As String ' e.g., "Operator"
    Property Status As String ' e.g., "Recording", "Idle", "Offline", "Error"
    Property SyncStatus As String ' e.g., "Synchronized", "NotSynchronized" status sinkronisasi data ke database
    Property StartDate As DateTime ' start date of the recording
    Property EndDate As DateTime ' end date of the recording
    Property EndRecordingDate As DateTime? ' auto end recording date (nullable)
End Class
