Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class TrayAppContext
    Inherits ApplicationContext

    Private mainFormHandler As MainFormNew
    Private trayIcon As NotifyIcon

    Public Sub New()
        Try
            ' Initialize settings first
            SettingsManager.InitializeDefaults()
            
            ' Add small delay to ensure all components are ready
            Threading.Thread.Sleep(100)
            
            ' Inisialisasi form default with null check
            Try
                mainFormHandler = New MainFormNew()
                If mainFormHandler IsNot Nothing Then
                    AddHandler mainFormHandler.FormClosing, AddressOf OnFormClosing
                End If
            Catch formEx As Exception
                Console.WriteLine($"MainFormNew creation error: {formEx.Message}")
                Throw New Exception($"Failed to create main form: {formEx.Message}", formEx)
            End Try

            ' Setup tray icon with null check
            Try
                SetupTrayIcon()
            Catch trayEx As Exception
                Console.WriteLine($"Tray icon setup error: {trayEx.Message}")
                ' Continue without tray icon if it fails
            End Try

            ' Tampilkan form saat start dengan delay
            If mainFormHandler IsNot Nothing Then
                Threading.Thread.Sleep(50)
                mainFormHandler.Show()
            End If
        Catch ex As Exception
            Console.WriteLine($"TrayAppContext Error: {ex.Message}")
            Console.WriteLine($"Stack: {ex.StackTrace}")
            Try
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Catch
                ' Ignore message box errors
            End Try
            
            If mainFormHandler IsNot Nothing Then
                Try
                    mainFormHandler.Close()
                Catch
                    ' Ignore close errors
                End Try
            End If
            ExitThread()
        End Try
    End Sub

    Private Sub SetupTrayIcon()
        trayIcon = New NotifyIcon() With {
            .Icon = My.Resources._24,
            .Text = "OPC UA Client",
            .Visible = True
        }

        ' Create context menu
        Dim contextMenu = New ContextMenuStrip()
        contextMenu.Items.Add("Show", Nothing, AddressOf ShowForm)
        contextMenu.Items.Add("-")
        contextMenu.Items.Add("Exit", Nothing, AddressOf ExitApplication)

        trayIcon.ContextMenuStrip = contextMenu
        AddHandler trayIcon.DoubleClick, AddressOf ShowForm
    End Sub

    Private Sub ShowForm(sender As Object, e As EventArgs)
        Try
            If mainFormHandler IsNot Nothing Then
                mainFormHandler.Show()
                mainFormHandler.WindowState = FormWindowState.Normal
                mainFormHandler.BringToFront()
            End If
        Catch ex As Exception
            Console.WriteLine($"ShowForm error: {ex.Message}")
        End Try
    End Sub

    Private Sub ExitApplication(sender As Object, e As EventArgs)
        Try
            ' Hide tray icon
            If trayIcon IsNot Nothing Then
                trayIcon.Visible = False
                trayIcon.Dispose()
            End If

            ' Close main form
            If mainFormHandler IsNot Nothing Then
                RemoveHandler mainFormHandler.FormClosing, AddressOf OnFormClosing
                mainFormHandler.Close()
            End If

            ' Exit application - BackgroundWorker stop dihandle oleh Main.vb
            ExitThread()
        Catch ex As Exception
            Console.WriteLine($"⚠️ Exit error: {ex.Message}")
            Environment.Exit(0)
        End Try
    End Sub

    Private Sub OnFormClosing(sender As Object, e As FormClosingEventArgs)
        Try
            e.Cancel = True
            If mainFormHandler IsNot Nothing Then
                mainFormHandler.Hide()
            End If
        Catch ex As Exception
            Console.WriteLine($"OnFormClosing error: {ex.Message}")
        End Try
    End Sub
End Class