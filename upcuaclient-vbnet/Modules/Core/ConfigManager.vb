Imports Newtonsoft.Json
Imports System.IO

Public Class ConfigManager
    <JsonProperty("endpoint")>
    Public Property Endpoint As String

    <JsonProperty("defaultTimeout")>
    Public Property DefaultTimeout As Integer

    <JsonProperty("intervalTime")>
    Public Property IntervalTime As Integer

    <JsonProperty("statusLabels")>
    Public Property StatusLabels As Dictionary(Of String, String)

    <JsonProperty("units")>
    Public Property Units As Dictionary(Of String, String)

    <JsonProperty("alerts")>
    Public Property Alerts As AlertConfig

    Public Shared Function Load(path As String) As ConfigManager
        Try
            Dim json = File.ReadAllText(path)
            Return JsonConvert.DeserializeObject(Of ConfigManager)(json)
        Catch ex As Exception
            Console.WriteLine($"❌ Gagal membaca config: {ex.Message}")
            Return Nothing
        End Try
    End Function
End Class

Public Class AlertConfig
    <JsonProperty("highPressure")>
    Public Property HighPressure As Double

    <JsonProperty("lowPressure")>
    Public Property LowPressure As Double

    <JsonProperty("temperatureThreshold")>
    Public Property TemperatureThreshold As Double

    <JsonProperty("typeLabels")>
    Public Property TypeLabels As Dictionary(Of String, String)
End Class