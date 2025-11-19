Imports System.IO
Imports Newtonsoft.Json
Imports Org.BouncyCastle.Math.EC.ECCurve
Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class MainFormNew
    Private configManager As ConfigManager
    Private refreshTimerTabPageRecording As New Timer() With {.Interval = 2000}
    Private refreshTimerTabPageSensor As New Timer() With {.Interval = 2000}
    Private connectionIndicatorTimer As New Timer() With {.Interval = 5000}
    Private connectionMonitorTimer As New Timer() With {.Interval = 120000}

    Sub InitializeTimers()
        ' Use My.Settings instead of config file
        SettingsManager.InitializeDefaults()
        refreshTimerTabPageRecording.Interval = My.Settings.intervalTime
        refreshTimerTabPageSensor.Interval = My.Settings.intervalTime

        AddHandler refreshTimerTabPageRecording.Tick, AddressOf RefreshDataTabPageRecording
        AddHandler refreshTimerTabPageSensor.Tick, AddressOf RefreshDataTabPageSensor
    End Sub

    Private Sub TabControlMainFormSelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControlMain.SelectedIndexChanged
        refreshTimerTabPageRecording.Stop()
        refreshTimerTabPageSensor.Stop()

        If TabControlMain.SelectedTab Is TabPageSensorState Then
            TimeManager.StartTimerWithInitialFetch(refreshTimerTabPageSensor, AddressOf RefreshDataTabPageSensor)
        ElseIf TabControlMain.SelectedTab Is TabPageRecording Then
            TimeManager.StartTimerWithInitialFetch(refreshTimerTabPageRecording, AddressOf RefreshDataTabPageRecording)
        End If
    End Sub

    Private Sub MainFormNew_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Setup table sensor state
        DataGridView1.Columns.Clear()
        DataGridView1.Columns.Add("No", "No")
        DataGridView1.Columns.Add("SensorId", "Sensor ID")
        DataGridView1.Columns.Add("Status", "Status")

        ' Setup table sensor recording
        DGVRecording.Columns.Clear()
        DGVRecording.Columns.Add("No", "No")
        DGVRecording.Columns.Add("BatchId", "Batch")
        DGVRecording.Columns.Add("SensorId", "Sensor ID")
        DGVRecording.Columns.Add("RunningDay", "Running Days")
        DGVRecording.Columns.Add("Size", "Size")
        DGVRecording.Columns.Add("State", "Status")
        DGVRecording.Columns.Add("UpdatedAt", "Last Updated")

        TabControlMain.SelectedTab = TabPageSensorState
        InitializeTimers()
        TimeManager.StartTimerWithInitialFetch(refreshTimerTabPageSensor, AddressOf RefreshDataTabPageRecording)

        ' Start connection monitoring
        AddHandler connectionMonitorTimer.Tick, AddressOf CheckConnections
        connectionMonitorTimer.Start()
        CheckConnections()

        ' Start UI indicator timer
        AddHandler connectionIndicatorTimer.Tick, AddressOf UpdateConnectionIndicators
        connectionIndicatorTimer.Start()
        UpdateConnectionIndicators()

        ' Initialize logger and alert system
        AddHandler LoggerDebug.LogMessage, AddressOf OnLogMessage
        InitializeDebugPanel()
        InitializeAlertSystem()
    End Sub

    Private Sub RefreshDataTabPageSensor(sender As Object, e As EventArgs)
        Try
            DataGridView1.Rows.Clear()
            Dim count As Integer = 1

            ' Read from selectedNodeSensor settings
            Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()
            Console.WriteLine($"🔍 Available keys: {String.Join(", ", selectedNodeSensor.Keys)}")

            For Each key In selectedNodeSensor.Keys
                Console.WriteLine($"🔍 Key '{key}' has {selectedNodeSensor(key).Count} sensors")
            Next

            ' Display PressureTire sensors
            If selectedNodeSensor.ContainsKey("PressureTire") Then
                Dim tireSensors = selectedNodeSensor("PressureTire")
                For Each sensor In tireSensors
                    If sensor.ContainsKey("NodeActive") AndAlso sensor("NodeActive").ToLower() = "true" Then
                        Dim status = If(sensor.ContainsKey("NodeStatus"), sensor("NodeStatus"), "idle")
                        Dim idx = DataGridView1.Rows.Add(count.ToString(), sensor("NodeText"), status)
                        Dim row = DataGridView1.Rows(idx)

                        Dim statusCell = row.Cells("Status")
                        Select Case status.ToLower()
                            Case "running"
                                statusCell.Style.BackColor = Color.LightGreen
                            Case "idle"
                                statusCell.Style.BackColor = Color.LightYellow
                            Case "offline"
                                statusCell.Style.BackColor = Color.LightGray
                        End Select
                        count += 1
                    End If
                Next
            End If

            ' Display PressureGauge sensors  
            If selectedNodeSensor.ContainsKey("PressureGauge") Then
                Dim gaugeSensors = selectedNodeSensor("PressureGauge")
                For Each sensor In gaugeSensors
                    If sensor.ContainsKey("NodeActive") AndAlso sensor("NodeActive").ToLower() = "true" Then
                        Dim status = If(sensor.ContainsKey("NodeStatus"), sensor("NodeStatus"), "idle")
                        Dim idx = DataGridView1.Rows.Add(count.ToString(), sensor("NodeText"), status)
                        Dim row = DataGridView1.Rows(idx)

                        Dim statusCell = row.Cells("Status")
                        Select Case status.ToLower()
                            Case "running"
                                statusCell.Style.BackColor = Color.LightGreen
                            Case "idle"
                                statusCell.Style.BackColor = Color.LightYellow
                            Case "offline"
                                statusCell.Style.BackColor = Color.LightGray
                        End Select
                        count += 1
                    End If
                Next
            End If

            Console.WriteLine($"✅ Displayed {count - 1} active sensors")
        Catch ex As Exception
            Console.WriteLine($"Gagal refresh data: {ex.Message}")
        End Try
    End Sub

    Private Sub RefreshDataTabPageRecording(sender As Object, e As EventArgs)
        Try
            Console.WriteLine("🔄 RefreshDataTabPageRecording called")
            DGVRecording.Rows.Clear()
            Dim count As Integer = 1

            ' Read from SQLite database instead of JSON files
            Dim sqlite As New SQLiteManager()
            Dim endDate = DateTime.Now.AddDays(1)
            Dim startDate = DateTime.Now.AddDays(-30) ' Last 30 days
            Dim records = sqlite.QueryBatchRange(startDate, endDate)

            Console.WriteLine($"✅ Total records found: {records.Count}")

            For Each record In records
                Dim runningDays = Math.Ceiling((DateTime.Now - record.StartDate).TotalDays)
                Dim lastUpdated = record.EndDate.ToString("yyyy-MM-dd HH:mm")

                Dim idx = DGVRecording.Rows.Add(
                    count.ToString(),
                    record.BatchId,
                    $"{record.PressureTireId},{record.PressureGaugeId}",
                    runningDays.ToString(),
                    record.Size.ToString(),
                    record.Status,
                    lastUpdated
                )

                Dim row = DGVRecording.Rows(idx)
                ' Color code based on status
                Dim statusCell = row.Cells("State")
                Select Case record.Status.ToLower()
                    Case "recording"
                        statusCell.Style.BackColor = Color.LightGreen
                    Case "idle"
                        statusCell.Style.BackColor = Color.LightYellow
                    Case "offline"
                        statusCell.Style.BackColor = Color.LightGray
                End Select

                count += 1
            Next
        Catch ex As Exception
            Console.WriteLine($"Gagal refresh data RefreshDataTabPageRecording: {ex.Message}")
        End Try
    End Sub

    Private Sub ConfigManagerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfigManagerToolStripMenuItem.Click
        Dim fromConfigManager As New FormConfigManager()
        fromConfigManager.ShowDialog()
    End Sub

    Private Sub CSVToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CSVToolStripMenuItem.Click

    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs)

    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs)

    End Sub

    Private Sub ToolStripLabel1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBoxOutputDebug.TextChanged

    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        SplitContainer1.Panel2Collapsed = True
        SplitContainer1.Dock = DockStyle.Fill
    End Sub

    Private Sub LogToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogToolStripMenuItem.Click
        SplitContainer1.Panel2Collapsed = False
    End Sub

    Private Async Sub CheckConnections(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing)
        Try
            Console.WriteLine("Checking connections...")

            ' Check OPC Connection
            Dim opcConnected = Await OpcConnection.CheckHealthServer()
            My.Settings.stateConnectionOPC = opcConnected
            Console.WriteLine($"OPC Status: {opcConnected}")

            ' Setup database connection string first
            Dim parts = My.Settings.hostDB.Split(";"c)
            Dim server = "localhost\SQLEXPRESS"
            Dim database = "OpcUaClient"

            For Each part In parts
                If part.ToLower().Contains("server") AndAlso part.Contains("=") Then
                    server = part.Split("="c)(1).Trim()
                ElseIf part.ToLower().Contains("database") AndAlso part.Contains("=") Then
                    database = part.Split("="c)(1).Trim()
                End If
            Next

            SqlServerConnection.SetConnectionString(server, database)

            ' Check Database Connection
            Dim dbConnected = Await SqlServerConnection.CheckHealth()
            My.Settings.stateConnectionDB = dbConnected
            Console.WriteLine($"DB Status: {dbConnected}")

            ' Save settings
            My.Settings.Save()

        Catch ex As Exception
            Console.WriteLine($"Connection check error: {ex.Message}")
            My.Settings.stateConnectionOPC = False
            My.Settings.stateConnectionDB = False
            My.Settings.Save()
        End Try
    End Sub

    Private Sub InitializeDebugPanel()
        ' Set default state - Debug active, Alert inactive
        ToolStripButtonDebug.BackColor = Color.LightBlue
        ToolStripButtonAlert.BackColor = Color.Transparent

        ' Show debug, hide alert
        TextBoxOutputDebug.Visible = True
        TextBoxOutputAlert.Visible = False

        ' Setup Debug TextBox properties
        TextBoxOutputDebug.Font = New Font("Consolas", 8)
        TextBoxOutputDebug.BackColor = Color.Black
        TextBoxOutputDebug.ForeColor = Color.LimeGreen
        TextBoxOutputDebug.ReadOnly = True

        ' Setup Alert TextBox properties
        TextBoxOutputAlert.Font = New Font("Consolas", 8)
        TextBoxOutputAlert.BackColor = Color.DarkRed
        TextBoxOutputAlert.ForeColor = Color.White
        TextBoxOutputAlert.ReadOnly = True
    End Sub

    Private Sub OnLogMessage(message As String, level As LoggerDebug.LogLevel, source As String)
        Try
            If Me.InvokeRequired Then
                Me.Invoke(Sub() OnLogMessage(message, level, source))
                Return
            End If

            Dim timestamp = DateTime.Now.ToString("HH:mm:ss")
            Dim levelIcon = GetLevelIcon(level)
            Dim logEntry = $"[{timestamp}] {levelIcon} [{source}] {message}{Environment.NewLine}"

            TextBoxOutputDebug.AppendText(logEntry)
            TextBoxOutputDebug.SelectionStart = TextBoxOutputDebug.Text.Length
            TextBoxOutputDebug.ScrollToCaret()
        Catch ex As Exception
            ' Fallback to console if UI update fails
            Console.WriteLine($"Log: {message}")
        End Try
    End Sub

    Private Function GetLevelIcon(level As LoggerDebug.LogLevel) As String
        Select Case level
            Case LoggerDebug.LogLevel.Debug
                Return "DEBUG"
            Case LoggerDebug.LogLevel.Info
                Return "INFO"
            Case LoggerDebug.LogLevel.Warning
                Return "WARN"
            Case LoggerDebug.LogLevel.Error
                Return "ERROR"
            Case LoggerDebug.LogLevel.Success
                Return "OK"
            Case Else
                Return "LOG"
        End Select
    End Function

    Private Sub ToolStripButtonDebug_Click(sender As Object, e As EventArgs) Handles ToolStripButtonDebug.Click
        ' Activate debug mode
        ToolStripButtonDebug.BackColor = Color.LightBlue
        ToolStripButtonAlert.BackColor = Color.Transparent

        TextBoxOutputDebug.Visible = True
        TextBoxOutputAlert.Visible = False
    End Sub

    Private Sub ToolStripButtonAlert_Click(sender As Object, e As EventArgs) Handles ToolStripButtonAlert.Click
        ' Activate alert mode
        ToolStripButtonAlert.BackColor = Color.LightCoral
        ToolStripButtonDebug.BackColor = Color.Transparent

        TextBoxOutputDebug.Visible = False
        TextBoxOutputAlert.Visible = True

        ' Clear alert notification (orange color)
        ToolStripButtonAlert.BackColor = Color.LightCoral
    End Sub

    Private Sub UpdateConnectionIndicators(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing)
        Try
            ' Update OPC indicator
            If My.Settings.stateConnectionOPC Then
                ToolStripStatusLabelOPC.Image = My.Resources.server_green_16
                ToolStripStatusLabelOPC.Text = $"OPC-UA {My.Settings.hostOpc}"
            Else
                ToolStripStatusLabelOPC.Image = My.Resources.server_red_16
                ToolStripStatusLabelOPC.Text = "OPC-UA not connected"
            End If

            ' Update DB indicator
            If My.Settings.stateConnectionDB Then
                ToolStripStatusLabelDB.Image = My.Resources.database_green_16
                Dim parts = My.Settings.hostDB.Split(";"c)
                Dim serverName = "Database"
                Dim dbName = ""

                For Each part In parts
                    If part.ToLower().Contains("server") AndAlso part.Contains("=") Then
                        serverName = part.Split("="c)(1).Trim()
                    ElseIf part.ToLower().Contains("database") AndAlso part.Contains("=") Then
                        dbName = part.Split("="c)(1).Trim()
                    End If
                Next

                ToolStripStatusLabelDB.Text = $"Database {serverName}:{dbName}"
            Else
                ToolStripStatusLabelDB.Image = My.Resources.database_red_16
                ToolStripStatusLabelDB.Text = "Database not connected"
            End If
        Catch ex As Exception
            ToolStripStatusLabelOPC.Text = "OPC-UA status unknown"
            ToolStripStatusLabelDB.Text = "Database status unknown"
        End Try
    End Sub

    Private Sub AddNewSensorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddNewSensorToolStripMenuItem.Click
        Dim formConfigSensor As New FormConfigSensor()
        formConfigSensor.ShowDialog()
    End Sub

    Private Sub AddNewRecordingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddNewRecordingToolStripMenuItem.Click
        Dim formNewRecord As New FormNewRecord()
        formNewRecord.ShowDialog()
    End Sub

    Private Sub InitializeAlertSystem()
        ' Start alert monitoring - check every 30 seconds
        Dim alertTimer As New Timer() With {.Interval = 30000}
        AddHandler alertTimer.Tick, AddressOf CheckForAlerts
        alertTimer.Start()

        ' Load initial alerts
        LoadAlertsFromDatabase()
    End Sub

    Private Sub CheckForAlerts(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing)
        Try
            ' Load alerts from SQLite database
            LoadAlertsFromDatabase()

        Catch ex As Exception
            Console.WriteLine($"Alert check error: {ex.Message}")
        End Try
    End Sub

    Private lastAlertId As Integer = 0

    Private Sub LoadAlertsFromDatabase()
        Try
            Dim sqlite As New SQLiteManager()
            Using conn As New Data.SQLite.SQLiteConnection($"Data Source={IO.Path.Combine(Application.StartupPath, "../../data/sensor.db")};Version=3;")
                conn.Open()

                ' Optimized query - only get new alerts since last check
                Dim query As String = "
                    SELECT sa.id, sa.timestamp, COALESCE(mt.batch_id, 'N/A') as batch_id, 
                           sa.node_id, sa.message, sa.current_value, sa.severity
                    FROM sensor_alerts sa 
                    LEFT JOIN record_metadata mt ON (mt.pressure_gauge_id = sa.node_id OR mt.pressure_tire_id = sa.node_id)
                    WHERE sa.id > @lastId AND sa.timestamp >= datetime('now', '-1 day')
                    ORDER BY sa.id DESC
                    LIMIT 20
                "

                Using cmd As New Data.SQLite.SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@lastId", lastAlertId)

                    Using reader = cmd.ExecuteReader()
                        Dim newAlerts As New List(Of String)
                        While reader.Read()
                            Dim alertId = Convert.ToInt32(reader("id"))
                            If alertId > lastAlertId Then lastAlertId = alertId

                            Dim timestamp = DateTime.Parse(reader("timestamp").ToString()).ToString("HH:mm:ss")
                            Dim severityIcon = GetSeverityIcon(reader("severity").ToString())
                            Dim alertEntry = $"[{timestamp}] {severityIcon} [{reader("batch_id")}] {reader("node_id")} - {reader("message")} (Value: {reader("current_value")})"

                            newAlerts.Add(alertEntry)
                        End While

                        ' Update UI only if there are new alerts
                        If newAlerts.Count > 0 Then
                            UpdateAlertDisplay(newAlerts)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine($"LoadAlertsFromDatabase error: {ex.Message}")
        End Try
    End Sub

    Private Sub UpdateAlertDisplay(alerts As List(Of String))
        Try
            If Me.InvokeRequired Then
                Me.Invoke(Sub() UpdateAlertDisplay(alerts))
                Return
            End If

            For Each alert In alerts
                TextBoxOutputAlert.AppendText(alert & Environment.NewLine)
            Next

            TextBoxOutputAlert.SelectionStart = TextBoxOutputAlert.Text.Length
            TextBoxOutputAlert.ScrollToCaret()

            ' Show alert notification
            If Not TextBoxOutputAlert.Visible Then
                ToolStripButtonAlert.BackColor = Color.Orange
            End If
        Catch ex As Exception
            Console.WriteLine($"UpdateAlertDisplay error: {ex.Message}")
        End Try
    End Sub



    Private Function GetSeverityIcon(severity As String) As String
        Select Case severity.ToLower()
            Case "critical"
                Return "🔴"
            Case "warning"
                Return "🟡"
            Case "info"
                Return "🔵"
            Case Else
                Return "⚠️"
        End Select
    End Function

    Private Sub AddAlert(message As String, category As String)
        Try
            If Me.InvokeRequired Then
                Me.Invoke(Sub() AddAlert(message, category))
                Return
            End If

            Dim timestamp = DateTime.Now.ToString("HH:mm:ss")
            Dim alertEntry = $"[{timestamp}] ⚠️ [{category}] {message}{Environment.NewLine}"

            TextBoxOutputAlert.AppendText(alertEntry)
            TextBoxOutputAlert.SelectionStart = TextBoxOutputAlert.Text.Length
            TextBoxOutputAlert.ScrollToCaret()

            ' Update alert button to show there are new alerts
            If Not TextBoxOutputAlert.Visible Then
                ToolStripButtonAlert.BackColor = Color.Orange
            End If
        Catch ex As Exception
            Console.WriteLine($"Alert: {message}")
        End Try
    End Sub

    Private Sub DGVRecording_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGVRecording.CellClick
        If e.RowIndex >= 0 Then
            Dim batchId = DGVRecording.Rows(e.RowIndex).Cells("BatchId").Value.ToString()
            Dim detailForm As New DetailRecord(batchId)
            detailForm.ShowDialog()
        End If
    End Sub

    Private Sub MenuStrip1_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles MenuStrip1.ItemClicked

    End Sub
End Class