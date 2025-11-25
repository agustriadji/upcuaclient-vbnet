<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DetailRecord
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxStartPressure = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TextBoxStartDate = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBoxOperator = New System.Windows.Forms.TextBox()
        Me.TextBoxSize = New System.Windows.Forms.TextBox()
        Me.TextBoxState = New System.Windows.Forms.TextBox()
        Me.TextBoxRunningDay = New System.Windows.Forms.TextBox()
        Me.TextBoxSensorId = New System.Windows.Forms.TextBox()
        Me.TextBoxBatchId = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.SplitContainerDetailRecord = New System.Windows.Forms.SplitContainer()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.CMBGroupingGraph = New System.Windows.Forms.ComboBox()
        Me.TabControlDetailRecord = New System.Windows.Forms.TabControl()
        Me.TabPageRecord = New System.Windows.Forms.TabPage()
        Me.DGVWatch = New System.Windows.Forms.DataGridView()
        Me.StartPressure = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Pressure = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.LeakPressure = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Timestamp = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TabPageGraph = New System.Windows.Forms.TabPage()
        Me.CartesianChart2 = New LiveCharts.WinForms.CartesianChart()
        Me.PanelBottom = New System.Windows.Forms.Panel()
        Me.BTNExport = New System.Windows.Forms.Button()
        Me.BTNClose = New System.Windows.Forms.Button()
        Me.BTNEndRecording = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        CType(Me.SplitContainerDetailRecord, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerDetailRecord.Panel1.SuspendLayout()
        Me.SplitContainerDetailRecord.Panel2.SuspendLayout()
        Me.SplitContainerDetailRecord.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.TabControlDetailRecord.SuspendLayout()
        Me.TabPageRecord.SuspendLayout()
        CType(Me.DGVWatch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPageGraph.SuspendLayout()
        Me.PanelBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.TextBoxStartPressure)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.TextBoxStartDate)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.TextBoxOperator)
        Me.GroupBox1.Controls.Add(Me.TextBoxSize)
        Me.GroupBox1.Controls.Add(Me.TextBoxState)
        Me.GroupBox1.Controls.Add(Me.TextBoxRunningDay)
        Me.GroupBox1.Controls.Add(Me.TextBoxSensorId)
        Me.GroupBox1.Controls.Add(Me.TextBoxBatchId)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 6, 4, 6)
        Me.GroupBox1.Size = New System.Drawing.Size(1099, 250)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Label1.Location = New System.Drawing.Point(32, 55)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(65, 20)
        Me.Label1.TabIndex = 18
        Me.Label1.Text = "Batch ID"
        '
        'TextBoxStartPressure
        '
        Me.TextBoxStartPressure.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TextBoxStartPressure.Location = New System.Drawing.Point(669, 131)
        Me.TextBoxStartPressure.Name = "TextBoxStartPressure"
        Me.TextBoxStartPressure.ReadOnly = True
        Me.TextBoxStartPressure.Size = New System.Drawing.Size(198, 27)
        Me.TextBoxStartPressure.TabIndex = 17
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Label6.Location = New System.Drawing.Point(488, 137)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(98, 20)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Start Pressure"
        '
        'TextBoxStartDate
        '
        Me.TextBoxStartDate.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TextBoxStartDate.Location = New System.Drawing.Point(668, 51)
        Me.TextBoxStartDate.Name = "TextBoxStartDate"
        Me.TextBoxStartDate.ReadOnly = True
        Me.TextBoxStartDate.Size = New System.Drawing.Size(198, 27)
        Me.TextBoxStartDate.TabIndex = 15
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Label5.Location = New System.Drawing.Point(488, 55)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(76, 20)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "Start Date"
        '
        'TextBoxOperator
        '
        Me.TextBoxOperator.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TextBoxOperator.Location = New System.Drawing.Point(210, 174)
        Me.TextBoxOperator.Name = "TextBoxOperator"
        Me.TextBoxOperator.ReadOnly = True
        Me.TextBoxOperator.Size = New System.Drawing.Size(198, 27)
        Me.TextBoxOperator.TabIndex = 13
        '
        'TextBoxSize
        '
        Me.TextBoxSize.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TextBoxSize.Location = New System.Drawing.Point(210, 131)
        Me.TextBoxSize.Name = "TextBoxSize"
        Me.TextBoxSize.ReadOnly = True
        Me.TextBoxSize.Size = New System.Drawing.Size(198, 27)
        Me.TextBoxSize.TabIndex = 12
        '
        'TextBoxState
        '
        Me.TextBoxState.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TextBoxState.Location = New System.Drawing.Point(669, 174)
        Me.TextBoxState.Name = "TextBoxState"
        Me.TextBoxState.ReadOnly = True
        Me.TextBoxState.Size = New System.Drawing.Size(198, 27)
        Me.TextBoxState.TabIndex = 11
        '
        'TextBoxRunningDay
        '
        Me.TextBoxRunningDay.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TextBoxRunningDay.Location = New System.Drawing.Point(669, 89)
        Me.TextBoxRunningDay.Name = "TextBoxRunningDay"
        Me.TextBoxRunningDay.ReadOnly = True
        Me.TextBoxRunningDay.Size = New System.Drawing.Size(198, 27)
        Me.TextBoxRunningDay.TabIndex = 10
        '
        'TextBoxSensorId
        '
        Me.TextBoxSensorId.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TextBoxSensorId.Location = New System.Drawing.Point(213, 87)
        Me.TextBoxSensorId.Name = "TextBoxSensorId"
        Me.TextBoxSensorId.ReadOnly = True
        Me.TextBoxSensorId.Size = New System.Drawing.Size(198, 27)
        Me.TextBoxSensorId.TabIndex = 9
        '
        'TextBoxBatchId
        '
        Me.TextBoxBatchId.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxBatchId.Location = New System.Drawing.Point(213, 44)
        Me.TextBoxBatchId.Name = "TextBoxBatchId"
        Me.TextBoxBatchId.ReadOnly = True
        Me.TextBoxBatchId.Size = New System.Drawing.Size(198, 27)
        Me.TextBoxBatchId.TabIndex = 8
        '
        'Label8
        '
        Me.Label8.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Label8.Location = New System.Drawing.Point(488, 96)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(99, 20)
        Me.Label8.TabIndex = 7
        Me.Label8.Text = "Running Days"
        '
        'Label7
        '
        Me.Label7.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Label7.Location = New System.Drawing.Point(488, 177)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(114, 20)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "State Operation"
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Label4.Location = New System.Drawing.Point(32, 177)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(69, 20)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Operator"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Label3.Location = New System.Drawing.Point(32, 137)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(36, 20)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Size"
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.Label2.Location = New System.Drawing.Point(32, 95)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(72, 20)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Sensor ID"
        '
        'SplitContainerDetailRecord
        '
        Me.SplitContainerDetailRecord.BackColor = System.Drawing.Color.Transparent
        Me.SplitContainerDetailRecord.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerDetailRecord.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainerDetailRecord.IsSplitterFixed = True
        Me.SplitContainerDetailRecord.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainerDetailRecord.Name = "SplitContainerDetailRecord"
        Me.SplitContainerDetailRecord.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainerDetailRecord.Panel1
        '
        Me.SplitContainerDetailRecord.Panel1.Controls.Add(Me.GroupBox1)
        '
        'SplitContainerDetailRecord.Panel2
        '
        Me.SplitContainerDetailRecord.Panel2.Controls.Add(Me.Panel2)
        Me.SplitContainerDetailRecord.Panel2.Controls.Add(Me.PanelBottom)
        Me.SplitContainerDetailRecord.Size = New System.Drawing.Size(1099, 753)
        Me.SplitContainerDetailRecord.SplitterDistance = 252
        Me.SplitContainerDetailRecord.SplitterWidth = 7
        Me.SplitContainerDetailRecord.TabIndex = 1
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.Transparent
        Me.Panel2.Controls.Add(Me.Panel1)
        Me.Panel2.Controls.Add(Me.TabControlDetailRecord)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Padding = New System.Windows.Forms.Padding(12, 14, 12, 14)
        Me.Panel2.Size = New System.Drawing.Size(1099, 429)
        Me.Panel2.TabIndex = 3
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Transparent
        Me.Panel1.Controls.Add(Me.CMBGroupingGraph)
        Me.Panel1.Location = New System.Drawing.Point(210, 2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(159, 38)
        Me.Panel1.TabIndex = 2
        '
        'CMBGroupingGraph
        '
        Me.CMBGroupingGraph.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.CMBGroupingGraph.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CMBGroupingGraph.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CMBGroupingGraph.ForeColor = System.Drawing.SystemColors.GrayText
        Me.CMBGroupingGraph.FormattingEnabled = True
        Me.CMBGroupingGraph.Items.AddRange(New Object() {"Default", "2m", "10m", "1h", "1d"})
        Me.CMBGroupingGraph.Location = New System.Drawing.Point(0, 10)
        Me.CMBGroupingGraph.Name = "CMBGroupingGraph"
        Me.CMBGroupingGraph.Size = New System.Drawing.Size(159, 28)
        Me.CMBGroupingGraph.TabIndex = 1
        '
        'TabControlDetailRecord
        '
        Me.TabControlDetailRecord.Controls.Add(Me.TabPageRecord)
        Me.TabControlDetailRecord.Controls.Add(Me.TabPageGraph)
        Me.TabControlDetailRecord.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlDetailRecord.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TabControlDetailRecord.Location = New System.Drawing.Point(12, 14)
        Me.TabControlDetailRecord.Name = "TabControlDetailRecord"
        Me.TabControlDetailRecord.SelectedIndex = 0
        Me.TabControlDetailRecord.Size = New System.Drawing.Size(1075, 401)
        Me.TabControlDetailRecord.TabIndex = 0
        '
        'TabPageRecord
        '
        Me.TabPageRecord.Controls.Add(Me.DGVWatch)
        Me.TabPageRecord.Location = New System.Drawing.Point(4, 29)
        Me.TabPageRecord.Name = "TabPageRecord"
        Me.TabPageRecord.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageRecord.Size = New System.Drawing.Size(1067, 368)
        Me.TabPageRecord.TabIndex = 0
        Me.TabPageRecord.Text = "Watch"
        Me.TabPageRecord.UseVisualStyleBackColor = True
        '
        'DGVWatch
        '
        Me.DGVWatch.AllowUserToAddRows = False
        Me.DGVWatch.AllowUserToDeleteRows = False
        Me.DGVWatch.AllowUserToResizeRows = False
        Me.DGVWatch.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVWatch.BackgroundColor = System.Drawing.SystemColors.ButtonFace
        Me.DGVWatch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVWatch.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.StartPressure, Me.Pressure, Me.LeakPressure, Me.Timestamp})
        Me.DGVWatch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVWatch.Location = New System.Drawing.Point(3, 3)
        Me.DGVWatch.MultiSelect = False
        Me.DGVWatch.Name = "DGVWatch"
        Me.DGVWatch.ReadOnly = True
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.ButtonHighlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVWatch.RowHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DGVWatch.RowHeadersVisible = False
        Me.DGVWatch.RowHeadersWidth = 51
        Me.DGVWatch.RowTemplate.Height = 24
        Me.DGVWatch.ShowCellErrors = False
        Me.DGVWatch.ShowCellToolTips = False
        Me.DGVWatch.ShowEditingIcon = False
        Me.DGVWatch.ShowRowErrors = False
        Me.DGVWatch.Size = New System.Drawing.Size(1061, 362)
        Me.DGVWatch.TabIndex = 0
        '
        'StartPressure
        '
        Me.StartPressure.HeaderText = "Start Pressure"
        Me.StartPressure.MinimumWidth = 6
        Me.StartPressure.Name = "StartPressure"
        Me.StartPressure.ReadOnly = True
        '
        'Pressure
        '
        Me.Pressure.HeaderText = "Current Pressure"
        Me.Pressure.MinimumWidth = 6
        Me.Pressure.Name = "Pressure"
        Me.Pressure.ReadOnly = True
        '
        'LeakPressure
        '
        Me.LeakPressure.HeaderText = "Leak Pressure"
        Me.LeakPressure.MinimumWidth = 6
        Me.LeakPressure.Name = "LeakPressure"
        Me.LeakPressure.ReadOnly = True
        '
        'Timestamp
        '
        Me.Timestamp.HeaderText = "Timestamp"
        Me.Timestamp.MinimumWidth = 6
        Me.Timestamp.Name = "Timestamp"
        Me.Timestamp.ReadOnly = True
        '
        'TabPageGraph
        '
        Me.TabPageGraph.Controls.Add(Me.CartesianChart2)
        Me.TabPageGraph.Location = New System.Drawing.Point(4, 29)
        Me.TabPageGraph.Name = "TabPageGraph"
        Me.TabPageGraph.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageGraph.Size = New System.Drawing.Size(1067, 337)
        Me.TabPageGraph.TabIndex = 1
        Me.TabPageGraph.Text = "Graph"
        Me.TabPageGraph.UseVisualStyleBackColor = True
        '
        'CartesianChart2
        '
        Me.CartesianChart2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CartesianChart2.Location = New System.Drawing.Point(3, 3)
        Me.CartesianChart2.Name = "CartesianChart2"
        Me.CartesianChart2.Size = New System.Drawing.Size(1061, 331)
        Me.CartesianChart2.TabIndex = 1
        Me.CartesianChart2.Text = "Graph"
        '
        'PanelBottom
        '
        Me.PanelBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.PanelBottom.Controls.Add(Me.BTNExport)
        Me.PanelBottom.Controls.Add(Me.BTNClose)
        Me.PanelBottom.Controls.Add(Me.BTNEndRecording)
        Me.PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBottom.Location = New System.Drawing.Point(0, 429)
        Me.PanelBottom.Name = "PanelBottom"
        Me.PanelBottom.Padding = New System.Windows.Forms.Padding(25, 26, 25, 26)
        Me.PanelBottom.Size = New System.Drawing.Size(1099, 65)
        Me.PanelBottom.TabIndex = 0
        '
        'BTNExport
        '
        Me.BTNExport.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.BTNExport.Location = New System.Drawing.Point(773, 17)
        Me.BTNExport.Name = "BTNExport"
        Me.BTNExport.Size = New System.Drawing.Size(90, 34)
        Me.BTNExport.TabIndex = 4
        Me.BTNExport.Text = "Export"
        Me.BTNExport.UseVisualStyleBackColor = True
        '
        'BTNClose
        '
        Me.BTNClose.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.BTNClose.Location = New System.Drawing.Point(994, 17)
        Me.BTNClose.Name = "BTNClose"
        Me.BTNClose.Size = New System.Drawing.Size(90, 34)
        Me.BTNClose.TabIndex = 2
        Me.BTNClose.Text = "Close"
        Me.BTNClose.UseVisualStyleBackColor = True
        '
        'BTNEndRecording
        '
        Me.BTNEndRecording.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.BTNEndRecording.Location = New System.Drawing.Point(883, 17)
        Me.BTNEndRecording.Name = "BTNEndRecording"
        Me.BTNEndRecording.Size = New System.Drawing.Size(90, 34)
        Me.BTNEndRecording.TabIndex = 3
        Me.BTNEndRecording.Text = "End Record"
        Me.BTNEndRecording.UseVisualStyleBackColor = True
        '
        'DetailRecord
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.AutoValidate = System.Windows.Forms.AutoValidate.Disable
        Me.ClientSize = New System.Drawing.Size(1099, 753)
        Me.ControlBox = False
        Me.Controls.Add(Me.SplitContainerDetailRecord)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(3, 7, 3, 7)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(1117, 800)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(1117, 800)
        Me.Name = "DetailRecord"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Detail Record"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.SplitContainerDetailRecord.Panel1.ResumeLayout(False)
        Me.SplitContainerDetailRecord.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerDetailRecord, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerDetailRecord.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.TabControlDetailRecord.ResumeLayout(False)
        Me.TabPageRecord.ResumeLayout(False)
        CType(Me.DGVWatch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPageGraph.ResumeLayout(False)
        Me.PanelBottom.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label7 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents TextBoxRunningDay As TextBox
    Friend WithEvents TextBoxSensorId As TextBox
    Friend WithEvents TextBoxBatchId As TextBox
    Friend WithEvents TextBoxOperator As TextBox
    Friend WithEvents TextBoxSize As TextBox
    Friend WithEvents TextBoxState As TextBox
    Friend WithEvents SplitContainerDetailRecord As SplitContainer
    Friend WithEvents BTNClose As Button
    Friend WithEvents BTNEndRecording As Button
    Friend WithEvents BTNExport As Button
    Friend WithEvents TextBoxStartPressure As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents TextBoxStartDate As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents CartesianChart1 As LiveCharts.WinForms.CartesianChart
    Friend WithEvents TabControlDetailRecord As TabControl
    Friend WithEvents TabPageRecord As TabPage
    Friend WithEvents DGVWatch As DataGridView
    Friend WithEvents StartPressure As DataGridViewTextBoxColumn
    Friend WithEvents Pressure As DataGridViewTextBoxColumn
    Friend WithEvents LeakPressure As DataGridViewTextBoxColumn
    Friend WithEvents Timestamp As DataGridViewTextBoxColumn
    Friend WithEvents TabPageGraph As TabPage
    Friend WithEvents PanelBottom As Panel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents CMBGroupingGraph As ComboBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents CartesianChart2 As LiveCharts.WinForms.CartesianChart
    Friend WithEvents Label1 As Label
End Class
