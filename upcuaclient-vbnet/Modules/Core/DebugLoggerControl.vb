Imports System.Windows.Forms

Public Class DebugLoggerControl
    Inherits UserControl

    Private WithEvents txtLog As TextBox

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.txtLog = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'txtLog
        '
        Me.txtLog.BackColor = System.Drawing.Color.Black
        Me.txtLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtLog.Font = New System.Drawing.Font("Consolas", 9.0!)
        Me.txtLog.ForeColor = System.Drawing.Color.LightGreen
        Me.txtLog.Location = New System.Drawing.Point(0, 0)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ReadOnly = True
        Me.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtLog.Size = New System.Drawing.Size(400, 300)
        Me.txtLog.TabIndex = 0
        '
        'DebugLoggerControl
        '
        Me.Controls.Add(Me.txtLog)
        Me.Name = "DebugLoggerControl"
        Me.Size = New System.Drawing.Size(400, 300)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Public Sub AppendLog(message As String)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() AppendLog(message))
            Return
        End If

        txtLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}")
        txtLog.SelectionStart = txtLog.Text.Length
        txtLog.ScrollToCaret()
    End Sub

    Public Sub ClearLog()
        If Me.InvokeRequired Then
            Me.Invoke(Sub() ClearLog())
            Return
        End If

        txtLog.Clear()
    End Sub

    Private Sub txtLog_TextChanged(sender As Object, e As EventArgs) Handles txtLog.TextChanged

    End Sub
End Class