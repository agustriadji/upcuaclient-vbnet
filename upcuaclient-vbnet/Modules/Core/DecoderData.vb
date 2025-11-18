Namespace upcuaclient_vbnet
    Public Module DecoderData
        ''' <summary>
        ''' Decode Modbus RTU frame (21 bytes) menjadi list SensorResult
        ''' </summary>
        ''' <param name="data">Byte array dari OPC UA</param>
        ''' <returns>List SensorResult jika valid, Nothing jika frame invalid</returns>
        Public Function DecodeModbusFrame(data() As Byte) As List(Of SensorResult)
            If data Is Nothing OrElse data.Length < 21 Then
                Console.WriteLine("Invalid Modbus frame length")
                Return Nothing
            End If

            If data(0) <> &H1 OrElse data(1) <> &H3 OrElse data(2) <> &H10 Then
                Console.WriteLine("Invalid Modbus frame header")
                Return Nothing
            End If

            Dim results As New List(Of SensorResult)
            For i = 0 To 7 ' 8 sensor (16 bytes data / 2 bytes per sensor)
                If 3 + (i * 2) + 1 < data.Length Then
                    Dim raw As Integer = (data(3 + i * 2) << 8) Or data(4 + i * 2)
                    Dim pressure As Double = raw / 100.0

                    Dim status As String = "normal"
                    If pressure >= 30 AndAlso pressure <= 40 Then
                        status = "normal"        ' Tekanan ideal
                    ElseIf pressure >= 25 AndAlso pressure < 30 Then
                        status = "low"          ' Kurang angin
                    ElseIf pressure > 40 Then
                        status = "high"         ' Overinflated
                    ElseIf pressure > 0 AndAlso pressure < 25 Then
                        status = "critical"     ' Bocor/Berbahaya
                    Else
                        status = "offline"      ' Sensor mati
                    End If

                    results.Add(New SensorResult With {
                    .SensorId = i + 1,
                    .Pressure = pressure,
                    .Status = status
                })
                End If
            Next

            Console.WriteLine($"📊 Decoded {results.Count} sensors data")
            For Each result In results
                Console.WriteLine($"   Sensor {result.SensorId}: {result.Pressure:F2} bar - {result.Status}")
            Next

            Return results
        End Function
    End Module
End Namespace