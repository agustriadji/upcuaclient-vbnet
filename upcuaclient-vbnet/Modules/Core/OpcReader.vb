Imports Opc.Ua.Client

Namespace upcuaclient_vbnet
    Public Module OpcReader
    ''' <summary>
    ''' Baca nilai dari node OPC UA dan konversi ke byte array
    ''' </summary>
    ''' <param name="session">Session aktif dari OpcConnection</param>
    ''' <param name="nodeId">NodeId string, contoh: "ns=2;s=Sensor.RawFrame"</param>
    ''' <returns>Byte array jika valid, Nothing jika gagal</returns>
    Public Function ReadFrameBytes(session As Session, nodeId As String) As Byte()
        Try
            Dim value = session.ReadValue(nodeId).Value?.ToString()
            If String.IsNullOrEmpty(value) Then
                Return Nothing
            End If

            Dim parts = value.Split(" "c)
            If parts.Length <> 21 Then
                Return Nothing
            End If

            Dim bytes(20) As Byte
            For i = 0 To 20
                bytes(i) = Convert.ToByte(parts(i), 16)
            Next

            Return bytes
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    End Module
End Namespace