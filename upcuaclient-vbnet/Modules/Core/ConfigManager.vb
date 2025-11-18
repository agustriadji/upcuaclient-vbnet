Imports Newtonsoft.Json
Imports System.IO

Namespace upcuaclient_vbnet
    Public Class ConfigManager
        <JsonProperty("hostOpc")>
        Public Property HostOpc As String

        <JsonProperty("namespaceOpc")>
        Public Property NamespaceOpc As String

        <JsonProperty("nodeOpc")>
        Public Property NodeOpc As String()

        <JsonProperty("hostDB")>
        Public Property HostDB As String

        <JsonProperty("defaultTimeout")>
        Public Property DefaultTimeout As Integer

        <JsonProperty("intervalTime")>
        Public Property IntervalTime As Integer

        <JsonProperty("intervalRefreshTimmer")>
        Public Property IntervalRefreshTimer As Integer

        <JsonProperty("statusLabels")>
        Public Property StatusLabels As Dictionary(Of String, String)

        <JsonProperty("units")>
        Public Property Units As Dictionary(Of String, String)

        <JsonProperty("alerts")>
        Public Property Alerts As AlertConfig

        Public Shared Function Load(path As String) As ConfigManager
            Try
                If Not File.Exists(path) Then
                    Return CreateDefault()
                End If
                Dim json = File.ReadAllText(path)
                Return JsonConvert.DeserializeObject(Of ConfigManager)(json)
            Catch ex As Exception
                Console.WriteLine($"❌ Failed to read config: {ex.Message}")
                Return CreateDefault()
            End Try
        End Function

        Public Sub Save(paths As String)
            Try
                Directory.CreateDirectory(Path.GetDirectoryName(paths))
                Dim json = JsonConvert.SerializeObject(Me, Formatting.Indented)
                File.WriteAllText(paths, json)
            Catch ex As Exception
                Throw New Exception($"Failed to save config: {ex.Message}")
            End Try
        End Sub

        Private Shared Function CreateDefault() As ConfigManager
            Return New ConfigManager With {
                .HostOpc = "opc.tcp://localhost:4840",
                .NamespaceOpc = "ns=2;s=sensor",
                .NodeOpc = {"PressureTire", "PressureGauge"},
                .HostDB = "",
                .DefaultTimeout = 30,
                .IntervalTime = 120000,
                .IntervalRefreshTimer = 60000,
                .StatusLabels = New Dictionary(Of String, String) From {
                    {"recording", "Recording"},
                    {"idle", "Idle"},
                    {"error", "Error"},
                    {"offline", "Offline"},
                    {"maintenance", "Maintenance"}
                },
                .Units = New Dictionary(Of String, String) From {
                    {"pressure", "PSI"},
                    {"temperature", "Celsius"},
                    {"flowRate", "L/min"}
                },
                .Alerts = New AlertConfig With {
                    .HighPressure = 45,
                    .LowPressure = 10,
                    .TemperatureThreshold = 75,
                    .TypeLabels = New Dictionary(Of String, String) From {
                        {"info", "Information"},
                        {"warning", "Warning"},
                        {"critical", "Critical"}
                    }
                }
            }
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
End Namespace