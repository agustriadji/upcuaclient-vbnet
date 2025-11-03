Imports upcuaclient.Modules.Core

Module Main
    Private worker As BackgroundWorkerManager

    <STAThread>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        ' Jalankan BackgroundWorker
        worker = New BackgroundWorkerManager()
        worker.Start()

        ' Tampilkan UI Dashboard
        ' Application.Run(New MainForm(worker))

        ' Saat UI ditutup, worker berhenti
        worker.StopWorker()
    End Sub
End Module
