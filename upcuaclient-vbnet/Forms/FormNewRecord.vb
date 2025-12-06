Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class FormNewRecord
    Private Sub FormNewRecord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeForm()
    End Sub

    Private Sub InitializeForm()
        Try
            ' Set default values
            TextBoxBatch.Text = GenerateDefaultBatchName()
            NumericUpDownAutoEndRecord.Value = 0
            TextBoxSizeTire.Text = "0"
            TextBoxOperator.Text = "Operator"

            ' Populate ComboBoxes with available sensors
            PopulateSensorComboBoxes()

            ' Hide sensor gauge combobox
            ComboBoxSensorGuage.Visible = False

            ' Console.WriteLine("✅ FormNewRecord initialized")
        Catch ex As Exception
            MessageBox.Show($"Error initializing form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GenerateDefaultBatchName() As String
        Dim random As New Random()
        Dim randomNumber = random.Next(1000, 9999)
        Dim dateTimes = DateTime.Now.ToString("yyyyMMdd-HHmmss")
        Return $"BATCH-{randomNumber}-{dateTimes}"
    End Function

    Private Sub PopulateSensorComboBoxes()
        Try
            ComboBoxSensorTire.Items.Clear()
            ComboBoxSensorGuage.Items.Clear()

            Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

            ' Populate PressureTire sensors (NodeActive = true AND NodeStatus = idle)
            If selectedNodeSensor.ContainsKey("PressureTire") Then
                Dim tireSensors = selectedNodeSensor("PressureTire")
                For Each sensor In tireSensors
                    If sensor.ContainsKey("NodeActive") AndAlso sensor.ContainsKey("NodeStatus") Then
                        If sensor("NodeActive").ToLower() = "true" AndAlso sensor("NodeStatus").ToLower() = "idle" Then
                            ComboBoxSensorTire.Items.Add(New With {
                                .Display = $"{sensor("NodeText")} [{sensor("NodeId")}]",
                                .NodeId = sensor("NodeId"),
                                .NodeText = sensor("NodeText")
                            })
                        End If
                    End If
                Next
            End If

            ' Populate PressureGauge sensors (NodeActive = true AND NodeStatus = idle)
            If selectedNodeSensor.ContainsKey("PressureGauge") Then
                Dim guageSensors = selectedNodeSensor("PressureGauge")
                For Each sensor In guageSensors
                    If sensor.ContainsKey("NodeActive") AndAlso sensor.ContainsKey("NodeStatus") Then
                        If sensor("NodeActive").ToLower() = "true" AndAlso sensor("NodeStatus").ToLower() = "idle" Then
                            ComboBoxSensorGuage.Items.Add(New With {
                                .Display = $"{sensor("NodeText")} [{sensor("NodeId")}]",
                                .NodeId = sensor("NodeId"),
                                .NodeText = sensor("NodeText")
                            })
                        End If
                    End If
                Next
            End If

            ComboBoxSensorTire.DisplayMember = "Display"
            ComboBoxSensorGuage.DisplayMember = "Display"

            ' Add event handler for tire sensor selection
            AddHandler ComboBoxSensorTire.SelectedIndexChanged, AddressOf ComboBoxSensorTire_SelectedIndexChanged

            ' Console.WriteLine($"✅ Loaded {ComboBoxSensorTire.Items.Count} tire sensors, {ComboBoxSensorGuage.Items.Count} guage sensors")

            ' Debug: Show what's in selectedNodeSensor
            ' Console.WriteLine($"🔍 Debug selectedNodeSensor keys: {String.Join(", ", selectedNodeSensor.Keys)}")
            ' If selectedNodeSensor.ContainsKey("PressureGauge") Then
            '     Dim guageSensors = selectedNodeSensor("PressureGauge")
            '     Console.WriteLine($"🔍 PressureGauge has {guageSensors.Count} sensors:")
            '     For Each sensor In guageSensors
            '         Dim status = If(sensor.ContainsKey("NodeStatus"), sensor("NodeStatus"), "N/A")
            '         Dim active = If(sensor.ContainsKey("NodeActive"), sensor("NodeActive"), "N/A")
            '         Console.WriteLine($"  - {sensor("NodeText")} | Status: {status} | Active: {active}")
            '     Next
            ' Else
            '     Console.WriteLine($"⚠️ PressureGauge key not found in selectedNodeSensor")
            ' End If
        Catch ex As Exception
            ' Console.WriteLine($"⚠️ Error populating sensors: {ex.Message}")
        End Try
    End Sub

    Private Sub ButtonCreate_Click(sender As Object, e As EventArgs) Handles ButtonCreate.Click
        Try
            ' Validate mandatory fields
            If String.IsNullOrWhiteSpace(TextBoxBatch.Text) Then
                MessageBox.Show("Batch name is required", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If ComboBoxSensorTire.SelectedItem Is Nothing Then
                MessageBox.Show("Please select a tire sensor", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If ComboBoxSensorGuage.SelectedItem Is Nothing Then
                MessageBox.Show("Please select a guage sensor", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Get selected sensors
            Dim selectedTire = ComboBoxSensorTire.SelectedItem
            Dim selectedGuage = ComboBoxSensorGuage.SelectedItem

            ' Create record metadata
            Dim recordMetadata As New InterfaceRecordMetadata With {
                .BatchId = TextBoxBatch.Text,
                .PressureTireId = selectedTire.NodeId,
                .PressureGaugeId = selectedGuage.NodeId,
                .Size = Convert.ToInt32(Val(TextBoxSizeTire.Text)),
                .CreatedBy = TextBoxOperator.Text,
                .Status = "Not-Start",
                .SyncStatus = "Pending",
                .StartDate = DateTime.UtcNow,
                .EndDate = If(NumericUpDownAutoEndRecord.Value > 0, DateTime.UtcNow.AddDays(NumericUpDownAutoEndRecord.Value), DateTime.UtcNow.AddYears(1)),
                .EndRecordingDate = If(NumericUpDownAutoEndRecord.Value > 0, DateTime.UtcNow.AddDays(NumericUpDownAutoEndRecord.Value), Nothing)
            }

            ' Save to SQLite
            Dim sqlite As New SQLiteManager()
            If sqlite.InsertOrUpdateRecordMetadata(recordMetadata) Then
                ' Console.WriteLine($"✅ Record metadata saved: {recordMetadata.BatchId}")

                ' Update sensor status from idle to running
                UpdateSensorStatusToRunning(selectedTire.NodeId, selectedGuage.NodeId)

                ' Set auto end recording in settings if enabled
                If NumericUpDownAutoEndRecord.Value > 0 Then
                    SettingsManager.AddEndRecording(selectedTire.NodeId, selectedGuage.NodeId, recordMetadata.EndRecordingDate.Value)
                End If

                MessageBox.Show($"Recording started successfully!\nBatch: {recordMetadata.BatchId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                MessageBox.Show("Failed to save record metadata", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error creating record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateSensorStatusToRunning(tireNodeId As String, guageNodeId As String)
        Try
            Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()
            Dim updated = False

            ' Update PressureTire sensor status
            If selectedNodeSensor.ContainsKey("PressureTire") Then
                Dim tireSensors = selectedNodeSensor("PressureTire")
                For Each sensor In tireSensors
                    If sensor("NodeId") = tireNodeId Then
                        sensor("NodeStatus") = "no-start"
                        updated = True
                        ' Console.WriteLine($"✅ Updated tire sensor {tireNodeId} to running")
                        Exit For
                    End If
                Next
            End If

            ' Update PressureGauge sensor status
            If selectedNodeSensor.ContainsKey("PressureGauge") Then
                Dim guageSensors = selectedNodeSensor("PressureGauge")
                For Each sensor In guageSensors
                    If sensor("NodeId") = guageNodeId Then
                        sensor("NodeStatus") = "no-start"
                        updated = True
                        ' Console.WriteLine($"✅ Updated guage sensor {guageNodeId} to running")
                        Exit For
                    End If
                Next
            End If

            ' Save updated settings
            If updated Then
                SettingsManager.SetSelectedNodeSensor(selectedNodeSensor)
                ' Console.WriteLine($"✅ Sensor status updated to running - BackgroundWorker will detect and start data collection")
            End If

        Catch ex As Exception
            Console.WriteLine($"⚠️ Error updating sensor status: {ex.Message}")
        End Try
    End Sub

    Private Sub ComboBoxSensorTire_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            If ComboBoxSensorTire.SelectedItem IsNot Nothing Then
                Dim selectedTire = ComboBoxSensorTire.SelectedItem
                Dim tireNodeText = selectedTire.NodeText

                ' Find matching gauge sensor with same NodeText
                For i As Integer = 0 To ComboBoxSensorGuage.Items.Count - 1
                    Dim gaugeItem = ComboBoxSensorGuage.Items(i)
                    If gaugeItem.NodeText = tireNodeText Then
                        ComboBoxSensorGuage.SelectedIndex = i
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            Console.WriteLine($"⚠️ Error auto-selecting gauge sensor: {ex.Message}")
        End Try
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class