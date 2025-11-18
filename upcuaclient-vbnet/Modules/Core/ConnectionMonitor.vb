Imports System.Threading

Public Class ConnectionMonitor
    Private Shared connectionTimer As Timer
    Private Shared isRunning As Boolean = False
    
    Public Shared Sub StartMonitoring()
        If isRunning Then Return
        
        isRunning = True
        connectionTimer = New Timer(AddressOf CheckConnections, Nothing, 0, My.Settings.intervalCheckConnections)
    End Sub
    
    Public Shared Sub StopMonitoring()
        isRunning = False
        connectionTimer?.Dispose()
        connectionTimer = Nothing
    End Sub
    
    Private Shared Async Sub CheckConnections(state As Object)
        Try
            ' Check OPC Connection
            Dim opcConnected = Await OpcConnection.CheckHealthServer()
            My.Settings.stateConnectionOPC = opcConnected
            
            ' Check Database Connection
            Dim dbConnected = Await SqlServerConnection.CheckHealth()
            My.Settings.stateConnectionDB = dbConnected
            
            ' Save settings
            My.Settings.Save()
            
        Catch ex As Exception
            ' Set both to false on error
            My.Settings.stateConnectionOPC = False
            My.Settings.stateConnectionDB = False
            My.Settings.Save()
        End Try
    End Sub
    
End Class