Public Class TrayAppContext
    Inherits ApplicationContext

    Private mainFormHandler As MainFormNew
    Private trayIcon As NotifyIcon

    Public Sub New()
        ' Inisialisasi form utama
        mainFormHandler = New MainFormNew()
        AddHandler mainFormHandler.FormClosing, AddressOf OnFormClosing

        ' Tampilkan form saat startup
        mainFormHandler.Show()

        ' Setup tray icon
        SetupTrayIcon()
    End Sub

    Private Sub SetupTrayIcon()
        trayIcon = New NotifyIcon() With {
            .Icon = My.Resources._24,
            .Text = "AirLM",
            .Visible = True
        }

        ' Context menu tray
        Dim contextMenu = New ContextMenuStrip()
        contextMenu.Items.Add("Show apps", Nothing, AddressOf ShowForm)
        contextMenu.Items.Add("-")
        contextMenu.Items.Add("Exit apps", Nothing, AddressOf ExitApplication)

        trayIcon.ContextMenuStrip = contextMenu
        AddHandler trayIcon.DoubleClick, AddressOf ShowForm
    End Sub

    Private Sub ShowForm(sender As Object, e As EventArgs)
        If mainFormHandler Is Nothing OrElse mainFormHandler.IsDisposed Then
            mainFormHandler = New MainFormNew()
            AddHandler mainFormHandler.FormClosing, AddressOf OnFormClosing
        End If
        mainFormHandler.Show()
        mainFormHandler.WindowState = FormWindowState.Normal
        mainFormHandler.BringToFront()
    End Sub

    Private Sub ExitApplication(sender As Object, e As EventArgs)
        ' Hapus tray icon
        trayIcon.Visible = False
        trayIcon.Dispose()

        ' Tutup form utama benar-benar
        RemoveHandler mainFormHandler.FormClosing, AddressOf OnFormClosing
        mainFormHandler.Close()

        ' Exit loop
        ExitThread()
    End Sub

    Private Sub OnFormClosing(sender As Object, e As FormClosingEventArgs)
        ' Cancel close → hide form
        e.Cancel = True
        mainFormHandler.Hide()
    End Sub
End Class