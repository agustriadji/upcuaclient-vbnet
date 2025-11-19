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

                Dim checkSql = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{database}'"
                Using checkCmd As New SqlCommand(checkSql, connection)
                    Dim exists = CInt(Await checkCmd.ExecuteScalarAsync()) > 0

                    If Not exists Then
                        Dim createSql = $"CREATE DATABASE [{database}]"
                        Using createCmd As New SqlCommand(createSql, connection)
                            Await createCmd.ExecuteNonQueryAsync()
                        End Using
                    End If
                End Using
            End Using

            Return True
        Catch
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
                    Console.WriteLine($"📋 record_metadata table exists: {tableExists}")

                    If tableExists Then
                        Console.WriteLine($"✅ Schema already exists, skipping creation")
                        Return True
                    End If
                End Using

                ' Create tables manually in correct order
                Console.WriteLine($"📋 Creating tables manually...")

                ' Create sensor_data table
                Dim createSensorData = "
                CREATE TABLE sensor_data (
                    id INT IDENTITY(1,1) PRIMARY KEY,
                    node_id NVARCHAR(100) NOT NULL,
                    sensor_type NVARCHAR(50) NOT NULL,
                    value FLOAT NOT NULL,
                    data_type NVARCHAR(50) NOT NULL,
                    status NVARCHAR(20) NOT NULL,
                    sync_status NVARCHAR(20) NOT NULL,
                    timestamp DATETIME2 NOT NULL DEFAULT GETDATE()
                )"

                ' Create sensor_alerts table
                Dim createSensorAlerts = "
                CREATE TABLE sensor_alerts (
                    id INT IDENTITY(1,1) PRIMARY KEY,
                    node_id NVARCHAR(100) NOT NULL,
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
                    end_date DATETIME2 NOT NULL
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
                        Console.WriteLine($"✅ Table {tableNames(i)} created successfully")
                    Catch ex As SqlException
                        If ex.Message.Contains("already exists") OrElse ex.Message.Contains("There is already an object") Then
                            Console.WriteLine($"⚠️ Table {tableNames(i)} already exists")
                        Else
                            Console.WriteLine($"❌ Error creating table {tableNames(i)}: {ex.Message}")
                            Return False
                        End If
                    End Try
                Next

                ' Verify table creation
                Using verifyCmd As New SqlCommand(checkTableSql, connection)
                    Dim tableCreated = CInt(Await verifyCmd.ExecuteScalarAsync()) > 0
                    Console.WriteLine($"✅ Schema execution completed. record_metadata table exists: {tableCreated}")
                    Return tableCreated
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine($"❌ ExecuteSchema error: {ex.Message}")
            Return False
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