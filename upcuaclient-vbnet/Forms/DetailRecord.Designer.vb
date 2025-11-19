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
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
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
        Me.Label1 = New System.Windows.Forms.Label()
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
        Me.CartesianChartDetailRecord = New LiveCharts.WinForms.CartesianChart()
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
        Me.GroupBox1.AutoSize = True
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
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Size = New System.Drawing.Size(942, 275)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        '
        'TextBoxStartPressure
        '
        Me.TextBoxStartPressure.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.TextBoxStartPressure.Location = New System.Drawing.Point(529, 150)
        Me.TextBoxStartPressure.Name = "TextBoxStartPressure"
        Me.TextBoxStartPressure.ReadOnly = True
        Me.TextBoxStartPressure.Size = New System.Drawing.Size(157, 32)
        Me.TextBoxStartPressure.TabIndex = 17
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(386, 154)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(114, 23)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Start Pressure"
        '
        'TextBoxStartDate
        '
        Me.TextBoxStartDate.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.TextBoxStartDate.Location = New System.Drawing.Point(528, 38)
        Me.TextBoxStartDate.Name = "TextBoxStartDate"
        Me.TextBoxStartDate.ReadOnly = True
        Me.TextBoxStartDate.Size = New System.Drawing.Size(157, 32)
        Me.TextBoxStartDate.TabIndex = 15
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(386, 42)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(86, 23)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "Start Date"
        '
        'TextBoxOperator
        '
        Me.TextBoxOperator.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.TextBoxOperator.Location = New System.Drawing.Point(166, 213)
        Me.TextBoxOperator.Name = "TextBoxOperator"
        Me.TextBoxOperator.ReadOnly = True
        Me.TextBoxOperator.Size = New System.Drawing.Size(157, 32)
        Me.TextBoxOperator.TabIndex = 13
        '
        'TextBoxSize
        '
        Me.TextBoxSize.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.TextBoxSize.Location = New System.Drawing.Point(166, 150)
        Me.TextBoxSize.Name = "TextBoxSize"
        Me.TextBoxSize.ReadOnly = True
        Me.TextBoxSize.Size = New System.Drawing.Size(157, 32)
        Me.TextBoxSize.TabIndex = 12
        '
        'TextBoxState
        '
        Me.TextBoxState.Font = New System.Drawing.Font("Segoe UI Semibold", 11.0!, System.Drawing.FontStyle.Bold)
        Me.TextBoxState.Location = New System.Drawing.Point(529, 209)
        Me.TextBoxState.Name = "TextBoxState"
        Me.TextBoxState.ReadOnly = True
        Me.TextBoxState.Size = New System.Drawing.Size(157, 32)
        Me.TextBoxState.TabIndex = 11
        '
        'TextBoxRunningDay
        '
        Me.TextBoxRunningDay.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.TextBoxRunningDay.Location = New System.Drawing.Point(529, 94)
        Me.TextBoxRunningDay.Name = "TextBoxRunningDay"
        Me.TextBoxRunningDay.ReadOnly = True
        Me.TextBoxRunningDay.Size = New System.Drawing.Size(157, 32)
        Me.TextBoxRunningDay.TabIndex = 10
        '
        'TextBoxSensorId
        '
        Me.TextBoxSensorId.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.TextBoxSensorId.Location = New System.Drawing.Point(168, 92)
        Me.TextBoxSensorId.Name = "TextBoxSensorId"
        Me.TextBoxSensorId.ReadOnly = True
        Me.TextBoxSensorId.Size = New System.Drawing.Size(157, 32)
        Me.TextBoxSensorId.TabIndex = 9
        '
        'TextBoxBatchId
        '
        Me.TextBoxBatchId.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.TextBoxBatchId.Location = New System.Drawing.Point(168, 33)
        Me.TextBoxBatchId.Name = "TextBoxBatchId"
        Me.TextBoxBatchId.ReadOnly = True
        Me.TextBoxBatchId.Size = New System.Drawing.Size(157, 32)
        Me.TextBoxBatchId.TabIndex = 8
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(386, 99)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(115, 23)
        Me.Label8.TabIndex = 7
        Me.Label8.Text = "Running Days"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(386, 214)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(130, 23)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "State Operation"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(25, 214)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(79, 23)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Operator"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(25, 154)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(40, 23)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Size"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(25, 98)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 23)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Sensor ID"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(25, 42)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(75, 23)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Batch ID"
        '
        'SplitContainerDetailRecord
        '
        Me.SplitContainerDetailRecord.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerDetailRecord.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
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
        Me.SplitContainerDetailRecord.Size = New System.Drawing.Size(942, 753)
        Me.SplitContainerDetailRecord.SplitterDistance = 283
        Me.SplitContainerDetailRecord.SplitterWidth = 5
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
        Me.Panel2.Padding = New System.Windows.Forms.Padding(10)
        Me.Panel2.Size = New System.Drawing.Size(942, 389)
        Me.Panel2.TabIndex = 3
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Transparent
        Me.Panel1.Controls.Add(Me.CMBGroupingGraph)
        Me.Panel1.Location = New System.Drawing.Point(166, 2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(125, 35)
        Me.Panel1.TabIndex = 2
        '
        'CMBGroupingGraph
        '
        Me.CMBGroupingGraph.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.CMBGroupingGraph.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CMBGroupingGraph.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CMBGroupingGraph.ForeColor = System.Drawing.SystemColors.GrayText
        Me.CMBGroupingGraph.FormattingEnabled = True
        Me.CMBGroupingGraph.Items.AddRange(New Object() {"Default", "2m", "10m", "1h", "1d"})
        Me.CMBGroupingGraph.Location = New System.Drawing.Point(0, 4)
        Me.CMBGroupingGraph.Name = "CMBGroupingGraph"
        Me.CMBGroupingGraph.Size = New System.Drawing.Size(125, 31)
        Me.CMBGroupingGraph.TabIndex = 1
        '
        'TabControlDetailRecord
        '
        Me.TabControlDetailRecord.Controls.Add(Me.TabPageRecord)
        Me.TabControlDetailRecord.Controls.Add(Me.TabPageGraph)
        Me.TabControlDetailRecord.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlDetailRecord.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.TabControlDetailRecord.Location = New System.Drawing.Point(10, 10)
        Me.TabControlDetailRecord.Name = "TabControlDetailRecord"
        Me.TabControlDetailRecord.SelectedIndex = 0
        Me.TabControlDetailRecord.Size = New System.Drawing.Size(922, 369)
        Me.TabControlDetailRecord.TabIndex = 0
        '
        'TabPageRecord
        '
        Me.TabPageRecord.Controls.Add(Me.DGVWatch)
        Me.TabPageRecord.Location = New System.Drawing.Point(4, 37)
        Me.TabPageRecord.Name = "TabPageRecord"
        Me.TabPageRecord.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageRecord.Size = New System.Drawing.Size(914, 328)
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
        Me.DGVWatch.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DGVWatch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVWatch.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.StartPressure, Me.Pressure, Me.LeakPressure, Me.Timestamp})
        Me.DGVWatch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVWatch.Location = New System.Drawing.Point(3, 3)
        Me.DGVWatch.MultiSelect = False
        Me.DGVWatch.Name = "DGVWatch"
        Me.DGVWatch.ReadOnly = True
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ButtonHighlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVWatch.RowHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.DGVWatch.RowHeadersVisible = False
        Me.DGVWatch.RowHeadersWidth = 51
        Me.DGVWatch.RowTemplate.Height = 24
        Me.DGVWatch.ShowCellErrors = False
        Me.DGVWatch.ShowCellToolTips = False
        Me.DGVWatch.ShowEditingIcon = False
        Me.DGVWatch.ShowRowErrors = False
        Me.DGVWatch.Size = New System.Drawing.Size(908, 322)
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
        Me.TabPageGraph.Controls.Add(Me.CartesianChartDetailRecord)
        Me.TabPageGraph.Location = New System.Drawing.Point(4, 37)
        Me.TabPageGraph.Name = "TabPageGraph"
        Me.TabPageGraph.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageGraph.Size = New System.Drawing.Size(914, 328)
        Me.TabPageGraph.TabIndex = 1
        Me.TabPageGraph.Text = "Graph"
        Me.TabPageGraph.UseVisualStyleBackColor = True
        '
        'CartesianChartDetailRecord
        '
        Me.CartesianChartDetailRecord.Location = New System.Drawing.Point(6, 6)
        Me.CartesianChartDetailRecord.Name = "CartesianChartDetailRecord"
        Me.CartesianChartDetailRecord.Size = New System.Drawing.Size(902, 316)
        Me.CartesianChartDetailRecord.TabIndex = 0
        Me.CartesianChartDetailRecord.Text = "CartesianChartDetailRecord"
        '
        'PanelBottom
        '
        Me.PanelBottom.AutoSize = True
        Me.PanelBottom.Controls.Add(Me.BTNExport)
        Me.PanelBottom.Controls.Add(Me.BTNClose)
        Me.PanelBottom.Controls.Add(Me.BTNEndRecording)
        Me.PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBottom.Location = New System.Drawing.Point(0, 389)
        Me.PanelBottom.Name = "PanelBottom"
        Me.PanelBottom.Padding = New System.Windows.Forms.Padding(20)
        Me.PanelBottom.Size = New System.Drawing.Size(942, 76)
        Me.PanelBottom.TabIndex = 0
        '
        'BTNExport
        '
        Me.BTNExport.Location = New System.Drawing.Point(485, 13)
        Me.BTNExport.Name = "BTNExport"
        Me.BTNExport.Size = New System.Drawing.Size(124, 40)
        Me.BTNExport.TabIndex = 4
        Me.BTNExport.Text = "Export"
        Me.BTNExport.UseVisualStyleBackColor = True
        '
        'BTNClose
        '
        Me.BTNClose.Location = New System.Drawing.Point(800, 13)
        Me.BTNClose.Name = "BTNClose"
        Me.BTNClose.Size = New System.Drawing.Size(124, 40)
        Me.BTNClose.TabIndex = 2
        Me.BTNClose.Text = "Close"
        Me.BTNClose.UseVisualStyleBackColor = True
        '
        'BTNEndRecording
        '
        Me.BTNEndRecording.Location = New System.Drawing.Point(644, 13)
        Me.BTNEndRecording.Name = "BTNEndRecording"
        Me.BTNEndRecording.Size = New System.Drawing.Size(124, 40)
        Me.BTNEndRecording.TabIndex = 3
        Me.BTNEndRecording.Text = "End Record"
        Me.BTNEndRecording.UseVisualStyleBackColor = True
        '
        'DetailRecord
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 23.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(942, 753)
        Me.Controls.Add(Me.SplitContainerDetailRecord)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Margin = New System.Windows.Forms.Padding(3, 5, 3, 5)
        Me.MaximizeBox = False
        Me.Name = "DetailRecord"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Detail Record"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.SplitContainerDetailRecord.Panel1.ResumeLayout(False)
        Me.SplitContainerDetailRecord.Panel1.PerformLayout()
        Me.SplitContainerDetailRecord.Panel2.ResumeLayout(False)
        Me.SplitContainerDetailRecord.Panel2.PerformLayout()
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
    Friend WithEvents Label1 As Label
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
    Friend WithEvents CartesianChartDetailRecord As LiveCharts.WinForms.CartesianChart
End Class
