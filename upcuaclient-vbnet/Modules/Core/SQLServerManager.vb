Imports System.Data.SqlClient
Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class SQLServerManager
    Private connectionString As String

    Public Sub New()
        ' Setup connection string using SqlServerConnection pattern
        Dim parts = My.Settings.hostDB.Split(";"c)
        Dim server = ""
        Dim database = ""

        For Each part In parts
            If part.ToLower().Contains("server") AndAlso part.Contains("=") Then
                server = part.Split("="c)(1).Trim()
            ElseIf part.ToLower().Contains("database") AndAlso part.Contains("=") Then
                database = part.Split("="c)(1).Trim()
            End If
        Next

        SqlServerConnection.SetConnectionString(server, database)
        connectionString = $"Server={server};Database={database};Integrated Security=true;TrustServerCertificate=true;"
        
        ' Initialize database and schema
        InitializeDatabaseAndSchema(server, database)
    End Sub
    
    Private Sub InitializeDatabaseAndSchema(server As String, database As String)
        Try
            ' Create database if not exists
            Task.Run(Async Function() Await SqlServerConnection.CreateDatabaseIfNotExists(server, database)).Wait()
            
            ' Execute schema
            Task.Run(Async Function() Await SqlServerConnection.ExecuteSchema()).Wait()
            
            Console.WriteLine($"✅ SQL Server database and schema initialized: {database}")
        Catch ex As Exception
            Console.WriteLine($"⚠️ SQL Server initialization error: {ex.Message}")
        End Try
    End Sub

    Public Function ExportRecordData(batchId As String) As Boolean
        Try
            ' Get data from SQLite
            Dim sqlite As New SQLiteManager()
            Dim recordMetadata = sqlite.QueryRecordMetadata(batchId)

            If String.IsNullOrEmpty(recordMetadata.BatchId) Then
                Console.WriteLine("❌ Record metadata not found")
                Return False
            End If

            ' Export record_metadata
            If Not ExportRecordMetadata(recordMetadata) Then Return False

            ' Export sensor_data
            If Not ExportSensorData(recordMetadata.PressureTireId, recordMetadata.PressureGaugeId) Then Return False

            ' Export sensor_alerts
            If Not ExportSensorAlerts(recordMetadata.PressureTireId, recordMetadata.PressureGaugeId) Then Return False

            Console.WriteLine($"✅ Successfully exported batch: {batchId}")
            Return True

        Catch ex As Exception
            Console.WriteLine($"❌ Export error: {ex.Message}")
            Return False
        End Try
    End Function

    Private Function ExportRecordMetadata(record As InterfaceRecordMetadata) As Boolean
        Try
            Using conn As New SqlConnection(connectionString)
                conn.Open()

                Dim query = "
                    INSERT INTO record_metadata 
                    (batch_id, pressure_tire_id, pressure_gauge_id, size, created_by, status, sync_status, start_date, end_date, end_recording_date)
                    VALUES 
                    (@batch_id, @pressure_tire_id, @pressure_gauge_id, @size, @created_by, @status, @sync_status, @start_date, @end_date, @end_recording_date)
                "

                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@batch_id", record.BatchId)
                    cmd.Parameters.AddWithValue("@pressure_tire_id", record.PressureTireId)
                    cmd.Parameters.AddWithValue("@pressure_gauge_id", record.PressureGaugeId)
                    cmd.Parameters.AddWithValue("@size", record.Size)
                    cmd.Parameters.AddWithValue("@created_by", record.CreatedBy)
                    cmd.Parameters.AddWithValue("@status", record.Status)
                    cmd.Parameters.AddWithValue("@sync_status", record.SyncStatus)
                    cmd.Parameters.AddWithValue("@start_date", record.StartDate)
                    cmd.Parameters.AddWithValue("@end_date", record.EndDate)
                    cmd.Parameters.AddWithValue("@end_recording_date", If(record.EndRecordingDate.HasValue, record.EndRecordingDate.Value, DBNull.Value))

                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Return True
        Catch ex As Exception
            Console.WriteLine($"❌ Export record_metadata error: {ex.Message}")
            Return False
        End Try
    End Function

    Private Function ExportSensorData(pressureTireId As String, pressureGaugeId As String) As Boolean
        Try
            Dim sqlite As New SQLiteManager()
            Using sqliteConn As New Data.SQLite.SQLiteConnection($"Data Source={IO.Path.Combine(Application.StartupPath, "../../data/sensor.db")};Version=3;")
                sqliteConn.Open()

                Dim query = "SELECT * FROM sensor_data WHERE node_id IN (@tire_id, @gauge_id)"
                Using cmd As New Data.SQLite.SQLiteCommand(query, sqliteConn)
                    cmd.Parameters.AddWithValue("@tire_id", pressureTireId)
                    cmd.Parameters.AddWithValue("@gauge_id", pressureGaugeId)

                    Using reader = cmd.ExecuteReader()
                        Using sqlConn As New SqlConnection(connectionString)
                            sqlConn.Open()

                            While reader.Read()
                                Dim insertQuery = "
                                    INSERT INTO sensor_data 
                                    (node_id, sensor_type, value, data_type, status, sync_status, timestamp)
                                    VALUES 
                                    (@node_id, @sensor_type, @value, @data_type, @status, @sync_status, @timestamp)
                                "

                                Using insertCmd As New SqlCommand(insertQuery, sqlConn)
                                    insertCmd.Parameters.AddWithValue("@node_id", reader("node_id").ToString())
                                    insertCmd.Parameters.AddWithValue("@sensor_type", reader("sensor_type").ToString())
                                    insertCmd.Parameters.AddWithValue("@value", Convert.ToDouble(reader("value")))
                                    insertCmd.Parameters.AddWithValue("@data_type", reader("data_type").ToString())
                                    insertCmd.Parameters.AddWithValue("@status", reader("status").ToString())
                                    insertCmd.Parameters.AddWithValue("@sync_status", reader("sync_status").ToString())
                                    insertCmd.Parameters.AddWithValue("@timestamp", DateTime.Parse(reader("timestamp").ToString()))

                                    insertCmd.ExecuteNonQuery()
                                End Using
                            End While
                        End Using
                    End Using
                End Using
            End Using
            Return True
        Catch ex As Exception
            Console.WriteLine($"❌ Export sensor_data error: {ex.Message}")
            Return False
        End Try
    End Function

    Private Function ExportSensorAlerts(pressureTireId As String, pressureGaugeId As String) As Boolean
        Try
            Using sqliteConn As New Data.SQLite.SQLiteConnection($"Data Source={IO.Path.Combine(Application.StartupPath, "../../data/sensor.db")};Version=3;")
                sqliteConn.Open()

                Dim query = "SELECT * FROM sensor_alerts WHERE node_id IN (@tire_id, @gauge_id)"
                Using cmd As New Data.SQLite.SQLiteCommand(query, sqliteConn)
                    cmd.Parameters.AddWithValue("@tire_id", pressureTireId)
                    cmd.Parameters.AddWithValue("@gauge_id", pressureGaugeId)

                    Using reader = cmd.ExecuteReader()
                        Using sqlConn As New SqlConnection(connectionString)
                            sqlConn.Open()

                            While reader.Read()
                                Dim insertQuery = "
                                    INSERT INTO sensor_alerts 
                                    (node_id, sensor_type, message, threshold, current_value, severity, timestamp)
                                    VALUES 
                                    (@node_id, @sensor_type, @message, @threshold, @current_value, @severity, @timestamp)
                                "

                                Using insertCmd As New SqlCommand(insertQuery, sqlConn)
                                    insertCmd.Parameters.AddWithValue("@node_id", reader("node_id").ToString())
                                    insertCmd.Parameters.AddWithValue("@sensor_type", reader("sensor_type").ToString())
                                    insertCmd.Parameters.AddWithValue("@message", reader("message").ToString())
                                    insertCmd.Parameters.AddWithValue("@threshold", Convert.ToDouble(reader("threshold")))
                                    insertCmd.Parameters.AddWithValue("@current_value", Convert.ToDouble(reader("current_value")))
                                    insertCmd.Parameters.AddWithValue("@severity", reader("severity").ToString())
                                    insertCmd.Parameters.AddWithValue("@timestamp", DateTime.Parse(reader("timestamp").ToString()))

                                    insertCmd.ExecuteNonQuery()
                                End Using
                            End While
                        End Using
                    End Using
                End Using
            End Using
            Return True
        Catch ex As Exception
            Console.WriteLine($"❌ Export sensor_alerts error: {ex.Message}")
            Return False
        End Try
    End Function
End Class