Imports System.Threading.Tasks
Imports System.IO
Imports Opc.Ua
Imports Opc.Ua.Client
Imports Opc.Ua.Configuration

Public Class MainForm2
    Private _session As Session

    Private ReadOnly ServerUrl As String = "opc.tcp://localhost:4840"

    Private Class NodeInfo
        Public Property NodeId As NodeId
        Public Property DisplayName As String
        Public Property NodeClass As NodeClass
    End Class

    Private Async Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Siapkan kolom DataGridView
        DataGridView1.Columns.Clear()
        DataGridView1.Columns.Add("ID", "ID")
        DataGridView1.Columns.Add("Name", "Name")
        DataGridView1.Columns.Add("Status", "Status")

        Try
            Await ConnectAsync()
            Await PopulateDataGridAsync()
            TimerDTGDashboard.Start()
            Me.Text = $"Sensor Monitoring Dashboard - Connected to {ServerUrl}"
        Catch ex As Exception
            Me.Text = "Sensor Monitoring Dashboard - Offline"
            MessageBox.Show("Connection error: " & ex.Message)
        End Try
    End Sub

    Private Async Function BuildApplicationConfigurationAsync() As Task(Of ApplicationConfiguration)
        Dim config = New ApplicationConfiguration() With {
            .ApplicationName = "OPC UA VB Client",
            .ApplicationType = ApplicationType.Client,
            .TransportQuotas = New TransportQuotas() With {
                .OperationTimeout = 15000
            },
            .ClientConfiguration = New ClientConfiguration()
        }

        ' Setup default PKI folders under LocalApplicationData\OPC Foundation\Pki
        Dim basePki = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OPC Foundation", "Pki")
        Dim ownStore = Path.Combine(basePki, "Own")
        Dim caStore = Path.Combine(basePki, "CA")
        Dim trustedStore = Path.Combine(basePki, "Trusted")
        Dim rejectedStore = Path.Combine(basePki, "Rejected")

        ' Ensure directories exist
        For Each d In New String() {ownStore, caStore, trustedStore, rejectedStore}
            If Not Directory.Exists(d) Then
                Directory.CreateDirectory(d)
            End If
        Next

        ' Security configuration - set store paths so ValidateAsync won't throw
        config.SecurityConfiguration = New SecurityConfiguration() With {
            .ApplicationCertificate = New CertificateIdentifier() With {
                .StoreType = "Directory",
                .StorePath = ownStore,
                .SubjectName = "CN=OPC UA VB Client"
            },
            .TrustedIssuerCertificates = New CertificateTrustList() With {
                .StoreType = "Directory",
                .StorePath = caStore
            },
            .TrustedPeerCertificates = New CertificateTrustList() With {
                .StoreType = "Directory",
                .StorePath = trustedStore
            },
            .AutoAcceptUntrustedCertificates = True,
            .RejectSHA1SignedCertificates = False
        }

        ' Validasi config (async)
        Await config.ValidateAsync(ApplicationType.Client)

        Return config
    End Function

    Private Async Function ConnectAsync() As Task
        If _session IsNot Nothing AndAlso _session.Connected Then
            Return
        End If

        Dim config = Await BuildApplicationConfigurationAsync()

        ' Secure Connection?
        Dim useSecurity As Boolean = False

        ' Panggil SelectEndpoint dengan ApplicationConfiguration sebagai argumen pertama
        Dim selectedEndpoint = CoreClientUtils.SelectEndpoint(config, ServerUrl, useSecurity)

        Dim endpointConfig = EndpointConfiguration.Create(config)
        Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)

        ' Jalankan Create pada background thread (paket lib Anda tidak expose CreateAsync overload yang sama)
        _session = Await Task.Run(Function()
                                      Return Session.Create(config, configuredEndpoint, False, "OPC UA VB Client", 60000, Nothing, Nothing)
                                  End Function)
    End Function

    Private Async Function PopulateDataGridAsync() As Task
        If _session Is Nothing OrElse Not _session.Connected Then
            Return
        End If

        Await Task.Run(Sub()
                           Try
                               Dim browser = New Browser(_session) With {
                                   .BrowseDirection = BrowseDirection.Forward,
                                   .NodeClassMask = NodeClass.Object Or NodeClass.Variable
                               }

                               ' Coba temukan node "Sensor" di bawah Objects, jika ada
                               Dim sensorNodeId As NodeId = Nothing
                               Dim topRefs = browser.Browse(ObjectIds.ObjectsFolder)
                               For Each rd As ReferenceDescription In topRefs
                                   Try
                                       If String.Equals(rd.DisplayName.Text, "Sensor", StringComparison.OrdinalIgnoreCase) _
                                          OrElse rd.DisplayName.Text.StartsWith("Sensor", StringComparison.OrdinalIgnoreCase) Then
                                           sensorNodeId = ExpandedNodeId.ToNodeId(rd.NodeId, _session.NamespaceUris)
                                           Exit For
                                       End If
                                   Catch
                                   End Try
                               Next

                               ' Browse rekursif
                               Dim nodes As New List(Of NodeInfo)
                               Dim actionBrowse As Action(Of NodeId) = Nothing
                               actionBrowse = Sub(nodeId As NodeId)
                                                  Dim refs = browser.Browse(nodeId)
                                                  For Each rd As ReferenceDescription In refs
                                                      Try
                                                          Dim childNodeId = ExpandedNodeId.ToNodeId(rd.NodeId, _session.NamespaceUris)
                                                          nodes.Add(New NodeInfo With {
                                                                       .NodeId = childNodeId,
                                                                       .DisplayName = rd.DisplayName.Text,
                                                                       .NodeClass = rd.NodeClass
                                                                   })
                                                          If rd.NodeClass = NodeClass.Object Then
                                                              actionBrowse(childNodeId)
                                                          End If
                                                      Catch
                                                      End Try
                                                  Next
                                              End Sub

                               If sensorNodeId IsNot Nothing Then
                                   actionBrowse(sensorNodeId)
                               Else
                                   actionBrowse(ObjectIds.ObjectsFolder)
                               End If

                               ' Batch read variable values
                               Dim readIds As New ReadValueIdCollection()
                               Dim variableNodes As New List(Of NodeInfo)
                               For Each n In nodes
                                   If n.NodeClass = NodeClass.Variable Then
                                       readIds.Add(New ReadValueId() With {.NodeId = n.NodeId, .AttributeId = Attributes.Value})
                                       variableNodes.Add(n)
                                   End If
                               Next

                               Dim results As DataValueCollection = Nothing
                               Dim diag As DiagnosticInfoCollection = Nothing
                               If readIds.Count > 0 Then
                                   _session.Read(Nothing, 0, TimestampsToReturn.Both, readIds, results, diag)
                               End If

                               ' Update UI
                               Me.Invoke(Sub()
                                             DataGridView1.Rows.Clear()
                                             If readIds.Count = 0 Then
                                                 For Each n In nodes
                                                     DataGridView1.Rows.Add(n.NodeId.ToString(), n.DisplayName, n.NodeClass.ToString())
                                                 Next
                                             Else
                                                 For i = 0 To variableNodes.Count - 1
                                                     Dim idStr = variableNodes(i).NodeId.ToString()
                                                     Dim display = variableNodes(i).DisplayName
                                                     Dim valStr As String = ""
                                                     Dim dv = If(results IsNot Nothing AndAlso i < results.Count, results(i), Nothing)
                                                     If dv IsNot Nothing AndAlso dv.Value IsNot Nothing Then
                                                         valStr = dv.Value.ToString()
                                                     ElseIf dv IsNot Nothing Then
                                                         valStr = dv.StatusCode.ToString()
                                                     End If
                                                     DataGridView1.Rows.Add(idStr, display, valStr)
                                                 Next
                                             End If
                                         End Sub)
                           Catch ex As Exception
                               Me.Invoke(Sub() MessageBox.Show("Browse/read error: " & ex.Message))
                           End Try
                       End Sub)
    End Function

    Private Async Sub TimerDTGDashboard_Tick(sender As Object, e As EventArgs) Handles TimerDTGDashboard.Tick
        If _session IsNot Nothing AndAlso _session.Connected Then
            Await PopulateDataGridAsync()
        Else
            Try
                Await ConnectAsync()
                Await PopulateDataGridAsync()
            Catch
            End Try
        End If
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        MyBase.OnFormClosing(e)
        Try
            If _session IsNot Nothing Then
                _session.Dispose()
                _session = Nothing
            End If
        Catch
        End Try
    End Sub

    ' Event (kompatibilitas designer)
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
    End Sub
End Class
