Public Class LoggerDebug
    
    ' Events untuk komunikasi dengan UI
    Public Shared Event LogMessage(message As String, level As LogLevel, source As String)
    
    ' Log levels
    Public Enum LogLevel
        Debug
        Info
        Warning
        [Error]
        Success
    End Enum
    
    ' Log methods
    Public Shared Sub LogDebug(message As String, Optional source As String = "")
        RaiseEvent LogMessage(message, LogLevel.Debug, GetCallerSource(source))
    End Sub
    
    Public Shared Sub LogInfo(message As String, Optional source As String = "")
        RaiseEvent LogMessage(message, LogLevel.Info, GetCallerSource(source))
    End Sub
    
    Public Shared Sub LogWarning(message As String, Optional source As String = "")
        RaiseEvent LogMessage(message, LogLevel.Warning, GetCallerSource(source))
    End Sub
    
    Public Shared Sub LogError(message As String, Optional source As String = "")
        RaiseEvent LogMessage(message, LogLevel.Error, GetCallerSource(source))
    End Sub
    
    Public Shared Sub LogSuccess(message As String, Optional source As String = "")
        RaiseEvent LogMessage(message, LogLevel.Success, GetCallerSource(source))
    End Sub
    
    ' Helper untuk mendapatkan caller source
    Private Shared Function GetCallerSource(customSource As String) As String
        If Not String.IsNullOrEmpty(customSource) Then
            Return customSource
        End If
        Return ""
        'Try
        '    Dim frame = New System.Diagnostics.StackFrame(2)
        '    Dim method = frame.GetMethod()
        '    Dim typeName = method.DeclaringType.Name
        '    Dim methodName = method.Name

        '    ' Clean up VB.NET async state machine names
        '    If typeName.Contains("VB$StateMachine_") Then
        '        ' Extract original method name from VB$StateMachine_123_MethodName
        '        Dim parts = typeName.Split("_"c)
        '        If parts.Length >= 3 Then
        '            methodName = String.Join("_", parts.Skip(2))
        '            ' Get the actual class name from outer type
        '            If method.DeclaringType.DeclaringType IsNot Nothing Then
        '                typeName = method.DeclaringType.DeclaringType.Name
        '            End If
        '        End If
        '    End If

        '    Return $"{typeName}.{methodName}"
        'Catch
        '    Return "Unknown"
        'End Try
    End Function
    
End Class