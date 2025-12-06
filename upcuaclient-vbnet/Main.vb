Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports OfficeOpenXml

Namespace upcuaclient_vbnet
    Module Main
        <DllImport("kernel32.dll")>
        Private Function AllocConsole() As Boolean
        End Function
        Private trayIcon As NotifyIcon
        Private bgWorker As upcuaclient_vbnet.BackgroundWorkerManager

        <STAThread()>
        Sub Main()
            ' Soft error handling - suppress all UI errors
            'DisableConsole()
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
            AddHandler Application.ThreadException, Sub(s, e)
                                                        ' Ignore UI errors silently
                                                    End Sub

            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)

            ' Start BackgroundWorker dari Main.vb bootstrap
            bgWorker = upcuaclient_vbnet.BackgroundWorkerManager.Instance
            bgWorker.Start()

            ' Alokasi console untuk debugging
            AllocConsole()
            Console.WriteLine("🚀 Starting OPC UA Client...")

            Try
                Dim context As New TrayAppContext()
                Application.Run(context)
            Catch ex As Exception
                Console.WriteLine($"❌ TrayAppContext Error: {ex.Message}")
                Console.WriteLine($"🔍 Stack: {ex.StackTrace}")

                ' Fallback: Run MainFormNew directly
                Try
                    'Console.WriteLine("🔄 Fallback: Starting MainFormNew directly...")
                    'Dim mainForm As New MainFormNew()
                    'Application.Run(mainForm)
                Catch fallbackEx As Exception
                    Console.WriteLine($"❌ Fallback failed: {fallbackEx.Message}")
                    MessageBox.Show($"Critical error starting application: {fallbackEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            End Try
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
