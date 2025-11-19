Imports System.Data.SQLite
Imports System.IO
Imports Microsoft.Extensions.Logging.Abstractions
Imports upcuaclient_vbnet.upcuaclient_vbnet


Public Class SQLiteManager
    Private dbPath As String = Path.Combine(Application.StartupPath, "../../data/sensor.db")
    Private schemaPath As String = "Config/sqlite.sql"

    Public Sub New()
        ' Auto-initialize database on creation
        InitDatabase()
    End Sub

    Public Sub InitDatabase()
        Try
            ' Create data directory if not exists
            Dim dataDir = Path.GetDirectoryName(dbPath)
            If Not Directory.Exists(dataDir) Then
                Directory.CreateDirectory(dataDir)
                Console.WriteLine($"📁 Created directory: {dataDir}")
            End If

            ' Create database file if not exists
            If Not File.Exists(dbPath) Then
                SQLiteConnection.CreateFile(dbPath)
                Console.WriteLine($"🗄️ Created database: {dbPath}")
            End If

            ' Show full path
            Dim fullPath = IO.Path.GetFullPath(dbPath)
            Console.WriteLine($"📁 Database location: {fullPath}")
            Console.WriteLine($"📊 Database size: {New IO.FileInfo(fullPath).Length} bytes")
        Catch ex As Exception
            Console.WriteLine($"❌ Database init error: {ex.Message}")
            Return
        End Try

        ' Try different paths for schema file
        Dim sqlScript As String = ""
        Dim possiblePaths = New String() {
            schemaPath,
            Path.Combine("../../", schemaPath),
            Path.Combine(Application.StartupPath, "../../", schemaPath)
        }

        For Each path In possiblePaths
            If File.Exists(path) Then
                sqlScript = File.ReadAllText(path)
                Console.WriteLine($"📄 Found schema at: {path}")
                Exit For
            End If
        Next

        If String.IsNullOrEmpty(sqlScript) Then
            Console.WriteLine($"⚠️ Schema file not found. Checked paths:")
            For Each path In possiblePaths
                Console.WriteLine($"  - {IO.Path.GetFullPath(path)}")
            Next
            Return
        End If
        Dim statements As String() = sqlScript.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries)

        Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
            conn.Open()
            For Each stmt In statements
                Using cmd As New SQLiteCommand(stmt.Trim(), conn)
                    cmd.ExecuteNonQuery()
                End Using
            Next
        End Using
    End Sub

    Public Function InsertOrUpdateRecordMetadata(entry As InterfaceRecordMetadata) As Boolean
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()

                Dim query As String = "
                INSERT INTO record_metadata 
                (batch_id, pressure_tire_id, pressure_gauge_id, size, created_by, status, sync_status, start_date, end_date)
                VALUES 
                (@batch_id, @pressure_tire_id, @pressure_gauge_id, @size, @created_by, @status, @sync_status, @start_date, @end_date)
                ON CONFLICT(batch_id) DO UPDATE SET
                    pressure_tire_id = excluded.pressure_tire_id,
                    pressure_gauge_id = excluded.pressure_gauge_id,
                    size = excluded.size,
                    created_by = excluded.created_by,
                    status = excluded.status,
                    sync_status = excluded.sync_status,
                    start_date = excluded.start_date,
                    end_date = excluded.end_date
            "

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@batch_id", entry.BatchId)
                    cmd.Parameters.AddWithValue("@pressure_tire_id", entry.PressureTireId)
                    cmd.Parameters.AddWithValue("@pressure_gauge_id", entry.PressureGaugeId)
                    cmd.Parameters.AddWithValue("@size", entry.Size)
                    cmd.Parameters.AddWithValue("@created_by", entry.CreatedBy)
                    cmd.Parameters.AddWithValue("@status", entry.Status)
                    cmd.Parameters.AddWithValue("@sync_status", entry.SyncStatus)
                    cmd.Parameters.AddWithValue("@start_date", entry.StartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                    cmd.Parameters.AddWithValue("@end_date", entry.EndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))

                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Return True
        Catch ex As Exception
            Console.WriteLine("InsertOrUpdateRecordMetadata Error: " & ex.Message)
            Return False
        End Try
    End Function



    Public Function InsertSensorData(data As InterfaceSensorData) As Boolean
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query As String = "
                    INSERT INTO sensor_data 
                    (node_id, sensor_type, value, data_type, status, sync_status, timestamp) 
                    VALUES 
                    (@node_id, @sensor_type, @value, @data_type, @status, @sync_status, @timestamp)
                "

                Using transaction = conn.BeginTransaction()
                    Using cmd As New SQLiteCommand(query, conn, transaction)
                        cmd.Parameters.AddWithValue("@node_id", data.NodeId)
                        cmd.Parameters.AddWithValue("@sensor_type", data.SensorType)
                        cmd.Parameters.AddWithValue("@value", data.Value)
                        cmd.Parameters.AddWithValue("@data_type", data.DataType)
                        cmd.Parameters.AddWithValue("@status", data.Status)
                        cmd.Parameters.AddWithValue("@sync_status", data.SyncStatus)
                        cmd.Parameters.AddWithValue("@timestamp", data.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))

                        cmd.ExecuteNonQuery()
                        transaction.Commit()
                        Console.WriteLine($"💾 Data committed to disk: {data.NodeId}")
                    End Using
                End Using
            End Using
            Return True
        Catch ex As Exception
            ' Kamu bisa log ke system_logs di sini juga
            Console.WriteLine("InsertSensorData Error: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function InsertAlert(alert As InterfaceAlertData) As Boolean
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query As String = "
                INSERT INTO sensor_alerts 
                (node_id, sensor_type, message, threshold, current_value, severity, timestamp) 
                VALUES 
                (@node_id, @sensor_type, @message, @threshold, @current_value, @severity, @timestamp)
            "

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@node_id", alert.NodeId)
                    cmd.Parameters.AddWithValue("@sensor_type", alert.SensorType)
                    cmd.Parameters.AddWithValue("@message", alert.Message)
                    cmd.Parameters.AddWithValue("@threshold", alert.Threshold)
                    cmd.Parameters.AddWithValue("@current_value", alert.CurrentValue)
                    cmd.Parameters.AddWithValue("@severity", alert.Severity)
                    cmd.Parameters.AddWithValue("@timestamp", alert.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))

                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Return True
        Catch ex As Exception
            Console.WriteLine("InsertAlert Error: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function QueryDaily(sensorName As String, Optional dates As String = "") As List(Of InterfaceSensorData)
        Dim results As New List(Of InterfaceSensorData)
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query As String = "
                    SELECT node_id, sensor_type, value, data_type, status, sync_status, timestamp 
                    FROM sensor_data 
                    WHERE node_id = @node_id AND DATE(timestamp) = @date
                    ORDER BY timestamp ASC
                "

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@node_id", sensorName)
                    cmd.Parameters.AddWithValue("@date", dates)

                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim data As New InterfaceSensorData With {
                                .NodeId = reader("node_id").ToString(),
                                .SensorType = reader("sensor_type").ToString(),
                                .Value = Convert.ToDouble(reader("value")),
                                .DataType = reader("data_type").ToString(),
                                .Status = reader("status").ToString(),
                                .SyncStatus = reader("sync_status").ToString(),
                                .Timestamp = DateTime.Parse(reader("timestamp").ToString())
                            }
                            results.Add(data)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("QueryDaily Error: " & ex.Message)
        End Try
        Return results
    End Function
    Public Function LogEvent(eventType As String, source As String, details As String) As Boolean
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query As String = "
                INSERT INTO system_logs 
                (event_type, source, details, timestamp) 
                VALUES 
                (@event_type, @source, @details, @timestamp)
            "

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@event_type", eventType)
                    cmd.Parameters.AddWithValue("@source", source)
                    cmd.Parameters.AddWithValue("@details", details)
                    cmd.Parameters.AddWithValue("@timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))

                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Return True
        Catch ex As Exception
            Console.WriteLine("LogEvent Error: " & ex.Message)
            Return False
        End Try
    End Function
    Public Function QueryRecordMetadata(batchId As String) As InterfaceRecordMetadata
        Dim result As New InterfaceRecordMetadata()
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query As String = "
                SELECT * FROM record_metadata 
                WHERE batch_id = @batch_id
            "

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@batch_id", batchId)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            result.BatchId = reader("batch_id").ToString()
                            result.PressureTireId = reader("pressure_tire_id").ToString()
                            result.PressureGaugeId = reader("pressure_gauge_id").ToString()
                            result.Size = Convert.ToInt32(reader("size"))
                            result.CreatedBy = reader("created_by").ToString()
                            result.Status = reader("status").ToString()
                            result.SyncStatus = reader("sync_status").ToString()
                            result.StartDate = DateTime.Parse(reader("start_date").ToString())
                            result.EndDate = DateTime.Parse(reader("end_date").ToString())
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("QueryRecordMetadata Error: " & ex.Message)
        End Try
        Return result
    End Function

    Public Function QueryAlerts(sensorName As String, dates As String) As List(Of AlertData)
        Dim results As New List(Of AlertData)
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query As String = "
                    SELECT node_id, sensor_type, message, threshold, current_value, severity, timestamp 
                    FROM sensor_alerts 
                    WHERE node_id = @node_id AND DATE(timestamp) = @date
                    ORDER BY timestamp ASC
                "

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@node_id", sensorName)
                    cmd.Parameters.AddWithValue("@date", dates)

                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim alert As New InterfaceAlertData With {
                                .NodeId = reader("node_id").ToString(),
                                .SensorType = reader("sensor_type").ToString(),
                                .Message = reader("message").ToString(),
                                .Threshold = Convert.ToDouble(reader("threshold")),
                                .CurrentValue = Convert.ToDouble(reader("current_value")),
                                .Severity = reader("severity").ToString(),
                                .Timestamp = DateTime.Parse(reader("timestamp").ToString())
                            }
                            'results.Add(alert)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("QueryAlerts Error: " & ex.Message)
        End Try
        Return results
    End Function
    Public Function QueryLogs(eventType As String, dates As String) As List(Of InterfaceLogs)
        Dim logs As New List(Of InterfaceLogs)
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query As String = "
                    SELECT event_type, source, details, timestamp 
                    FROM system_logs 
                    WHERE event_type = @event_type AND DATE(timestamp) = @date
                    ORDER BY timestamp ASC
                "

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@event_type", eventType)
                    cmd.Parameters.AddWithValue("@date", dates)

                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim log As New InterfaceLogs With {
                                .EventType = reader("event_type").ToString(),
                                .Source = reader("source").ToString(),
                                .Details = reader("details").ToString(),
                                .Timestamp = DateTime.Parse(reader("timestamp").ToString())
                            }
                            logs.Add(log)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("QueryLogs Error: " & ex.Message)
        End Try
        Return logs
    End Function



    Public Function QueryBatchRange(startDate As DateTime, endDate As DateTime) As List(Of InterfaceRecordMetadata)
        Dim results As New List(Of InterfaceRecordMetadata)
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query As String = "
                SELECT rm.*,
                       CASE 
                           WHEN rm.status = 'Recording' THEN 
                               COALESCE(MAX(sd.timestamp), rm.end_date)
                           ELSE rm.end_date
                       END as actual_end_date
                FROM record_metadata rm
                LEFT JOIN sensor_data sd ON 
                    (sd.node_id = rm.pressure_tire_id AND sd.sensor_type IN ('pressureTire', 'PressureTire')) OR
                    (sd.node_id = rm.pressure_gauge_id AND sd.sensor_type IN ('pressureGuage', 'PressureGuage', 'pressureGauge', 'PressureGauge'))
                GROUP BY rm.batch_id
                ORDER BY rm.start_date DESC
            "
                Console.WriteLine("
                SELECT * FROM record_metadata 
                WHERE start_date >= @start AND end_date <= @end
                ORDER BY start_date ASC
            ")

                Using cmd As New SQLiteCommand(query, conn)
                    Console.WriteLine($"🔍 QueryBatchRange: Getting all records")

                    ' Debug: Check what sensor_data exists
                    Using debugCmd As New SQLiteCommand("SELECT DISTINCT node_id, sensor_type FROM sensor_data ORDER BY node_id", conn)
                        Using debugReader = debugCmd.ExecuteReader()
                            Console.WriteLine("🔍 Available sensor_data:")
                            While debugReader.Read()
                                Console.WriteLine($"  - NodeId: {debugReader("node_id")} | SensorType: '{debugReader("sensor_type")}'")
                            End While
                        End Using
                    End Using

                    Using reader = cmd.ExecuteReader()
                        Dim recordCount = 0
                        While reader.Read()
                            recordCount += 1
                            Console.WriteLine($"📄 Reading record #{recordCount}")
                            Try
                                Dim entry As New InterfaceRecordMetadata With {
                                    .BatchId = If(reader("batch_id") IsNot DBNull.Value, reader("batch_id").ToString(), ""),
                                    .PressureTireId = If(reader("pressure_tire_id") IsNot DBNull.Value, reader("pressure_tire_id").ToString(), ""),
                                    .PressureGaugeId = If(reader("pressure_gauge_id") IsNot DBNull.Value, reader("pressure_gauge_id").ToString(), ""),
                                    .Size = If(reader("size") IsNot DBNull.Value, Convert.ToInt32(reader("size")), 0),
                                    .CreatedBy = If(reader("created_by") IsNot DBNull.Value, reader("created_by").ToString(), ""),
                                    .Status = If(reader("status") IsNot DBNull.Value, reader("status").ToString(), ""),
                                    .SyncStatus = If(reader("sync_status") IsNot DBNull.Value, reader("sync_status").ToString(), ""),
                                    .StartDate = If(reader("start_date") IsNot DBNull.Value, DateTime.Parse(reader("start_date").ToString()), DateTime.MinValue),
                                    .EndDate = If(reader("actual_end_date") IsNot DBNull.Value, DateTime.Parse(reader("actual_end_date").ToString()), DateTime.MinValue)
                                }

                                Console.WriteLine($"  ✅ Added record: {entry.BatchId} | Status: {entry.Status}")
                                Console.WriteLine($"      Start: {entry.StartDate:yyyy-MM-dd HH:mm:ss}")
                                Console.WriteLine($"      End: {entry.EndDate:yyyy-MM-dd HH:mm:ss}")
                                Console.WriteLine($"      Raw actual_end_date: {reader("actual_end_date")}")
                                Console.WriteLine($"      Raw end_date: {reader("end_date")}")
                                results.Add(entry)
                            Catch readerEx As Exception
                                Console.WriteLine($"Error reading record row: {readerEx.Message}")
                                Continue While
                            End Try
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("QueryBatchRange Error: " & ex.Message)
        End Try
        Return results
    End Function

    Public Function GetTotalSensorDataCount() As Integer
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query = "SELECT COUNT(*) FROM sensor_data"
                Using cmd As New SQLiteCommand(query, conn)
                    Return Convert.ToInt32(cmd.ExecuteScalar())
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine($"GetTotalSensorDataCount Error: {ex.Message}")
            Return 0
        End Try
    End Function

    Public Function GetRecentSensorData(limit As Integer) As List(Of InterfaceSensorData)
        Dim results As New List(Of InterfaceSensorData)
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query = $"SELECT * FROM sensor_data ORDER BY timestamp DESC LIMIT {limit}"
                Using cmd As New SQLiteCommand(query, conn)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            results.Add(New InterfaceSensorData With {
                                .NodeId = reader("node_id").ToString(),
                                .SensorType = reader("sensor_type").ToString(),
                                .Value = Convert.ToDouble(reader("value")),
                                .DataType = reader("data_type").ToString(),
                                .Status = reader("status").ToString(),
                                .SyncStatus = reader("sync_status").ToString(),
                                .Timestamp = DateTime.Parse(reader("timestamp").ToString())
                            })
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine($"GetRecentSensorData Error: {ex.Message}")
        End Try
        Return results
    End Function

    Public Function GetSensorDataByNodeId(nodeId As String) As List(Of InterfaceSensorData)
        Dim results As New List(Of InterfaceSensorData)
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query As String = "
                    SELECT node_id, sensor_type, value, data_type, status, sync_status, timestamp 
                    FROM sensor_data 
                    WHERE node_id = @node_id
                    ORDER BY timestamp ASC
                "

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@node_id", nodeId)

                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim data As New InterfaceSensorData With {
                                .NodeId = reader("node_id").ToString(),
                                .SensorType = reader("sensor_type").ToString(),
                                .Value = Convert.ToDouble(reader("value")),
                                .DataType = reader("data_type").ToString(),
                                .Status = reader("status").ToString(),
                                .SyncStatus = reader("sync_status").ToString(),
                                .Timestamp = DateTime.Parse(reader("timestamp").ToString())
                            }
                            results.Add(data)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("GetSensorDataByNodeId Error: " & ex.Message)
        End Try
        Return results
    End Function

    Public Function GetLatestSensorData(nodeId As String, sensorType As String) As InterfaceSensorData
        Dim result As InterfaceSensorData = Nothing
        Try
            Using conn As New SQLiteConnection($"Data Source={dbPath};Version=3;")
                conn.Open()
                Dim query As String = "
                    SELECT node_id, sensor_type, value, data_type, status, sync_status, timestamp 
                    FROM sensor_data 
                    WHERE node_id = @node_id AND sensor_type = @sensor_type
                    ORDER BY timestamp DESC
                    LIMIT 1
                "

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@node_id", nodeId)
                    cmd.Parameters.AddWithValue("@sensor_type", sensorType)

                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            result = New InterfaceSensorData With {
                                .NodeId = reader("node_id").ToString(),
                                .SensorType = reader("sensor_type").ToString(),
                                .Value = Convert.ToDouble(reader("value")),
                                .DataType = reader("data_type").ToString(),
                                .Status = reader("status").ToString(),
                                .SyncStatus = reader("sync_status").ToString(),
                                .Timestamp = DateTime.Parse(reader("timestamp").ToString())
                            }
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("GetLatestSensorData Error: " & ex.Message)
        End Try
        Return result
    End Function
End Class