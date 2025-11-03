Imports Newtonsoft.Json
Imports System.Drawing
Imports System.IO
Imports upcuaclient.Modules.Core

Public Class MainForm
    Private refreshTimer As New Timer() With {.Interval = 2000}
    Private workerRef As BackgroundWorkerManager

    Public Sub New(worker As BackgroundWorkerManager)
        InitializeComponent()
        workerRef = worker
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Setup kolom DataGridView
        DataGridView1.Columns.Clear()
        DataGridView1.Columns.Add("SensorId", "Sensor ID")
        DataGridView1.Columns.Add("Pressure", "Pressure")
        DataGridView1.Columns.Add("Status", "Status")

        ' Start refresh timer
        AddHandler refreshTimer.Tick, AddressOf RefreshData
        refreshTimer.Start()
    End Sub

    Private Sub RefreshData(sender As Object, e As EventArgs)
        Try
            Dim baseDir As String = "logs"
            If Not Directory.Exists(baseDir) Then
                Me.Text = "Dashboard - folder logs belum ditemukan"
                Exit Sub
            End If

            Dim sensorFolders = Directory.GetDirectories(baseDir)
            DataGridView1.Rows.Clear()

            For Each sensorFolder In sensorFolders
                Try
                    Dim sensorName As String = Path.GetFileName(sensorFolder)
                    Dim logFile As String = Path.Combine(sensorFolder, $"{sensorName}.log.json")

                    If Not File.Exists(logFile) Then Continue For

                    ' Baca file log utama
                    Dim json As String = File.ReadAllText(logFile)
                    Dim result = JsonConvert.DeserializeObject(Of SensorResult)(json)

                    ' Tambahkan ke tabel
                    Dim rowIndex = DataGridView1.Rows.Add(result.sensor_id, $"{result.pressure:F2}", result.status)
                    Dim row = DataGridView1.Rows(rowIndex)

                    ' Gaya warna berdasarkan status
                    Select Case result.status.ToLower()
                        Case "recording"
                            row.DefaultCellStyle.BackColor = Color.LightGreen
                            row.DefaultCellStyle.ForeColor = Color.Black

                        Case "idle"
                            row.DefaultCellStyle.BackColor = Color.LightYellow
                            row.DefaultCellStyle.ForeColor = Color.Black

                        Case "offline"
                            row.DefaultCellStyle.BackColor = Color.LightGray
                            row.DefaultCellStyle.ForeColor = Color.DarkGray

                        Case "error"
                            row.DefaultCellStyle.BackColor = Color.LightCoral
                            row.DefaultCellStyle.ForeColor = Color.White

                        Case Else
                            row.DefaultCellStyle.BackColor = Color.White
                            row.DefaultCellStyle.ForeColor = Color.Black
                    End Select

                Catch innerEx As Exception
                    Console.WriteLine($"⚠️ Gagal membaca data dari {sensorFolder}: {innerEx.Message}")
                End Try
            Next

            Me.Text = $"Dashboard - {sensorFolders.Length} sensor terdeteksi"
        Catch ex As Exception
            Console.WriteLine("❌ Gagal membaca data log: " & ex.Message)
            Me.Text = "Dashboard - error membaca data"
        End Try
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        MyBase.OnFormClosing(e)
        refreshTimer.Stop()
        workerRef?.StopWorker()
    End Sub
End Class
