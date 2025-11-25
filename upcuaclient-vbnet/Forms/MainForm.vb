Imports System.IO
Imports Newtonsoft.Json
Imports System.Windows.Forms
Imports System.Threading.Tasks

Public Class MainForm
    Private trayIcon As NotifyIcon
    Private refreshTimer As New Timer() With {.Interval = 2000}
    Private bgWorker As upcuaclient_vbnet.BackgroundWorkerManager

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Console.WriteLine("🚀 MainForm loaded, starting BackgroundWorker...")

        ' Inisialisasi BackgroundWorker
        bgWorker = New upcuaclient_vbnet.BackgroundWorkerManager()
        bgWorker.Start()

        ' Tray Icon Setup
        trayIcon = New NotifyIcon() With {
            .Icon = SystemIcons.Application,
            .Visible = True,
            .Text = "Sensor Monitor"
        }

        Dim menu As New ContextMenuStrip()
        menu.Items.Add("Tampilkan Dashboard", Nothing, AddressOf ShowDashboard)
        menu.Items.Add("Keluar", Nothing, AddressOf ExitApp)
        trayIcon.ContextMenuStrip = menu

        ' Setup tabel UI
        DataGridView1.Columns.Clear()
        DataGridView1.Columns.Add("SensorId", "Sensor ID")
        DataGridView1.Columns.Add("Pressure", "Pressure")
        DataGridView1.Columns.Add("Status", "Status")

        AddHandler refreshTimer.Tick, AddressOf RefreshData
        refreshTimer.Start()
    End Sub

    Private Sub RefreshData(sender As Object, e As EventArgs)
        Try
            Dim rootDir As String = "Temp"
            If Not Directory.Exists(rootDir) Then Return

            DataGridView1.Rows.Clear()
            For Each sensorDir In Directory.GetDirectories(rootDir)
                Dim name = Path.GetFileName(sensorDir)
                Dim jsonPath = Path.Combine(sensorDir, $"{name}.log.json")
                If Not File.Exists(jsonPath) Then Continue For

                Dim json = File.ReadAllText(jsonPath)
                Dim result = JsonConvert.DeserializeObject(Of upcuaclient_vbnet.SensorResult)(json)

                Dim idx = DataGridView1.Rows.Add(result.SensorId, $"{result.Pressure:F2}", result.Status)
                Dim row = DataGridView1.Rows(idx)

                Select Case result.Status.ToLower()
                    Case "normal"
                        row.DefaultCellStyle.BackColor = Color.LightGreen
                    Case "low"
                        row.DefaultCellStyle.BackColor = Color.LightYellow
                    Case "offline"
                        row.DefaultCellStyle.BackColor = Color.LightGray
                    Case "critical"
                        row.DefaultCellStyle.BackColor = Color.LightCoral
                End Select
            Next
        Catch ex As Exception
            Console.WriteLine("Gagal refresh data: " & ex.Message)
        End Try
    End Sub

    Private Sub ShowDashboard(sender As Object, e As EventArgs)
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        Me.BringToFront()
    End Sub

    Private Async Sub ExitApp(sender As Object, e As EventArgs)
        Try
            trayIcon.Visible = False
            If bgWorker IsNot Nothing Then
                bgWorker.Stop()
                ' Give some time for cleanup
                Await Task.Delay(1000)
            End If
            Application.Exit()
        Catch ex As Exception
            ' Force exit if cleanup fails
            Environment.Exit(0)
        End Try
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Hide()
    End Sub
End Class
