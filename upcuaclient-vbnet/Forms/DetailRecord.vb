Imports System.IO
Imports LiveCharts
Imports LiveCharts.WinForms
Imports LiveCharts.Wpf
Imports Newtonsoft.Json
Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class DetailRecord
    Private configManager As ConfigManager
    Private sensorId As String
    Private sensorDirAnalytics As String
    Private Shared startPressureShared As Double = 0
    Private rawData As List(Of InterfacePressureRecords)
    Private rawDataRaw As List(Of InterfacePressureRecords) 'mentah
    Private intervalAggregate As String = ""
    Private refreshTimerWatch As New Timer() With {.Interval = 2000}
    Private refreshTimerGraph As New Timer() With {.Interval = 2000}

    Public Sub New(sensorIdParam As String)
        If String.IsNullOrEmpty(sensorIdParam) Then
            MessageBox.Show("⚠️ Sensor ID not found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If
        sensorDirAnalytics = Path.GetFullPath("..\..\Analytics\" & sensorIdParam)
        InitializeComponent()
        sensorId = sensorIdParam
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
        Dim rootDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath)), "Analytics")
        Dim filePath = Path.Combine(rootDir, "analytic-sensor.json")
        If Not File.Exists(filePath) Then Return

        Dim json = File.ReadAllText(filePath)
        Dim sensorDict = JsonConvert.DeserializeObject(Of Dictionary(Of String, InterfaceMetadaRecord))(json)
        If sensorDict Is Nothing OrElse Not sensorDict.ContainsKey(sensorId) Then Return

        Dim data = sensorDict(sensorId)
        TextBoxBatchId.Text = data.BatchId
        TextBoxSensorId.Text = data.SensorId
        TextBoxSize.Text = data.Size
        TextBoxOperator.Text = data.OperatorName
        TextBoxRunningDay.Text = data.RunningDay.ToString()

        Dim parsedDate As DateTime = DateTime.Parse(data.CreatedAt)
        TextBoxStartDate.Text = parsedDate.ToString("yyyy-MM-dd HH:mm")

        Dim startPressureShared = data.StartPressure
        TextBoxStartPressure.Text = startPressureShared.ToString("F2")

        Dim currentPressure = data.CurrentPressure.ToString("F2")
        TextBoxState.Text = data.State
    End Sub
    ' For Graph
    Private Sub LoadSensorPressureGraph()
        If rawData Is Nothing Then Return

        Dim pressureValues As New ChartValues(Of Double)()
        Dim timeLabels As New List(Of String)()

        For Each DL In rawData
            Dim parsedTime As DateTime
            If Not DateTime.TryParse(DL.Timestamp, parsedTime) Then Continue For

            Dim pressureValue = DL.Pressure.ToString("F2")
            Dim timestampFormatted = parsedTime.ToString("yyyy-MM-dd HH:mm")

            pressureValues.Add(pressureValue)
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
        If rawData Is Nothing Then Return

        Dim sortedData = rawData.OrderByDescending(Function(d) DateTime.Parse(d.Timestamp)).ToList()
        DGVWatch.Rows.Clear()


        DGVWatch.RowHeadersVisible = False
        DGVWatch.Rows.Clear()

        For Each DL In sortedData
            Dim parsedTime As DateTime
            If Not DateTime.TryParse(DL.Timestamp, parsedTime) Then
                Console.WriteLine($"⚠️ Format timestamp invalid: {DL.Timestamp}")
                Continue For
            End If

            Dim pressureValue = DL.Pressure.ToString("F2")
            Dim timestampFormatted = parsedTime.ToString("yyyy-MM-dd HH:mm")

            Dim idx = DGVWatch.Rows.Add(TextBoxStartPressure.Text, pressureValue, timestampFormatted)
            Dim row = DGVWatch.Rows(idx)
        Next
    End Sub
    Private Sub RefreshRawData()
        rawDataRaw = ReadSortedPressureData(sensorDirAnalytics, isSorted:=False)
        rawData = AggregatePressureData(rawDataRaw, intervalAggregate)
    End Sub

    Function ReadSortedPressureData(sensorDir As String, Optional isSorted As Boolean = True) As List(Of InterfacePressureRecords)
        Dim allData As New List(Of InterfacePressureRecords)
        If Not Directory.Exists(sensorDir) Then Return allData

        Dim files = Directory.GetFiles(sensorDir).OrderBy(Function(f) f).ToList()
        For Each fileSensor In files
            Try
                Dim json = File.ReadAllText(fileSensor)
                Dim dataList = JsonConvert.DeserializeObject(Of List(Of InterfacePressureRecords))(json)
                If dataList IsNot Nothing Then allData.AddRange(dataList)
            Catch ex As Exception
                Console.WriteLine($"❌ Gagal proses file {fileSensor}: {ex.Message}")
            End Try
        Next

        Dim filtered = allData.
        Where(Function(d) Not String.IsNullOrWhiteSpace(d.Timestamp)).
        Where(Function(d) DateTime.TryParse(d.Timestamp, Nothing))

        Return If(isSorted,
              filtered.OrderByDescending(Function(d) DateTime.Parse(d.Timestamp)).ToList(),
              filtered.ToList())
    End Function

    Function ReadSortedPressureData2(sensorDir As String, Optional isSorted As Boolean = True) As List(Of InterfacePressureRecords)
        Dim allData As New List(Of InterfacePressureRecords)
        If Not Directory.Exists(sensorDir) Then Return allData

        Dim files = Directory.GetFiles(sensorDir).OrderBy(Function(f) f).ToList()
        For Each fileSensor In files
            Try
                Dim json = File.ReadAllText(fileSensor)
                Dim dataList = JsonConvert.DeserializeObject(Of List(Of InterfacePressureRecords))(json)
                If dataList IsNot Nothing Then allData.AddRange(dataList)
            Catch ex As Exception
                Console.WriteLine($"❌ Gagal proses file {fileSensor}: {ex.Message}")
            End Try
        Next

        Dim filtered = allData.
        Where(Function(d) Not String.IsNullOrWhiteSpace(d.Timestamp)).
        Where(Function(d) DateTime.TryParse(d.Timestamp, Nothing))

        rawDataRaw = filtered.ToList()
        If isSorted Then
            filtered = filtered.OrderByDescending(Function(d) DateTime.Parse(d.Timestamp)).ToList()
        Else
            filtered = filtered.ToList()
        End If

        rawData = AggregatePressureData(filtered, intervalAggregate)
        Return rawData
    End Function
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
    Private Sub BTNClose_Click(sender As Object, e As EventArgs) Handles BTNClose.Click
        refreshTimerWatch.Stop()
        refreshTimerGraph.Stop()
        'Close Form
        Me.Close()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CMBGroupingGraph.SelectedIndexChanged
        If Me.CMBGroupingGraph.SelectedIndex = 0 Then
            Return
        End If
        intervalAggregate = CMBGroupingGraph.SelectedItem.ToString()
        rawData = AggregatePressureData(rawDataRaw, intervalAggregate)

        LoadSensorPressureTable()
        LoadSensorPressureGraph()
    End Sub
End Class