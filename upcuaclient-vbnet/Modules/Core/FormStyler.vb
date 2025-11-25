Imports System.Drawing
Imports System.Windows.Forms

Public Module FormStyler
    ''' <summary>
    ''' Terapkan gaya standar ke form: font, autoscale, dan sinkronisasi kontrol.
    ''' </summary>
    ''' <param name="targetForm">Form yang akan distyling</param>
    Public Sub ApplyStandardStyle(ByVal targetForm As Form)
        ' Set AutoScaleMode dan baseline dimensions
        targetForm.AutoScaleMode = AutoScaleMode.None
        targetForm.AutoScaleDimensions = New SizeF(9.2F, 20.0F)

        ' Set font standar
        Dim standardFont As New Font("Segoe UI", 9.2F)
        targetForm.Font = standardFont

        ' Terapkan font ke semua kontrol
        For Each ctrl As Control In targetForm.Controls
            ctrl.Font = standardFont
        Next
    End Sub
End Module