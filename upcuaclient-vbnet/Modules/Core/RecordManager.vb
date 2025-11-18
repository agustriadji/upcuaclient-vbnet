Namespace upcuaclient_vbnet
    Public Class RecordManager

        Public Shared Sub EndRecord(sensorId As Integer)
            Try
                Console.WriteLine($"🔚 End record for sensor{sensorId}")

                ' Get current sync attempt and increment
                Dim currentAttempt = TempManager.GetSyncAttempt(sensorId)
                Dim newAttempt = currentAttempt + 1

                ' Update state with new sync attempt
                TempManager.UpdateState(sensorId, newAttempt)

                ' Add alert for end record
                TempManager.AddAlert(sensorId, "INFO", "Recording ended, data sync initiated")

                ' Sync analytics files to database
                ' SyncSensorToDatabase(sensorId)

                Console.WriteLine($"✅ End record completed for sensor{sensorId}")

            Catch ex As Exception
                Console.WriteLine($"⚠️ End record error: {ex.Message}")
            End Try
        End Sub

        Public Shared Sub NewRecord(sensorId As Integer)
            Try
                Console.WriteLine($"🆕 New record for sensor{sensorId}")

                ' Delete old analytics files and create new
                AnalyticsManager.NewRecord(sensorId)

                ' Reset state
                TempManager.UpdateState(sensorId, 0)

                ' Add alert for new record
                TempManager.AddAlert(sensorId, "INFO", "New recording session started")

                Console.WriteLine($"✅ New record created for sensor{sensorId}")

            Catch ex As Exception
                Console.WriteLine($"⚠️ New record error: {ex.Message}")
            End Try
        End Sub

    End Class
End Namespace