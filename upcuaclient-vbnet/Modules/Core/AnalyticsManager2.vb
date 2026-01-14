Imports System
Imports Opc.Ua
Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class AnalyticsManager2
    Private config As ConfigManager

    Public Sub New()
        config = ConfigManager.Load("Config/meta.json")
    End Sub

    ' Map OPC sensor data to SQLite sensor_data structure
    Public Function MapSensorData(nodeId As String, sensorType As String, value As Double, status As String, nodeText As String) As InterfaceSensorData
        ' Convert raw value to PSI
        Dim psiValue = ConvertToPSI(value)

        Return New InterfaceSensorData With {
            .NodeId = nodeId,
            .NodeText = nodeText,
            .SensorType = sensorType,
            .Value = psiValue,
            .DataType = "Float",
            .Status = status,
            .SyncStatus = "Pending",
            .Timestamp = DateTime.UtcNow
        }
    End Function

    ' Map OPC sensor to metadata structure
    Public Function MapSensorMetadataTire(nodeId As String, isActive As Boolean, isRunning As Boolean) As InterfacePressureTireMetadata
        Return New InterfacePressureTireMetadata With {
            .NodeId = nodeId,
            .DataType = "Float",
            .Unit = "PSI",
            .IsActive = isActive,
            .IsRunning = isRunning
        }
    End Function

    Public Function MapSensorMetadataGuage(nodeId As String, isActive As Boolean, isRunning As Boolean) As InterfacePressureGaugeMetadata
        Return New InterfacePressureGaugeMetadata With {
            .NodeId = nodeId,
            .DataType = "Float",
            .Unit = "PSI",
            .IsActive = isActive,
            .IsRunning = isRunning
        }
    End Function

    ' Generate alert - khusus untuk PressureGauge leaking detection
    Public Function GenerateAlert(nodeId As String, value As Double, nodeText As String) As InterfaceAlertData
        Dim sensorType = GetSensorType(nodeId)

        ' Alert hanya untuk PressureGauge
        If sensorType <> "pressureGauge" Then
            Return Nothing
        End If

        ' Get threshold from settings
        Dim alertThreshold As Double = 0.2 ' Default fallback
        Try
            ' Use invariant culture to handle both "0.2" and "0,2" formats
            Dim thresholdStr = My.Settings.thresholdPressureGauge.Replace(",", ".")
            alertThreshold = Convert.ToDouble(thresholdStr, System.Globalization.CultureInfo.InvariantCulture)
        Catch ex As Exception
            alertThreshold = 0.2
        End Try

        ' PressureGauge: nilai > threshold = leaking
        If value > alertThreshold Then
            Return New InterfaceAlertData With {
                .NodeId = nodeId,
                .NodeText = nodeText,
                .SensorType = sensorType,
                .Message = $"LEAKING DETECTED: Pressure {value.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)} PSI (threshold: {alertThreshold.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)})",
                .Threshold = alertThreshold,
                .CurrentValue = value.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
                .Severity = "CRITICAL",
                .Timestamp = DateTime.UtcNow
            }
        End If

        ' Nilai <= threshold dianggap normal
        Return Nothing
    End Function

    ' Map batch metadata for recording session
    Public Function MapRecordMetadata(batchId As String, tireNodeId As String, guageNodeId As String, size As Integer, createdBy As String) As InterfaceRecordMetadata
        Return New InterfaceRecordMetadata With {
            .BatchId = batchId,
            .PressureTireId = tireNodeId,
            .PressureGaugeId = guageNodeId,
            .Size = size,
            .CreatedBy = createdBy,
            .Status = "Recording",
            .SyncStatus = "Pending",
            .StartDate = DateTime.UtcNow,
            .EndDate = DateTime.UtcNow.AddHours(1) ' Default 1 hour session
        }
    End Function

    Private Function GetSensorDisplayName(nodeId As String) As String
        Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

        For Each kvp In selectedNodeSensor
            If TypeOf kvp.Value Is List(Of Dictionary(Of String, String)) Then
                Dim sensorList = DirectCast(kvp.Value, List(Of Dictionary(Of String, String)))
                For Each sensor In sensorList
                    If sensor.ContainsKey("NodeId") AndAlso sensor("NodeId") = nodeId Then
                        If sensor.ContainsKey("DisplayName") Then
                            Return sensor("DisplayName")
                        End If
                    End If
                Next
            End If
        Next

        ' Fallback to nodeId if display name not found
        Return nodeId
    End Function

    Private Function GetSensorType(nodeId As String) As String
        ' Check berdasarkan parent root object dari selectedNodeSensor
        Dim selectedNodeSensor = SettingsManager.GetSelectedNodeSensor()

        For Each kvp In selectedNodeSensor
            Dim parentKey = kvp.Key ' "PressureTire" atau "PressureGauge"
            If TypeOf kvp.Value Is List(Of Dictionary(Of String, String)) Then
                Dim sensorList = DirectCast(kvp.Value, List(Of Dictionary(Of String, String)))
                For Each sensor In sensorList
                    If sensor.ContainsKey("NodeId") AndAlso sensor("NodeId") = nodeId Then
                        If parentKey.ToLower().Contains("tire") Then
                            Return "pressureTire"
                        ElseIf parentKey.ToLower().Contains("gauge") OrElse parentKey.ToLower().Contains("gauge") Then
                            Return "pressureGauge"
                        End If
                    End If
                Next
            End If
        Next

        ' Fallback ke detection berdasarkan nodeId pattern
        If nodeId.Contains("PressureTire") OrElse nodeId.Contains("Tire") Then
            Return "pressureTire"
        ElseIf nodeId.Contains("PressureGauge") OrElse nodeId.Contains("PressureGauge") OrElse nodeId.Contains("Gauge") OrElse nodeId.Contains("Gauge") Then
            Return "pressureGauge"
        End If

        Return "unknown"
    End Function

    ' Convert raw sensor value to 3 decimal places
    Private Function ConvertToPSI(rawValue As Double) As Double
        ' Simply round to 3 decimal places without unit conversion
        Return Math.Round(rawValue, 3)
    End Function

    Public Function ComputeDailyStats(sensorId As String, dates As String) As Dictionary(Of String, Double)
        Dim sqlite As New SQLiteManager()
        Dim dataList = sqlite.QueryDaily(sensorId, dates)
        If dataList.Count = 0 Then Return Nothing

        Return New Dictionary(Of String, Double) From {
            {"min", dataList.Min(Function(d) d.Value)},
            {"max", dataList.Max(Function(d) d.Value)},
            {"avg", dataList.Average(Function(d) d.Value)},
            {"count", dataList.Count}
        }
    End Function
End Class