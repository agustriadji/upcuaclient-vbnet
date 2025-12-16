Imports System.Data.SqlClient
Imports System.IO

Public Class SqlServerConnection
    Private Shared connectionString As String = ""

    Public Shared Sub SetConnectionString(server As String, database As String, Optional username As String = "", Optional password As String = "")
        If String.IsNullOrEmpty(username) Then
            connectionString = $"Server={server};Database={database};Integrated Security=true;TrustServerCertificate=true;"
        Else
            connectionString = $"Server={server};Database={database};User Id={username};Password={password};TrustServerCertificate=true;"
        End If
    End Sub

    Public Shared Async Function TestConnection() As Task(Of Boolean)
        Try
            Using connection As New SqlConnection(connectionString)
                Await connection.OpenAsync()
                Return True
            End Using
        Catch
            Return False
        End Try
    End Function

    Public Shared Async Function CreateDatabaseIfNotExists(server As String, database As String, Optional username As String = "", Optional password As String = "") As Task(Of Boolean)
        Try
            Dim masterConnString As String
            If String.IsNullOrEmpty(username) Then
                masterConnString = $"Server={server};Database=master;Integrated Security=true;TrustServerCertificate=true;"
            Else
                masterConnString = $"Server={server};Database=master;User Id={username};Password={password};TrustServerCertificate=true;"
            End If

            Using connection As New SqlConnection(masterConnString)
                Await connection.OpenAsync()

                ' Drop database if exists (to recreate clean)
                Dim checkSql = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{database}'"
                Using checkCmd As New SqlCommand(checkSql, connection)
                    Dim exists = CInt(Await checkCmd.ExecuteScalarAsync()) > 0
                    If Not exists Then
                        ' Create new database
                        Dim createSql = $"CREATE DATABASE [{database}]"
                        Using createCmd As New SqlCommand(createSql, connection)
                            Await createCmd.ExecuteNonQueryAsync()
                        End Using
                    Else
                        Console.WriteLine($"📋 Database {database} already exists")
                    End If
                End Using
            End Using

            Return True
        Catch ex As Exception
            Console.WriteLine($"❌ CreateDatabase error: {ex.Message}")
            Return False
        End Try
    End Function

    Public Shared Async Function ExecuteSchema() As Task(Of Boolean)
        Try
            Using connection As New SqlConnection(connectionString)
                Await connection.OpenAsync()
                Console.WriteLine($"🔗 Connected to SQL Server for schema execution")

                ' Check if record_metadata table exists
                Dim checkTableSql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'record_metadata'"
                Using checkCmd As New SqlCommand(checkTableSql, connection)
                    Dim tableExists = CInt(Await checkCmd.ExecuteScalarAsync()) > 0


                    If tableExists Then

                        ' Run database migrations for existing tables
                        Await RunDatabaseMigrations(connection)
                        Return True
                    End If
                End Using

                ' Create tables manually in correct order


                ' Create sensor_data table
                Dim createSensorData = "
                CREATE TABLE sensor_data (
                    id INT IDENTITY(1,1) PRIMARY KEY,
                    node_id NVARCHAR(100) NOT NULL,
                    node_text NVARCHAR(200) NULL,
                    sensor_type NVARCHAR(50) NOT NULL,
                    value FLOAT NOT NULL,
                    data_type NVARCHAR(50) NOT NULL,
                    status NVARCHAR(20) NOT NULL,
                    sync_status NVARCHAR(20) NOT NULL,
                    batch_id NVARCHAR(50) NULL,
                    timestamp DATETIME2 NOT NULL DEFAULT GETDATE()
                )"

                ' Create sensor_alerts table
                Dim createSensorAlerts = "
                CREATE TABLE sensor_alerts (
                    id INT IDENTITY(1,1) PRIMARY KEY,
                    node_id NVARCHAR(100) NOT NULL,
                    node_text NVARCHAR(200) NULL,
                    sensor_type NVARCHAR(50) NOT NULL,
                    message NVARCHAR(500) NOT NULL,
                    threshold FLOAT NOT NULL,
                    current_value FLOAT NOT NULL,
                    severity NVARCHAR(20) NOT NULL,
                    timestamp DATETIME2 NOT NULL DEFAULT GETDATE()
                )"

                ' Create record_metadata table
                Dim createRecordMetadata = "
                CREATE TABLE record_metadata (
                    batch_id NVARCHAR(50) PRIMARY KEY,
                    pressure_tire_id NVARCHAR(100) NOT NULL,
                    pressure_gauge_id NVARCHAR(100) NOT NULL,
                    size INT NOT NULL,
                    created_by NVARCHAR(100) NOT NULL,
                    status NVARCHAR(20) NOT NULL,
                    sync_status NVARCHAR(20) NOT NULL,
                    start_date DATETIME2 NOT NULL,
                    end_date DATETIME2 NOT NULL,
                    end_recording_date DATETIME2 NULL
                )"

                ' Create system_logs table
                Dim createSystemLogs = "
                CREATE TABLE system_logs (
                    id INT IDENTITY(1,1) PRIMARY KEY,
                    event_type NVARCHAR(50) NOT NULL,
                    source NVARCHAR(100) NOT NULL,
                    details NVARCHAR(MAX) NOT NULL,
                    timestamp DATETIME2 NOT NULL DEFAULT GETDATE()
                )"

                ' Execute table creation
                Dim tables() = {createSensorData, createSensorAlerts, createRecordMetadata, createSystemLogs}
                Dim tableNames() = {"sensor_data", "sensor_alerts", "record_metadata", "system_logs"}

                For i = 0 To tables.Length - 1
                    Try
                        Console.WriteLine($"🔧 Creating table: {tableNames(i)}")
                        Using command As New SqlCommand(tables(i), connection)
                            Await command.ExecuteNonQueryAsync()
                        End Using

                    Catch ex As SqlException
                        If ex.Message.Contains("already exists") OrElse ex.Message.Contains("There is already an object") Then
                            Console.WriteLine($"⚠️ Table {tableNames(i)} already exists")
                        Else
                            Console.WriteLine($"❌ Error creating table {tableNames(i)}: {ex.Message}")
                            Return False
                        End If
                    End Try
                Next

                ' Run database migrations for existing tables
                Await RunDatabaseMigrations(connection)
                
                ' Verify table creation
                Using verifyCmd As New SqlCommand(checkTableSql, connection)
                    Dim tableCreated = CInt(Await verifyCmd.ExecuteScalarAsync()) > 0
                    Return tableCreated
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine($"❌ ExecuteSchema error: {ex.Message}")
            Return False
        End Try
    End Function
    
    Private Shared Async Function RunDatabaseMigrations(connection As SqlConnection) As Task
        Try
            ' Check if end_recording_date column exists in record_metadata table
            Dim checkColumnQuery = "
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'record_metadata' 
                AND COLUMN_NAME = 'end_recording_date'
            "
            
            Using checkCmd As New SqlCommand(checkColumnQuery, connection)
                Dim columnExists = CInt(Await checkCmd.ExecuteScalarAsync()) > 0
                
                If Not columnExists Then
                    ' Add end_recording_date column to existing record_metadata table
                    Dim alterQuery = "ALTER TABLE record_metadata ADD end_recording_date DATETIME2 NULL"
                    Using alterCmd As New SqlCommand(alterQuery, connection)
                        Await alterCmd.ExecuteNonQueryAsync()
                    End Using
                End If
            End Using
            
            ' Check if batch_id column exists in sensor_data table
            Dim checkBatchIdQuery = "
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'sensor_data' 
                AND COLUMN_NAME = 'batch_id'
            "
            
            Using checkBatchCmd As New SqlCommand(checkBatchIdQuery, connection)
                Dim batchColumnExists = CInt(Await checkBatchCmd.ExecuteScalarAsync()) > 0
                
                If Not batchColumnExists Then
                    ' Add batch_id column to existing sensor_data table
                    Dim alterBatchQuery = "ALTER TABLE sensor_data ADD batch_id NVARCHAR(50) NULL"
                    Using alterBatchCmd As New SqlCommand(alterBatchQuery, connection)
                        Await alterBatchCmd.ExecuteNonQueryAsync()

                    End Using
                End If
            End Using
            
            ' Check if node_text column exists in sensor_data table
            Dim checkNodeTextQuery = "
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'sensor_data' 
                AND COLUMN_NAME = 'node_text'
            "
            
            Using checkNodeTextCmd As New SqlCommand(checkNodeTextQuery, connection)
                Dim nodeTextExists = CInt(Await checkNodeTextCmd.ExecuteScalarAsync()) > 0
                
                If Not nodeTextExists Then
                    Dim alterNodeTextQuery = "ALTER TABLE sensor_data ADD node_text NVARCHAR(200) NULL"
                    Using alterNodeTextCmd As New SqlCommand(alterNodeTextQuery, connection)
                        Await alterNodeTextCmd.ExecuteNonQueryAsync()

                    End Using
                End If
            End Using
            
            ' Check if node_text column exists in sensor_alerts table
            Dim checkAlertsNodeTextQuery = "
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'sensor_alerts' 
                AND COLUMN_NAME = 'node_text'
            "
            
            Using checkAlertsNodeTextCmd As New SqlCommand(checkAlertsNodeTextQuery, connection)
                Dim alertsNodeTextExists = CInt(Await checkAlertsNodeTextCmd.ExecuteScalarAsync()) > 0
                
                If Not alertsNodeTextExists Then
                    Dim alterAlertsNodeTextQuery = "ALTER TABLE sensor_alerts ADD node_text NVARCHAR(200) NULL"
                    Using alterAlertsNodeTextCmd As New SqlCommand(alterAlertsNodeTextQuery, connection)
                        Await alterAlertsNodeTextCmd.ExecuteNonQueryAsync()

                    End Using
                End If
            End Using
            
        Catch ex As Exception
            Console.WriteLine($"⚠️ SQL Server migration error: {ex.Message}")
        End Try
    End Function

    Public Shared Async Function InsertSensorData(nodeId As String, sensorType As String, value As Double, dataType As String, status As String) As Task(Of Boolean)
        Try
            Dim sql = "INSERT INTO sensor_data (node_id, sensor_type, value, data_type, status, sync_status) VALUES (@nodeId, @sensorType, @value, @dataType, @status, 'pending')"

            Using connection As New SqlConnection(connectionString)
                Await connection.OpenAsync()
                Using command As New SqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@nodeId", nodeId)
                    command.Parameters.AddWithValue("@sensorType", sensorType)
                    command.Parameters.AddWithValue("@value", value)
                    command.Parameters.AddWithValue("@dataType", dataType)
                    command.Parameters.AddWithValue("@status", status)

                    Await command.ExecuteNonQueryAsync()
                End Using
            End Using

            Return True
        Catch
            Return False
        End Try
    End Function

    Public Shared Async Function CheckHealth() As Task(Of Boolean)
        Try
            Using connection As New SqlConnection(connectionString)
                Await connection.OpenAsync()
                Using command As New SqlCommand("SELECT 1", connection)
                    Await command.ExecuteScalarAsync()
                End Using
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function

End Class