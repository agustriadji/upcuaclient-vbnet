<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormConfigManager
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormConfigManager))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxThresholdPressureGauge = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LabelInfoInterval = New System.Windows.Forms.Label()
        Me.NumericUpDownInterval = New System.Windows.Forms.NumericUpDown()
        Me.LabelInternal = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.ButtonSelectObjectOpc = New System.Windows.Forms.Button()
        Me.ComboBoxSelectObjectOpc = New System.Windows.Forms.ComboBox()
        Me.DGVSelectedNodeOpc = New System.Windows.Forms.DataGridView()
        Me.NodeText = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NodeId = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NodeType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.LabelExampleHostOpc = New System.Windows.Forms.Label()
        Me.LabelMessageStateNamespaceOpc = New System.Windows.Forms.Label()
        Me.ButtonTestNamespaceOpc = New System.Windows.Forms.Button()
        Me.ButtonBrowseNodes = New System.Windows.Forms.Button()
        Me.TextBoxNamespaceOpc = New System.Windows.Forms.TextBox()
        Me.LabelNamespaceOpc = New System.Windows.Forms.Label()
        Me.LabelMessageStateHostOpc = New System.Windows.Forms.Label()
        Me.ButtonTestHostOpc = New System.Windows.Forms.Button()
        Me.TextBoxHostOpc = New System.Windows.Forms.TextBox()
        Me.LabelHostOpc = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.LabelExampleHostDB = New System.Windows.Forms.Label()
        Me.LabelMessageStateHostDB = New System.Windows.Forms.Label()
        Me.LabelHostDB = New System.Windows.Forms.Label()
        Me.ButtonTestHostDB = New System.Windows.Forms.Button()
        Me.TextBoxHostDB = New System.Windows.Forms.TextBox()
        Me.ButtonSaveForm = New System.Windows.Forms.Button()
        Me.ButtonCancelForm = New System.Windows.Forms.Button()
        Me.ButtonClear = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        CType(Me.NumericUpDownInterval, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        CType(Me.DGVSelectedNodeOpc, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.TextBoxThresholdPressureGauge)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.LabelInfoInterval)
        Me.GroupBox1.Controls.Add(Me.NumericUpDownInterval)
        Me.GroupBox1.Controls.Add(Me.LabelInternal)
        Me.GroupBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(12, 21)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(777, 70)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "General"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 7.8!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label1.Location = New System.Drawing.Point(433, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 17)
        Me.Label1.TabIndex = 24
        Me.Label1.Text = "(Default: 0.2)"
        Me.Label1.Visible = False
        '
        'TextBoxThresholdPressureGauge
        '
        Me.TextBoxThresholdPressureGauge.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxThresholdPressureGauge.Location = New System.Drawing.Point(358, 34)
        Me.TextBoxThresholdPressureGauge.Name = "TextBoxThresholdPressureGauge"
        Me.TextBoxThresholdPressureGauge.Size = New System.Drawing.Size(67, 27)
        Me.TextBoxThresholdPressureGauge.TabIndex = 23
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Label2.Location = New System.Drawing.Point(186, 37)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(166, 20)
        Me.Label2.TabIndex = 21
        Me.Label2.Text = "Threshold Leak Pressure"
        '
        'LabelInfoInterval
        '
        Me.LabelInfoInterval.AutoSize = True
        Me.LabelInfoInterval.BackColor = System.Drawing.Color.Transparent
        Me.LabelInfoInterval.Font = New System.Drawing.Font("Segoe UI", 7.8!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelInfoInterval.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.LabelInfoInterval.Location = New System.Drawing.Point(115, 37)
        Me.LabelInfoInterval.Name = "LabelInfoInterval"
        Me.LabelInfoInterval.Size = New System.Drawing.Size(59, 17)
        Me.LabelInfoInterval.TabIndex = 20
        Me.LabelInfoInterval.Text = "(Minutes)"
        Me.LabelInfoInterval.Visible = False
        '
        'NumericUpDownInterval
        '
        Me.NumericUpDownInterval.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.NumericUpDownInterval.Location = New System.Drawing.Point(68, 34)
        Me.NumericUpDownInterval.Name = "NumericUpDownInterval"
        Me.NumericUpDownInterval.Size = New System.Drawing.Size(39, 27)
        Me.NumericUpDownInterval.TabIndex = 1
        '
        'LabelInternal
        '
        Me.LabelInternal.AutoSize = True
        Me.LabelInternal.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LabelInternal.Location = New System.Drawing.Point(5, 35)
        Me.LabelInternal.Name = "LabelInternal"
        Me.LabelInternal.Size = New System.Drawing.Size(58, 20)
        Me.LabelInternal.TabIndex = 0
        Me.LabelInternal.Text = "Interval"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.ButtonSelectObjectOpc)
        Me.GroupBox2.Controls.Add(Me.ComboBoxSelectObjectOpc)
        Me.GroupBox2.Controls.Add(Me.DGVSelectedNodeOpc)
        Me.GroupBox2.Controls.Add(Me.LabelExampleHostOpc)
        Me.GroupBox2.Controls.Add(Me.LabelMessageStateNamespaceOpc)
        Me.GroupBox2.Controls.Add(Me.ButtonTestNamespaceOpc)
        Me.GroupBox2.Controls.Add(Me.ButtonBrowseNodes)
        Me.GroupBox2.Controls.Add(Me.TextBoxNamespaceOpc)
        Me.GroupBox2.Controls.Add(Me.LabelNamespaceOpc)
        Me.GroupBox2.Controls.Add(Me.LabelMessageStateHostOpc)
        Me.GroupBox2.Controls.Add(Me.ButtonTestHostOpc)
        Me.GroupBox2.Controls.Add(Me.TextBoxHostOpc)
        Me.GroupBox2.Controls.Add(Me.LabelHostOpc)
        Me.GroupBox2.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(12, 97)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(776, 213)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "OPC Connection"
        '
        'ButtonSelectObjectOpc
        '
        Me.ButtonSelectObjectOpc.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonSelectObjectOpc.Location = New System.Drawing.Point(655, 35)
        Me.ButtonSelectObjectOpc.Name = "ButtonSelectObjectOpc"
        Me.ButtonSelectObjectOpc.Size = New System.Drawing.Size(71, 26)
        Me.ButtonSelectObjectOpc.TabIndex = 22
        Me.ButtonSelectObjectOpc.Text = "Add"
        Me.ButtonSelectObjectOpc.UseVisualStyleBackColor = True
        '
        'ComboBoxSelectObjectOpc
        '
        Me.ComboBoxSelectObjectOpc.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ComboBoxSelectObjectOpc.FormattingEnabled = True
        Me.ComboBoxSelectObjectOpc.Location = New System.Drawing.Point(476, 37)
        Me.ComboBoxSelectObjectOpc.Name = "ComboBoxSelectObjectOpc"
        Me.ComboBoxSelectObjectOpc.Size = New System.Drawing.Size(160, 28)
        Me.ComboBoxSelectObjectOpc.TabIndex = 21
        '
        'DGVSelectedNodeOpc
        '
        Me.DGVSelectedNodeOpc.AllowUserToAddRows = False
        Me.DGVSelectedNodeOpc.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVSelectedNodeOpc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVSelectedNodeOpc.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.NodeText, Me.NodeId, Me.NodeType})
        Me.DGVSelectedNodeOpc.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.DGVSelectedNodeOpc.Location = New System.Drawing.Point(3, 130)
        Me.DGVSelectedNodeOpc.Name = "DGVSelectedNodeOpc"
        Me.DGVSelectedNodeOpc.ReadOnly = True
        Me.DGVSelectedNodeOpc.RowHeadersVisible = False
        Me.DGVSelectedNodeOpc.RowHeadersWidth = 51
        Me.DGVSelectedNodeOpc.RowTemplate.Height = 24
        Me.DGVSelectedNodeOpc.ShowCellToolTips = False
        Me.DGVSelectedNodeOpc.Size = New System.Drawing.Size(770, 80)
        Me.DGVSelectedNodeOpc.TabIndex = 20
        '
        'NodeText
        '
        Me.NodeText.HeaderText = "Name"
        Me.NodeText.MinimumWidth = 6
        Me.NodeText.Name = "NodeText"
        Me.NodeText.ReadOnly = True
        '
        'NodeId
        '
        Me.NodeId.HeaderText = "Node ID"
        Me.NodeId.MinimumWidth = 6
        Me.NodeId.Name = "NodeId"
        Me.NodeId.ReadOnly = True
        '
        'NodeType
        '
        Me.NodeType.HeaderText = "Type"
        Me.NodeType.MinimumWidth = 6
        Me.NodeType.Name = "NodeType"
        Me.NodeType.ReadOnly = True
        '
        'LabelExampleHostOpc
        '
        Me.LabelExampleHostOpc.AutoSize = True
        Me.LabelExampleHostOpc.BackColor = System.Drawing.Color.Transparent
        Me.LabelExampleHostOpc.Font = New System.Drawing.Font("Segoe UI", 7.8!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelExampleHostOpc.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.LabelExampleHostOpc.Location = New System.Drawing.Point(56, 65)
        Me.LabelExampleHostOpc.Name = "LabelExampleHostOpc"
        Me.LabelExampleHostOpc.Size = New System.Drawing.Size(181, 17)
        Me.LabelExampleHostOpc.TabIndex = 18
        Me.LabelExampleHostOpc.Text = "Ex: opc.tcp<Localhost>:<Port>"
        Me.LabelExampleHostOpc.Visible = False
        '
        'LabelMessageStateNamespaceOpc
        '
        Me.LabelMessageStateNamespaceOpc.AutoSize = True
        Me.LabelMessageStateNamespaceOpc.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.LabelMessageStateNamespaceOpc.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LabelMessageStateNamespaceOpc.Location = New System.Drawing.Point(476, 67)
        Me.LabelMessageStateNamespaceOpc.Name = "LabelMessageStateNamespaceOpc"
        Me.LabelMessageStateNamespaceOpc.Size = New System.Drawing.Size(203, 20)
        Me.LabelMessageStateNamespaceOpc.TabIndex = 9
        Me.LabelMessageStateNamespaceOpc.Text = "Message For Coonection Opc"
        Me.LabelMessageStateNamespaceOpc.Visible = False
        '
        'ButtonTestNamespaceOpc
        '
        Me.ButtonTestNamespaceOpc.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.ButtonTestNamespaceOpc.Location = New System.Drawing.Point(678, 32)
        Me.ButtonTestNamespaceOpc.Name = "ButtonTestNamespaceOpc"
        Me.ButtonTestNamespaceOpc.Size = New System.Drawing.Size(75, 35)
        Me.ButtonTestNamespaceOpc.TabIndex = 12
        Me.ButtonTestNamespaceOpc.Text = "Test"
        Me.ButtonTestNamespaceOpc.UseVisualStyleBackColor = True
        Me.ButtonTestNamespaceOpc.Visible = False
        '
        'ButtonBrowseNodes
        '
        Me.ButtonBrowseNodes.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.ButtonBrowseNodes.Location = New System.Drawing.Point(678, 74)
        Me.ButtonBrowseNodes.Name = "ButtonBrowseNodes"
        Me.ButtonBrowseNodes.Size = New System.Drawing.Size(75, 35)
        Me.ButtonBrowseNodes.TabIndex = 13
        Me.ButtonBrowseNodes.Text = "Browse"
        Me.ButtonBrowseNodes.UseVisualStyleBackColor = True
        Me.ButtonBrowseNodes.Visible = False
        '
        'TextBoxNamespaceOpc
        '
        Me.TextBoxNamespaceOpc.Location = New System.Drawing.Point(476, 37)
        Me.TextBoxNamespaceOpc.Name = "TextBoxNamespaceOpc"
        Me.TextBoxNamespaceOpc.Size = New System.Drawing.Size(185, 27)
        Me.TextBoxNamespaceOpc.TabIndex = 11
        Me.TextBoxNamespaceOpc.Visible = False
        '
        'LabelNamespaceOpc
        '
        Me.LabelNamespaceOpc.AutoSize = True
        Me.LabelNamespaceOpc.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LabelNamespaceOpc.Location = New System.Drawing.Point(403, 43)
        Me.LabelNamespaceOpc.Name = "LabelNamespaceOpc"
        Me.LabelNamespaceOpc.Size = New System.Drawing.Size(59, 20)
        Me.LabelNamespaceOpc.TabIndex = 10
        Me.LabelNamespaceOpc.Text = "Objects"
        '
        'LabelMessageStateHostOpc
        '
        Me.LabelMessageStateHostOpc.AutoSize = True
        Me.LabelMessageStateHostOpc.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.LabelMessageStateHostOpc.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LabelMessageStateHostOpc.Location = New System.Drawing.Point(56, 88)
        Me.LabelMessageStateHostOpc.Name = "LabelMessageStateHostOpc"
        Me.LabelMessageStateHostOpc.Size = New System.Drawing.Size(203, 20)
        Me.LabelMessageStateHostOpc.TabIndex = 6
        Me.LabelMessageStateHostOpc.Text = "Message For Coonection Opc"
        Me.LabelMessageStateHostOpc.Visible = False
        '
        'ButtonTestHostOpc
        '
        Me.ButtonTestHostOpc.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.ButtonTestHostOpc.Location = New System.Drawing.Point(260, 38)
        Me.ButtonTestHostOpc.Name = "ButtonTestHostOpc"
        Me.ButtonTestHostOpc.Size = New System.Drawing.Size(76, 26)
        Me.ButtonTestHostOpc.TabIndex = 8
        Me.ButtonTestHostOpc.Text = "Test"
        Me.ButtonTestHostOpc.UseVisualStyleBackColor = True
        '
        'TextBoxHostOpc
        '
        Me.TextBoxHostOpc.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxHostOpc.Location = New System.Drawing.Point(59, 38)
        Me.TextBoxHostOpc.Name = "TextBoxHostOpc"
        Me.TextBoxHostOpc.Size = New System.Drawing.Size(185, 27)
        Me.TextBoxHostOpc.TabIndex = 7
        '
        'LabelHostOpc
        '
        Me.LabelHostOpc.AutoSize = True
        Me.LabelHostOpc.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LabelHostOpc.Location = New System.Drawing.Point(6, 44)
        Me.LabelHostOpc.Name = "LabelHostOpc"
        Me.LabelHostOpc.Size = New System.Drawing.Size(40, 20)
        Me.LabelHostOpc.TabIndex = 6
        Me.LabelHostOpc.Text = "Host"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.LabelExampleHostDB)
        Me.GroupBox3.Controls.Add(Me.LabelMessageStateHostDB)
        Me.GroupBox3.Controls.Add(Me.LabelHostDB)
        Me.GroupBox3.Controls.Add(Me.ButtonTestHostDB)
        Me.GroupBox3.Controls.Add(Me.TextBoxHostDB)
        Me.GroupBox3.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(15, 317)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(776, 134)
        Me.GroupBox3.TabIndex = 2
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Database Connection"
        '
        'LabelExampleHostDB
        '
        Me.LabelExampleHostDB.AutoSize = True
        Me.LabelExampleHostDB.BackColor = System.Drawing.Color.Transparent
        Me.LabelExampleHostDB.Font = New System.Drawing.Font("Segoe UI", 7.8!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelExampleHostDB.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.LabelExampleHostDB.Location = New System.Drawing.Point(56, 66)
        Me.LabelExampleHostDB.Name = "LabelExampleHostDB"
        Me.LabelExampleHostDB.Size = New System.Drawing.Size(113, 17)
        Me.LabelExampleHostDB.TabIndex = 21
        Me.LabelExampleHostDB.Text = "Ex: url_to_database"
        Me.LabelExampleHostDB.Visible = False
        '
        'LabelMessageStateHostDB
        '
        Me.LabelMessageStateHostDB.AutoSize = True
        Me.LabelMessageStateHostDB.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.LabelMessageStateHostDB.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LabelMessageStateHostDB.Location = New System.Drawing.Point(56, 92)
        Me.LabelMessageStateHostDB.Name = "LabelMessageStateHostDB"
        Me.LabelMessageStateHostDB.Size = New System.Drawing.Size(196, 20)
        Me.LabelMessageStateHostDB.TabIndex = 9
        Me.LabelMessageStateHostDB.Text = "Message For Coonection DB"
        Me.LabelMessageStateHostDB.Visible = False
        '
        'LabelHostDB
        '
        Me.LabelHostDB.AutoSize = True
        Me.LabelHostDB.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.LabelHostDB.Location = New System.Drawing.Point(6, 45)
        Me.LabelHostDB.Name = "LabelHostDB"
        Me.LabelHostDB.Size = New System.Drawing.Size(40, 20)
        Me.LabelHostDB.TabIndex = 9
        Me.LabelHostDB.Text = "Host"
        '
        'ButtonTestHostDB
        '
        Me.ButtonTestHostDB.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.ButtonTestHostDB.Location = New System.Drawing.Point(370, 37)
        Me.ButtonTestHostDB.Name = "ButtonTestHostDB"
        Me.ButtonTestHostDB.Size = New System.Drawing.Size(71, 26)
        Me.ButtonTestHostDB.TabIndex = 11
        Me.ButtonTestHostDB.Text = "Test"
        Me.ButtonTestHostDB.UseVisualStyleBackColor = True
        '
        'TextBoxHostDB
        '
        Me.TextBoxHostDB.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxHostDB.Location = New System.Drawing.Point(59, 39)
        Me.TextBoxHostDB.Name = "TextBoxHostDB"
        Me.TextBoxHostDB.Size = New System.Drawing.Size(294, 27)
        Me.TextBoxHostDB.TabIndex = 10
        '
        'ButtonSaveForm
        '
        Me.ButtonSaveForm.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ButtonSaveForm.Location = New System.Drawing.Point(711, 458)
        Me.ButtonSaveForm.Name = "ButtonSaveForm"
        Me.ButtonSaveForm.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButtonSaveForm.Size = New System.Drawing.Size(80, 35)
        Me.ButtonSaveForm.TabIndex = 3
        Me.ButtonSaveForm.Text = "Save"
        Me.ButtonSaveForm.UseVisualStyleBackColor = True
        '
        'ButtonCancelForm
        '
        Me.ButtonCancelForm.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ButtonCancelForm.Location = New System.Drawing.Point(619, 458)
        Me.ButtonCancelForm.Name = "ButtonCancelForm"
        Me.ButtonCancelForm.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButtonCancelForm.Size = New System.Drawing.Size(80, 35)
        Me.ButtonCancelForm.TabIndex = 4
        Me.ButtonCancelForm.Text = "Cancel"
        Me.ButtonCancelForm.UseVisualStyleBackColor = True
        '
        'ButtonClear
        '
        Me.ButtonClear.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ButtonClear.Location = New System.Drawing.Point(527, 458)
        Me.ButtonClear.Name = "ButtonClear"
        Me.ButtonClear.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButtonClear.Size = New System.Drawing.Size(80, 35)
        Me.ButtonClear.TabIndex = 5
        Me.ButtonClear.Text = "Clear"
        Me.ButtonClear.UseVisualStyleBackColor = True
        '
        'FormConfigManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 498)
        Me.Controls.Add(Me.ButtonClear)
        Me.Controls.Add(Me.ButtonCancelForm)
        Me.Controls.Add(Me.ButtonSaveForm)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximumSize = New System.Drawing.Size(818, 545)
        Me.MinimumSize = New System.Drawing.Size(818, 545)
        Me.Name = "FormConfigManager"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Config Manager"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.NumericUpDownInterval, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.DGVSelectedNodeOpc, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents LabelInternal As Label
    Friend WithEvents NumericUpDownInterval As NumericUpDown
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents LabelMessageStateHostOpc As Label
    Friend WithEvents ButtonTestHostOpc As Button
    Friend WithEvents TextBoxHostOpc As TextBox
    Friend WithEvents LabelHostOpc As Label
    Friend WithEvents LabelMessageStateHostDB As Label
    Friend WithEvents LabelHostDB As Label
    Friend WithEvents ButtonTestHostDB As Button
    Friend WithEvents TextBoxHostDB As TextBox
    Friend WithEvents ButtonSaveForm As Button
    Friend WithEvents ButtonCancelForm As Button
    Friend WithEvents LabelMessageStateNamespaceOpc As Label
    Friend WithEvents ButtonTestNamespaceOpc As Button
    Friend WithEvents TextBoxNamespaceOpc As TextBox
    Friend WithEvents LabelNamespaceOpc As Label
    Friend WithEvents LabelInfoInterval As Label
    Friend WithEvents LabelExampleHostOpc As Label
    Friend WithEvents DGVSelectedNodeOpc As DataGridView
    Friend WithEvents LabelExampleHostDB As Label
    Friend WithEvents ButtonBrowseNodes As Button
    Friend WithEvents ButtonSelectObjectOpc As Button
    Friend WithEvents ComboBoxSelectObjectOpc As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBoxThresholdPressureGauge As TextBox
    Friend WithEvents NodeText As DataGridViewTextBoxColumn
    Friend WithEvents NodeId As DataGridViewTextBoxColumn
    Friend WithEvents NodeType As DataGridViewTextBoxColumn
    Friend WithEvents ButtonClear As Button
End Class
