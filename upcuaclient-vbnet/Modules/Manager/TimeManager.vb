Public Module TimeManager
    ''' <summary>
    ''' Jalankan action sekali, lalu mulai timer sesuai interval.
    ''' </summary>
    ''' <param name="timer">Timer yang akan dijalankan</param>
    ''' <param name="action">EventHandler yang akan dipanggil</param>
    Public Sub StartTimerWithInitialFetch(timer As Timer, action As EventHandler)
        Try
            timer.Stop()
            action.Invoke(Nothing, EventArgs.Empty) ' Fetch awal langsung
            timer.Start()                           ' Timer mulai
        Catch ex As Exception
            Console.WriteLine($"⚠️ Gagal start timer: {ex.Message}")
        End Try
    End Sub
End Module