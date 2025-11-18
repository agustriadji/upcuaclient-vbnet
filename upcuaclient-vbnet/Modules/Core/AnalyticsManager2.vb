Imports System
Imports upcuaclient_vbnet.upcuaclient_vbnet

Public Class AnalyticsManager2
    Private config As ConfigManager

    Public Sub New()
        config = ConfigManager.Load("Config/meta.json")
    End Sub

    ' Map OPC sensor data to SQLite sensor_data structure
    Public Function MapSensorData(nodeId As String, displayName As String, value As Double, status As String) As InterfaceSensorData
        Return New InterfaceSensorData With {
            .NodeId = nodeId,
            .SensorType = GetSensorType(nodeId),
            .Value = value,
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
    Public Function GenerateAlert(nodeId As String, value As Double) As InterfaceAlertData
        Dim sensorType = GetSensorType(nodeId)

        ' Alert hanya untuk PressureGauge
        If sensorType <> "pressureGauge" Then
            Return Nothing
        End If

        ' PressureGauge: nilai > 0 = leaking, nilai = 0 = normal
        If value > 0 Then
            Return New InterfaceAlertData With {
                .NodeId = nodeId,
                .SensorType = sensorType,
                .Message = $"LEAKING DETECTED: Pressure spike {value:F2} PSI",
                .Threshold = 0.0,
                .CurrentValue = value,
                .Severity = "CRITICAL",
                .Timestamp = DateTime.UtcNow
            }
        End If

        ' Nilai = 0 dianggap normal, tidak perlu alert
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