Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace upcuaclient_vbnet
    Module Main
        <DllImport("kernel32.dll")>
        Private Function AllocConsole() As Boolean
        End Function
        Private trayIcon As NotifyIcon
        Private bgWorker As upcuaclient_vbnet.BackgroundWorkerManager

        <STAThread()>
        Sub Main()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)

            ' Start BackgroundWorker dari Main.vb bootstrap
            bgWorker = upcuaclient_vbnet.BackgroundWorkerManager.Instance
            bgWorker.Start()

            ' Tray Icon Setup

            trayIcon = New NotifyIcon() With {
                .Icon = My.Resources._321,
                .Visible = True,
                .Text = "Sensor Monitor"
            }
            Dim menu As New ContextMenuStrip()
            menu.Items.Add("Open", Nothing, AddressOf ShowDashboard)
            menu.Items.Add("Exit", Nothing, AddressOf ExitApp)
            trayIcon.ContextMenuStrip = menu


            ' Alokasi console untuk debugging
            AllocConsole()
            Console.WriteLine("🚀 Starting OPC UA Client...")

            ' 🔒 Cegah duplikasi instance
            Dim isNew As Boolean
            Dim mutex As New Mutex(True, "SensorTrayApp", isNew)
            If Not isNew Then
                MessageBox.Show("Aplikasi sudah berjalan di tray.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim context As New TrayAppContext()
            Application.Run(context)

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
                Console.WriteLine("Error during exit: " & ex.Message)
                Application.Exit()
            End Try
        End Sub

        Private Sub ShowDashboard(sender As Object, e As EventArgs)
            Dim mainFormNew As MainFormNew = CType(Application.OpenForms("MainFormNew"), MainFormNew)
            If mainFormNew Is Nothing Then
                mainFormNew = New MainFormNew()
            End If
            mainFormNew.Show()
            mainFormNew.WindowState = FormWindowState.Normal
            mainFormNew.BringToFront()
        End Sub
    End Module
End Namespace
