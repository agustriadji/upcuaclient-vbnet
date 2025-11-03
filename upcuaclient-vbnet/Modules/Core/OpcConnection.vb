Imports Opc.Ua
Imports Opc.Ua.Client
Imports Opc.Ua.Configuration
Imports upcuaclient.Modules.Core

Public Class OpcConnection
    Private Shared _session As Session
    Public Shared ReadOnly Property Session As Session
        Get
            Return _session
        End Get
    End Property

    Public Shared Async Function InitializeAsync(configPath As String) As Task(Of Boolean)
        Try
            Dim configData = ConfigManager.Load(configPath)
            If configData Is Nothing OrElse String.IsNullOrEmpty(configData.Endpoint) Then
                Console.WriteLine("❌ Endpoint tidak ditemukan di config")
                Return False
            End If

            Dim appConfig = Await BuildApplicationConfigurationAsync()
            Dim useSecurity = False
            Dim selectedEndpoint = CoreClientUtils.SelectEndpoint(appConfig, configData.Endpoint, useSecurity)
            Dim endpointConfig = EndpointConfiguration.Create(appConfig)
            Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)

            _session = Await Task.Run(Function()
                                          Return Session.Create(appConfig, configuredEndpoint, False, "OPC UA VB Client", 60000, Nothing, Nothing)
                                      End Function)

            Console.WriteLine($"✅ Connected to OPC UA at {configData.Endpoint}")
            Return True
        Catch ex As Exception
            Console.WriteLine($"❌ OPC UA connection failed: {ex.Message}")
            Return False
        End Try
    End Function

    Private Shared Async Function BuildApplicationConfigurationAsync() As Task(Of ApplicationConfiguration)
        Dim config = New ApplicationConfiguration() With {
            .ApplicationName = "OPC UA VB Client",
            .ApplicationType = ApplicationType.Client,
            .TransportQuotas = New TransportQuotas() With {.OperationTimeout = 15000},
            .ClientConfiguration = New ClientConfiguration()
        }

        Dim basePki = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OPC Foundation", "Pki")
        Dim ownStore = Path.Combine(basePki, "Own")
        Dim caStore = Path.Combine(basePki, "CA")
        Dim trustedStore = Path.Combine(basePki, "Trusted")
        Dim rejectedStore = Path.Combine(basePki, "Rejected")

        For Each d In New String() {ownStore, caStore, trustedStore, rejectedStore}
            If Not Directory.Exists(d) Then Directory.CreateDirectory(d)
        Next

        config.SecurityConfiguration = New SecurityConfiguration() With {
            .ApplicationCertificate = New CertificateIdentifier() With {
                .StoreType = "Directory",
                .StorePath = ownStore,
                .SubjectName = "CN=OPC UA VB Client"
            },
            .TrustedIssuerCertificates = New CertificateTrustList() With {.StoreType = "Directory", .StorePath = caStore},
            .TrustedPeerCertificates = New CertificateTrustList() With {.StoreType = "Directory", .StorePath = trustedStore},
            .AutoAcceptUntrustedCertificates = True,
            .RejectSHA1SignedCertificates = False
        }

        Await config.ValidateAsync(ApplicationType.Client)
        Return config
    End Function

    Public Shared Function ReadNodeValue(nodeId As String) As String
        Try
            Dim value = _session.ReadValue(nodeId).Value
            Return value?.ToString()
        Catch ex As Exception
            Console.WriteLine($"⚠️ Failed to read node {nodeId}: {ex.Message}")
            Return Nothing
        End Try
    End Function

    Public Shared Sub Disconnect()
        Try
            If _session IsNot Nothing AndAlso _session.Connected Then
                _session.Close()
                Console.WriteLine("🔌 OPC UA disconnected")
            End If
        Catch ex As Exception
            Console.WriteLine($"⚠️ Disconnect error: {ex.Message}")
        End Try
    End Sub
End Class