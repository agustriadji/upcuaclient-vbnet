<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormNewRecord
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxBatch = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBoxSizeTire = New System.Windows.Forms.TextBox()
        Me.TextBoxOperator = New System.Windows.Forms.TextBox()
        Me.NumericUpDownAutoEndRecord = New System.Windows.Forms.NumericUpDown()
        Me.ButtonCreate = New System.Windows.Forms.Button()
        Me.ComboBoxSensorTire = New System.Windows.Forms.ComboBox()
        Me.ComboBoxSensorGuage = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        CType(Me.NumericUpDownAutoEndRecord, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(18, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(90, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Batch Name"
        '
        'TextBoxBatch
        '
        Me.TextBoxBatch.Location = New System.Drawing.Point(141, 36)
        Me.TextBoxBatch.Name = "TextBoxBatch"
        Me.TextBoxBatch.Size = New System.Drawing.Size(150, 27)
        Me.TextBoxBatch.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(18, 88)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(88, 20)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Sensor Tire*"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(18, 134)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(106, 20)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "End Recording"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(18, 181)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(36, 20)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "Size"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(18, 226)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(69, 20)
        Me.Label5.TabIndex = 6
        Me.Label5.Text = "Operator"
        '
        'TextBoxSizeTire
        '
        Me.TextBoxSizeTire.Location = New System.Drawing.Point(141, 181)
        Me.TextBoxSizeTire.Name = "TextBoxSizeTire"
        Me.TextBoxSizeTire.Size = New System.Drawing.Size(150, 27)
        Me.TextBoxSizeTire.TabIndex = 7
        '
        'TextBoxOperator
        '
        Me.TextBoxOperator.Location = New System.Drawing.Point(141, 226)
        Me.TextBoxOperator.MaximumSize = New System.Drawing.Size(150, 30)
        Me.TextBoxOperator.MinimumSize = New System.Drawing.Size(150, 30)
        Me.TextBoxOperator.Name = "TextBoxOperator"
        Me.TextBoxOperator.Size = New System.Drawing.Size(150, 30)
        Me.TextBoxOperator.TabIndex = 8
        '
        'NumericUpDownAutoEndRecord
        '
        Me.NumericUpDownAutoEndRecord.Location = New System.Drawing.Point(141, 134)
        Me.NumericUpDownAutoEndRecord.Name = "NumericUpDownAutoEndRecord"
        Me.NumericUpDownAutoEndRecord.Size = New System.Drawing.Size(150, 27)
        Me.NumericUpDownAutoEndRecord.TabIndex = 9
        '
        'ButtonCreate
        '
        Me.ButtonCreate.Location = New System.Drawing.Point(225, 340)
        Me.ButtonCreate.Name = "ButtonCreate"
        Me.ButtonCreate.Size = New System.Drawing.Size(80, 30)
        Me.ButtonCreate.TabIndex = 10
        Me.ButtonCreate.Text = "New"
        Me.ButtonCreate.UseVisualStyleBackColor = True
        '
        'ComboBoxSensorTire
        '
        Me.ComboBoxSensorTire.FormattingEnabled = True
        Me.ComboBoxSensorTire.Location = New System.Drawing.Point(141, 85)
        Me.ComboBoxSensorTire.Name = "ComboBoxSensorTire"
        Me.ComboBoxSensorTire.Size = New System.Drawing.Size(150, 28)
        Me.ComboBoxSensorTire.TabIndex = 11
        '
        'ComboBoxSensorGuage
        '
        Me.ComboBoxSensorGuage.FormattingEnabled = True
        Me.ComboBoxSensorGuage.Location = New System.Drawing.Point(140, 132)
        Me.ComboBoxSensorGuage.Name = "ComboBoxSensorGuage"
        Me.ComboBoxSensorGuage.Size = New System.Drawing.Size(150, 28)
        Me.ComboBoxSensorGuage.TabIndex = 13
        Me.ComboBoxSensorGuage.Visible = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(17, 135)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(87, 20)
        Me.Label6.TabIndex = 12
        Me.Label6.Text = "Sensor Leak"
        Me.Label6.Visible = False
        '
        'ButtonCancel
        '
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Location = New System.Drawing.Point(128, 340)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(80, 30)
        Me.ButtonCancel.TabIndex = 14
        Me.ButtonCancel.Text = "Close"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'FormNewRecord
        '
        Me.AcceptButton = Me.ButtonCreate
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonCancel
        Me.ClientSize = New System.Drawing.Size(312, 373)
        Me.ControlBox = False
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ComboBoxSensorGuage)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.ComboBoxSensorTire)
        Me.Controls.Add(Me.ButtonCreate)
        Me.Controls.Add(Me.NumericUpDownAutoEndRecord)
        Me.Controls.Add(Me.TextBoxOperator)
        Me.Controls.Add(Me.TextBoxSizeTire)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBoxBatch)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.MaximumSize = New System.Drawing.Size(330, 420)
        Me.MinimumSize = New System.Drawing.Size(330, 420)
        Me.Name = "FormNewRecord"
        Me.Padding = New System.Windows.Forms.Padding(6)
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "New Record"
        CType(Me.NumericUpDownAutoEndRecord, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents TextBoxBatch As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents TextBoxSizeTire As TextBox
    Friend WithEvents TextBoxOperator As TextBox
    Friend WithEvents NumericUpDownAutoEndRecord As NumericUpDown
    Friend WithEvents ButtonCreate As Button
    Friend WithEvents ComboBoxSensorTire As ComboBox
    Friend WithEvents ComboBoxSensorGuage As ComboBox
    Friend WithEvents Label6 As Label
    Friend WithEvents ButtonCancel As Button
End Class
