Imports Newtonsoft.Json

Public Class FormConfigSensor

    ' Temporary storage for sensor configurations
    Private tempSensorConfig As New Dictionary(Of String, List(Of Dictionary(Of String, String)))

    Private Sub FormConfigSensor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Load initial data from settings to temporary storage
        Try
            Dim settingsData = SettingsManager.GetSelectedNodeSensor()
            ' LoggerDebug.LogInfo($"FormConfigSensor_Load: Loading {settingsData.Count} objects from settings")

            tempSensorConfig = New Dictionary(Of String, List(Of Dictionary(Of String, String)))

            For Each kvp In settingsData
                Dim objectName = kvp.Key
                Dim sensorList = kvp.Value

                ' LoggerDebug.LogInfo($"  Processing object: {objectName}, Found {sensorList.Count} sensors")
                tempSensorConfig(objectName) = sensorList
            Next

            ' LoggerDebug.LogInfo($"FormConfigSensor_Load: Total loaded {tempSensorConfig.Count} objects to temp storage")
        Catch ex As Exception
            ' LoggerDebug.LogError($"Failed to load sensor config: {ex.Message}")
            tempSensorConfig = New Dictionary(Of String, List(Of Dictionary(Of String, String)))
        End Try
        ' Setup NodeActive ComboBox items
        Dim nodeActiveColumn = CType(DGVNodeSensor.Columns("NodeActive"), DataGridViewComboBoxColumn)
        nodeActiveColumn.Items.Clear()
        nodeActiveColumn.Items.Add(True)
        nodeActiveColumn.Items.Add(False)
        nodeActiveColumn.ValueType = GetType(Boolean)

        PopulateTreeView()
        LabelSelectedObjects.Text = ""
    End Sub

    Private Sub PopulateTreeView()
        TreeViewSelectedObjects.Nodes.Clear()

        ' Add root node
        Dim rootNode = TreeViewSelectedObjects.Nodes.Add("Available Objects")

        ' Get selected objects from settings
        Dim selectedObjects = SettingsManager.GetSelectedNodeIdOpc()
        ' LoggerDebug.LogInfo($"FormConfigSensor: Found {selectedObjects.Count} selected objects")

        For i = 0 To selectedObjects.Count - 1
            Dim obj = selectedObjects(i)


            If obj.ContainsKey("NodeText") AndAlso obj.ContainsKey("ChildNodeId") Then
                Dim nodeText = obj("NodeText").ToString()

                ' Debug ChildNodeId type and content
                Dim childNodeIdValue = obj("ChildNodeId")

                Dim childNodes As List(Of Dictionary(Of String, String)) = Nothing

                ' Try different casting approaches
                If TypeOf childNodeIdValue Is List(Of Dictionary(Of String, String)) Then
                    childNodes = DirectCast(childNodeIdValue, List(Of Dictionary(Of String, String)))
                ElseIf TypeOf childNodeIdValue Is String Then
                    ' Handle JSON string - deserialize it
                    Try
                        Dim jsonString = childNodeIdValue.ToString()
                        childNodes = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(jsonString)
                    Catch jsonEx As Exception
                        ' LoggerDebug.LogError($"  Failed to deserialize ChildNodeId JSON: {jsonEx.Message}")
                    End Try
                ElseIf TypeOf childNodeIdValue Is Newtonsoft.Json.Linq.JArray Then
                    ' Handle JArray from JSON deserialization
                    Try
                        Dim jArray = DirectCast(childNodeIdValue, Newtonsoft.Json.Linq.JArray)
                        childNodes = jArray.ToObject(Of List(Of Dictionary(Of String, String)))()
                    Catch jEx As Exception
                        ' LoggerDebug.LogError($"  Failed to convert JArray: {jEx.Message}")
                    End Try
                ElseIf TypeOf childNodeIdValue Is List(Of Object) Then
                    ' Convert from List(Of Object) to List(Of Dictionary(Of String, String))
                    Dim objList = DirectCast(childNodeIdValue, List(Of Object))
                    childNodes = New List(Of Dictionary(Of String, String))
                    For Each item In objList
                        If TypeOf item Is Dictionary(Of String, String) Then
                            childNodes.Add(DirectCast(item, Dictionary(Of String, String)))
                        ElseIf TypeOf item Is Dictionary(Of String, Object) Then
                            ' Convert Dictionary(Of String, Object) to Dictionary(Of String, String)
                            Dim objDict = DirectCast(item, Dictionary(Of String, Object))
                            Dim strDict As New Dictionary(Of String, String)
                            For Each kvp In objDict
                                strDict(kvp.Key) = kvp.Value.ToString()
                            Next
                            childNodes.Add(strDict)
                        End If
                    Next
                End If

                Dim childCount = If(childNodes IsNot Nothing, childNodes.Count, 0)

                Dim objectNode = rootNode.Nodes.Add($"{nodeText}({childCount})")
                objectNode.Tag = obj
            Else
                ' LoggerDebug.LogWarning($"  → Object missing NodeText or ChildNodeId keys")
            End If
        Next

        rootNode.Expand()
    End Sub

    Private Sub TreeViewSelectedObjects_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeViewSelectedObjects.AfterSelect
        ' Save current object changes before switching
        SaveCurrentObjectChanges()

        If e.Node.Tag IsNot Nothing Then
            Dim selectedObj = CType(e.Node.Tag, Dictionary(Of String, Object))
            Dim nodeText = selectedObj("NodeText").ToString()

            LabelSelectedObjects.Text = nodeText
            PopulateDGVFromObject(selectedObj)
        Else
            LabelSelectedObjects.Text = ""
            DGVNodeSensor.Rows.Clear()
        End If
    End Sub

    Private Sub SaveCurrentObjectChanges()
        Try
            If Not String.IsNullOrEmpty(LabelSelectedObjects.Text) AndAlso DGVNodeSensor.Rows.Count > 0 Then
                Dim objectName = LabelSelectedObjects.Text
                Dim sensors As New List(Of Dictionary(Of String, String))

                For Each row As DataGridViewRow In DGVNodeSensor.Rows
                    If Not row.IsNewRow Then
                        Dim isActive = CBool(row.Cells("NodeActive").Value)
                        If isActive Then
                            sensors.Add(New Dictionary(Of String, String) From {
                                {"NodeText", row.Cells("NodeText").Value.ToString()},
                                {"NodeId", row.Cells("NodeId").Value.ToString()},
                                {"NodeType", row.Cells("NodeType").Value.ToString()},
                                {"NodeStatus", "Idle"},
                                {"NodeActive", "True"}
                            })
                        End If
                    End If
                Next

                ' Save to temporary storage
                tempSensorConfig(objectName) = sensors

            End If
        Catch ex As Exception
            Console.WriteLine("Error")
        End Try
    End Sub

    Private Sub PopulateDGVFromObject(selectedObj As Dictionary(Of String, Object))
        DGVNodeSensor.Rows.Clear()

        If selectedObj.ContainsKey("ChildNodeId") Then
            Dim childNodeIdValue = selectedObj("ChildNodeId")
            Dim childNodes As List(Of Dictionary(Of String, String)) = Nothing

            ' Handle different types from JSON deserialization
            If TypeOf childNodeIdValue Is List(Of Dictionary(Of String, String)) Then
                childNodes = DirectCast(childNodeIdValue, List(Of Dictionary(Of String, String)))
            ElseIf TypeOf childNodeIdValue Is String Then
                ' Handle JSON string - deserialize it
                Try
                    childNodes = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(childNodeIdValue.ToString())
                Catch jsonEx As Exception
                    'LoggerDebug.LogError($"PopulateDGV: Failed to deserialize ChildNodeId JSON: {jsonEx.Message}")
                End Try
            ElseIf TypeOf childNodeIdValue Is Newtonsoft.Json.Linq.JArray Then
                ' Handle JArray from JSON deserialization
                Try
                    Dim jArray = DirectCast(childNodeIdValue, Newtonsoft.Json.Linq.JArray)
                    childNodes = jArray.ToObject(Of List(Of Dictionary(Of String, String)))()
                Catch jEx As Exception
                    ' LoggerDebug.LogError($"PopulateDGV: Failed to convert JArray: {jEx.Message}")
                End Try
            ElseIf TypeOf childNodeIdValue Is List(Of Object) Then
                ' Convert from List(Of Object) to List(Of Dictionary(Of String, String))
                Dim objList = DirectCast(childNodeIdValue, List(Of Object))
                childNodes = New List(Of Dictionary(Of String, String))
                For Each item In objList
                    If TypeOf item Is Dictionary(Of String, Object) Then
                        ' Convert Dictionary(Of String, Object) to Dictionary(Of String, String)
                        Dim objDict = DirectCast(item, Dictionary(Of String, Object))
                        Dim strDict As New Dictionary(Of String, String)
                        For Each kvp In objDict
                            strDict(kvp.Key) = kvp.Value.ToString()
                        Next
                        childNodes.Add(strDict)
                    End If
                Next
            End If

            If childNodes IsNot Nothing Then
                Dim objectName = selectedObj("NodeText").ToString()
                Dim activeSensors = GetActiveSensors(objectName)

                For Each childNode In childNodes
                    Dim rowIndex = DGVNodeSensor.Rows.Add()
                    DGVNodeSensor.Rows(rowIndex).Cells("NodeText").Value = childNode("NodeText")
                    DGVNodeSensor.Rows(rowIndex).Cells("NodeId").Value = childNode("NodeId")
                    DGVNodeSensor.Rows(rowIndex).Cells("NodeType").Value = childNode("NodeType")

                    ' Use default values from childNode or check active sensors
                    Dim isActive As Boolean = False
                    Dim status As String = "Idle"

                    If activeSensors.Count > 0 Then
                        ' Only check if there are active sensors
                        isActive = activeSensors.Any(Function(s) s("NodeId").ToString() = childNode("NodeId"))
                        status = If(isActive, "Running", "Idle")
                    Else
                        ' Use defaults from childNode if available
                        If childNode.ContainsKey("NodeActive") Then
                            Boolean.TryParse(childNode("NodeActive"), isActive)
                        End If
                        If childNode.ContainsKey("NodeStatus") Then
                            status = childNode("NodeStatus")
                        End If
                    End If

                    DGVNodeSensor.Rows(rowIndex).Cells("NodeStatus").Value = status
                    DGVNodeSensor.Rows(rowIndex).Cells("NodeActive").Value = isActive

                    ' LoggerDebug.LogInfo($"  Set {childNode("NodeText")}: NodeActive = {isActive}, NodeStatus = {status}")
                Next
            End If
        End If
    End Sub

    Private Function GetActiveSensors(objectName As String) As List(Of Dictionary(Of String, String))
        Try
            ' Check temporary data first, then settings
            If tempSensorConfig.ContainsKey(objectName) Then
                Return tempSensorConfig(objectName)
            End If
        Catch
        End Try
        Return New List(Of Dictionary(Of String, String))
    End Function

    Private Sub ButtonSave_Click(sender As Object, e As EventArgs) Handles ButtonSave.Click
        SaveSensorConfiguration()
    End Sub

    Private Sub SaveSensorConfiguration()
        Try
            ' Save current object changes to temp first
            SaveCurrentObjectChanges()


            ' Save all temporary data to settings
            SettingsManager.SetSelectedNodeSensor(tempSensorConfig)

            ' Verify save by reading back
            Dim savedData = SettingsManager.GetSelectedNodeSensor()
            'LoggerDebug.LogInfo($"Verification: Saved data has {savedData.Count} objects")

            MessageBox.Show("Sensor configuration saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            ' LoggerDebug.LogError($"SaveSensorConfiguration error: {ex.Message}")
            MessageBox.Show($"Error saving sensor config: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        If HasUnsavedChanges() Then
            Dim result = MessageBox.Show("You have unsaved changes. Do you want to save before closing?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                ' Save and close
                SaveSensorConfiguration()
                Return
            ElseIf result = DialogResult.No Then
                ' Don't save, clear temp and close
                tempSensorConfig.Clear()
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
            ElseIf result = DialogResult.Cancel Then
                ' Don't close
                Return
            End If
        Else
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End If
    End Sub

    Private Function HasUnsavedChanges() As Boolean
        Try
            ' Save current object changes to temp first
            SaveCurrentObjectChanges()

            ' Compare temp data with settings
            Dim currentSettings = SettingsManager.GetSelectedNodeSensor()

            ' Check if temp has different data than settings
            If tempSensorConfig.Count <> currentSettings.Count Then
                Return True
            End If

            For Each kvp In tempSensorConfig
                If Not currentSettings.ContainsKey(kvp.Key) Then
                    Return True
                End If

                Dim tempSensors = kvp.Value
                Dim settingsSensors = currentSettings(kvp.Key)

                If settingsSensors Is Nothing OrElse tempSensors.Count <> settingsSensors.Count Then
                    Return True
                End If

                ' Check individual sensors
                For i = 0 To tempSensors.Count - 1
                    If tempSensors(i)("NodeId") <> settingsSensors(i)("NodeId") OrElse
                       tempSensors(i)("NodeActive") <> settingsSensors(i)("NodeActive") Then
                        Return True
                    End If
                Next
            Next

            Return False
        Catch
            Return True ' Assume changes if error
        End Try
    End Function

    Private Sub DGVNodeSensor_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DGVNodeSensor.CellValueChanged
        Try
            ' Check if columns exist
            Dim nodeActiveColumn = DGVNodeSensor.Columns("NodeActive")
            Dim nodeStatusColumn = DGVNodeSensor.Columns("NodeStatus")

            If nodeActiveColumn Is Nothing OrElse nodeStatusColumn Is Nothing Then
                Return
            End If

            If e.ColumnIndex = nodeActiveColumn.Index AndAlso e.RowIndex >= 0 Then
                Dim cellValue = DGVNodeSensor.Rows(e.RowIndex).Cells("NodeActive").Value
                If cellValue IsNot Nothing Then
                    Dim isActive = CBool(cellValue)
                    ' Keep status as Idle - sensor is only activated, not running
                    DGVNodeSensor.Rows(e.RowIndex).Cells("NodeStatus").Value = "Idle"
                    ' LoggerDebug.LogInfo($"Sensor activated: {isActive}, Status remains: Idle")
                End If
            End If
        Catch ex As Exception
            'LoggerDebug.LogError($"CellValueChanged error: {ex.Message}")
        End Try
    End Sub
End Class