Imports System.IO

Namespace upcuaclient_vbnet
    Public Class AppPaths
        ' Base application data folder
        Public Shared ReadOnly AppDataFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpcUaClient")
        
        ' Database paths
        Public Shared ReadOnly DatabaseFolder As String = Path.Combine(AppDataFolder, "data")
        Public Shared ReadOnly SQLiteDatabase As String = Path.Combine(DatabaseFolder, "sensor.db")
        
        ' Log paths
        Public Shared ReadOnly LogFolder As String = Path.Combine(AppDataFolder, "logs")
        
        ' Settings paths
        Public Shared ReadOnly SettingsFolder As String = Path.Combine(AppDataFolder, "settings")
        
        ' Export paths
        Public Shared ReadOnly ExportFolder As String = Path.Combine(AppDataFolder, "exports")
        
        ' Ensure all directories exist
        Shared Sub New()
            Try
                EnsureDirectoryExists(AppDataFolder)
                EnsureDirectoryExists(DatabaseFolder)
                EnsureDirectoryExists(LogFolder)
                EnsureDirectoryExists(SettingsFolder)
                EnsureDirectoryExists(ExportFolder)
            Catch
                ' Ignore directory creation errors
            End Try
        End Sub
        
        Private Shared Sub EnsureDirectoryExists(path As String)
            If Not Directory.Exists(path) Then
                Directory.CreateDirectory(path)
            End If
        End Sub
    End Class
End Namespace