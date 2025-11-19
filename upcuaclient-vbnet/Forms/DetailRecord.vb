Imports System.IO
Imports LiveCharts
Imports LiveCharts.WinForms
Imports LiveCharts.Wpf
Imports Newtonsoft.Json
Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class DetailRecord
    Private sqlite As New SQLiteManager()
    Private batchId As String
    Private recordMetadata As InterfaceRecordMetadata
    Private rawData As List(Of InterfacePressureRecords)
    Private rawDataRaw As List(Of InterfacePressureRecords) 'mentah
    Private gaugeData As List(Of InterfaceSensorData) 'PressureGauge data
    Private gaugeDataRaw As List(Of InterfacePressureRecords) 'gauge mentah
    Private gaugeDataProcessed As List(Of InterfacePressureRecords) 'gauge processed
    Private intervalAggregate As String = ""
    Private refreshTimerWatch As New Timer() With {.Interval = 2000}
    Private refreshTimerGraph As New Timer() With {.Interval = 2000}

    Public Sub New(batchIdParam As String)
        If String.IsNullOrEmpty(batchIdParam) Then
            MessageBox.Show("⚠️ Batch ID not found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If
        InitializeComponent()
        batchId = batchIdParam
    End Sub

    Public Sub InitializeTimers()
        Dim config = ConfigManager.Load("Config/meta.json")
        refreshTimerWatch.Interval = config.IntervalTime

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
        TimeManager.StartTimerWithInitialFetch(refreshTimerWatch,
        Sub()
            If TabControlDetailRecord.SelectedTab Is TabPageRecord Then
                LoadSensorPressureTable()
            ElseIf TabControlDetailRecord.SelectedTab Is TabPageGraph Then
                LoadSensorPressureGraph()
            End If
        End Sub)

    End Sub


    Private Sub DetailRecord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGraph()
        LoadSensorMetadata()
        RefreshRawData()

        TabControlDetailRecord.SelectedTab = TabPageRecord
        InitializeTimers()
        TimeManager.StartTimerWithInitialFetch(refreshTimerWatch, Sub() LoadSensorPressureTable())
    End Sub

    ' Init Graph
    Private Sub SetupGraph()
        CartesianChartDetailRecord.Dock = DockStyle.Fill
        CartesianChartDetailRecord.Zoom = ZoomingOptions.X
        CartesianChartDetailRecord.Pan = PanningOptions.X
        CartesianChartDetailRecord.Series = New SeriesCollection From {
            New LineSeries With {
                .Title = "Pressure",
                .Values = New ChartValues(Of Double)(),
                .PointGeometry = DefaultGeometries.Circle,
                .PointGeometrySize = 8
            }
        }

        CartesianChartDetailRecord.AxisX.Add(New Axis With {.Title = "Time", .Labels = New List(Of String)()})
        CartesianChartDetailRecord.AxisY.Add(New Axis With {.Title = "Pressure (PSI)"})
    End Sub

    ' For TextBox
    Private Sub LoadSensorMetadata()
        recordMetadata = sqlite.QueryRecordMetadata(batchId)
        If recordMetadata Is Nothing OrElse String.IsNullOrEmpty(recordMetadata.BatchId) Then Return

        TextBoxBatchId.Text = recordMetadata.BatchId
        TextBoxSensorId.Text = $"{recordMetadata.PressureTireId} | {recordMetadata.PressureGaugeId}"
        TextBoxSize.Text = recordMetadata.Size.ToString()
        TextBoxOperator.Text = recordMetadata.CreatedBy
        TextBoxStartDate.Text = recordMetadata.StartDate.ToString("yyyy-MM-dd HH:mm")
        TextBoxState.Text = recordMetadata.Status

        ' Running days and start pressure will be calculated after RefreshRawData
        Console.WriteLine($"🔍 Metadata loaded for batch: {recordMetadata.BatchId}")
        Console.WriteLine($"🔍 Start date: {recordMetadata.StartDate}")
    End Sub

    Private Sub UpdateRunningDaysAndStartPressure()
        If recordMetadata Is Nothing OrElse rawDataRaw Is Nothing OrElse rawDataRaw.Count = 0 Then
            TextBoxRunningDay.Text = "0"
            TextBoxStartPressure.Text = "0.00"
            Return
        End If

        ' Get sorted data (earliest to latest)
        Dim sortedData = rawDataRaw.OrderBy(Function(d) DateTime.Parse(d.Timestamp)).ToList()

        ' Get first pressure (StartPressure) and last timestamp for running days
        Dim firstPressure = sortedData.First().Pressure
        Dim lastTimestamp = DateTime.Parse(sortedData.Last().Timestamp)

        ' Calculate running days using TotalDays and ceiling for accurate count
        Dim runningDays = Math.Ceiling((lastTimestamp - recordMetadata.StartDate).TotalDays)
        TextBoxRunningDay.Text = runningDays.ToString()
        TextBoxStartPressure.Text = firstPressure.ToString("F2")

        Console.WriteLine($"✅ Running days calculated: {runningDays} days")
        Console.WriteLine($"✅ Start pressure: {firstPressure}, Last timestamp: {lastTimestamp}")
    End Sub

    ' For Graph
    Private Sub LoadSensorPressureGraph()
        If rawData Is Nothing Then Return

        Dim pressureValues As New ChartValues(Of Double)()
        Dim timeLabels As New List(Of String)()

        For Each DL In rawData
            Dim parsedTime As DateTime
            If Not DateTime.TryParse(DL.Timestamp, parsedTime) Then Continue For

            Dim timestampFormatted = parsedTime.ToString("yyyy-MM-dd HH:mm")

            pressureValues.Add(DL.Pressure)
            timeLabels.Add(timestampFormatted)
        Next


        'Dim sortedData = ReadSortedPressureData(sensorDirAnalytics, isSorted:=False)
        'For Each DL In sortedData
        '    Dim parsedTime As DateTime
        '    If Not DateTime.TryParse(DL.Timestamp, parsedTime) Then
        '        Console.WriteLine($"⚠️ Format timestamp invalid: {DL.Timestamp}")
        '        Continue For
        '    End If
        '    Dim pressureValue = DL.Pressure.ToString("F2")
        '    Dim timestampFormatted = parsedTime.ToString("yyyy-MM-dd HH:mm")

        '    pressureValues.Add(pressureValue)
        '    timeLabels.Add(timestampFormatted)
        'Next

        Dim lineSeries = CartesianChartDetailRecord.Series(0)
        lineSeries.Values = pressureValues
        CartesianChartDetailRecord.AxisX(0).Labels = timeLabels
        CartesianChartDetailRecord.Refresh()
    End Sub
    ' For DGV
    Private Sub LoadSensorPressureTable()
        If rawData Is Nothing OrElse rawData.Count = 0 Then Return

        DGVWatch.Rows.Clear()
        DGVWatch.RowHeadersVisible = False

        ' Get StartPressure (same as TextBoxStartPressure - first data)
        Dim startPressure = TextBoxStartPressure.Text

        ' Sort data DESC (newest first)
        Dim sortedTireData = rawData.OrderByDescending(Function(d) DateTime.Parse(d.Timestamp)).ToList()

        ' Add all rows with same StartPressure, but different CurrentPressure per timestamp
        For Each DL In sortedTireData
            Dim currentPressure = DL.Pressure.ToString("F2")
            Dim currentTimestamp = DateTime.Parse(DL.Timestamp).ToString("yyyy-MM-dd HH:mm")

            ' Find matching gauge data for this timestamp
            Dim matchingGauge = gaugeDataProcessed?.FirstOrDefault(Function(g) Math.Abs((DateTime.Parse(DL.Timestamp) - DateTime.Parse(g.Timestamp)).TotalMinutes) < 1)
            Dim currentLeakPressure = If(matchingGauge?.Pressure, 0).ToString("F2")

            DGVWatch.Rows.Add(startPressure, currentPressure, currentLeakPressure, currentTimestamp)
        Next
    End Sub
    Private Sub RefreshRawData()
        If recordMetadata Is Nothing OrElse String.IsNullOrEmpty(recordMetadata.BatchId) Then Return

        ' Get PressureTire data untuk chart dan DGV
        Dim tireSensorData = sqlite.GetSensorDataByNodeId(recordMetadata.PressureTireId)

        ' Get PressureGauge data untuk DGV LeakPressure column
        gaugeData = sqlite.GetSensorDataByNodeId(recordMetadata.PressureGaugeId)

        ' Convert PressureGauge data ke existing format untuk compatibility
        gaugeDataRaw = New List(Of InterfacePressureRecords)
        For Each sensorData In gaugeData
            gaugeDataRaw.Add(New InterfacePressureRecords With {
                .Timestamp = sensorData.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                .Pressure = sensorData.Value
            })
        Next

        ' Apply aggregation to gauge data
        gaugeDataProcessed = If(String.IsNullOrEmpty(intervalAggregate), gaugeDataRaw, AggregatePressureData(gaugeDataRaw, intervalAggregate))

        ' Convert PressureTire data ke existing format untuk compatibility
        rawDataRaw = New List(Of InterfacePressureRecords)
        For Each sensorData In tireSensorData
            rawDataRaw.Add(New InterfacePressureRecords With {
                .Timestamp = sensorData.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                .Pressure = sensorData.Value
            })
        Next

        ' Apply existing aggregation logic
        rawData = If(String.IsNullOrEmpty(intervalAggregate), rawDataRaw, AggregatePressureData(rawDataRaw, intervalAggregate))

        ' Update running days and start pressure after data is loaded
        UpdateRunningDaysAndStartPressure()
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
            .Timestamp = g.Key.ToString("yyyy-MM-dd HH:mm:ss"),
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
        .Timestamp = g.Key.ToString("yyyy-MM-dd HH:mm:ss"),
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
            ' 1. Update record_metadata status
            UpdateRecordMetadataStatus()

            ' 2. Update sensor status to Idle
            UpdateSensorStatusToIdle()

            ' 3. Export data to SQL Server
            ExportDataToSQLServer()

            ' 4. Stop timer and close form
            refreshTimerWatch.Stop()
            MessageBox.Show("Recording ended successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()

        Catch ex As Exception
            MessageBox.Show($"Error ending recording: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateRecordMetadataStatus()
        recordMetadata.Status = "Finished"
        recordMetadata.SyncStatus = "Finished"
        recordMetadata.EndDate = DateTime.Now
        sqlite.InsertOrUpdateRecordMetadata(recordMetadata)
    End Sub

    Private Sub UpdateSensorStatusToIdle()
        Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

        ' Update PressureTire status
        If selectedNodeSensor.ContainsKey("PressureTire") Then
            For Each sensor In selectedNodeSensor("PressureTire")
                If sensor("NodeId") = recordMetadata.PressureTireId Then
                    sensor("NodeStatus") = "Idle"
                End If
            Next
        End If

        ' Update PressureGauge status
        If selectedNodeSensor.ContainsKey("PressureGauge") Then
            For Each sensor In selectedNodeSensor("PressureGauge")
                If sensor("NodeId") = recordMetadata.PressureGaugeId Then
                    sensor("NodeStatus") = "Idle"
                End If
            Next
        End If

        SettingsManager.SetSelectedNodeSensor(selectedNodeSensor)
    End Sub

    Private Sub ExportDataToSQLServer()
        Try
            Console.WriteLine($"🔄 Starting export to SQL Server for batch: {recordMetadata.BatchId}")

            ' Check SQL Server connection first
            Dim sqlServerManager As New SQLServerManager()

            ' Test connection
            Console.WriteLine($"🔍 Testing SQL Server connection...")
            Dim connectionTest = Task.Run(Async Function() Await SqlServerConnection.CheckHealth()).Result

            If Not connectionTest Then
                Console.WriteLine($"❌ SQL Server connection failed - skipping export")
                Return
            End If

            Console.WriteLine($"✅ SQL Server connection OK - proceeding with export")
            Dim success = sqlServerManager.ExportRecordData(recordMetadata.BatchId)

            If success Then
                Console.WriteLine($"📤 Successfully exported batch: {recordMetadata.BatchId}")
            Else
                Console.WriteLine($"❌ Failed to export batch: {recordMetadata.BatchId}")
            End If
        Catch ex As Exception
            Console.WriteLine($"❌ Export error: {ex.Message}")
            Console.WriteLine($"🔍 Export error details: {ex.ToString()}")
        End Try
    End Sub

    Private Sub BTNExport_Click(sender As Object, e As EventArgs) Handles BTNExport.Click
        If rawData Is Nothing OrElse rawData.Count = 0 Then
            MessageBox.Show("No data to export", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using saveDialog As New SaveFileDialog()
            saveDialog.Filter = "CSV files (*.csv)|*.csv"
            saveDialog.FileName = $"PressureData_{recordMetadata.BatchId}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"

            If saveDialog.ShowDialog() = DialogResult.OK Then
                Try
                    ExportToCSV(saveDialog.FileName)
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
            writer.WriteLine("BatchId,StartPressure,CurrentPressure,LeakPressure,Timestamp")

            ' Write data (same as DGV display)
            Dim startPressure = TextBoxStartPressure.Text
            Dim sortedTireData = rawData.OrderByDescending(Function(d) DateTime.Parse(d.Timestamp)).ToList()

            For Each DL In sortedTireData
                Dim currentPressure = DL.Pressure.ToString("F2")
                Dim currentTimestamp = DateTime.Parse(DL.Timestamp).ToString("yyyy-MM-dd HH:mm")

                ' Find matching gauge data
                Dim matchingGauge = gaugeDataProcessed?.FirstOrDefault(Function(g) Math.Abs((DateTime.Parse(DL.Timestamp) - DateTime.Parse(g.Timestamp)).TotalMinutes) < 1)
                Dim currentLeakPressure = If(matchingGauge?.Pressure, 0).ToString("F2")

                writer.WriteLine($"{recordMetadata.BatchId},{startPressure},{currentPressure},{currentLeakPressure},{currentTimestamp}")
            Next
        End Using
    End Sub

    Private Sub BTNClose_Click(sender As Object, e As EventArgs) Handles BTNClose.Click
        refreshTimerWatch.Stop()
        refreshTimerGraph.Stop()
        'Close Form
        Me.Close()
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
End Class