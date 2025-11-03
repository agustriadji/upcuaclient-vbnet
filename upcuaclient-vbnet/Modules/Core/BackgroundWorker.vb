Imports System.ComponentModel
Imports upcuaclient.Modules.Core
Imports upcuaclient.Modules.Decoder

Public Class BackgroundWorkerManager
    Private WithEvents worker As New BackgroundWorker With {
        .WorkerReportsProgress = False,
        .WorkerSupportsCancellation = True
    }

    Private config As ConfigManager

    Public Async Sub Start()
        config = ConfigManager.Load("config/meta.json")
        If config Is Nothing Then
            Console.WriteLine("❌ Gagal load konfigurasi")
            Exit Sub
        End If

        Dim connected = Await OpcConnection.InitializeAsync("config/meta.json")
        If Not connected Then
            Console.WriteLine("❌ Gagal koneksi OPC UA")
            Exit Sub
        End If

        worker.RunWorkerAsync()
    End Sub

    Private Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles worker.DoWork
        Do While Not worker.CancellationPending
            Console.WriteLine($"⏱️ Polling at {DateTime.Now}")

            ' Baca frame mentah dari OPC UA
            Dim rawHex = OpcConnection.ReadNodeValue("ns=2;s=Sensor.RawFrame")
            If String.IsNullOrEmpty(rawHex) Then
                Console.WriteLine("⚠️ Tidak ada data dari node RawFrame")
                Threading.Thread.Sleep(config.IntervalTime)
                Continue Do
            End If

            ' Konversi ke byte array
            Dim frameBytes = OpcReader.ReadFrameBytes(OpcConnection.Session, "ns=2;s=Sensor.RawFrame")
            If frameBytes Is Nothing Then
                Console.WriteLine("⚠️ Gagal konversi frame")
                Threading.Thread.Sleep(config.IntervalTime)
                Continue Do
            End If

            ' Decode frame
            Dim results = DecoderData.DecodeModbusFrame(frameBytes)
            If results Is Nothing Then
                Console.WriteLine("⚠️ Frame tidak valid")
                Threading.Thread.Sleep(config.IntervalTime)
                Continue Do
            End If

            ' Tampilkan hasil ke console
            For Each result In results
                Console.WriteLine($"Sensor {result.SensorId}: {result.Pressure:F2} {config.Units("pressure")} → {config.StatusLabels(result.StatusKey)}")
            Next

            Threading.Thread.Sleep(config.IntervalTime)
        Loop
    End Sub

    Public Sub StopWorker()
        worker.CancelAsync()
        OpcConnection.Disconnect()
    End Sub

End Class