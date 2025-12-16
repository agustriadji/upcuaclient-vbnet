Imports System.IO

Namespace upcuaclient_vbnet
    Public Class AppLogger
        Private Shared ReadOnly logPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpcUaClient", "logs")
        Private Shared ReadOnly lockObject As New Object()

        Shared Sub New()
            Try
                If Not Directory.Exists(logPath) Then
                    Directory.CreateDirectory(logPath)
                End If
            Catch
                ' Ignore directory creation errors
            End Try
        End Sub

        Public Shared Sub LogInfo(message As String, Optional source As String = "")
            WriteLog("INFO", message, source)
        End Sub

        Public Shared Sub LogError(message As String, Optional source As String = "")
            WriteLog("ERROR", message, source)
        End Sub

        Public Shared Sub LogWarning(message As String, Optional source As String = "")
            WriteLog("WARNING", message, source)
        End Sub

        Public Shared Sub LogDebug(message As String, Optional source As String = "")
            WriteLog("DEBUG", message, source)
        End Sub

        Private Shared Sub WriteLog(level As String, message As String, source As String)
            Try
                SyncLock lockObject
                    Dim logFile = Path.Combine(logPath, $"app_{DateTime.Now:yyyyMMdd}.log")
                    Dim logEntry = $"{DateTime.Now:HH:mm:ss.fff} [{level}] {If(String.IsNullOrEmpty(source), "", $"[{source}] ")}{message}{Environment.NewLine}"
                    File.AppendAllText(logFile, logEntry)
                End SyncLock
            Catch
                ' Ignore logging errors
            End Try
        End Sub
    End Class
End Namespace