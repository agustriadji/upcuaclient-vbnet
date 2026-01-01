<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainFormNew
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainFormNew))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TabControlMain = New System.Windows.Forms.TabControl()
        Me.TabPageSensorState = New System.Windows.Forms.TabPage()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.DTG1_NameSensor = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DTG1_StatusSensor = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TabPageRecording = New System.Windows.Forms.TabPage()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.ButtonRefresh = New System.Windows.Forms.Button()
        Me.DGVRecording = New System.Windows.Forms.DataGridView()
        Me.BatchId = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SensorId = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.RunningDay = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SizeTire = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.State = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CreatedAt = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.UpdatedAt = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Delete = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButtonDebug = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButtonAlert = New System.Windows.Forms.ToolStripButton()
        Me.TextBoxOutputDebug = New System.Windows.Forms.TextBox()
        Me.TextBoxOutputAlert = New System.Windows.Forms.TextBox()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigManagerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddNewSensorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddNewRecordingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutAirLMToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FeedbackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FeedbackToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabelOPC = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabelDB = New System.Windows.Forms.ToolStripStatusLabel()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TabControlMain.SuspendLayout()
        Me.TabPageSensorState.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPageRecording.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.DGVRecording, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(4, 32)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TabControlMain)
        Me.SplitContainer1.Panel1.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.SplitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SplitContainer1.Panel2Collapsed = True
        Me.SplitContainer1.Size = New System.Drawing.Size(1174, 491)
        Me.SplitContainer1.SplitterDistance = 361
        Me.SplitContainer1.TabIndex = 2
        '
        'TabControlMain
        '
        Me.TabControlMain.Controls.Add(Me.TabPageSensorState)
        Me.TabControlMain.Controls.Add(Me.TabPageRecording)
        Me.TabControlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlMain.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TabControlMain.Location = New System.Drawing.Point(0, 10)
        Me.TabControlMain.Margin = New System.Windows.Forms.Padding(0)
        Me.TabControlMain.Name = "TabControlMain"
        Me.TabControlMain.SelectedIndex = 0
        Me.TabControlMain.Size = New System.Drawing.Size(1174, 481)
        Me.TabControlMain.TabIndex = 1
        '
        'TabPageSensorState
        '
        Me.TabPageSensorState.Controls.Add(Me.DataGridView1)
        Me.TabPageSensorState.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TabPageSensorState.Location = New System.Drawing.Point(4, 29)
        Me.TabPageSensorState.Name = "TabPageSensorState"
        Me.TabPageSensorState.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageSensorState.Size = New System.Drawing.Size(1166, 448)
        Me.TabPageSensorState.TabIndex = 0
        Me.TabPageSensorState.Text = "Sensor State"
        Me.TabPageSensorState.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.AllowUserToResizeColumns = False
        Me.DataGridView1.AllowUserToResizeRows = False
        Me.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace
        Me.DataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SunkenHorizontal
        Me.DataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable
        Me.DataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DTG1_NameSensor, Me.DTG1_StatusSensor})
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.Location = New System.Drawing.Point(3, 3)
        Me.DataGridView1.MultiSelect = False
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.RowHeadersWidth = 51
        Me.DataGridView1.RowTemplate.Height = 24
        Me.DataGridView1.ShowCellToolTips = False
        Me.DataGridView1.ShowEditingIcon = False
        Me.DataGridView1.Size = New System.Drawing.Size(1160, 442)
        Me.DataGridView1.TabIndex = 1
        '
        'DTG1_NameSensor
        '
        Me.DTG1_NameSensor.HeaderText = "Sensor ID"
        Me.DTG1_NameSensor.MinimumWidth = 6
        Me.DTG1_NameSensor.Name = "DTG1_NameSensor"
        Me.DTG1_NameSensor.ReadOnly = True
        '
        'DTG1_StatusSensor
        '
        Me.DTG1_StatusSensor.HeaderText = "Status"
        Me.DTG1_StatusSensor.MinimumWidth = 6
        Me.DTG1_StatusSensor.Name = "DTG1_StatusSensor"
        Me.DTG1_StatusSensor.ReadOnly = True
        '
        'TabPageRecording
        '
        Me.TabPageRecording.Controls.Add(Me.SplitContainer3)
        Me.TabPageRecording.Location = New System.Drawing.Point(4, 29)
        Me.TabPageRecording.Name = "TabPageRecording"
        Me.TabPageRecording.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageRecording.Size = New System.Drawing.Size(1166, 446)
        Me.TabPageRecording.TabIndex = 1
        Me.TabPageRecording.Text = "Recording"
        Me.TabPageRecording.UseVisualStyleBackColor = True
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer3.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.ButtonRefresh)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.DGVRecording)
        Me.SplitContainer3.Size = New System.Drawing.Size(1160, 440)
        Me.SplitContainer3.SplitterDistance = 53
        Me.SplitContainer3.TabIndex = 2
        '
        'ButtonRefresh
        '
        Me.ButtonRefresh.Image = CType(resources.GetObject("ButtonRefresh.Image"), System.Drawing.Image)
        Me.ButtonRefresh.Location = New System.Drawing.Point(3, 7)
        Me.ButtonRefresh.Name = "ButtonRefresh"
        Me.ButtonRefresh.Size = New System.Drawing.Size(40, 40)
        Me.ButtonRefresh.TabIndex = 1
        Me.ButtonRefresh.UseVisualStyleBackColor = True
        '
        'DGVRecording
        '
        Me.DGVRecording.AllowUserToAddRows = False
        Me.DGVRecording.AllowUserToDeleteRows = False
        Me.DGVRecording.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVRecording.BackgroundColor = System.Drawing.SystemColors.ButtonFace
        Me.DGVRecording.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DGVRecording.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SunkenHorizontal
        Me.DGVRecording.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken
        Me.DGVRecording.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVRecording.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.BatchId, Me.SensorId, Me.RunningDay, Me.SizeTire, Me.State, Me.CreatedAt, Me.UpdatedAt, Me.Delete})
        Me.DGVRecording.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVRecording.GridColor = System.Drawing.SystemColors.ButtonFace
        Me.DGVRecording.Location = New System.Drawing.Point(0, 0)
        Me.DGVRecording.Name = "DGVRecording"
        Me.DGVRecording.ReadOnly = True
        Me.DGVRecording.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.DGVRecording.RowHeadersVisible = False
        Me.DGVRecording.RowHeadersWidth = 51
        Me.DGVRecording.RowTemplate.Height = 24
        Me.DGVRecording.Size = New System.Drawing.Size(1160, 383)
        Me.DGVRecording.TabIndex = 0
        '
        'BatchId
        '
        Me.BatchId.HeaderText = "Batch"
        Me.BatchId.MinimumWidth = 6
        Me.BatchId.Name = "BatchId"
        Me.BatchId.ReadOnly = True
        '
        'SensorId
        '
        Me.SensorId.HeaderText = "Sensor ID"
        Me.SensorId.MinimumWidth = 6
        Me.SensorId.Name = "SensorId"
        Me.SensorId.ReadOnly = True
        '
        'RunningDay
        '
        Me.RunningDay.HeaderText = "Running Days"
        Me.RunningDay.MinimumWidth = 6
        Me.RunningDay.Name = "RunningDay"
        Me.RunningDay.ReadOnly = True
        '
        'SizeTire
        '
        Me.SizeTire.HeaderText = "SizeTire"
        Me.SizeTire.MinimumWidth = 6
        Me.SizeTire.Name = "SizeTire"
        Me.SizeTire.ReadOnly = True
        '
        'State
        '
        Me.State.HeaderText = "Status"
        Me.State.MinimumWidth = 6
        Me.State.Name = "State"
        Me.State.ReadOnly = True
        '
        'CreatedAt
        '
        Me.CreatedAt.HeaderText = "Start date"
        Me.CreatedAt.MinimumWidth = 6
        Me.CreatedAt.Name = "CreatedAt"
        Me.CreatedAt.ReadOnly = True
        '
        'UpdatedAt
        '
        Me.UpdatedAt.HeaderText = "Last updated"
        Me.UpdatedAt.MinimumWidth = 6
        Me.UpdatedAt.Name = "UpdatedAt"
        Me.UpdatedAt.ReadOnly = True
        '
        'Delete
        '
        Me.Delete.HeaderText = "Action"
        Me.Delete.MinimumWidth = 6
        Me.Delete.Name = "Delete"
        Me.Delete.ReadOnly = True
        Me.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Delete.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Delete.ToolTipText = "Delete Record"
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer2.IsSplitterFixed = True
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.ToolStrip1)
        Me.SplitContainer2.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.TextBoxOutputDebug)
        Me.SplitContainer2.Panel2.Controls.Add(Me.TextBoxOutputAlert)
        Me.SplitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SplitContainer2.Size = New System.Drawing.Size(150, 46)
        Me.SplitContainer2.SplitterDistance = 25
        Me.SplitContainer2.TabIndex = 0
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton1, Me.ToolStripButtonDebug, Me.ToolStripSeparator1, Me.ToolStripButtonAlert})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(0, 5, 4, 2)
        Me.ToolStrip1.Size = New System.Drawing.Size(150, 25)
        Me.ToolStrip1.Stretch = True
        Me.ToolStrip1.TabIndex = 1
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = Global.upcuaclient_vbnet.My.Resources.Resources.x_black_16
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Margin = New System.Windows.Forms.Padding(0, 1, 20, 2)
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(29, 15)
        Me.ToolStripButton1.ToolTipText = "Close"
        '
        'ToolStripButtonDebug
        '
        Me.ToolStripButtonDebug.AutoSize = False
        Me.ToolStripButtonDebug.BackColor = System.Drawing.SystemColors.ControlDark
        Me.ToolStripButtonDebug.Checked = True
        Me.ToolStripButtonDebug.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ToolStripButtonDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripButtonDebug.Image = CType(resources.GetObject("ToolStripButtonDebug.Image"), System.Drawing.Image)
        Me.ToolStripButtonDebug.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.ToolStripButtonDebug.Name = "ToolStripButtonDebug"
        Me.ToolStripButtonDebug.Padding = New System.Windows.Forms.Padding(4)
        Me.ToolStripButtonDebug.Size = New System.Drawing.Size(80, 30)
        Me.ToolStripButtonDebug.Text = "Debug"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Margin = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 31)
        '
        'ToolStripButtonAlert
        '
        Me.ToolStripButtonAlert.AutoSize = False
        Me.ToolStripButtonAlert.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ToolStripButtonAlert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripButtonAlert.Image = CType(resources.GetObject("ToolStripButtonAlert.Image"), System.Drawing.Image)
        Me.ToolStripButtonAlert.ImageTransparentColor = System.Drawing.Color.Transparent
        Me.ToolStripButtonAlert.Margin = New System.Windows.Forms.Padding(10, 1, 0, 2)
        Me.ToolStripButtonAlert.Name = "ToolStripButtonAlert"
        Me.ToolStripButtonAlert.Padding = New System.Windows.Forms.Padding(4)
        Me.ToolStripButtonAlert.Size = New System.Drawing.Size(80, 30)
        Me.ToolStripButtonAlert.Text = "Alert"
        '
        'TextBoxOutputDebug
        '
        Me.TextBoxOutputDebug.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBoxOutputDebug.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TextBoxOutputDebug.Location = New System.Drawing.Point(0, 0)
        Me.TextBoxOutputDebug.Multiline = True
        Me.TextBoxOutputDebug.Name = "TextBoxOutputDebug"
        Me.TextBoxOutputDebug.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBoxOutputDebug.Size = New System.Drawing.Size(150, 25)
        Me.TextBoxOutputDebug.TabIndex = 0
        '
        'TextBoxOutputAlert
        '
        Me.TextBoxOutputAlert.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBoxOutputAlert.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TextBoxOutputAlert.Location = New System.Drawing.Point(0, 0)
        Me.TextBoxOutputAlert.Multiline = True
        Me.TextBoxOutputAlert.Name = "TextBoxOutputAlert"
        Me.TextBoxOutputAlert.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBoxOutputAlert.Size = New System.Drawing.Size(150, 25)
        Me.TextBoxOutputAlert.TabIndex = 2
        '
        'MenuStrip1
        '
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.ViewToolStripMenuItem, Me.ToolsToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(4, 4)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1174, 28)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ConfigManagerToolStripMenuItem, Me.ImportToolStripMenuItem, Me.ExportToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(46, 24)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'ConfigManagerToolStripMenuItem
        '
        Me.ConfigManagerToolStripMenuItem.Name = "ConfigManagerToolStripMenuItem"
        Me.ConfigManagerToolStripMenuItem.Size = New System.Drawing.Size(199, 26)
        Me.ConfigManagerToolStripMenuItem.Text = "Config manager"
        '
        'ImportToolStripMenuItem
        '
        Me.ImportToolStripMenuItem.Name = "ImportToolStripMenuItem"
        Me.ImportToolStripMenuItem.Size = New System.Drawing.Size(199, 26)
        Me.ImportToolStripMenuItem.Text = "Import"
        Me.ImportToolStripMenuItem.ToolTipText = "Import setting"
        '
        'ExportToolStripMenuItem
        '
        Me.ExportToolStripMenuItem.Name = "ExportToolStripMenuItem"
        Me.ExportToolStripMenuItem.Size = New System.Drawing.Size(199, 26)
        Me.ExportToolStripMenuItem.Text = "Export"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(199, 26)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LogToolStripMenuItem})
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(55, 24)
        Me.ViewToolStripMenuItem.Text = "View"
        '
        'LogToolStripMenuItem
        '
        Me.LogToolStripMenuItem.Name = "LogToolStripMenuItem"
        Me.LogToolStripMenuItem.Size = New System.Drawing.Size(117, 26)
        Me.LogToolStripMenuItem.Text = "Log"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddNewSensorToolStripMenuItem, Me.AddNewRecordingToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(58, 24)
        Me.ToolsToolStripMenuItem.Text = "Tools"
        '
        'AddNewSensorToolStripMenuItem
        '
        Me.AddNewSensorToolStripMenuItem.Name = "AddNewSensorToolStripMenuItem"
        Me.AddNewSensorToolStripMenuItem.Size = New System.Drawing.Size(226, 26)
        Me.AddNewSensorToolStripMenuItem.Text = "Add New Sensor"
        '
        'AddNewRecordingToolStripMenuItem
        '
        Me.AddNewRecordingToolStripMenuItem.Name = "AddNewRecordingToolStripMenuItem"
        Me.AddNewRecordingToolStripMenuItem.Size = New System.Drawing.Size(226, 26)
        Me.AddNewRecordingToolStripMenuItem.Text = "Add New Recording"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutAirLMToolStripMenuItem, Me.FeedbackToolStripMenuItem, Me.FeedbackToolStripMenuItem1})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(55, 24)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'AboutAirLMToolStripMenuItem
        '
        Me.AboutAirLMToolStripMenuItem.Name = "AboutAirLMToolStripMenuItem"
        Me.AboutAirLMToolStripMenuItem.Size = New System.Drawing.Size(176, 26)
        Me.AboutAirLMToolStripMenuItem.Text = "About AirLM"
        '
        'FeedbackToolStripMenuItem
        '
        Me.FeedbackToolStripMenuItem.Name = "FeedbackToolStripMenuItem"
        Me.FeedbackToolStripMenuItem.Size = New System.Drawing.Size(176, 26)
        Me.FeedbackToolStripMenuItem.Text = "Report Issue"
        Me.FeedbackToolStripMenuItem.Visible = False
        '
        'FeedbackToolStripMenuItem1
        '
        Me.FeedbackToolStripMenuItem1.Name = "FeedbackToolStripMenuItem1"
        Me.FeedbackToolStripMenuItem1.Size = New System.Drawing.Size(224, 26)
        Me.FeedbackToolStripMenuItem1.Text = "Feedback"
        Me.FeedbackToolStripMenuItem1.Visible = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabelOPC, Me.ToolStripStatusLabelDB})
        Me.StatusStrip1.Location = New System.Drawing.Point(4, 523)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.StatusStrip1.Size = New System.Drawing.Size(1174, 26)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabelOPC
        '
        Me.ToolStripStatusLabelOPC.Image = Global.upcuaclient_vbnet.My.Resources.Resources.server_red_16
        Me.ToolStripStatusLabelOPC.Margin = New System.Windows.Forms.Padding(0, 4, 50, 2)
        Me.ToolStripStatusLabelOPC.Name = "ToolStripStatusLabelOPC"
        Me.ToolStripStatusLabelOPC.Size = New System.Drawing.Size(161, 20)
        Me.ToolStripStatusLabelOPC.Text = "OPC Not Connected"
        '
        'ToolStripStatusLabelDB
        '
        Me.ToolStripStatusLabelDB.Image = Global.upcuaclient_vbnet.My.Resources.Resources.database_red_16
        Me.ToolStripStatusLabelDB.Margin = New System.Windows.Forms.Padding(0, 4, 50, 2)
        Me.ToolStripStatusLabelDB.Name = "ToolStripStatusLabelDB"
        Me.ToolStripStatusLabelDB.Padding = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.ToolStripStatusLabelDB.Size = New System.Drawing.Size(165, 20)
        Me.ToolStripStatusLabelDB.Text = "DB Not Connected"
        '
        'MainFormNew
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1182, 553)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.HelpButton = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MinimumSize = New System.Drawing.Size(1024, 600)
        Me.Name = "MainFormNew"
        Me.Padding = New System.Windows.Forms.Padding(4)
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AirLM"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TabControlMain.ResumeLayout(False)
        Me.TabPageSensorState.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPageRecording.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        CType(Me.DGVRecording, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.PerformLayout()
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.Panel2.PerformLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfigManagerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ViewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LogToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabelDB As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabelOPC As ToolStripStatusLabel
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents TextBoxOutputDebug As TextBox
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents TabControlMain As TabControl
    Friend WithEvents TabPageSensorState As TabPage
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents DTG1_NameSensor As DataGridViewTextBoxColumn
    Friend WithEvents DTG1_StatusSensor As DataGridViewTextBoxColumn
    Friend WithEvents TabPageRecording As TabPage
    Friend WithEvents DGVRecording As DataGridView
    Friend WithEvents ToolStripButtonDebug As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripButtonAlert As ToolStripButton
    Friend WithEvents TextBoxOutputAlert As TextBox
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents AddNewSensorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddNewRecordingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ImportToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AboutAirLMToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FeedbackToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FeedbackToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents BatchId As DataGridViewTextBoxColumn
    Friend WithEvents SensorId As DataGridViewTextBoxColumn
    Friend WithEvents RunningDay As DataGridViewTextBoxColumn
    Friend WithEvents SizeTire As DataGridViewTextBoxColumn
    Friend WithEvents State As DataGridViewTextBoxColumn
    Friend WithEvents CreatedAt As DataGridViewTextBoxColumn
    Friend WithEvents UpdatedAt As DataGridViewTextBoxColumn
    Friend WithEvents Delete As DataGridViewTextBoxColumn
    Friend WithEvents ButtonRefresh As Button
    Friend WithEvents SplitContainer3 As SplitContainer
End Class
