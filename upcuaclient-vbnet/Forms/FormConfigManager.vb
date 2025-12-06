Imports Newtonsoft.Json
Imports System.IO

Public Class FormConfigManager
    ' Temporary variables to track original settings
    Private hostOpcTemp As String = ""
    Private selectedNodeIdOpcTemp As String = ""
    ' Temporary variable for DataGridView data (separate from settings)
    Private dgvTempData As New List(Of Dictionary(Of String, String))

    Private Sub FormConfigManager_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FormStyler.ApplyStandardStyle(Me)
        SettingsManager.InitializeDefaults()
        PopulateForm()

        ' Add event handler for hostOpc changes
        AddHandler TextBoxHostOpc.TextChanged, AddressOf TextBoxHostOpc_TextChanged
    End Sub

    Private Sub TextBoxHostOpc_TextChanged(sender As Object, e As EventArgs)
        ' Clear UI and temporary data when hostOpc changes
        Try
            DGVSelectedNodeOpc.Rows.Clear()
            dgvTempData.Clear() ' Clear temporary DGV data
            ComboBoxSelectObjectOpc.Items.Clear()
            LabelMessageStateHostOpc.Visible = False
            LabelMessageStateNamespaceOpc.Visible = False
        Catch ex As Exception
            ' Ignore errors during cleanup
        End Try
    End Sub

    Private Async Function ForceStopAllRecordings() As Task
        Try
            Console.WriteLine("🚨 Force stopping all recordings due to hostOpc change...")

            ' 1. Update all "Recording" status to "Force Stop" in SQLite
            Dim sqlite As New SQLiteManager()
            Dim activeRecordings = sqlite.QueryBatchRange(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow)
            Dim recordingBatches = activeRecordings.Where(Function(r) r.Status.ToLower() = "recording").ToList()

            Console.WriteLine($"📊 Found {recordingBatches.Count} active recordings to force stop")

            For Each batch In recordingBatches
                ' Update status to Force Stop
                batch.Status = "Force Stop"
                batch.SyncStatus = "Force Stop"
                batch.EndDate = DateTime.UtcNow
                sqlite.InsertOrUpdateRecordMetadata(batch)

                ' Export to SQL Server if connected
                If My.Settings.stateConnectionDB Then
                    Try
                        Dim sqlServerManager As New SQLServerManager()
                        sqlServerManager.ExportRecordData(batch.BatchId)
                        Console.WriteLine($"✅ Exported force stopped batch: {batch.BatchId}")
                    Catch exportEx As Exception
                        Console.WriteLine($"⚠️ Export failed for {batch.BatchId}: {exportEx.Message}")
                    End Try
                End If
            Next

            ' 2. Reset all OPC-related settings
            My.Settings.namespaceOpc = ""
            My.Settings.nodeIdOpc = "[]"
            My.Settings.selectedNodeIdOpc = "[]"
            My.Settings.selectedNodeSensor = "{}"
            My.Settings.endRecording = "[]"
            My.Settings.Save()

            Console.WriteLine($"✅ Force stopped {recordingBatches.Count} recordings and reset OPC settings")

        Catch ex As Exception
            Console.WriteLine($"⚠️ ForceStopAllRecordings Error: {ex.Message}")
        End Try
    End Function



    Private Sub PopulateForm()
        ' Store original settings in temporary variables
        hostOpcTemp = My.Settings.hostOpc
        selectedNodeIdOpcTemp = My.Settings.selectedNodeIdOpc

        ' General Settings
        NumericUpDownInterval.Value = My.Settings.intervalRefreshTimer \ 60000 ' Convert ms to minutes
        TextBoxThresholdPressureGauge.Text = My.Settings.thresholdPressureGauge

        ' OPC Settings
        TextBoxHostOpc.Text = My.Settings.hostOpc
        TextBoxNamespaceOpc.Text = My.Settings.namespaceOpc

        ' Database Settings
        TextBoxHostDB.Text = My.Settings.hostDB

        ' Load nodes into DataGridView
        LoadNodesIntoGrid()

        ' Populate ComboBox from saved nodeIdOpc settings
        PopulateObjectsComboBoxFromSettings()
    End Sub

    Private Sub LoadNodesIntoGrid()
        DGVSelectedNodeOpc.Rows.Clear()

        ' Setup DGV for row selection and delete
        DGVSelectedNodeOpc.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DGVSelectedNodeOpc.MultiSelect = True

        ' Add context menu if not exists
        If DGVSelectedNodeOpc.ContextMenuStrip Is Nothing Then
            Dim contextMenu As New ContextMenuStrip()
            Dim deleteItem As New ToolStripMenuItem("Delete Selected")
            AddHandler deleteItem.Click, AddressOf DeleteMenuItem_Click
            contextMenu.Items.Add(deleteItem)
            DGVSelectedNodeOpc.ContextMenuStrip = contextMenu
        End If

        ' NodeText is now TextBox, no need to populate ComboBox items

        ' Use temporary data if available, otherwise load from settings
        Dim nodesToDisplay As List(Of Dictionary(Of String, String))
        If dgvTempData.Count > 0 Then
            ' Use temporary data (currently being edited)
            nodesToDisplay = dgvTempData
            Console.WriteLine($"📋 Using temporary DGV data: {dgvTempData.Count} nodes")
        Else
            ' Load from settings and convert to temp format
            Dim selectedNodes = SettingsManager.GetSelectedNodeIdOpc()
            nodesToDisplay = New List(Of Dictionary(Of String, String))
            For Each node In selectedNodes
                nodesToDisplay.Add(New Dictionary(Of String, String) From {
                    {"NodeText", If(node.ContainsKey("NodeText"), node("NodeText").ToString(), "")},
                    {"NodeId", If(node.ContainsKey("NodeId"), node("NodeId").ToString(), "")},
                    {"NodeType", If(node.ContainsKey("NodeType"), node("NodeType").ToString(), "Object")}
                })
            Next
            dgvTempData = nodesToDisplay ' Store in temp for future edits
            Console.WriteLine($"📋 Loaded from settings: {nodesToDisplay.Count} nodes")
        End If
        ' LoggerDebug.LogInfo($"LoadNodesIntoGrid: Found {selectedNodes.Count} selected nodes")

        ' Debug selected nodes
        ' For i = 0 To selectedNodes.Count - 1
        '     LoggerDebug.LogInfo($"Selected node {i}: {selectedNodes(i).Keys.Count} keys")
        '     For Each key In selectedNodes(i).Keys
        '         LoggerDebug.LogInfo($"  Key: {key} = {selectedNodes(i)(key)}")
        '     Next
        ' Next

        ' NodeText is now TextBox, no ComboBox items needed

        ' Add rows with nodes to display
        For Each nodeItem In nodesToDisplay
            Dim rowIndex = DGVSelectedNodeOpc.Rows.Add()
            DGVSelectedNodeOpc.Rows(rowIndex).Cells("NodeText").Value = nodeItem("NodeText")
            DGVSelectedNodeOpc.Rows(rowIndex).Cells("NodeId").Value = nodeItem("NodeId")
            DGVSelectedNodeOpc.Rows(rowIndex).Cells("NodeType").Value = nodeItem("NodeType")
        Next
    End Sub

    Private Sub ButtonSaveForm_Click(sender As Object, e As EventArgs) Handles ButtonSaveForm.Click
        SaveConfiguration()
    End Sub

    Private Async Sub SaveConfiguration()
        Try
            ' Check if hostOpc changed using temporary variable
            Dim hostOpcChanged = (hostOpcTemp <> TextBoxHostOpc.Text)

            If hostOpcChanged Then
                ' Determine if this is first-time setup or actual change
                Dim isFirstTimeSetup = (selectedNodeIdOpcTemp = "[]")
                Dim hasExistingNodes = (selectedNodeIdOpcTemp <> "[]")

                Console.WriteLine($"🔍 hostOpcChanged: {hostOpcChanged}")
                Console.WriteLine($"🔍 isFirstTimeSetup: {isFirstTimeSetup}")
                Console.WriteLine($"🔍 hasExistingNodes: {hasExistingNodes}")
                Console.WriteLine($"🔍 selectedNodeIdOpcTemp: {selectedNodeIdOpcTemp}")

                If isFirstTimeSetup Then
                    ' First-time setup - no warning needed
                    Console.WriteLine("🎆 First-time OPC setup - no warning needed")
                ElseIf hasExistingNodes Then
                    ' Has existing configuration - show warning
                    Dim result = MessageBox.Show(
                        "Changing OPC host will force stop all active recordings and reset OPC settings. Continue?",
                        "Confirm OPC Host Change",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning)

                    If result = DialogResult.No Then
                        Return
                    End If

                    ' Force stop all recordings and reset OPC settings (synchronous)
                    ForceStopAllRecordingsSync()

                    ' After force stop, save the new settings from form
                    SaveNewOpcSettings()

                    ' Auto-discover objects for new host
                    Try
                        Await DiscoverAndSaveObjects()
                        PopulateObjectsComboBoxFromSettings()
                    Catch ex As Exception
                        Console.WriteLine($"⚠️ Auto-discover failed: {ex.Message}")
                        ComboBoxSelectObjectOpc.Items.Clear()
                    End Try
                End If
            End If

            ' Update settings with form values
            My.Settings.intervalTime = CInt(NumericUpDownInterval.Value) * 60000 ' Convert minutes to ms
            My.Settings.thresholdPressureGauge = TextBoxThresholdPressureGauge.Text
            My.Settings.hostOpc = TextBoxHostOpc.Text
            My.Settings.namespaceOpc = TextBoxNamespaceOpc.Text
            My.Settings.hostDB = TextBoxHostDB.Text

            ' Save temporary DGV data to selectedNodeIdOpc settings with child nodes
            Dim selectedNodes As New List(Of Dictionary(Of String, Object))
            For Each tempNode In dgvTempData
                ' Browse child nodes for each parent
                Dim childNodes As New List(Of Object)
                Try
                    Dim childNodesList = Await BrowseChildNodes(tempNode("NodeId"))
                    For Each child In childNodesList
                        childNodes.Add(New Dictionary(Of String, Object) From {
                            {"NodeText", child("NodeText")},
                            {"NodeId", child("NodeId")},
                            {"NodeType", child("NodeType")}
                        })
                    Next
                    Console.WriteLine($"📋 Found {childNodes.Count} child nodes for {tempNode("NodeText")}")
                Catch ex As Exception
                    Console.WriteLine($"⚠️ Failed to browse children for {tempNode("NodeText")}: {ex.Message}")
                End Try

                selectedNodes.Add(New Dictionary(Of String, Object) From {
                    {"NodeText", tempNode("NodeText")},
                    {"NodeId", tempNode("NodeId")},
                    {"NodeType", tempNode("NodeType")},
                    {"ChildNodeId", childNodes}
                })
            Next
            SettingsManager.SetSelectedNodeIdOpc(selectedNodes)
            Console.WriteLine($"💾 Saved {selectedNodes.Count} nodes to settings")

            ' Save all settings
            SettingsManager.SaveAll()

            MessageBox.Show("Configuration saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ' LoggerDebug.LogInfo("Configuration saved successfully!")
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show($"Error saving config:  {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ForceStopAllRecordingsSync()
        Try
            Console.WriteLine("🚨 Force stopping all recordings due to hostOpc change...")

            ' 1. Update all "Recording" status to "Force Stop" in SQLite
            Dim sqlite As New SQLiteManager()
            Dim activeRecordings = sqlite.QueryBatchRange(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow)
            Dim recordingBatches = activeRecordings.Where(Function(r) r.Status.ToLower() = "recording").ToList()

            Console.WriteLine($"📊 Found {recordingBatches.Count} active recordings to force stop")

            For Each batch In recordingBatches
                ' Update status to Force Stop
                batch.Status = "Force Stop"
                batch.SyncStatus = "Force Stop"
                batch.EndDate = DateTime.UtcNow
                sqlite.InsertOrUpdateRecordMetadata(batch)

                ' Export to SQL Server if connected (synchronous)
                If My.Settings.stateConnectionDB Then
                    Try
                        If Not String.IsNullOrEmpty(batch.PressureTireId) AndAlso Not String.IsNullOrEmpty(batch.PressureGaugeId) Then
                            Dim sqlServerManager As New SQLServerManager()
                            Dim exportSuccess = sqlServerManager.ExportRecordData(batch.BatchId)
                            If exportSuccess Then
                                Console.WriteLine($"✅ Exported force stopped batch: {batch.BatchId}")
                                ' Clean up sensor_data after successful export
                                sqlite.DeleteSensorDataByNodeIds(batch.PressureTireId, batch.PressureGaugeId)
                                Console.WriteLine($"🧹 Cleaned sensor_data for nodes: {batch.PressureTireId}, {batch.PressureGaugeId}")
                            Else
                                Console.WriteLine($"⚠️ Export failed for {batch.BatchId} - keeping sensor_data")
                            End If
                        Else
                            Console.WriteLine($"⚠️ Invalid node IDs for {batch.BatchId} - skipping export")
                        End If
                    Catch exportEx As Exception
                        Console.WriteLine($"⚠️ Export failed for {batch.BatchId}: {exportEx.Message}")
                    End Try
                End If
            Next

            ' 2. Reset all OPC-related settings
            My.Settings.namespaceOpc = ""
            My.Settings.nodeIdOpc = "[]"
            My.Settings.selectedNodeIdOpc = "[]"
            My.Settings.selectedNodeSensor = "{}"
            My.Settings.endRecording = "[]"
            My.Settings.Save()

            Console.WriteLine($"✅ Force stopped {recordingBatches.Count} recordings and reset OPC settings")

        Catch ex As Exception
            Console.WriteLine($"⚠️ ForceStopAllRecordings Error: {ex.Message}")
        End Try
    End Sub

    Private Async Sub SaveNewOpcSettings()
        Try
            ' Save the new OPC settings from form after force stop
            My.Settings.hostOpc = TextBoxHostOpc.Text
            My.Settings.namespaceOpc = TextBoxNamespaceOpc.Text

            ' Save selected nodes from temporary DGV data to selectedNodeIdOpc with child nodes
            Dim selectedNodes As New List(Of Dictionary(Of String, Object))
            For Each tempNode In dgvTempData
                ' Browse child nodes for each parent
                Dim childNodes As New List(Of Object)
                Try
                    Dim childNodesList = Await BrowseChildNodes(tempNode("NodeId"))
                    For Each child In childNodesList
                        childNodes.Add(New Dictionary(Of String, Object) From {
                            {"NodeText", child("NodeText")},
                            {"NodeId", child("NodeId")},
                            {"NodeType", child("NodeType")}
                        })
                    Next
                Catch ex As Exception
                    Console.WriteLine($"⚠️ Failed to browse children for {tempNode("NodeText")}: {ex.Message}")
                End Try

                selectedNodes.Add(New Dictionary(Of String, Object) From {
                    {"NodeText", tempNode("NodeText")},
                    {"NodeId", tempNode("NodeId")},
                    {"NodeType", tempNode("NodeType")},
                    {"ChildNodeId", childNodes}
                })
            Next

            SettingsManager.SetSelectedNodeIdOpc(selectedNodes)
            My.Settings.Save()

            Console.WriteLine($"✅ Saved new OPC settings after force stop: {selectedNodes.Count} nodes")
        Catch ex As Exception
            Console.WriteLine($"⚠️ SaveNewOpcSettings Error: {ex.Message}")
        End Try
    End Sub

    Private Sub ButtonCancelForm_Click(sender As Object, e As EventArgs) Handles ButtonCancelForm.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Async Sub ButtonTestHostOpc_Click(sender As Object, e As EventArgs) Handles ButtonTestHostOpc.Click
        Await TestOpcConnection()
    End Sub

    Private Async Function TestOpcConnection() As Task
        Try
            LabelMessageStateHostOpc.Visible = True
            LabelMessageStateHostOpc.Text = "Testing connection..."
            LabelMessageStateHostOpc.BackColor = Color.Yellow

            ' LoggerDebug.LogDebug("Testing OPC connection...")

            Dim success = Await OpcConnection.TestConnectionWithDetails(TextBoxHostOpc.Text)

            If success Then
                LabelMessageStateHostOpc.Text = "Connection successful!"
                LabelMessageStateHostOpc.BackColor = Color.LightGreen
                ' LoggerDebug.LogSuccess("OPC connection OK")

                ' Always discover and save objects when testing connection
                Await DiscoverAndSaveObjects()


                ' Populate ComboBox from saved settings
                PopulateObjectsComboBoxFromSettings()
            Else
                LabelMessageStateHostOpc.Text = "Connection failed"
                LabelMessageStateHostOpc.BackColor = Color.LightCoral
                ' LoggerDebug.LogError("OPC connection failed")
            End If
        Catch ex As Exception
            LabelMessageStateHostOpc.Text = $"Connection failed: {ex.Message}"
            LabelMessageStateHostOpc.BackColor = Color.LightCoral
            ' LoggerDebug.LogError("OPC connection error")
        End Try
    End Function

    Private Async Function DiscoverAndSaveObjects() As Task
        Try
            ' LoggerDebug.LogInfo("Discovering objects...")

            ' Clear old data first
            My.Settings.nodeIdOpc = "[]"
            My.Settings.Save()
            ' LoggerDebug.LogInfo("Cleared old object data")

            Dim objects = Await OpcConnection.GetAvailableObjects(TextBoxHostOpc.Text)
            ' LoggerDebug.LogInfo($"Retrieved {objects.Count} objects from OPC")

            ' Debug: show what keys we got
            ' If objects.Count > 0 Then
            '     LoggerDebug.LogInfo($"First object keys: {String.Join(", ", objects(0).Keys)}")
            ' End If

            ' Save discovered objects to settings.nodeIdOpc
            For Each obj In objects
                SettingsManager.AddNodeIdOpc(obj("NodeText"), obj("NodeId"), obj("NodeType"))
            Next

            SettingsManager.SaveAll()
            ' LoggerDebug.LogSuccess($"Saved {objects.Count} objects to settings")
        Catch ex As Exception
            ' LoggerDebug.LogError($"Failed to discover objects: {ex.Message}")
        End Try
    End Function

    Private Sub PopulateObjectsComboBoxFromSettings()
        Try
            ComboBoxSelectObjectOpc.Items.Clear()

            Dim savedNodes = SettingsManager.GetNodeIdOpc()
            ' LoggerDebug.LogInfo($"Retrieved {savedNodes.Count} nodes from settings")

            For Each nodeItem In savedNodes
                ' LoggerDebug.LogInfo($"Processing node: {nodeItem.Keys.Count} keys")

                ' Debug: show all keys in the dictionary
                ' For Each key In nodeItem.Keys
                '     LoggerDebug.LogInfo($"Key: {key} = {nodeItem(key)}")
                ' Next

                Dim displayText = $"{nodeItem("NodeText")} [{nodeItem("NodeId")}]"
                ComboBoxSelectObjectOpc.Items.Add(New With {.Display = displayText, .NodeId = nodeItem("NodeId"), .NodeText = nodeItem("NodeText"), .NodeType = If(nodeItem.ContainsKey("NodeType"), nodeItem("NodeType"), "Object")})
            Next

            ComboBoxSelectObjectOpc.DisplayMember = "Display"
            ' LoggerDebug.LogSuccess($"Loaded {savedNodes.Count} objects from settings")
        Catch ex As Exception
            ' LoggerDebug.LogError($"Failed to load saved objects: {ex.Message}")
        End Try
    End Sub

    Private Async Sub ButtonSelectObjectOpc_Click(sender As Object, e As EventArgs) Handles ButtonSelectObjectOpc.Click
        Await SelectObject()
    End Sub

    Private Async Function SelectObject() As Task
        Try
            If ComboBoxSelectObjectOpc.SelectedItem Is Nothing Then
                ' LoggerDebug.LogWarning("Please select an object")
                Return
            End If

            Dim selectedObj = ComboBoxSelectObjectOpc.SelectedItem
            Dim nodeId = selectedObj.NodeId
            Dim nodeText = selectedObj.NodeText
            Dim nodeType = selectedObj.NodeType

            ' LoggerDebug.LogInfo($"Selected: {nodeText}")
            ' LoggerDebug.LogInfo("Browsing child nodes...")

            ' Browse child nodes for the selected object
            Dim childNodes = Await BrowseChildNodes(nodeId)
            ' LoggerDebug.LogInfo($"SelectObject: Found {childNodes.Count} child nodes")

            ' Debug what we're about to save
            ' LoggerDebug.LogInfo($"SelectObject: Saving object - NodeText: {nodeText}, NodeId: {nodeId}, NodeType: {nodeType}")
            ' LoggerDebug.LogInfo($"SelectObject: Child nodes to save: {childNodes.Count}")

            ' Add to temporary DGV data (don't save to settings yet)
            dgvTempData.Add(New Dictionary(Of String, String) From {
                {"NodeText", nodeText},
                {"NodeId", nodeId},
                {"NodeType", nodeType}
            })

            Console.WriteLine($"➕ Added to temp data: {nodeText} - Total: {dgvTempData.Count}")

            ' Refresh grid with temporary data
            LoadNodesIntoGrid()

            ' LoggerDebug.LogSuccess("Object saved with child nodes")

        Catch ex As Exception
            ' LoggerDebug.LogError($"Failed to save object: {ex.Message}")
        End Try
    End Function

    Private Async Function BrowseChildNodes(parentNodeId As String) As Task(Of List(Of Dictionary(Of String, String)))
        Try
            ' LoggerDebug.LogInfo($"BrowseChildNodes: Starting browse for parent: {parentNodeId}")

            Dim childNodes = Await OpcConnection.BrowseChildNodes(TextBoxHostOpc.Text, parentNodeId)

            ' LoggerDebug.LogInfo($"BrowseChildNodes: Retrieved {childNodes.Count} child nodes")

            ' Debug each child node
            ' For i = 0 To childNodes.Count - 1
            '     LoggerDebug.LogInfo($"  Child {i}: {childNodes(i).Keys.Count} keys")
            '     For Each key In childNodes(i).Keys
            '         LoggerDebug.LogInfo($"    Key: {key} = {childNodes(i)(key)}")
            '     Next
            ' Next

            Return childNodes
        Catch ex As Exception
            ' LoggerDebug.LogError($"Failed to browse child nodes: {ex.Message}")
            LoggerDebug.LogError($"Stack trace: {ex.StackTrace}")
            Return New List(Of Dictionary(Of String, String))
        End Try
    End Function

    Private Sub DGVSelectedNodeOpc_KeyDown(sender As Object, e As KeyEventArgs) Handles DGVSelectedNodeOpc.KeyDown
        If e.KeyCode = Keys.Delete AndAlso DGVSelectedNodeOpc.SelectedRows.Count > 0 Then
            DeleteSelectedRows()
        End If
    End Sub

    Private Sub DeleteSelectedRows()
        Try
            If DGVSelectedNodeOpc.SelectedRows.Count = 0 Then Return

            Dim result = MessageBox.Show("Delete selected objects?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                ' Get NodeIds to delete from selected rows (exclude new row)
                Dim nodeIdsToDelete As New List(Of String)
                For Each row As DataGridViewRow In DGVSelectedNodeOpc.SelectedRows
                    If Not row.IsNewRow AndAlso row.Cells("NodeId").Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(row.Cells("NodeId").Value.ToString()) Then
                        nodeIdsToDelete.Add(row.Cells("NodeId").Value.ToString())
                    End If
                Next

                ' Remove from temporary data
                For Each nodeIdToDelete In nodeIdsToDelete
                    dgvTempData.RemoveAll(Function(node) node("NodeId") = nodeIdToDelete)
                    Console.WriteLine($"🗑️ Removed from temp data: {nodeIdToDelete}")
                Next

                ' Clear grid and reload only if there's remaining data
                DGVSelectedNodeOpc.Rows.Clear()
                If dgvTempData.Count > 0 Then
                    LoadNodesIntoGrid()
                End If
                Console.WriteLine($"✅ Deleted {nodeIdsToDelete.Count} objects - Remaining: {dgvTempData.Count}")
            End If
        Catch ex As Exception
            Console.WriteLine($"⚠️ Failed to delete objects: {ex.Message}")
        End Try
    End Sub

    Private Sub DeleteMenuItem_Click(sender As Object, e As EventArgs)
        DeleteSelectedRows()
    End Sub


    Private Async Sub ButtonTestHostDB_Click(sender As Object, e As EventArgs) Handles ButtonTestHostDB.Click
        Await TestDatabaseConnection()
    End Sub

    Private Async Function TestDatabaseConnection() As Task
        Try
            LabelMessageStateHostDB.Visible = True
            LabelMessageStateHostDB.Text = "Testing database connection..."
            LabelMessageStateHostDB.BackColor = Color.Yellow

            ' LoggerDebug.LogDebug("Testing database connection...")

            ' Parse connection string or use default format
            Dim server = "localhost\SQLEXPRESS"
            Dim database = "OpcUaClient"

            If Not String.IsNullOrEmpty(TextBoxHostDB.Text) Then
                Dim parts = TextBoxHostDB.Text.Split(";"c)
                For Each part In parts
                    If part.ToLower().Contains("server") AndAlso part.Contains("=") Then
                        server = part.Split("="c)(1).Trim()
                    ElseIf part.ToLower().Contains("database") AndAlso part.Contains("=") Then
                        database = part.Split("="c)(1).Trim()
                    End If
                Next
            End If

            ' First try to create database if not exists
            ' LoggerDebug.LogInfo("Preparing database...")
            Dim dbCreated = Await SqlServerConnection.CreateDatabaseIfNotExists(server, database)

            ' If dbCreated Then
            '     LoggerDebug.LogSuccess("Database ready")
            ' Else
            '     LoggerDebug.LogWarning("Using existing database")
            ' End If

            SqlServerConnection.SetConnectionString(server, database)
            Dim success = Await SqlServerConnection.TestConnection()

            If success Then
                LabelMessageStateHostDB.Text = "Database connection successful!"
                LabelMessageStateHostDB.BackColor = Color.LightGreen
                LoggerDebug.LogSuccess("Database connection OK")

                ' Try to create schema
                Dim schemaSuccess = Await SqlServerConnection.ExecuteSchema()
                ' If schemaSuccess Then
                '     LoggerDebug.LogSuccess("Database schema OK")
                ' Else
                '     LoggerDebug.LogWarning("Schemaa creation failed (tables may already exist)")
                ' End If
            Else
                LabelMessageStateHostDB.Text = "Database connection failed"
                LabelMessageStateHostDB.BackColor = Color.LightCoral
                LoggerDebug.LogError("SQL Server connection test failed")
            End If
        Catch ex As Exception
            LabelMessageStateHostDB.Text = $"Database error: {ex.Message}"
            LabelMessageStateHostDB.BackColor = Color.LightCoral
            LoggerDebug.LogError($"Database connection error: {ex.Message}")
        End Try
    End Function

    Private Async Sub ButtonTestNamespaceOpc_Click(sender As Object, e As EventArgs) Handles ButtonTestNamespaceOpc.Click
        Await TestNamespaceAndNodes()
    End Sub

    Private Async Sub ButtonBrowseNodes_Click(sender As Object, e As EventArgs) Handles ButtonBrowseNodes.Click
        Await BrowseAndPopulateNodes()
    End Sub

    Private Async Function BrowseAndPopulateNodes() As Task
        Try
            ' LoggerDebug.LogInfo($"Browsing nodes in namespace: {TextBoxNamespaceOpc.Text}")

            ' First browse all namespaces for debugging
            ' LoggerDebug.LogInfo("Discovering all available namespaces...")
            Dim namespaces = Await OpcConnection.BrowseAllNamespaces(TextBoxHostOpc.Text)

            ' Search for PressureTire specifically
            ' LoggerDebug.LogInfo("Searching for PressureTire object...")
            Dim pressureTirePaths = Await OpcConnection.FindObjectByName(TextBoxHostOpc.Text, "PressureTire")

            ' If pressureTirePaths.Count > 0 Then
            '     LoggerDebug.LogSuccess($"Found {pressureTirePaths.Count} PressureTire objects:")
            '     For Each path In pressureTirePaths
            '         LoggerDebug.LogInfo($"  → {path}")
            '     Next
            ' Else
            '     LoggerDebug.LogWarning("No PressureTire objects found")
            ' End If

            ' Also search for any Pressure related objects
            ' LoggerDebug.LogInfo("Searching for any Pressure related objects...")
            Dim pressurePaths = Await OpcConnection.FindObjectByName(TextBoxHostOpc.Text, "Pressure")

            ' If pressurePaths.Count > 0 Then
            '     LoggerDebug.LogSuccess($"Found {pressurePaths.Count} Pressure related objects:")
            '     For Each path In pressurePaths
            '         LoggerDebug.LogInfo($"  → {path}")
            '     Next
            ' End If

            ' Then browse specific namespace
            Dim discoveredNodes = Await OpcConnection.BrowseNamespaceNodes(TextBoxHostOpc.Text, TextBoxNamespaceOpc.Text)

            If discoveredNodes.Count > 0 Then
                ' Clear existing nodes
                DGVSelectedNodeOpc.Rows.Clear()
                Dim nodeColumn = CType(DGVSelectedNodeOpc.Columns("Node"), DataGridViewComboBoxColumn)
                nodeColumn.Items.Clear()

                ' Add discovered nodes
                For Each nodeIds In discoveredNodes
                    nodeColumn.Items.Add(nodeIds)
                    Dim rowIndex = DGVSelectedNodeOpc.Rows.Add()
                    DGVSelectedNodeOpc.Rows(rowIndex).Cells(0).Value = nodeIds
                    DGVSelectedNodeOpc.Rows(rowIndex).Cells(1).Value = ""
                    DGVSelectedNodeOpc.Rows(rowIndex).Cells(2).Value = ""
                Next

                ' LoggerDebug.LogSuccess($"Found and added {discoveredNodes.Count} nodes")
            Else
                ' LoggerDebug.LogWarning("No nodes found in namespace")
                ' LoggerDebug.LogInfo("Check the PressureTire paths found above for correct NodeIds")
            End If
        Catch ex As Exception
            ' LoggerDebug.LogError($"Browse error: {ex.Message}")
        End Try
    End Function

    Private Async Function TestNamespaceAndNodes() As Task
        Try
            LabelMessageStateNamespaceOpc.Visible = True
            LabelMessageStateNamespaceOpc.Text = "Testing namespace and nodes..."
            LabelMessageStateNamespaceOpc.BackColor = Color.Yellow

            ' LoggerDebug.LogDebug($"Testing namespace: {TextBoxNamespaceOpc.Text}")

            ' First try to test the namespace as a direct NodeId
            If TextBoxNamespaceOpc.Text.StartsWith("ns=") Then
                ' LoggerDebug.LogDebug($"Testing direct NodeId: {TextBoxNamespaceOpc.Text}")
                Try
                    Dim testNodes = {TextBoxNamespaceOpc.Text}
                    Dim directSuccess = Await OpcConnection.TestNamespaceAndNodes(TextBoxHostOpc.Text, "", testNodes)
                    If directSuccess Then
                        LabelMessageStateNamespaceOpc.Text = "Direct NodeId test OK!"
                        LabelMessageStateNamespaceOpc.BackColor = Color.LightGreen
                        ' LoggerDebug.LogSuccess("Direct NodeId test successful")
                        Return
                    End If
                Catch directEx As Exception
                    ' LoggerDebug.LogWarning($"Direct NodeId test failed: {directEx.Message}")
                End Try
            End If

            ' Get nodes from DataGridView
            Dim nodes As New List(Of String)
            For Each row As DataGridViewRow In DGVSelectedNodeOpc.Rows
                If Not row.IsNewRow AndAlso row.Cells(0).Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(row.Cells(0).Value.ToString()) Then
                    nodes.Add(row.Cells(0).Value.ToString())
                End If
            Next

            If nodes.Count = 0 Then
                ' LoggerDebug.LogWarning("No nodes to test")
                LabelMessageStateNamespaceOpc.Text = "No nodes to test"
                LabelMessageStateNamespaceOpc.BackColor = Color.Orange
                Return
            End If

            Dim success = Await OpcConnection.TestNamespaceAndNodes(TextBoxHostOpc.Text, TextBoxNamespaceOpc.Text, nodes.ToArray())

            If success Then
                LabelMessageStateNamespaceOpc.Text = "Namespace and nodes OK!"
                LabelMessageStateNamespaceOpc.BackColor = Color.LightGreen
                ' LoggerDebug.LogSuccess("Namespace and nodes test successful")
            Else
                LabelMessageStateNamespaceOpc.Text = "Namespace/nodes failed"
                LabelMessageStateNamespaceOpc.BackColor = Color.LightCoral
                ' LoggerDebug.LogError("Namespace and nodes test failed")
            End If
        Catch ex As Exception
            LabelMessageStateNamespaceOpc.Text = $"Test failed: {ex.Message}"
            LabelMessageStateNamespaceOpc.BackColor = Color.LightCoral
            ' LoggerDebug.LogError($"Namespace test error: {ex.Message}")
        End Try
    End Function
End Class