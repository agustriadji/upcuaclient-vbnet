Public Module DecoderData
    ''' <summary>
    ''' Decode Modbus RTU frame (21 bytes) menjadi list tekanan ban
    ''' </summary>
    ''' <param name="data">Byte array dari TCP listener</param>
    ''' <returns>List tekanan (Double) jika valid, Nothing jika frame invalid</returns>
    Public Function DecodeModbusFrame(data() As Byte) As List(Of Double)
        If data.Length <> 21 OrElse data(0) <> &H1 OrElse data(1) <> &H3 OrElse data(2) <> &H10 Then
            Console.WriteLine("Invalid Modbus frame")
            Return Nothing
        End If

        Dim pressures As New List(Of Double)
        For i = 0 To 14 ' 15 sensor
            Dim raw As Integer = (data(3 + i * 2) << 8) Or data(4 + i * 2)
            pressures.Add(raw / 100.0)
        Next
        Return pressures
    End Function
End Module