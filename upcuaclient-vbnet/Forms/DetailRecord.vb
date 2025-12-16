Imports System.IO
Imports LiveCharts
Imports LiveCharts.WinForms
Imports LiveCharts.Wpf
Imports Newtonsoft.Json
Imports Microsoft.Office.Interop
Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class DetailRecord
    Private sqlite As New SQLiteManager()
    Private sqlServer As New SQLServerManager()
    Private batchId As String
    Private recordMetadata As InterfaceRecordMetadata
    Private rawData As List(Of InterfacePressureRecords)
    Private rawDataRaw As List(Of InterfacePressureRecords) 'mentah
    Private gaugeData As List(Of InterfaceSensorData) 'PressureGauge data
    Private gaugeDataRaw As List(Of InterfacePressureRecords) 'gauge mentah
    Private gaugeDataProcessed As List(Of InterfacePressureRecords) 'gauge processed
    Private intervalAggregate As String = ""
    Private refreshTimerWatch As New Timer() With {.Interval = 3000}
    Private refreshTimerGraph As New Timer() With {.Interval = 3000}


    Public Sub New(batchIdParam As String)
        ' Always call InitializeComponent first
        InitializeComponent()

        ' Disable AutoScale to prevent layout issues
        Me.AutoScaleMode = AutoScaleMode.None
        Me.AutoSize = False

        If String.IsNullOrEmpty(batchIdParam) Then
            Try
                MessageBox.Show("⚠️ Batch ID not found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Catch
                ' Ignore message box errors
            End Try
            Return
        End If

        batchId = batchIdParam
    End Sub

    Public Sub InitializeTimers()
        ' Jika batch sudah finished, tidak perlu timer
        If recordMetadata IsNot Nothing AndAlso recordMetadata.Status.ToLower() = "finished" Then
            ' Console.WriteLine($"📋 Batch {recordMetadata.BatchId} is finished - timer disabled")
            Return
        End If

        refreshTimerWatch.Interval = My.Settings.intervalTime

        AddHandler refreshTimerWatch.Tick,
            Sub()
                RefreshRawData()

                If TabControlDetailRecord.SelectedTab Is TabPageRecord Then
                    LoadSensorPressureTable()
                ElseIf TabControlDetailRecord.SelectedTab Is TabPageGraph Then
                    LoadSensorPressureGraph()
                End If
            End Sub

    End Sub

    Private Sub TabControlDetailRecordSelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControlDetailRecord.SelectedIndexChanged
        refreshTimerWatch.Stop()
        RefreshRawData()

        ' Hanya start timer jika batch masih running
        If recordMetadata IsNot Nothing AndAlso recordMetadata.Status.ToLower() <> "finished" Then
            TimeManager.StartTimerWithInitialFetch(refreshTimerWatch,
            Sub()
                If TabControlDetailRecord.SelectedTab Is TabPageRecord Then
                    LoadSensorPressureTable()
                ElseIf TabControlDetailRecord.SelectedTab Is TabPageGraph Then
                    LoadSensorPressureGraph()
                End If
            End Sub)
        Else
            ' Untuk finished batch, langsung load data tanpa timer
            If TabControlDetailRecord.SelectedTab Is TabPageRecord Then
                LoadSensorPressureTable()
            ElseIf TabControlDetailRecord.SelectedTab Is TabPageGraph Then
                LoadSensorPressureGraph()
            End If
        End If

    End Sub


    Private Sub DetailRecord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Disable UI updates during initialization
            Me.SuspendLayout()
            
            ' Quick setup without heavy operations
            SetupGraph()
            LoadSensorMetadata()
            
            ' Set default ComboBox selection
            If CMBGroupingGraph?.Items.Count > 0 Then
                CMBGroupingGraph.SelectedIndex = 0
                intervalAggregate = ""
            End If

            ' Set default tab
            TabControlDetailRecord.SelectedTab = TabPageRecord
            
            ' Load data asynchronously
            Task.Run(Sub()
                RefreshRawData()
                Me.Invoke(Sub()
                    rawData = rawDataRaw
                    gaugeDataProcessed = gaugeDataRaw
                    InitializeTimers()
                    
                    ' Start timer only if batch is still running
                    If recordMetadata?.Status.ToLower() <> "finished" Then
                        TimeManager.StartTimerWithInitialFetch(refreshTimerWatch, Sub() LoadSensorPressureTable())
                    Else
                        LoadSensorPressureTable()
                    End If
                End Sub)
            End Sub)
            
        Catch ex As Exception
            MessageBox.Show($"Error loading detail record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Finally
            Me.ResumeLayout()
        End Try
    End Sub

    ' Init Graph
    Private Sub SetupGraph()
        CartesianChart2.Dock = DockStyle.Fill
        CartesianChart2.Pan = PanningOptions.X
        CartesianChart2.Series = New SeriesCollection From {
            New LineSeries With {
                .Title = "Pressure",
                .Values = New ChartValues(Of Double)(),
                .PointGeometry = DefaultGeometries.Circle,
                .PointGeometrySize = 8
            }
        }

        CartesianChart2.AxisX.Add(New Axis With {.Title = "Time", .Labels = New List(Of String)()})
        CartesianChart2.AxisY.Add(New Axis With {.Title = "Pressure (PSI)"})
    End Sub

    ' For TextBox
    Private Sub LoadSensorMetadata()
        recordMetadata = sqlite.QueryRecordMetadata(batchId)
        If recordMetadata Is Nothing OrElse String.IsNullOrEmpty(recordMetadata.BatchId) Then Return

        TextBoxBatchId.Text = recordMetadata.BatchId

        ' Get NodeText for sensor display in format [key_object].nodeText
        Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()
        Dim tireNodeText = Helper.GetNodeTextFromId(selectedNodeSensor, "PressureTire", recordMetadata.PressureTireId)

        TextBoxSensorId.Text = $"PressureTire.{tireNodeText}"

        TextBoxSize.Text = recordMetadata.Size.ToString()
        TextBoxOperator.Text = recordMetadata.CreatedBy
        ' Convert UTC to local time for display
        Dim localStartDate = If(recordMetadata.StartDate.Kind = DateTimeKind.Utc, recordMetadata.StartDate.ToLocalTime(), recordMetadata.StartDate)
        TextBoxStartDate.Text = localStartDate.ToString("yyyy-MM-dd HH:mm")
        TextBoxState.Text = recordMetadata.Status

        ' Update button states based on status
        UpdateButtonStates()

        ' Running days and start pressure will be calculated after RefreshRawData
        ' Console.WriteLine($"🔍 Metadata loaded for batch: {recordMetadata.BatchId}")
        ' Console.WriteLine($"🔍 Start date: {recordMetadata.StartDate}")
    End Sub

    Private Sub UpdateRunningDaysAndStartPressure()
        If recordMetadata Is Nothing OrElse rawDataRaw Is Nothing OrElse rawDataRaw.Count = 0 Then
            TextBoxRunningDay.Text = "0"
            TextBoxStartPressure.Text = "0.00"
            Return
        End If

        Try
            ' Get sorted data (earliest to latest)
            Dim sortedData = rawDataRaw.OrderBy(Function(d) DateTime.Parse(d.Timestamp)).ToList()

            ' Get first pressure (StartPressure) and last timestamp for running days
            Dim firstPressure = sortedData.First().Pressure
            Dim lastTimestamp = DateTime.Parse(sortedData.Last().Timestamp)

            ' Calculate running days using TotalDays and ceiling for accurate count
            Dim runningDays = Math.Ceiling((lastTimestamp - recordMetadata.StartDate).TotalDays)
            TextBoxRunningDay.Text = runningDays.ToString()
            TextBoxStartPressure.Text = firstPressure.ToString("F2")

            ' Console.WriteLine($"✅ Running days calculated: {runningDays} days")
            ' Console.WriteLine($"✅ Start pressure: {firstPressure}, Last timestamp: {lastTimestamp}")
        Catch ex As Exception
            TextBoxRunningDay.Text = "0"
            TextBoxStartPressure.Text = "0.00"
            ' Console.WriteLine($"❌ Error calculating running days: {ex.Message}")
        End Try
    End Sub

    ' For Graph
    Private Sub LoadSensorPressureGraph()
        Try
            If rawData Is Nothing OrElse rawData.Count = 0 Then Return
            If CartesianChart2 Is Nothing OrElse CartesianChart2.Series Is Nothing OrElse CartesianChart2.Series.Count = 0 Then Return

            ' Sort data ASC (oldest first) for graph
            Dim sortedData = rawData.OrderBy(Function(d)
                                                 Dim dt As DateTime
                                                 Return If(DateTime.TryParse(d.Timestamp, dt), dt, DateTime.MinValue)
                                             End Function).ToList()

            Dim lineSeries = DirectCast(CartesianChart2.Series(0), LineSeries)
            Dim currentValues = DirectCast(lineSeries.Values, ChartValues(Of Double))
            Dim currentLabels = DirectCast(CartesianChart2.AxisX(0).Labels, List(Of String))

            ' Get current count
            Dim currentCount = currentValues.Count
            Dim newDataCount = sortedData.Count

            ' Only append new data
            If newDataCount > currentCount Then
                For i As Integer = currentCount To newDataCount - 1
                    Dim DL = sortedData(i)
                    currentValues.Add(DL.Pressure)
                    currentLabels.Add(DL.Timestamp)
                Next
            ElseIf newDataCount < currentCount Then
                ' Data changed (aggregation changed), reload all
                currentValues.Clear()
                currentLabels.Clear()
                For Each DL In sortedData
                    currentValues.Add(DL.Pressure)
                    currentLabels.Add(DL.Timestamp)
                Next
            End If
        Catch ex As Exception
            Console.WriteLine($"LoadSensorPressureGraph Error: {ex.Message}")
        End Try
    End Sub
    ' For DGV - Show realtime data, not fixed values
    Private Sub LoadSensorPressureTable()
        Try
            If rawData Is Nothing OrElse rawData.Count = 0 Then Return
            If DGVWatch?.IsDisposed <> False Then Return
            
            If DGVWatch.InvokeRequired Then
                DGVWatch.Invoke(Sub() LoadSensorPressureTable())
                Return
            End If

            ' Check if data count changed to avoid unnecessary updates
            Dim newDataCount = rawData.Count
            If DGVWatch.Rows.Count = newDataCount Then Return

            DGVWatch.SuspendLayout()
            DGVWatch.Rows.Clear()
            DGVWatch.RowHeadersVisible = False

            Dim startPressure = TextBoxStartPressure.Text
            Dim sortedTireData = rawData.OrderByDescending(Function(d)
                                                               Dim dt As DateTime
                                                               Return If(DateTime.TryParse(d.Timestamp, dt), dt, DateTime.MinValue)
                                                           End Function).Take(100).ToList() ' Limit to 100 rows for performance

            For Each DL In sortedTireData
                Dim currentPressure = DL.Pressure.ToString("F2")
                Dim matchingGauge = gaugeDataProcessed?.FirstOrDefault(Function(g) g.Timestamp = DL.Timestamp)
                Dim leakPressure = If(matchingGauge?.Pressure, 0).ToString("F2")
                DGVWatch.Rows.Add(startPressure, currentPressure, leakPressure, DL.Timestamp)
            Next
            
        Catch ex As Exception
            AppLogger.LogError($"LoadSensorPressureTable Error: {ex.Message}", "DetailRecord")
        Finally
            DGVWatch?.ResumeLayout()
        End Try
    End Sub
    Private Sub RefreshRawData()
        If recordMetadata Is Nothing OrElse String.IsNullOrEmpty(recordMetadata.BatchId) Then Return

        Try
            ' Optimize connection string only once
            If Not My.Settings.hostDB.Contains("Integrated Security") Then
                My.Settings.hostDB += ";Integrated Security=true;TrustServerCertificate=true"
                My.Settings.Save()
            End If

            ' Load data based on batch status
            If recordMetadata.Status.ToLower() = "finished" Then
                LoadDataFromSQLServer()
            Else
                LoadDataFromSQLite()
            End If

            ' Apply aggregation if needed
            If Not String.IsNullOrEmpty(intervalAggregate) Then
                rawData = AggregatePressureData(rawDataRaw, intervalAggregate)
                gaugeDataProcessed = AggregatePressureData(gaugeDataRaw, intervalAggregate)
            End If

            ' Update UI on main thread
            If Me.InvokeRequired Then
                Me.Invoke(Sub() UpdateRunningDaysAndStartPressure())
            Else
                UpdateRunningDaysAndStartPressure()
            End If
            
        Catch ex As Exception
            AppLogger.LogError($"RefreshRawData Error: {ex.Message}", "DetailRecord")
        End Try
    End Sub

    Private Sub LoadDataFromSQLite()
        ' Get PressureTire data untuk chart dan DGV
        Dim tireSensorData = sqlite.GetSensorDataByNodeId(recordMetadata.PressureTireId)

        ' Get PressureGauge data untuk DGV LeakPressure column
        gaugeData = sqlite.GetSensorDataByNodeId(recordMetadata.PressureGaugeId)

        ' Convert ke format existing
        ConvertSensorDataToInterfaceRecords(tireSensorData, gaugeData)
    End Sub

    Private Sub LoadDataFromSQLServer()
        Try
            ' Get data dari SQL Server berdasarkan NodeId
            Dim tireSensorData = GetSensorDataFromSQLServer(recordMetadata.PressureTireId)
            Dim gaugeSensorData = GetSensorDataFromSQLServer(recordMetadata.PressureGaugeId)

            ' Convert ke format existing
            ConvertSensorDataToInterfaceRecords(tireSensorData, gaugeSensorData)

        Catch ex As Exception
            AppLogger.LogError($"Error loading from SQL Server: {ex.Message}", "DetailRecord")
            ' Fallback ke SQLite jika SQL Server gagal
            LoadDataFromSQLite()
        End Try
    End Sub


    Private Function GetSensorDataFromSQLServer(nodeId As String) As List(Of InterfaceSensorData)
        Dim sensorDataList As New List(Of InterfaceSensorData)
        Try
            Dim connectionString = My.Settings.hostDB
            If connectionString.Contains("Integrated Security=false") Then
                connectionString = connectionString.Replace("Integrated Security=false", "Integrated Security=true")
            End If
            If Not connectionString.Contains("TrustServerCertificate") Then
                connectionString += ";TrustServerCertificate=true"
            End If

            Using conn As New System.Data.SqlClient.SqlConnection(connectionString)
                conn.Open()
                
                Dim query = "SELECT node_id, sensor_type, value, timestamp FROM sensor_data WHERE node_id = @node_id AND batch_id = @batch_id ORDER BY timestamp"
                Using cmd As New System.Data.SqlClient.SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@node_id", nodeId)
                    cmd.Parameters.AddWithValue("@batch_id", recordMetadata.BatchId)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            sensorDataList.Add(New InterfaceSensorData With {
                                .NodeId = reader("node_id").ToString(),
                                .SensorType = reader("sensor_type").ToString(),
                                .Value = Convert.ToDouble(reader("value")),
                                .Timestamp = DateTime.Parse(reader("timestamp").ToString())
                            })
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            AppLogger.LogError($"Error querying SQL Server for {nodeId}: {ex.Message}", "DetailRecord")
        End Try
        Return sensorDataList
    End Function

    Private Sub ConvertSensorDataToInterfaceRecords(tireSensorData As List(Of InterfaceSensorData), gaugeSensorData As List(Of InterfaceSensorData))
        ' Clear existing data first
        If gaugeDataRaw Is Nothing Then gaugeDataRaw = New List(Of InterfacePressureRecords)
        If rawDataRaw Is Nothing Then rawDataRaw = New List(Of InterfacePressureRecords)

        gaugeDataRaw.Clear()
        rawDataRaw.Clear()

        ' Convert PressureGauge data ke existing format
        For Each sensorData In gaugeSensorData
            ' Convert UTC to Local time
            Dim localTime = If(sensorData.Timestamp.Kind = DateTimeKind.Utc, sensorData.Timestamp.ToLocalTime(), sensorData.Timestamp)
            gaugeDataRaw.Add(New InterfacePressureRecords With {
                .Timestamp = localTime.ToString("yyyy-MM-dd HH:mm"),
                .Pressure = sensorData.Value
            })
        Next

        ' Convert PressureTire data ke existing format
        For Each sensorData In tireSensorData
            ' Convert UTC to Local time
            Dim localTime = If(sensorData.Timestamp.Kind = DateTimeKind.Utc, sensorData.Timestamp.ToLocalTime(), sensorData.Timestamp)
            rawDataRaw.Add(New InterfacePressureRecords With {
                .Timestamp = localTime.ToString("yyyy-MM-dd HH:mm"),
                .Pressure = sensorData.Value
            })
        Next
    End Sub

    ' OLD JSON-based function - commented out
    'Function ReadSortedPressureData(sensorDir As String, Optional isSorted As Boolean = True) As List(Of InterfacePressureRecords)
    '    Dim allData As New List(Of InterfacePressureRecords)
    '    If Not Directory.Exists(sensorDir) Then Return allData

    '    Dim files = Directory.GetFiles(sensorDir).OrderBy(Function(f) f).ToList()
    '    For Each fileSensor In files
    '        Try
    '            Dim json = File.ReadAllText(fileSensor)
    '            Dim dataList = JsonConvert.DeserializeObject(Of List(Of InterfacePressureRecords))(json)
    '            If dataList IsNot Nothing Then allData.AddRange(dataList)
    '        Catch experbaiki As Exception
    '            Console.WriteLine($"❌ Gagal proses file {fileSensor}: {ex.Message}")
    '        End Try
    '    Next

    '    Dim filtered = allData.
    '    Where(Function(d) Not String.IsNullOrWhiteSpace(d.Timestamp)).
    '    Where(Function(d) DateTime.TryParse(d.Timestamp, Nothing))

    '    Return If(isSorted,
    '          filtered.OrderByDescending(Function(d) DateTime.Parse(d.Timestamp)).ToList(),
    '          filtered.ToList())
    'End Function

    ' OLD JSON-based function - commented out
    'Function ReadSortedPressureData2(sensorDir As String, Optional isSorted As Boolean = True) As List(Of InterfacePressureRecords)
    '    Dim allData As New List(Of InterfacePressureRecords)
    '    If Not Directory.Exists(sensorDir) Then Return allData

    '    Dim files = Directory.GetFiles(sensorDir).OrderBy(Function(f) f).ToList()
    '    For Each fileSensor In files
    '        Try
    '            Dim json = File.ReadAllText(fileSensor)
    '            Dim dataList = JsonConvert.DeserializeObject(Of List(Of InterfacePressureRecords))(json)
    '            If dataList IsNot Nothing Then allData.AddRange(dataList)
    '        Catch ex As Exception
    '            Console.WriteLine($"❌ Gagal proses file {fileSensor}: {ex.Message}")
    '        End Try
    '    Next

    '    Dim filtered = allData.
    '    Where(Function(d) Not String.IsNullOrWhiteSpace(d.Timestamp)).
    '    Where(Function(d) DateTime.TryParse(d.Timestamp, Nothing))

    '    rawDataRaw = filtered.ToList()
    '    If isSorted Then
    '        filtered = filtered.OrderByDescending(Function(d) DateTime.Parse(d.Timestamp)).ToList()
    '    Else
    '        filtered = filtered.ToList()
    '    End If

    '    rawData = AggregatePressureData(filtered, intervalAggregate)
    '    Return rawData
    'End Function
    Function AggregatePressureData(data As List(Of InterfacePressureRecords), interval As String) As List(Of InterfacePressureRecords)
        Dim grouped = New Dictionary(Of DateTime, List(Of Double))()

        For Each d In data
            Dim ts As DateTime
            If Not DateTime.TryParse(d.Timestamp, ts) Then Continue For

            Dim key As DateTime
            Select Case interval
                Case "2m" : key = New DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, ts.Minute \ 2 * 2, 0)
                Case "10m" : key = New DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, ts.Minute \ 10 * 10, 0)
                Case "1h" : key = New DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, 0, 0)
                Case "1d" : key = New DateTime(ts.Year, ts.Month, ts.Day)
                Case Else : key = ts
            End Select

            If Not grouped.ContainsKey(key) Then grouped(key) = New List(Of Double)
            grouped(key).Add(d.Pressure)
        Next

        ' Hanya ambil waktu yang benar-benar punya data
        Dim result = grouped.
        Where(Function(g) g.Value.Count > 0).
        Select(Function(g) New InterfacePressureRecords With {
            .Timestamp = g.Key.ToString("yyyy-MM-dd HH:mm"),
            .Pressure = g.Value.Average()
        }).
        OrderBy(Function(d) DateTime.Parse(d.Timestamp)).
        ToList()

        Return result
    End Function
    Function AggregatePressureData2(data As List(Of InterfacePressureRecords), interval As String) As List(Of InterfacePressureRecords)
        Dim grouped = New Dictionary(Of DateTime, List(Of Double))()
        For Each d In data
            Dim ts As DateTime
            If Not DateTime.TryParse(d.Timestamp, ts) Then Continue For

            Dim key As DateTime
            Select Case interval
                Case "10m" : key = New DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, ts.Minute \ 10 * 10, 0)
                Case "1h" : key = New DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, 0, 0)
                Case "1d" : key = New DateTime(ts.Year, ts.Month, ts.Day)
                Case Else : key = ts ' raw
            End Select

            If Not grouped.ContainsKey(key) Then grouped(key) = New List(Of Double)
            grouped(key).Add(d.Pressure)
        Next

        Return grouped.Select(Function(g) New InterfacePressureRecords With {
        .Timestamp = g.Key.ToString("yyyy-MM-dd HH:mm"),
        .Pressure = g.Value.Average()
    }).OrderBy(Function(d) DateTime.Parse(d.Timestamp)).ToList()
    End Function
    Private Sub BTNEndRecording_Click(sender As Object, e As EventArgs) Handles BTNEndRecording.Click
        If recordMetadata Is Nothing OrElse recordMetadata.Status.ToLower() <> "recording" Then
            MessageBox.Show("Recording is not active", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim result = MessageBox.Show($"End recording for batch {recordMetadata.BatchId}?", "Confirm End Recording", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            EndRecordingProcess()
        End If
    End Sub

    Private Sub EndRecordingProcess()
        Try
            ' 0. Remove auto end recording if exists (manual end recording)
            CleanupAutoEndRecording()

            ' 1. Update record_metadata status
            UpdateRecordMetadataStatus()

            ' 2. Update sensor status to ready
            UpdateSensorStatusToReady()

            ' 3. Export data to SQL Server
            ExportDataToSQLServer()

            ' 4. Update UI and stop timer
            TextBoxState.Text = "Finished"
            UpdateButtonStates()
            refreshTimerWatch.Stop()
            MessageBox.Show("Recording ended successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()

        Catch ex As Exception
            MessageBox.Show($"Error ending recording: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CleanupAutoEndRecording()
        Try
            If recordMetadata IsNot Nothing AndAlso Not String.IsNullOrEmpty(recordMetadata.PressureTireId) Then
                ' Check if there's an auto end recording for this tire sensor
                Dim endRecordings = SettingsManager.GetEndRecording()
                Dim hasAutoEndRecording = endRecordings.Any(Function(e) e("pressureTire") = recordMetadata.PressureTireId)

                If hasAutoEndRecording Then
                    ' Remove auto end recording since we're ending manually
                    SettingsManager.RemoveEndRecording(recordMetadata.PressureTireId)
                    Console.WriteLine($"✅ Removed auto end recording for manual end: {recordMetadata.PressureTireId}")
                End If
            End If
        Catch ex As Exception
            Console.WriteLine($"⚠️ CleanupAutoEndRecording Error: {ex.Message}")
        End Try
    End Sub

    Private Sub UpdateRecordMetadataStatus()
        recordMetadata.Status = "Finished"
        recordMetadata.SyncStatus = "Finished"
        recordMetadata.EndDate = DateTime.UtcNow
        sqlite.InsertOrUpdateRecordMetadata(recordMetadata)
    End Sub

    Private Sub UpdateSensorStatusToReady()
        Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

        ' Update PressureTire status
        If selectedNodeSensor.ContainsKey("PressureTire") Then
            For Each sensor In selectedNodeSensor("PressureTire")
                If sensor("NodeId") = recordMetadata.PressureTireId Then
                    sensor("NodeStatus") = "ready"
                End If
            Next
        End If

        ' Update PressureGauge status
        If selectedNodeSensor.ContainsKey("PressureGauge") Then
            For Each sensor In selectedNodeSensor("PressureGauge")
                If sensor("NodeId") = recordMetadata.PressureGaugeId Then
                    sensor("NodeStatus") = "ready"
                End If
            Next
        End If

        SettingsManager.SetSelectedNodeSensor(selectedNodeSensor)
    End Sub

    Private Sub ExportDataToSQLServer()
        Try

            Dim dbPath = SQLiteManager.GetDatabasePath()

            ' Check SQL Server connection status from settings
            If Not My.Settings.stateConnectionDB Then
                Console.WriteLine($"❌ SQL Server not connected - skipping export and cleanup")
                MessageBox.Show("SQL Server not connected. Export skipped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim sqlServerManager As New SQLServerManager()
            Dim success = sqlServerManager.ExportRecordData(recordMetadata.BatchId)
            Console.WriteLine($"🔍 Export result: {success}")

            If success Then
                Console.WriteLine($"✅ Successfully exported batch: {recordMetadata.BatchId}")
                ' Clean up sensor_data after successful export
                If Not String.IsNullOrEmpty(recordMetadata.PressureTireId) AndAlso Not String.IsNullOrEmpty(recordMetadata.PressureGaugeId) Then
                    Console.WriteLine($"🧹 Starting sensor_data cleanup...")
                    Dim deleteSuccess = sqlite.DeleteSensorDataByNodeIds(recordMetadata.PressureTireId, recordMetadata.PressureGaugeId)
                    Console.WriteLine($"🧹 Cleanup result: {deleteSuccess} for nodes: {recordMetadata.PressureTireId}, {recordMetadata.PressureGaugeId}")
                Else
                    Console.WriteLine($"⚠️ Invalid node IDs - skipping sensor_data cleanup")
                End If
            Else
                Console.WriteLine($"⚠️ Export failed for {recordMetadata.BatchId} - keeping sensor_data")
                MessageBox.Show($"Export failed for batch {recordMetadata.BatchId}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            Console.WriteLine($"❌ Export error: {ex.Message}")
            Console.WriteLine($"🔍 Export error details: {ex.ToString()}")
            MessageBox.Show($"Export error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BTNExport_Click(sender As Object, e As EventArgs) Handles BTNExport.Click
        If rawData Is Nothing OrElse rawData.Count = 0 Then
            MessageBox.Show("No data to export", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Create custom dialog form
        Dim exportDialog As New Form()
        exportDialog.Text = "Export Format"
        exportDialog.Size = New Size(300, 150)
        exportDialog.StartPosition = FormStartPosition.CenterParent
        exportDialog.FormBorderStyle = FormBorderStyle.FixedDialog
        exportDialog.MaximizeBox = False
        exportDialog.MinimizeBox = False

        Dim lblMessage As New Label()
        lblMessage.Text = "Choose export format:"
        lblMessage.Location = New Point(20, 20)
        lblMessage.Size = New Size(250, 20)
        exportDialog.Controls.Add(lblMessage)

        Dim btnExcel As New Button()
        btnExcel.Text = "Excel"
        btnExcel.Location = New Point(20, 50)
        btnExcel.Size = New Size(70, 30)
        btnExcel.DialogResult = DialogResult.Yes
        exportDialog.Controls.Add(btnExcel)

        Dim btnCSV As New Button()
        btnCSV.Text = "CSV"
        btnCSV.Location = New Point(100, 50)
        btnCSV.Size = New Size(70, 30)
        btnCSV.DialogResult = DialogResult.No
        exportDialog.Controls.Add(btnCSV)

        Dim btnCancel As New Button()
        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(180, 50)
        btnCancel.Size = New Size(70, 30)
        btnCancel.DialogResult = DialogResult.Cancel
        exportDialog.Controls.Add(btnCancel)

        Dim result = exportDialog.ShowDialog()
        exportDialog.Dispose()

        If result = DialogResult.Cancel Then Return

        Dim isExcel = (result = DialogResult.Yes)
        Dim fileExtension = If(isExcel, "xlsx", "csv")
        Dim filterText = If(isExcel, "Excel files (*.xlsx)|*.xlsx", "CSV files (*.csv)|*.csv")

        Using saveDialog As New SaveFileDialog()
            saveDialog.Filter = filterText
            saveDialog.FileName = $"PressureData_{recordMetadata.BatchId}_{DateTime.Now:yyyyMMdd_HHmmss}.{fileExtension}"

            If saveDialog.ShowDialog() = DialogResult.OK Then
                Try
                    If isExcel Then
                        ExportToExcel(saveDialog.FileName)
                    Else
                        ExportToCSV(saveDialog.FileName)
                    End If
                    MessageBox.Show($"Data exported successfully to {saveDialog.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    Private Sub ExportToCSV(filePath As String)
        Using writer As New IO.StreamWriter(filePath)
            ' Write header
            writer.WriteLine("BatchId,StartPressure,CurrentPressure,LeakPressure,PressureTire,PressureGauge,Timestamp")

            ' Get sensor names in format [key_object].nodeText
            Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()
            Dim tireNodeText = Helper.GetNodeTextFromId(selectedNodeSensor, "PressureTire", recordMetadata.PressureTireId)
            Dim gaugeNodeText = Helper.GetNodeTextFromId(selectedNodeSensor, "PressureGauge", recordMetadata.PressureGaugeId)
            Dim pressureTireName = $"PressureTire.{tireNodeText}"
            Dim pressureGaugeName = $"PressureGauge.{gaugeNodeText}"

            ' Get fixed values (same as DGV display)
            Dim startPressure = TextBoxStartPressure.Text

            ' Get CurrentPressure (last data from pressureTire)
            Dim sortedTireDataAsc = rawData.OrderBy(Function(d) DateTime.Parse(d.Timestamp)).ToList()
            Dim currentPressure = If(sortedTireDataAsc.Count > 0, sortedTireDataAsc.Last().Pressure.ToString("F2"), "0.00")

            ' Get LeakPressure (last data from pressureGauge)
            Dim sortedGaugeDataAsc = If(gaugeDataProcessed IsNot Nothing AndAlso gaugeDataProcessed.Count > 0,
                                       gaugeDataProcessed.OrderBy(Function(g) DateTime.Parse(g.Timestamp)).ToList(),
                                       New List(Of InterfacePressureRecords)())
            Dim leakPressure = If(sortedGaugeDataAsc.Count > 0, sortedGaugeDataAsc.Last().Pressure.ToString("F2"), "0.00")

            Dim sortedTireData = rawData.OrderByDescending(Function(d) DateTime.Parse(d.Timestamp)).ToList()

            For Each DL In sortedTireData
                Dim currentTimestamp = DateTime.Parse(DL.Timestamp).ToString("yyyy-MM-dd HH:mm")
                writer.WriteLine($"{recordMetadata.BatchId},{startPressure},{currentPressure},{leakPressure},{pressureTireName},{pressureGaugeName},{currentTimestamp}")
            Next
        End Using
    End Sub

    Private Sub ExportToExcel(filePath As String)
        Dim xlApp As Excel.Application = Nothing
        Dim xlWorkbook As Excel.Workbook = Nothing
        Dim xlWorksheet As Excel.Worksheet = Nothing

        Try
            xlApp = New Excel.Application()
            xlWorkbook = xlApp.Workbooks.Add()
            xlWorksheet = xlWorkbook.ActiveSheet

            ' Set headers
            xlWorksheet.Cells(1, 1) = "BatchId"
            xlWorksheet.Cells(1, 2) = "StartPressure"
            xlWorksheet.Cells(1, 3) = "CurrentPressure"
            'xlWorksheet.Cells(1, 4) = "LeakPressure"
            xlWorksheet.Cells(1, 4) = "PressureTire"
            xlWorksheet.Cells(1, 5) = "PressureGauge"
            xlWorksheet.Cells(1, 6) = "Timestamp"

            ' Get sensor names
            Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()
            Dim tireNodeText = Helper.GetNodeTextFromId(selectedNodeSensor, "PressureTire", recordMetadata.PressureTireId)
            Dim gaugeNodeText = Helper.GetNodeTextFromId(selectedNodeSensor, "PressureGauge", recordMetadata.PressureGaugeId)
            Dim pressureTireName = $"PressureTire.{tireNodeText}"
            Dim pressureGaugeName = $"PressureGauge.{gaugeNodeText}"

            ' Write data
            Dim startPressure = TextBoxStartPressure.Text
            Dim sortedTireData = rawData.OrderByDescending(Function(d) DateTime.Parse(d.Timestamp)).ToList()
            Dim row = 2

            For Each DL In sortedTireData
                Dim currentPressure = DL.Pressure.ToString("F2")
                Dim currentTimestamp = DateTime.Parse(DL.Timestamp).ToString("yyyy-MM-dd HH:mm")

                Dim matchingGauge = gaugeDataProcessed?.FirstOrDefault(Function(g) Math.Abs((DateTime.Parse(DL.Timestamp) - DateTime.Parse(g.Timestamp)).TotalMinutes) < 1)
                Dim currentLeakPressure = If(matchingGauge?.Pressure, 0).ToString("F2")

                xlWorksheet.Cells(row, 1) = recordMetadata.BatchId
                xlWorksheet.Cells(row, 2) = startPressure
                xlWorksheet.Cells(row, 3) = currentPressure
                'xlWorksheet.Cells(row, 4) = currentLeakPressure
                xlWorksheet.Cells(row, 4) = pressureTireName
                xlWorksheet.Cells(row, 5) = pressureGaugeName
                xlWorksheet.Cells(row, 6) = currentTimestamp
                row += 1
            Next

            ' Auto-fit columns
            xlWorksheet.Columns.AutoFit()

            ' Save and close
            xlWorkbook.SaveAs(filePath)
            xlWorkbook.Close()
            xlApp.Quit()

        Catch ex As Exception
            Throw New Exception($"Excel export failed: {ex.Message}")
        Finally
            ' Clean up COM objects
            If xlWorksheet IsNot Nothing Then System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorksheet)
            If xlWorkbook IsNot Nothing Then System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook)
            If xlApp IsNot Nothing Then System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp)
        End Try
    End Sub

    Private Sub BTNClose_Click(sender As Object, e As EventArgs) Handles BTNClose.Click
        Try
            If refreshTimerWatch IsNot Nothing Then refreshTimerWatch.Stop()
            If refreshTimerGraph IsNot Nothing Then refreshTimerGraph.Stop()
        Catch
            ' Ignore timer errors
        End Try
        Me.Close()
    End Sub

    Private Sub DetailRecord_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            If refreshTimerWatch IsNot Nothing Then
                refreshTimerWatch.Stop()
                refreshTimerWatch.Dispose()
            End If
            If refreshTimerGraph IsNot Nothing Then
                refreshTimerGraph.Stop()
                refreshTimerGraph.Dispose()
            End If
            If DGVWatch IsNot Nothing Then
                DGVWatch.DataSource = Nothing
                DGVWatch.Rows.Clear()
            End If
        Catch
            ' Ignore cleanup errors
        End Try
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CMBGroupingGraph.SelectedIndexChanged
        If Me.CMBGroupingGraph.SelectedIndex = 0 Then
            ' Reset to raw data (no aggregation)
            intervalAggregate = ""
            rawData = rawDataRaw
            gaugeDataProcessed = gaugeDataRaw
        Else
            ' Apply aggregation
            intervalAggregate = CMBGroupingGraph.SelectedItem.ToString()
            rawData = AggregatePressureData(rawDataRaw, intervalAggregate)
            gaugeDataProcessed = AggregatePressureData(gaugeDataRaw, intervalAggregate)
        End If

        LoadSensorPressureTable()
        LoadSensorPressureGraph()
    End Sub

    Private Sub UpdateButtonStates()
        If recordMetadata Is Nothing Then Return

        Dim status = recordMetadata.Status.ToLower()

        Select Case status
            Case "not-start"
                BTNStart.Enabled = True
                BTNStart.Text = "Start"
                BTNEndRecording.Enabled = True
            Case "recording"
                BTNStart.Enabled = True
                BTNStart.Text = "Pause"
                BTNEndRecording.Enabled = True
            Case "finished"
                BTNStart.Enabled = False
                BTNStart.Text = "Start"
                BTNEndRecording.Enabled = False
        End Select
    End Sub

    Private Sub BTNStart_Click(sender As Object, e As EventArgs) Handles BTNStart.Click
        If recordMetadata Is Nothing Then Return

        If recordMetadata.Status.ToLower() = "recording" Then
            ' Stop recording - update to Not-Start
            Dim result = MessageBox.Show($"Pause recording for batch {recordMetadata.BatchId}?", "Confirm Pause Recording", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                StopRecordingProcess()
            End If
        Else
            ' Start recording
            Dim result = MessageBox.Show($"Start recording for batch {recordMetadata.BatchId}?", "Confirm Start Recording", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                StartRecordingProcess()
                RefreshRawData()
            End If
        End If
    End Sub

    Private Sub StartRecordingProcess()
        Try
            ' 1. Update record_metadata status to Recording
            recordMetadata.Status = "Recording"
            recordMetadata.SyncStatus = "Recording"
            sqlite.InsertOrUpdateRecordMetadata(recordMetadata)

            ' 2. Update sensor status to Recording in My.Settings.selectedNodeSensor
            UpdateSensorStatusToRecording()

            ' 3. Update UI
            TextBoxState.Text = "Recording"
            UpdateButtonStates()

            ' 4. Start timer if not already running
            If Not refreshTimerWatch.Enabled Then
                TimeManager.StartTimerWithInitialFetch(refreshTimerWatch, Sub() LoadSensorPressureTable())
            End If

            MessageBox.Show("Recording started successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show($"Error starting recording: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateSensorStatusToRecording()
        Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

        ' Update PressureTire status to running
        If selectedNodeSensor.ContainsKey("PressureTire") Then
            For Each sensor In selectedNodeSensor("PressureTire")
                If sensor("NodeId") = recordMetadata.PressureTireId Then
                    sensor("NodeStatus") = "running"
                End If
            Next
        End If

        ' Update PressureGauge status to running
        If selectedNodeSensor.ContainsKey("PressureGauge") Then
            For Each sensor In selectedNodeSensor("PressureGauge")
                If sensor("NodeId") = recordMetadata.PressureGaugeId Then
                    sensor("NodeStatus") = "running"
                End If
            Next
        End If

        SettingsManager.SetSelectedNodeSensor(selectedNodeSensor)
    End Sub

    Private Sub StopRecordingProcess()
        Try
            ' 1. Update record_metadata status to Not-Start
            recordMetadata.Status = "Not-Start"
            recordMetadata.SyncStatus = "Not-Start"
            sqlite.InsertOrUpdateRecordMetadata(recordMetadata)

            ' 2. Update sensor status to Not-Start
            UpdateSensorStatusToNotStart()

            ' 3. Update UI
            TextBoxState.Text = "Not-Start"
            UpdateButtonStates()

            ' 4. Stop timer
            refreshTimerWatch.Stop()

            MessageBox.Show("Recording pause successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show($"Error pause recording: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateSensorStatusToNotStart()
        Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

        ' Update PressureTire status to Not-Start
        If selectedNodeSensor.ContainsKey("PressureTire") Then
            For Each sensor In selectedNodeSensor("PressureTire")
                If sensor("NodeId") = recordMetadata.PressureTireId Then
                    sensor("NodeStatus") = "Not-Start"
                End If
            Next
        End If

        ' Update PressureGauge status to Not-Start
        If selectedNodeSensor.ContainsKey("PressureGauge") Then
            For Each sensor In selectedNodeSensor("PressureGauge")
                If sensor("NodeId") = recordMetadata.PressureGaugeId Then
                    sensor("NodeStatus") = "Not-Start"
                End If
            Next
        End If

        SettingsManager.SetSelectedNodeSensor(selectedNodeSensor)
    End Sub
End Class