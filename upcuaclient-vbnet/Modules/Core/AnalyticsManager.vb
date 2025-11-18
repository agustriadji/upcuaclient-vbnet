Imports System.IO
Imports Newtonsoft.Json
Imports System.Globalization

Namespace upcuaclient_vbnet
    Public Class AnalyticsManager
        Private Shared ReadOnly projectRoot As String = Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath))
        Private Shared ReadOnly analyticsPath As String = Path.Combine(projectRoot, "Analytics")

        Public Shared Sub SaveSensorMetadata(sensorId As Integer, pressure As Double)
            Try
                Dim sensorKey = $"sensor{sensorId}"
                Dim metadataPath = Path.Combine(analyticsPath, "analytic-sensor.json")

                If Not Directory.Exists(analyticsPath) Then
                    Directory.CreateDirectory(analyticsPath)
                End If

                Dim dict As Dictionary(Of String, InterfaceAnalyticsData)

                ' Load existing metadata dictionary
                If File.Exists(metadataPath) Then
                    Dim json = File.ReadAllText(metadataPath)
                    dict = JsonConvert.DeserializeObject(Of Dictionary(Of String, InterfaceAnalyticsData))(json)
                Else
                    dict = New Dictionary(Of String, InterfaceAnalyticsData)
                End If

                ' Create or update metadata
                If dict.ContainsKey(sensorKey) Then
                    Dim meta = dict(sensorKey)
                    meta.CurrentPressure = pressure
                    meta.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")

                    ' Update running day
                    Dim createdDate As DateTime
                    If DateTime.TryParseExact(meta.CreatedAt, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, createdDate) Then
                        Dim daysRunning = (DateTime.UtcNow.Date - createdDate.Date).Days + 1
                        meta.RunningDay = Math.Max(1, daysRunning)
                    End If

                    dict(sensorKey) = meta
                Else
                    dict(sensorKey) = New InterfaceAnalyticsData With {
                .BatchId = $"BATCH-1",
                .SensorId = sensorKey,
                .StartPressure = pressure,
                .CurrentPressure = pressure,
                .RunningDay = 1,
                .OperatorName = "Operator1",
                .Size = "Medium",
                .CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                .UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                .State = "Recording"
            }
                End If

                ' Save back to file
                Dim updatedJson = JsonConvert.SerializeObject(dict, Formatting.Indented)
                File.WriteAllText(metadataPath, updatedJson)

                Console.WriteLine($"📊 Metadata updated for {sensorKey}")

            Catch ex As Exception
                Console.WriteLine($"⚠️ Metadata save error: {ex.Message}")
            End Try
        End Sub

        Public Shared Sub SaveSensorDataPressure(sensorId As Integer, pressure As Double)
            Try
                Dim today = DateTime.UtcNow.ToString("yyyy-MM-dd")
                Dim sensorFolder = Path.Combine(analyticsPath, $"sensor{sensorId}")
                Dim fileName = $"sensor{sensorId}-{today}.json"
                Dim filePath = Path.Combine(sensorFolder, fileName)

                ' Ensure directory exists
                If Not Directory.Exists(sensorFolder) Then
                    Directory.CreateDirectory(sensorFolder)
                End If

                Dim analyticsDataPressure As List(Of InterfacePressureRecords) = New List(Of InterfacePressureRecords)

                ' Load existing data or create new
                If File.Exists(filePath) Then
                    Dim json = File.ReadAllText(filePath)

                    Try
                        Dim tempList = JsonConvert.DeserializeObject(Of List(Of InterfacePressureRecords))(json)
                        If tempList IsNot Nothing Then
                            analyticsDataPressure = tempList
                        Else
                            Console.WriteLine($"⚠️ File empty: {filePath}")
                            Return ' ⛔ Skip
                        End If
                    Catch ex As Exception
                        Console.WriteLine($"⚠️ File corrupt: {filePath} → {ex.Message}")
                        Return ' ⛔ Skip
                    End Try
                End If

                ' Add new reading
                analyticsDataPressure.Add(New InterfacePressureRecords With {
                    .Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    .Pressure = pressure
                })


                ' Save back to file
                Dim updatedJson = JsonConvert.SerializeObject(analyticsDataPressure, Formatting.Indented)
                File.WriteAllText(filePath, updatedJson)

                Console.WriteLine($"📊 Analytics Pressure saved: {fileName}")

            Catch ex As Exception
                Console.WriteLine($"⚠️ Analytics Pressure save error: {ex.Message}")
            End Try
        End Sub


        Public Shared Sub NewRecord(sensorId As Integer)
            Try
                Dim sensorFolder = Path.Combine(analyticsPath, $"sensor{sensorId}")

                ' Delete old files if exists
                If Directory.Exists(sensorFolder) Then
                    Directory.Delete(sensorFolder, True)
                    Console.WriteLine($"🗑️ Deleted old analytics for sensor{sensorId}")
                End If

                ' Create new folder
                Directory.CreateDirectory(sensorFolder)
                Console.WriteLine($"📁 Created new analytics folder for sensor{sensorId}")

            Catch ex As Exception
                Console.WriteLine($"⚠️ New record error: {ex.Message}")
            End Try
        End Sub
    End Class

End Namespace