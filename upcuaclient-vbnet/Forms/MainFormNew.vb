Imports System.IO
Imports Newtonsoft.Json
Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class MainFormNew
    Private configManager As ConfigManager
    Private refreshTimerTabPageRecording As New Timer() With {.Interval = 2000}
    Private refreshTimerTabPageSensor As New Timer() With {.Interval = 2000}
    Private connectionIndicatorTimer As New Timer() With {.Interval = 3000}
    Private isDialogOpen As Boolean = False
    Private lastClickTime As DateTime = DateTime.MinValue
    Private isDeleting As Boolean = False

    Sub InitializeTimers()
        ' Use My.Settings instead of config file
        SettingsManager.InitializeDefaults()
        refreshTimerTabPageRecording.Interval = My.Settings.intervalTime
        refreshTimerTabPageSensor.Interval = My.Settings.intervalRefreshMain


        AddHandler refreshTimerTabPageRecording.Tick, AddressOf RefreshDataTabPageRecording
        AddHandler refreshTimerTabPageSensor.Tick, AddressOf RefreshDataTabPageSensor
    End Sub

    Public Sub New()
        InitializeComponent()
        Me.AutoScaleMode = AutoScaleMode.None
        Me.PerformAutoScale()
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
        Try
            ' Add delay to ensure form is fully initialized
            Application.DoEvents()
            Threading.Thread.Sleep(100)

            ' Setup table sensor state
            If DataGridView1 IsNot Nothing Then
                DataGridView1.SuspendLayout()
                DataGridView1.Columns.Clear()
                DataGridView1.Columns.Add("No", "No")
                DataGridView1.Columns.Add("SensorId", "Sensor ID")
                DataGridView1.Columns.Add("Status", "Status")
                If DataGridView1.Columns.Count > 0 Then
                    DataGridView1.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                End If
                DataGridView1.ResumeLayout()
            End If

            ' Setup table sensor recording
            If DGVRecording IsNot Nothing Then
                DGVRecording.SuspendLayout()
                DGVRecording.Columns.Clear()
                DGVRecording.Columns.Add("No", "No")
                DGVRecording.Columns.Add("BatchId", "Batch")
                DGVRecording.Columns.Add("SensorId", "Sensor ID")
                DGVRecording.Columns.Add("RunningDay", "Running Days")
                DGVRecording.Columns.Add("Size", "Size")
                DGVRecording.Columns.Add("State", "Status")
                DGVRecording.Columns.Add("CreatedAt", "Start Date")
                DGVRecording.Columns.Add("UpdatedAt", "Last Updated")
                Dim deleteColumn As New DataGridViewTextBoxColumn()
                deleteColumn.Name = "Delete"
                deleteColumn.HeaderText = "Action"
                deleteColumn.ReadOnly = True
                DGVRecording.Columns.Add(deleteColumn)
                If DGVRecording.Columns.Count > 0 Then
                    DGVRecording.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                End If
                DGVRecording.ResumeLayout()
            End If

            If TabControlMain IsNot Nothing AndAlso TabPageSensorState IsNot Nothing Then
                TabControlMain.SelectedTab = TabPageSensorState
            End If

            InitializeTimers()

            If refreshTimerTabPageSensor IsNot Nothing Then
                TimeManager.StartTimerWithInitialFetch(refreshTimerTabPageSensor, AddressOf RefreshDataTabPageSensor)
            End If

            ' Start UI indicator timer only (BackgroundWorker handles connection checking)
            If connectionIndicatorTimer IsNot Nothing Then
                AddHandler connectionIndicatorTimer.Tick, AddressOf UpdateConnectionIndicators
                connectionIndicatorTimer.Start()
            End If
            UpdateConnectionIndicators()

            ' Initialize logger and alert system
            AddHandler LoggerDebug.LogMessage, AddressOf OnLogMessage
            InitializeDebugPanel()
            InitializeAlertSystem()
        Catch ex As Exception
            Console.WriteLine($"Stack: {ex.StackTrace}")
            Try
                MessageBox.Show($"Error loading form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Catch
                ' Ignore message box errors
            End Try
        End Try
    End Sub

    Private Sub RefreshDataTabPageSensor(sender As Object, e As EventArgs)
        Dim currentFirstDisplayedScrollingRowIndex = -1
        Try
            If DataGridView1 Is Nothing Then Return

            ' Save current scroll position
            If DataGridView1.FirstDisplayedScrollingRowIndex >= 0 Then
                currentFirstDisplayedScrollingRowIndex = DataGridView1.FirstDisplayedScrollingRowIndex
            End If

            DataGridView1.SuspendLayout()
            DataGridView1.Rows.Clear()
            Dim count As Integer = 1

            ' Read from selectedNodeSensor settings
            Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()
            'Console.WriteLine($"🔍 Available keys: {String.Join(", ", selectedNodeSensor.Keys)}")


            ' Display PressureTire sensors
            If selectedNodeSensor.ContainsKey("PressureTire") Then
                Dim tireSensors = selectedNodeSensor("PressureTire")
                For Each sensor In tireSensors
                    If sensor.ContainsKey("NodeActive") AndAlso sensor("NodeActive").ToLower() = "true" Then
                        Dim status = If(sensor.ContainsKey("NodeStatus"), sensor("NodeStatus"), "ready")
                        Try
                            Dim idx = DataGridView1.Rows.Add(count.ToString(), "PressureTire." + sensor("NodeText"), status)
                            If idx >= 0 AndAlso idx < DataGridView1.Rows.Count Then
                                Dim row = DataGridView1.Rows(idx)
                                If row IsNot Nothing AndAlso row.Cells.Count > 2 AndAlso row.Cells("Status") IsNot Nothing Then
                                    Dim statusCell = row.Cells("Status")
                                    Select Case status.ToLower()
                                        Case "running"
                                            statusCell.Style.BackColor = Color.LightGreen
                                        Case "ready"
                                            statusCell.Style.BackColor = Color.LightYellow
                                    End Select
                                End If
                            End If
                        Catch
                            ' Skip problematic rows
                        End Try
                        count += 1
                    End If
                Next
            End If

            ' Display PressureGauge sensors  
            If selectedNodeSensor.ContainsKey("PressureGauge") Then
                Dim gaugeSensors = selectedNodeSensor("PressureGauge")
                For Each sensor In gaugeSensors
                    If sensor.ContainsKey("NodeActive") AndAlso sensor("NodeActive").ToLower() = "true" Then
                        Dim status = If(sensor.ContainsKey("NodeStatus"), sensor("NodeStatus"), "ready")
                        Try
                            Dim idx = DataGridView1.Rows.Add(count.ToString(), "PressureGauge." + sensor("NodeText"), status)
                            If idx >= 0 AndAlso idx < DataGridView1.Rows.Count Then
                                Dim row = DataGridView1.Rows(idx)
                                If row IsNot Nothing AndAlso row.Cells.Count > 2 AndAlso row.Cells("Status") IsNot Nothing Then
                                    Dim statusCell = row.Cells("Status")
                                    Select Case status.ToLower()
                                        Case "running"
                                            statusCell.Style.BackColor = Color.LightGreen
                                        Case "ready"
                                            statusCell.Style.BackColor = Color.LightYellow
                                    End Select
                                End If
                            End If
                        Catch
                            ' Skip problematic rows
                        End Try
                        count += 1
                    End If
                Next
            End If

            'Console.WriteLine($"✅ Displayed {count - 1} active sensors")
        Catch ex As Exception
            Console.WriteLine($"Gagal refresh data: {ex.Message}")
        Finally
            Try
                If DataGridView1 IsNot Nothing AndAlso Not DataGridView1.IsDisposed Then
                    DataGridView1.ResumeLayout()

                    ' Restore scroll position
                    If currentFirstDisplayedScrollingRowIndex >= 0 AndAlso currentFirstDisplayedScrollingRowIndex < DataGridView1.Rows.Count Then
                        DataGridView1.FirstDisplayedScrollingRowIndex = currentFirstDisplayedScrollingRowIndex
                    End If
                End If
            Catch
                ' Ignore layout errors
            End Try
        End Try
    End Sub

    Private Sub RefreshDataTabPageRecording(sender As Object, e As EventArgs)
        Dim currentFirstDisplayedScrollingRowIndex = -1
        Try
            If isDialogOpen OrElse isDeleting Then Return
            If DGVRecording Is Nothing OrElse DGVRecording.IsDisposed Then Return
            If Not DGVRecording.Enabled Then Return
            If DGVRecording.InvokeRequired Then
                DGVRecording.Invoke(Sub() RefreshDataTabPageRecording(sender, e))
                Return
            End If

            ' Save current scroll position
            If DGVRecording.FirstDisplayedScrollingRowIndex >= 0 Then
                currentFirstDisplayedScrollingRowIndex = DGVRecording.FirstDisplayedScrollingRowIndex
            End If

            DGVRecording.SuspendLayout()
            DGVRecording.Rows.Clear()
            Dim count As Integer = 1

            ' Read from SQLite database instead of JSON files
            Dim sqlite As New SQLiteManager()
            Dim endDate = DateTime.Now.AddDays(1)
            Dim startDate = DateTime.Now.AddDays(-30) ' Last 30 days
            Dim records = sqlite.QueryBatchRange(startDate, endDate)


            ' Get selectedNodeSensor for NodeText lookup
            Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

            For Each record In records
                Dim runningDays = Math.Ceiling((DateTime.Now - record.StartDate).TotalDays)
                Dim lastUpdated = record.EndDate.ToString("yyyy-MM-dd HH:mm")

                Dim startDateFormatted = record.StartDate.ToString("yyyy-MM-dd HH:mm")

                ' Get NodeText for PressureTire
                Dim tireNodeText = Helper.GetNodeTextFromId(selectedNodeSensor, "PressureTire", record.PressureTireId)

                Dim idx = DGVRecording.Rows.Add(
                    count.ToString(),
                    record.BatchId,
                    tireNodeText,
                    runningDays.ToString(),
                    record.Size.ToString(),
                    record.Status,
                    startDateFormatted,
                    lastUpdated
                )

                Dim row = DGVRecording.Rows(idx)
                ' Color code based on status
                Dim statusCell = row.Cells("State")
                Select Case record.Status.ToLower()
                    Case "recording"
                        statusCell.Style.BackColor = Color.LightGreen
                    Case "ready"
                        statusCell.Style.BackColor = Color.LightYellow
                End Select

                ' Show Delete button only for Finished status
                If record.Status.ToLower() = "finished" Or record.Status.ToLower() = "force stop" Then
                    row.Cells("Delete").Value = "Delete"
                    row.Cells("Delete").Style.BackColor = Color.LightCoral
                    row.Cells("Delete").Style.ForeColor = Color.DarkRed
                    row.Cells("Delete").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                Else
                    row.Cells("Delete").Value = ""
                    row.Cells("Delete").Style.BackColor = Color.LightGray
                End If

                count += 1
            Next
        Catch ex As Exception
            Console.WriteLine($"Gagal refresh data RefreshDataTabPageRecording: {ex.Message}")
        Finally
            Try
                If DGVRecording IsNot Nothing AndAlso Not DGVRecording.IsDisposed Then
                    DGVRecording.ResumeLayout()

                    ' Restore scroll position
                    If currentFirstDisplayedScrollingRowIndex >= 0 AndAlso currentFirstDisplayedScrollingRowIndex < DGVRecording.Rows.Count Then
                        DGVRecording.FirstDisplayedScrollingRowIndex = currentFirstDisplayedScrollingRowIndex
                    End If
                End If
            Catch
                ' Ignore layout errors
            End Try
        End Try
    End Sub

    Private Sub ConfigManagerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfigManagerToolStripMenuItem.Click
        Dim fromConfigManager As New FormConfigManager()
        fromConfigManager.ShowDialog()
    End Sub

    Private Sub CSVToolStripMenuItem_Click(sender As Object, e As EventArgs)

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



    Private Sub InitializeDebugPanel()
        ' Set default state - Debug active, Alert inactive
        ToolStripButtonDebug.BackColor = Color.LightBlue
        ToolStripButtonAlert.BackColor = Color.LightBlue

        ' Show debug, hide alert
        TextBoxOutputDebug.Visible = True
        TextBoxOutputAlert.Visible = False

        ' Setup Debug TextBox properties
        TextBoxOutputDebug.Font = New Font("Consolas", 9)
        TextBoxOutputDebug.BackColor = Color.DarkRed
        TextBoxOutputDebug.ForeColor = Color.White
        TextBoxOutputDebug.ReadOnly = True

        ' Setup Alert TextBox properties
        TextBoxOutputAlert.Font = New Font("Consolas", 9)
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

    Private Function GetNodeTextFromId(selectedNodeSensor As Dictionary(Of String, List(Of Dictionary(Of String, String))), sensorType As String, nodeId As String) As String
        Try
            If selectedNodeSensor.ContainsKey(sensorType) Then
                Dim sensors = selectedNodeSensor(sensorType)
                For Each sensor In sensors
                    If sensor.ContainsKey("NodeId") AndAlso sensor("NodeId") = nodeId Then
                        Return If(sensor.ContainsKey("NodeText"), sensor("NodeText"), nodeId)
                    End If
                Next
            End If
        Catch
            ' Return nodeId if lookup fails
        End Try
        Return nodeId
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
            Dim dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpcUaClient", "data", "sensor.db")
            Using conn As New Data.SQLite.SQLiteConnection($"Data Source={dbPath};Version=3;")
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
        Try
            If isDeleting Then Return
            If DGVRecording Is Nothing OrElse DGVRecording.IsDisposed Then Return
            If e Is Nothing OrElse e.RowIndex < 0 Then Return
            If DGVRecording.Rows Is Nothing OrElse e.RowIndex >= DGVRecording.Rows.Count Then Return

            Dim row = DGVRecording.Rows(e.RowIndex)
            If row Is Nothing OrElse row.IsNewRow Then Return

            ' Check if Delete button clicked
            If e.ColumnIndex = DGVRecording.Columns("Delete").Index Then
                Dim deleteValue = row.Cells("Delete").Value?.ToString()
                If String.IsNullOrEmpty(deleteValue) OrElse deleteValue <> "Delete" Then Return

                Dim batchId = row.Cells("BatchId").Value?.ToString()
                If String.IsNullOrEmpty(batchId) Then Return

                isDeleting = True
                refreshTimerTabPageRecording.Stop()

                Dim result = MessageBox.Show($"Delete record {batchId}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Try
                        Dim sqlite As New SQLiteManager()
                        sqlite.DeleteRecordMetadata(batchId)
                        RefreshDataTabPageRecording(Nothing, Nothing)
                        MessageBox.Show("Record deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        MessageBox.Show($"Delete failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If

                isDeleting = False
                refreshTimerTabPageRecording.Start()
                Return
            End If

            ' Prevent double-click for detail view
            If isDialogOpen Then Return
            Dim now = DateTime.Now
            If (now - lastClickTime).TotalMilliseconds < 500 Then Return
            lastClickTime = now

            Dim batchIdDetail = row.Cells("BatchId").Value?.ToString()
            If String.IsNullOrEmpty(batchIdDetail) Then Return

            isDialogOpen = True
            DGVRecording.Enabled = False

            Try
                If refreshTimerTabPageRecording IsNot Nothing Then refreshTimerTabPageRecording.Stop()
                If refreshTimerTabPageSensor IsNot Nothing Then refreshTimerTabPageSensor.Stop()
                If connectionIndicatorTimer IsNot Nothing Then connectionIndicatorTimer.Stop()

                Using detailForm As New DetailRecord(batchIdDetail)
                    detailForm.ShowDialog()
                End Using

                If TabControlMain IsNot Nothing AndAlso TabControlMain.SelectedTab Is TabPageRecording Then
                    If refreshTimerTabPageRecording IsNot Nothing Then refreshTimerTabPageRecording.Start()
                ElseIf TabControlMain IsNot Nothing AndAlso TabControlMain.SelectedTab Is TabPageSensorState Then
                    If refreshTimerTabPageSensor IsNot Nothing Then refreshTimerTabPageSensor.Start()
                End If
                If connectionIndicatorTimer IsNot Nothing Then connectionIndicatorTimer.Start()
            Finally
                isDialogOpen = False
                If DGVRecording IsNot Nothing AndAlso Not DGVRecording.IsDisposed Then
                    DGVRecording.Enabled = True
                End If
            End Try

        Catch ex As Exception
            isDialogOpen = False
            Try
                If DGVRecording IsNot Nothing AndAlso Not DGVRecording.IsDisposed Then DGVRecording.Enabled = True
                If refreshTimerTabPageRecording IsNot Nothing Then refreshTimerTabPageRecording.Start()
                If connectionIndicatorTimer IsNot Nothing Then connectionIndicatorTimer.Start()
            Catch
            End Try
        End Try
    End Sub

    Private Sub MenuStrip1_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles MenuStrip1.ItemClicked

    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Try
            Application.Exit()
        Catch
            Environment.Exit(0)
        End Try
    End Sub

    Private Sub ExportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportToolStripMenuItem.Click
        Try
            Using saveDialog As New SaveFileDialog()
                saveDialog.Filter = "JSON files (*.json)|*.json"
                saveDialog.FileName = $"settings_{DateTime.Now:yyyyMMdd_HHmmss}.json"

                If saveDialog.ShowDialog() = DialogResult.OK Then
                    Dim settings As New Dictionary(Of String, Object) From {
                        {"hostDB", My.Settings.hostDB},
                        {"hostOpc", My.Settings.hostOpc},
                        {"nodeIdOpc", My.Settings.nodeIdOpc},
                        {"selectedNodeIdOpc", My.Settings.selectedNodeIdOpc},
                        {"intervalTime", My.Settings.intervalTime},
                        {"intervalRefreshTimer", My.Settings.intervalRefreshTimer},
                        {"selectedNodeSensor", My.Settings.selectedNodeSensor},
                        {"intervalRefreshMain", My.Settings.intervalRefreshMain},
                        {"thresholdPressureGauge", My.Settings.thresholdPressureGauge}
                    }

                    File.WriteAllText(saveDialog.FileName, JsonConvert.SerializeObject(settings, Formatting.Indented))
                    MessageBox.Show("Settings exported successfully", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ImportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportToolStripMenuItem.Click
        Try
            Using openDialog As New OpenFileDialog()
                openDialog.Filter = "JSON files (*.json)|*.json"

                If openDialog.ShowDialog() = DialogResult.OK Then
                    Dim json = File.ReadAllText(openDialog.FileName)
                    Dim settings = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(json)

                    If settings.ContainsKey("hostDB") Then My.Settings.hostDB = settings("hostDB").ToString()
                    If settings.ContainsKey("hostOpc") Then My.Settings.hostOpc = settings("hostOpc").ToString()
                    If settings.ContainsKey("nodeIdOpc") Then My.Settings.nodeIdOpc = settings("nodeIdOpc").ToString()
                    If settings.ContainsKey("selectedNodeIdOpc") Then My.Settings.selectedNodeIdOpc = settings("selectedNodeIdOpc").ToString()
                    If settings.ContainsKey("intervalTime") Then My.Settings.intervalTime = Convert.ToInt32(settings("intervalTime"))
                    If settings.ContainsKey("intervalRefreshTimer") Then My.Settings.intervalRefreshTimer = Convert.ToInt32(settings("intervalRefreshTimer"))
                    If settings.ContainsKey("selectedNodeSensor") Then My.Settings.selectedNodeSensor = settings("selectedNodeSensor").ToString()
                    If settings.ContainsKey("intervalRefreshMain") Then My.Settings.intervalRefreshMain = Convert.ToInt32(settings("intervalRefreshMain"))
                    If settings.ContainsKey("thresholdPressureGauge") Then My.Settings.thresholdPressureGauge = Convert.ToDouble(settings("thresholdPressureGauge"))

                    My.Settings.Save()
                    MessageBox.Show("Settings imported successfully. Please restart the application.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Import failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AboutAirLMToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutAirLMToolStripMenuItem.Click
        Try
            Process.Start(New ProcessStartInfo("https://github.com/agustriadji/upcuaclient-vbnet/wiki") With {.UseShellExecute = True})
        Catch ex As Exception
            MessageBox.Show($"Failed to open URL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub FeedbackToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FeedbackToolStripMenuItem1.Click
        Try
            Process.Start(New ProcessStartInfo("mailto:agus.triadji@gmail.com") With {.UseShellExecute = True})
        Catch ex As Exception
            MessageBox.Show($"Failed to open email client: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DGVRecording_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles DGVRecording.CellPainting
        Try
            If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
            If DGVRecording Is Nothing OrElse DGVRecording.IsDisposed Then Return
            If DGVRecording.Columns.Count = 0 OrElse Not DGVRecording.Columns.Contains("Delete") Then Return
            If e.ColumnIndex <> DGVRecording.Columns("Delete").Index Then Return
            If e.RowIndex >= DGVRecording.Rows.Count Then Return

            Dim cellValue = DGVRecording.Rows(e.RowIndex).Cells("Delete").Value?.ToString()
            If String.IsNullOrEmpty(cellValue) Then
                e.Paint(e.CellBounds, DataGridViewPaintParts.All And Not DataGridViewPaintParts.ContentForeground)
                e.Handled = True
            End If
        Catch
            ' Ignore painting errors
        End Try
    End Sub

    Private Sub ButtonRefresh_Click(sender As Object, e As EventArgs) Handles ButtonRefresh.Click
        RefreshDataTabPageRecording(Nothing, Nothing)
    End Sub

End Class