Imports System.IO
Imports System.Text

Module ConsoleRedirect
    Public Sub DisableConsole()
        Console.SetOut(TextWriter.Null)
        Console.SetError(TextWriter.Null)
    End Sub
End Module