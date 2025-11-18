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
            Dim schemaPath = Path.Combine(Application.StartupPath, "Config", "sqlserver.sql")
            If Not File.Exists(schemaPath) Then
                Throw New FileNotFoundException("Schema file not found")
            End If
            
            Dim schema = File.ReadAllText(schemaPath)
            ' Split by CREATE TABLE statements
            Dim statements = New List(Of String)
            Dim lines = schema.Split({Environment.NewLine}, StringSplitOptions.None)
            Dim currentStatement = New List(Of String)
            
            For Each line In lines
                If line.Trim().StartsWith("CREATE TABLE") AndAlso currentStatement.Count > 0 Then
                    statements.Add(String.Join(Environment.NewLine, currentStatement))
                    currentStatement.Clear()
                End If
                
                If line.Trim().StartsWith("CREATE") OrElse currentStatement.Count > 0 Then
                    currentStatement.Add(line)
                    If line.Trim().EndsWith(";") Then
                        statements.Add(String.Join(Environment.NewLine, currentStatement))
                        currentStatement.Clear()
                    End If
                End If
            Next
            
            If currentStatement.Count > 0 Then
                statements.Add(String.Join(Environment.NewLine, currentStatement))
            End If
            
            Using connection As New SqlConnection(connectionString)
                Await connection.OpenAsync()
                
                For Each statement In statements
                    If Not String.IsNullOrWhiteSpace(statement) Then
                        Try
                            Using command As New SqlCommand(statement.Trim().TrimEnd(";"c), connection)
                                Await command.ExecuteNonQueryAsync()
                            End Using
                        Catch ex As SqlException
                            ' Ignore "already exists" errors
                            If Not ex.Message.Contains("already exists") AndAlso Not ex.Message.Contains("There is already an object") Then
                                Throw
                            End If
                        End Try
                    End If
                Next
            End Using
            
            Return True
        Catch
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