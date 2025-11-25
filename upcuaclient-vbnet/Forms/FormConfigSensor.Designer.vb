<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormConfigSensor
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
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Available Objects")
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.TreeViewSelectedObjects = New System.Windows.Forms.TreeView()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.LabelSelectedObjects = New System.Windows.Forms.Label()
        Me.DGVNodeSensor = New System.Windows.Forms.DataGridView()
        Me.NodeText = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NodeId = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NodeType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NodeStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NodeActive = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.ButtonSave = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        CType(Me.DGVNodeSensor, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.SplitContainer1.Location = New System.Drawing.Point(6, 32)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.ButtonCancel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ButtonSave)
        Me.SplitContainer1.Size = New System.Drawing.Size(770, 470)
        Me.SplitContainer1.SplitterDistance = 406
        Me.SplitContainer1.SplitterWidth = 1
        Me.SplitContainer1.TabIndex = 0
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.TreeViewSelectedObjects)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer2.Size = New System.Drawing.Size(770, 406)
        Me.SplitContainer2.SplitterDistance = 242
        Me.SplitContainer2.TabIndex = 0
        '
        'TreeViewSelectedObjects
        '
        Me.TreeViewSelectedObjects.Location = New System.Drawing.Point(3, 3)
        Me.TreeViewSelectedObjects.Name = "TreeViewSelectedObjects"
        TreeNode1.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        TreeNode1.Name = "Root"
        TreeNode1.Text = "Available Objects"
        Me.TreeViewSelectedObjects.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode1})
        Me.TreeViewSelectedObjects.Size = New System.Drawing.Size(240, 403)
        Me.TreeViewSelectedObjects.TabIndex = 0
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.LabelSelectedObjects)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.DGVNodeSensor)
        Me.SplitContainer3.Size = New System.Drawing.Size(524, 406)
        Me.SplitContainer3.SplitterDistance = 33
        Me.SplitContainer3.TabIndex = 1
        '
        'LabelSelectedObjects
        '
        Me.LabelSelectedObjects.AutoSize = True
        Me.LabelSelectedObjects.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelSelectedObjects.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.LabelSelectedObjects.Location = New System.Drawing.Point(3, 3)
        Me.LabelSelectedObjects.Name = "LabelSelectedObjects"
        Me.LabelSelectedObjects.Size = New System.Drawing.Size(65, 20)
        Me.LabelSelectedObjects.TabIndex = 1
        Me.LabelSelectedObjects.Text = "(Sensor)"
        '
        'DGVNodeSensor
        '
        Me.DGVNodeSensor.AllowUserToAddRows = False
        Me.DGVNodeSensor.AllowUserToDeleteRows = False
        Me.DGVNodeSensor.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVNodeSensor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DGVNodeSensor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVNodeSensor.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.NodeText, Me.NodeId, Me.NodeType, Me.NodeStatus, Me.NodeActive})
        Me.DGVNodeSensor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVNodeSensor.Location = New System.Drawing.Point(0, 0)
        Me.DGVNodeSensor.Name = "DGVNodeSensor"
        Me.DGVNodeSensor.RowHeadersVisible = False
        Me.DGVNodeSensor.RowHeadersWidth = 51
        Me.DGVNodeSensor.RowTemplate.Height = 24
        Me.DGVNodeSensor.Size = New System.Drawing.Size(524, 369)
        Me.DGVNodeSensor.TabIndex = 0
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
        Me.NodeId.HeaderText = "NodeId"
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
        'NodeStatus
        '
        Me.NodeStatus.HeaderText = "Status"
        Me.NodeStatus.MinimumWidth = 6
        Me.NodeStatus.Name = "NodeStatus"
        Me.NodeStatus.ReadOnly = True
        '
        'NodeActive
        '
        Me.NodeActive.HeaderText = "Active"
        Me.NodeActive.MinimumWidth = 6
        Me.NodeActive.Name = "NodeActive"
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Location = New System.Drawing.Point(582, 24)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(80, 30)
        Me.ButtonCancel.TabIndex = 1
        Me.ButtonCancel.Text = "Cancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'ButtonSave
        '
        Me.ButtonSave.Location = New System.Drawing.Point(690, 23)
        Me.ButtonSave.Name = "ButtonSave"
        Me.ButtonSave.Size = New System.Drawing.Size(80, 30)
        Me.ButtonSave.TabIndex = 0
        Me.ButtonSave.Text = "Save"
        Me.ButtonSave.UseVisualStyleBackColor = True
        '
        'FormConfigSensor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(782, 508)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.MaximumSize = New System.Drawing.Size(800, 555)
        Me.MinimumSize = New System.Drawing.Size(800, 555)
        Me.Name = "FormConfigSensor"
        Me.Padding = New System.Windows.Forms.Padding(6)
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "FormConfigSensor"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel1.PerformLayout()
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        CType(Me.DGVNodeSensor, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents TreeViewSelectedObjects As TreeView
    Friend WithEvents DGVNodeSensor As DataGridView
    Friend WithEvents NodeText As DataGridViewTextBoxColumn
    Friend WithEvents NodeId As DataGridViewTextBoxColumn
    Friend WithEvents NodeType As DataGridViewTextBoxColumn
    Friend WithEvents NodeStatus As DataGridViewTextBoxColumn
    Friend WithEvents NodeActive As DataGridViewComboBoxColumn
    Friend WithEvents SplitContainer3 As SplitContainer
    Friend WithEvents ButtonCancel As Button
    Friend WithEvents ButtonSave As Button
    Friend WithEvents LabelSelectedObjects As Label
End Class
