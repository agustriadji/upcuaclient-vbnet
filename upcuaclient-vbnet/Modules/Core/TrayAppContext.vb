Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class TrayAppContext
    Inherits ApplicationContext

    Private mainFormHandler As MainFormNew
    Private trayIcon As NotifyIcon

    Public Sub New()
        Try
            ' Inisialisasi form default
            mainFormHandler = New MainFormNew()
            AddHandler mainFormHandler.FormClosing, AddressOf OnFormClosing

            ' Setup tray icon
            SetupTrayIcon()

            ' Tampilkan form saat start
            mainFormHandler.Show()
        Catch ex As Exception
            MessageBox.Show($"❌ Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            If mainFormHandler IsNot Nothing Then
                mainFormHandler.Close()
            End If
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
        mainFormHandler.Show()
        mainFormHandler.WindowState = FormWindowState.Normal
        mainFormHandler.BringToFront()
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
        e.Cancel = True
        mainFormHandler.Hide()
    End Sub
End Class