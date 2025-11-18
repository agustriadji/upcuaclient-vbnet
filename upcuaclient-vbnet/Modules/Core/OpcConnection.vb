Imports Opc.Ua
Imports Opc.Ua.Client
Imports Opc.Ua.Configuration
Imports System.IO
Imports System.Threading
Imports upcuaclient_vbnet.upcuaclient_vbnet
Public Class OpcConnection
    Private Shared _session As Session
    Private Shared _configData As ConfigManager
    Private Shared ReadOnly _lock As New Object()
    Private Shared _reconnectTimer As Timer

    Public Shared ReadOnly Property Session As Session
        Get
            Return _session
        End Get
    End Property

    Public Shared ReadOnly Property IsConnected As Boolean
        Get
            Return _session IsNot Nothing AndAlso _session.Connected
        End Get
    End Property

    ' === Inisialisasi koneksi ===
    Public Shared Async Function InitializeAsync() As Task(Of Boolean)
        Try
            SettingsManager.InitializeDefaults()
            If String.IsNullOrEmpty(My.Settings.hostOpc) Then
                Console.WriteLine("❌ Endpoint OPC tidak ditemukan di settings.")
                Return False
            End If
            Return Await ConnectAsync(My.Settings.hostOpc)
        Catch ex As Exception
            Console.WriteLine($"❌ Gagal inisialisasi OPC: {ex.Message}")
            Return False
        End Try
    End Function

    ' === Koneksi ke OPC UA Server ===
    Private Shared Async Function ConnectAsync(endpointUrl As String) As Task(Of Boolean)
        Try
            Dim appConfig = Await BuildApplicationConfigurationAsync()
            Dim useSecurity = False
            Dim selectedEndpoint = Await CoreClientUtils.SelectEndpointAsync(appConfig, endpointUrl, useSecurity)
            Dim endpointConfig = EndpointConfiguration.Create(appConfig)
            Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)

            _session = Await Session.Create(appConfig, configuredEndpoint, False, "VB OPC UA Client", 60000, Nothing, Nothing)
            AddHandler _session.KeepAlive, AddressOf OnKeepAlive

            Console.WriteLine($"✅ Connected to OPC UA at {endpointUrl}")
            Return True
        Catch ex As Exception
            Console.WriteLine($"❌ OPC UA connection failed: {ex.Message}")
            Return False
        End Try
    End Function

    ' === Build konfigurasi aplikasi ===
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

    ' === Event KeepAlive (cek koneksi otomatis) ===
    Private Shared Sub OnKeepAlive(sender As Session, e As KeepAliveEventArgs)
        If e.Status IsNot Nothing AndAlso ServiceResult.IsBad(e.Status) Then
            Console.WriteLine($"⚠️ Koneksi OPC terputus: {e.Status}")
            StartReconnect()
        End If
    End Sub

    ' === Reconnect otomatis ===
    Private Shared Sub StartReconnect()
        SyncLock _lock
            If _reconnectTimer Is Nothing Then
                _reconnectTimer = New Timer(Async Sub(state)
                                                Console.WriteLine("🔁 Mencoba reconnect ke OPC...")
                                                If Await ConnectAsync(My.Settings.hostOpc) Then
                                                    _reconnectTimer.Dispose()
                                                    _reconnectTimer = Nothing
                                                    Console.WriteLine("✅ Reconnected successfully.")
                                                End If
                                            End Sub, Nothing, 5000, Timeout.Infinite)
            End If
        End SyncLock
    End Sub

    ' === Baca nilai dari NodeId langsung ===
    Public Shared Async Function ReadNodeValue(nodeId As String) As Task(Of String)
        Try
            If Not IsConnected Then
                Console.WriteLine("⚠️ Session tidak aktif, mencoba reconnect...")
                If Not Await ConnectAsync(My.Settings.hostOpc) Then Return Nothing
            End If

            Dim value = (Await _session.ReadValueAsync(nodeId)).Value
            Return value?.ToString()
        Catch ex As Exception
            Console.WriteLine($"⚠️ Gagal membaca node {nodeId}: {ex.Message}")
            Return Nothing
        End Try
    End Function

    ' === Baca berdasarkan sensor key (misal: pressureTire.sensor1) === (NOT USE)
    Public Shared Async Function ReadBySensorKey(sensorKey As String) As Task(Of String)
        Try
            Dim parts = sensorKey.Split("."c)
            If parts.Length <> 2 Then Return Nothing

            Dim nodeId = $"ns=2;s=PLC.{parts(1)}.{If(parts(0) = "pressureTire", "Tire", "Gauge")}"
            Return Await ReadNodeValue(nodeId)
        Catch
            Return Nothing
        End Try
    End Function

    ' === Health check koneksi server ===
    Public Shared Async Function CheckHealthServer() As Task(Of Boolean)
        If IsConnected Then Return True
        Console.WriteLine("⚠️ Server OPC tidak terhubung, mencoba reconnect...")
        Return Await ConnectAsync(My.Settings.hostOpc)
    End Function

    ' === Health check tag/object === (NOT USE)
    Public Shared Async Function CheckHealthObject() As Task(Of Boolean)
        Try
            ' Use a default test node since TestNode is not in ConfigManager
            Dim testNode = "ns=2;s=PLC.sensor1.Tire"
            Dim val = Await ReadNodeValue(testNode)
            Return val IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ' === Browse OPC nodes and subnodes ===
    Public Shared Async Function BrowseNodes() As Task
        Try
            If Not IsConnected Then
                Console.WriteLine("⚠️ Session tidak aktif")
                Return
            End If

            Console.WriteLine("🔍 Browsing OPC UA nodes...")

            Dim browser = New Browser(_session) With {
                    .BrowseDirection = BrowseDirection.Forward,
                    .NodeClassMask = NodeClass.Object Or NodeClass.Variable
                }

            ' Browse root Objects folder
            Dim rootRefs = Await browser.BrowseAsync(ObjectIds.ObjectsFolder)
            For Each rootRef As ReferenceDescription In rootRefs
                'rootRef.NodeId
                'rootRef.NodeClass
                'rootRef.DisplayName
                Console.WriteLine($"📁 Root: {rootRef.DisplayName} [{rootRef.NodeClass}]")

                ' Focus on PressureGauge and PressureTire nodes
                If rootRef.DisplayName.Text.Contains("Pressure") Then
                    Dim nodeId = ExpandedNodeId.ToNodeId(rootRef.NodeId, _session.NamespaceUris)
                    Console.WriteLine($"🎯 Found {rootRef.DisplayName}: {nodeId}")

                    ' Browse sensors
                    Dim sensorRefs = Await browser.BrowseAsync(nodeId)
                    For Each sensorRef As ReferenceDescription In sensorRefs
                        If sensorRef.NodeClass = NodeClass.Variable Then
                            Try
                                Dim sensorNodeId = ExpandedNodeId.ToNodeId(sensorRef.NodeId, _session.NamespaceUris)
                                Dim value = Await _session.ReadValueAsync(sensorNodeId)
                                If StatusCode.IsGood(value.StatusCode) Then
                                    Console.WriteLine($"  🔧 {sensorRef.DisplayName}: {value.Value} PSI [{value.StatusCode}]")
                                Else
                                    Console.WriteLine($"  ⚠️ {sensorRef.DisplayName}: {value.StatusCode}")
                                End If
                            Catch ex As Exception
                                Console.WriteLine($"  ⚠️ {sensorRef.DisplayName}: Read error - {ex.Message}")
                            End Try
                        End If
                    Next
                End If
            Next

        Catch ex As Exception
            Console.WriteLine($"⚠️ Browse error: {ex.Message}")
        End Try
    End Function


    ' === Test connection to specific endpoint ===
    Public Shared Async Function TestConnection(endpointUrl As String) As Task(Of Boolean)
        Try
            Dim appConfig = Await BuildApplicationConfigurationAsync()
            Dim useSecurity = False
            Dim selectedEndpoint = Await CoreClientUtils.SelectEndpointAsync(appConfig, endpointUrl, useSecurity)
            Dim endpointConfig = EndpointConfiguration.Create(appConfig)
            Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)

            Dim testSession = Await Session.Create(appConfig, configuredEndpoint, False, "Test Session", 30000, Nothing, Nothing)

            Await testSession.CloseAsync()
            testSession.Dispose()

            Return True
        Catch ex As Exception
            Console.WriteLine($"❌ Test connection failed: {ex.Message}")
            Return False
        End Try
    End Function

    ' === Test connection with detailed logging ===
    Public Shared Async Function TestConnectionWithDetails(endpointUrl As String) As Task(Of Boolean)
        Try
            Console.WriteLine($"🔍 Testing connection to: {endpointUrl}")

            Dim appConfig = Await BuildApplicationConfigurationAsync()
            Console.WriteLine("✅ Application configuration built")

            Dim useSecurity = False
            Dim selectedEndpoint = Await CoreClientUtils.SelectEndpointAsync(appConfig, endpointUrl, useSecurity)
            Console.WriteLine($"✅ Endpoint selected: {selectedEndpoint.EndpointUrl}")

            Dim endpointConfig = EndpointConfiguration.Create(appConfig)
            Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)
            Console.WriteLine("✅ Endpoint configured")

            Dim testSession = Await Session.Create(appConfig, configuredEndpoint, False, "Test Session", 30000, Nothing, Nothing)
            Console.WriteLine($"✅ Session created: {testSession.SessionName}")

            'Dim Avaliable = Await GetAvailableObjects(endpointUrl)

            'Dim browser = New Browser(testSession) With {
            '    .BrowseDirection = BrowseDirection.Forward,
            '    .NodeClassMask = NodeClass.Object Or NodeClass.Variable
            '}

            Await testSession.CloseAsync()
            testSession.Dispose()
            Console.WriteLine("✅ Test session closed successfully")

            Return True
        Catch ex As Exception
            Console.WriteLine($"❌ Test connection failed: {ex.Message}")
            Console.WriteLine($"❌ Stack trace: {ex.StackTrace}")
            Return False
        End Try
    End Function

    ' === Deep search for specific object ===
    Public Shared Async Function FindObjectByName(endpointUrl As String, objectName As String) As Task(Of List(Of String))
        Dim testSession As Session = Nothing
        Dim foundPaths As New List(Of String)
        Try
            Console.WriteLine($"🔍 Deep searching for object: {objectName}")

            ' Create test session
            Dim appConfig = Await BuildApplicationConfigurationAsync()
            Dim useSecurity = False
            Dim selectedEndpoint = Await CoreClientUtils.SelectEndpointAsync(appConfig, endpointUrl, useSecurity)
            Dim endpointConfig = EndpointConfiguration.Create(appConfig)
            Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)
            testSession = Await Session.Create(appConfig, configuredEndpoint, False, "Deep Search Session", 30000, Nothing, Nothing)

            Console.WriteLine("✅ Deep search session created")

            Dim browser = New Browser(testSession) With {
                .BrowseDirection = BrowseDirection.Forward,
                .NodeClassMask = NodeClass.Object Or NodeClass.Variable
            }

            ' Start recursive search from Objects folder
            Await SearchRecursive(browser, ObjectIds.ObjectsFolder, objectName, foundPaths, testSession.NamespaceUris, "Objects", 0)

            Console.WriteLine($"✅ Search completed. Found {foundPaths.Count} matches for '{objectName}'")
            Return foundPaths

        Catch ex As Exception
            Console.WriteLine($"❌ Deep search failed: {ex.Message}")
            Return foundPaths
        Finally
            If testSession IsNot Nothing Then
                Try
                    testSession.Close()
                    testSession.Dispose()
                Catch
                End Try
            End If
        End Try
    End Function

    Private Shared Async Function SearchRecursive(browser As Browser, nodeId As NodeId, searchName As String, foundPaths As List(Of String), namespaceUris As NamespaceTable, currentPath As String, depth As Integer) As Task
        Try
            ' Limit recursion depth to prevent infinite loops
            If depth > 5 Then Return

            Dim refs = Await browser.BrowseAsync(nodeId)
            For Each ref As ReferenceDescription In refs
                Dim childNodeId = ExpandedNodeId.ToNodeId(ref.NodeId, namespaceUris)
                Dim childPath = $"{currentPath}/{ref.DisplayName}"

                Console.WriteLine($"{New String(" "c, depth * 2)}📝 {ref.DisplayName} [{childNodeId}] - {ref.NodeClass}")

                ' Check if this matches our search
                If ref.DisplayName.Text.ToLower().Contains(searchName.ToLower()) Then
                    Dim fullPath = $"Path: {childPath} | NodeId: {childNodeId} | Class: {ref.NodeClass}"
                    foundPaths.Add(fullPath)
                    Console.WriteLine($"✅ FOUND MATCH: {fullPath}")

                    ' If it's a variable, try to read its value
                    If ref.NodeClass = NodeClass.Variable Then
                        Try
                            Dim session = browser.Session
                            Dim value = Await session.ReadValueAsync(childNodeId)
                            If StatusCode.IsGood(value.StatusCode) Then
                                Console.WriteLine($"  → Value: {value.Value} [{value.StatusCode}]")
                            Else
                                Console.WriteLine($"  → Status: {value.StatusCode}")
                            End If
                        Catch readEx As Exception
                            Console.WriteLine($"  → Read error: {readEx.Message}")
                        End Try
                    End If
                End If

                ' Continue recursive search if it's an object
                If ref.NodeClass = NodeClass.Object Then
                    Try
                        Await SearchRecursive(browser, childNodeId, searchName, foundPaths, namespaceUris, childPath, depth + 1)
                    Catch recursiveEx As Exception
                        Console.WriteLine($"{New String(" "c, depth * 2)}⚠️ Cannot browse {ref.DisplayName}: {recursiveEx.Message}")
                    End Try
                End If
            Next
        Catch ex As Exception
            Console.WriteLine($"Search error at depth {depth}: {ex.Message}")
        End Try
    End Function

    ' === Get available objects for ComboBox ===
    Public Shared Async Function GetAvailableObjects(endpointUrl As String) As Task(Of List(Of Dictionary(Of String, String)))
        Dim testSession As Session = Nothing
        Dim objects As New List(Of Dictionary(Of String, String))
        Try
            Console.WriteLine("🔍 Getting available objects...")

            ' Create test session
            Dim appConfig = Await BuildApplicationConfigurationAsync()
            Dim useSecurity = False
            Dim selectedEndpoint = Await CoreClientUtils.SelectEndpointAsync(appConfig, endpointUrl, useSecurity)
            Dim endpointConfig = EndpointConfiguration.Create(appConfig)
            Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)
            testSession = Await Session.Create(appConfig, configuredEndpoint, False, "Objects Session", 30000, Nothing, Nothing)

            Console.WriteLine("✅ Objects session created")

            Dim browser = New Browser(testSession) With {
                .BrowseDirection = BrowseDirection.Forward,
                .NodeClassMask = NodeClass.Object Or NodeClass.Variable
            }

            'Dim browser = New Browser(_session) With {
            '        .BrowseDirection = BrowseDirection.Forward,
            '        .NodeClassMask = NodeClass.Object Or NodeClass.Variable
            '    }

            '' Browse root Objects folder
            'Dim rootRefs = Await browser.BrowseAsync(ObjectIds.ObjectsFolder)
            'For Each rootRef As ReferenceDescription In rootRefs
            '    rootRef.NodeId
            '    rootRef.NodeClass
            '    rootRef.DisplayName

            ' Browse Objects folder for all objects
            Dim objectsRefs = Await browser.BrowseAsync(ObjectIds.ObjectsFolder)
            For Each objRef As ReferenceDescription In objectsRefs
                ' Include all objects (Server, Locations, custom objects, etc.)
                Dim objectInfo As New Dictionary(Of String, String) From {
                    {"NodeId", objRef.NodeId.ToString()},
                    {"NodeText", objRef.DisplayName.Text},
                    {"NodeType", objRef.NodeClass.ToString()}
                }
                objects.Add(objectInfo)
                Console.WriteLine($"✅ Found object: {objRef.DisplayName} [{objRef.NodeId}] - {objRef.NodeClass}")
            Next

            Return objects

        Catch ex As Exception
            Console.WriteLine($"❌ Get objects failed: {ex.Message}")
            Return objects
        Finally
            If testSession IsNot Nothing Then
                Try
                    testSession.Close()
                    testSession.Dispose()
                Catch
                End Try
            End If
        End Try
    End Function

    ' === Browse all available namespaces ===
    Public Shared Async Function BrowseAllNamespaces(endpointUrl As String) As Task(Of List(Of String))
        Dim testSession As Session = Nothing
        Dim namespaces As New List(Of String)
        Try
            Console.WriteLine("🔍 Browsing all available namespaces...")

            ' Create test session
            Dim appConfig = Await BuildApplicationConfigurationAsync()
            Dim useSecurity = False
            Dim selectedEndpoint = Await CoreClientUtils.SelectEndpointAsync(appConfig, endpointUrl, useSecurity)
            Dim endpointConfig = EndpointConfiguration.Create(appConfig)
            Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)
            testSession = Await Session.Create(appConfig, configuredEndpoint, False, "Namespace Browse Session", 30000, Nothing, Nothing)

            Console.WriteLine("✅ Namespace browse session created")

            ' Browse namespace array
            Console.WriteLine("🔍 Available namespaces:")
            For i = 0 To testSession.NamespaceUris.Count - 1
                Dim namespaceUri = testSession.NamespaceUris.GetString(CUInt(i))
                Console.WriteLine($"  ns={i}: {namespaceUri}")
                namespaces.Add($"ns={i}")
            Next

            ' Browse Objects folder
            Dim browser = New Browser(testSession) With {
                .BrowseDirection = BrowseDirection.Forward,
                .NodeClassMask = NodeClass.Object Or NodeClass.Variable
            }

            Console.WriteLine("🔍 Objects in root folder:")
            Dim objectsRefs = Await browser.BrowseAsync(ObjectIds.ObjectsFolder)
            For Each objRef As ReferenceDescription In objectsRefs
                Dim objNodeId = ExpandedNodeId.ToNodeId(objRef.NodeId, testSession.NamespaceUris)
                Console.WriteLine($"  📝 {objRef.DisplayName} [{objNodeId}] - {objRef.NodeClass}")
            Next

            Return namespaces

        Catch ex As Exception
            Console.WriteLine($"❌ Namespace browse failed: {ex.Message}")
            Return namespaces
        Finally
            If testSession IsNot Nothing Then
                Try
                    testSession.Close()
                    testSession.Dispose()
                Catch
                End Try
            End If
        End Try
    End Function

    ' === Browse and discover nodes ===
    Public Shared Async Function BrowseNamespaceNodes(endpointUrl As String, namespaceUri As String) As Task(Of List(Of String))
        Dim testSession As Session = Nothing
        Dim discoveredNodes As New List(Of String)
        Try
            Console.WriteLine($"🔍 Browsing namespace: {namespaceUri}")

            ' Create test session
            Dim appConfig = Await BuildApplicationConfigurationAsync()
            Dim useSecurity = False
            Dim selectedEndpoint = Await CoreClientUtils.SelectEndpointAsync(appConfig, endpointUrl, useSecurity)
            Dim endpointConfig = EndpointConfiguration.Create(appConfig)
            Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)
            testSession = Await Session.Create(appConfig, configuredEndpoint, False, "Browse Session", 30000, Nothing, Nothing)

            Console.WriteLine("✅ Browse session created")

            Dim browser = New Browser(testSession) With {
                .BrowseDirection = BrowseDirection.Forward,
                .NodeClassMask = NodeClass.Variable Or NodeClass.Object
            }

            ' First try to browse from Objects folder
            Console.WriteLine("🔍 Browsing from Objects folder...")
            Dim objectsRefs = Await browser.BrowseAsync(ObjectIds.ObjectsFolder)
            Console.WriteLine($"✅ Found {objectsRefs.Count} objects in root")

            For Each objRef As ReferenceDescription In objectsRefs
                Dim objNodeId = ExpandedNodeId.ToNodeId(objRef.NodeId, testSession.NamespaceUris)
                Console.WriteLine($"📝 Object: {objRef.DisplayName} [{objNodeId}] - {objRef.NodeClass}")

                ' Browse each object for variables
                Try
                    Dim childRefs = Await browser.BrowseAsync(objNodeId)
                    For Each childRef As ReferenceDescription In childRefs
                        Dim childNodeId = ExpandedNodeId.ToNodeId(childRef.NodeId, testSession.NamespaceUris)
                        discoveredNodes.Add(childNodeId.ToString())
                        Console.WriteLine($"  → Child: {childRef.DisplayName} [{childNodeId}] - {childRef.NodeClass}")
                    Next
                Catch childEx As Exception
                    Console.WriteLine($"  ⚠️ Cannot browse {objRef.DisplayName}: {childEx.Message}")
                End Try
            Next

            ' Also try to browse the specific namespace node if provided
            If Not String.IsNullOrEmpty(namespaceUri) Then
                Try
                    Console.WriteLine($"🔍 Trying specific namespace: {namespaceUri}")
                    Dim namespaceNodeId = NodeId.Parse(namespaceUri)
                    Dim nsRefs = Await browser.BrowseAsync(namespaceNodeId)
                    Console.WriteLine($"✅ Namespace node has {nsRefs.Count} children")

                    For Each nsRef As ReferenceDescription In nsRefs
                        Dim nsChildNodeId = ExpandedNodeId.ToNodeId(nsRef.NodeId, testSession.NamespaceUris)
                        If Not discoveredNodes.Contains(nsChildNodeId.ToString()) Then
                            discoveredNodes.Add(nsChildNodeId.ToString())
                        End If
                        Console.WriteLine($"  → NS Child: {nsRef.DisplayName} [{nsChildNodeId}] - {nsRef.NodeClass}")
                    Next
                Catch nsEx As Exception
                    Console.WriteLine($"⚠️ Cannot browse namespace {namespaceUri}: {nsEx.Message}")
                End Try
            End If

            Return discoveredNodes

        Catch ex As Exception
            Console.WriteLine($"❌ Browse failed: {ex.Message}")
            Return discoveredNodes
        Finally
            If testSession IsNot Nothing Then
                Try
                    testSession.Close()
                    testSession.Dispose()
                Catch
                End Try
            End If
        End Try
    End Function

    ' === Test namespace and nodes ===
    Public Shared Async Function TestNamespaceAndNodes(endpointUrl As String, namespaceUri As String, nodes As String()) As Task(Of Boolean)
        Dim testSession As Session = Nothing
        Try
            Console.WriteLine($"🔍 Testing namespace: {namespaceUri}")
            Console.WriteLine($"🔍 Testing {nodes.Length} nodes")

            ' Create test session
            Dim appConfig = Await BuildApplicationConfigurationAsync()
            Dim useSecurity = False
            Dim selectedEndpoint = Await CoreClientUtils.SelectEndpointAsync(appConfig, endpointUrl, useSecurity)
            Dim endpointConfig = EndpointConfiguration.Create(appConfig)
            Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)
            testSession = Await Session.Create(appConfig, configuredEndpoint, False, "Namespace Test Session", 30000, Nothing, Nothing)

            Console.WriteLine("✅ Test session created for namespace testing")

            ' Test the namespace node itself
            Dim namespaceValid = False
            Try
                Console.WriteLine($"🔍 Testing namespace node: {namespaceUri}")
                Dim namespaceNodeId = NodeId.Parse(namespaceUri)

                ' Try to browse the namespace node
                Dim browser = New Browser(testSession)
                Dim refs = Await browser.BrowseAsync(namespaceNodeId)
                Console.WriteLine($"✅ Namespace node browsed successfully, found {refs.Count} references")
                namespaceValid = True

                ' Try to read the namespace node (if it's a variable)
                Try
                    Dim value = Await testSession.ReadValueAsync(namespaceNodeId)
                    If StatusCode.IsGood(value.StatusCode) Then
                        Console.WriteLine($"✅ Namespace node value: {value.Value} [{value.StatusCode}]")
                    Else
                        Console.WriteLine($"ℹ️ Namespace node status: {value.StatusCode} (might be an object)")
                    End If
                Catch readEx As Exception
                    Console.WriteLine($"ℹ️ Namespace node is not readable (object type): {readEx.Message}")
                End Try

            Catch nsEx As Exception
                Console.WriteLine($"❌ Namespace node test failed: {nsEx.Message}")
                Console.WriteLine($"❌ Trying to browse from Objects folder instead...")
            End Try

            ' Fallback: browse from Objects folder if namespace test failed
            If Not namespaceValid Then
                Try
                    Dim browser = New Browser(testSession)
                    Dim objectsRefs = Await browser.BrowseAsync(ObjectIds.ObjectsFolder)
                    Console.WriteLine($"✅ Objects folder browsed, found {objectsRefs.Count} objects")
                    namespaceValid = True
                Catch objEx As Exception
                    Console.WriteLine($"❌ Cannot browse Objects folder: {objEx.Message}")
                    Return False
                End Try
            End If

            If Not namespaceValid Then
                Return False
            End If

            ' Test individual nodes directly
            Dim allNodesValid = True
            For Each nodeStr In nodes
                Try
                    Console.WriteLine($"🔍 Testing node: {nodeStr}")

                    Dim nodeIds = NodeId.Parse(nodeStr)
                    Dim value = Await testSession.ReadValueAsync(nodeIds)

                    If StatusCode.IsGood(value.StatusCode) Then
                        Console.WriteLine($"✅ Node {nodeStr}: {value.Value} [{value.StatusCode}]")
                    Else
                        Console.WriteLine($"⚠️ Node {nodeStr}: Status - {value.StatusCode}")
                        allNodesValid = False
                    End If
                    
                Catch nodeEx As Exception
                    Console.WriteLine($"❌ Node {nodeStr}: Error - {nodeEx.Message}")
                    allNodesValid = False
                End Try
            Next
            
            Return allNodesValid
            
        Catch ex As Exception
            Console.WriteLine($"❌ Namespace test failed: {ex.Message}")
            Return False
        Finally
            If testSession IsNot Nothing Then
                Try
                    testSession.Close()
                    testSession.Dispose()
                    Console.WriteLine("✅ Namespace test session closed")
                Catch
                    ' Ignore cleanup errors
                End Try
            End If
        End Try
    End Function

    ' === Browse child nodes of a specific parent ===
    Public Shared Async Function BrowseChildNodes(endpointUrl As String, parentNodeId As String) As Task(Of List(Of Dictionary(Of String, String)))
        Dim testSession As Session = Nothing
        Dim childNodes As New List(Of Dictionary(Of String, String))
        Try
            Console.WriteLine($"🔍 Browsing child nodes of: {parentNodeId}")

            ' Create test session
            Dim appConfig = Await BuildApplicationConfigurationAsync()
            Dim useSecurity = False
            Dim selectedEndpoint = Await CoreClientUtils.SelectEndpointAsync(appConfig, endpointUrl, useSecurity)
            Dim endpointConfig = EndpointConfiguration.Create(appConfig)
            Dim configuredEndpoint = New ConfiguredEndpoint(Nothing, selectedEndpoint, endpointConfig)
            testSession = Await Session.Create(appConfig, configuredEndpoint, False, "Child Browse Session", 30000, Nothing, Nothing)

            Dim browser = New Browser(testSession) With {
                .BrowseDirection = BrowseDirection.Forward,
                .NodeClassMask = NodeClass.Variable
            }

            ' Parse the parent NodeId
            Dim parentNode = NodeId.Parse(parentNodeId)
            Dim childRefs = Await browser.BrowseAsync(parentNode)

            For Each childRef As ReferenceDescription In childRefs
                If childRef.NodeClass = NodeClass.Variable Then
                    Dim childNodeId = ExpandedNodeId.ToNodeId(childRef.NodeId, testSession.NamespaceUris)
                    childNodes.Add(New Dictionary(Of String, String) From {
                        {"NodeText", childRef.DisplayName.Text},
                        {"NodeId", childNodeId.ToString()},
                        {"NodeType", "Float"},
                        {"NodeStatus", "Idle"},
                        {"NodeActive", "False"}
                    })
                    'Console.WriteLine($"✅ Child=======: {childRef.DisplayName} [{childNodeId}] {childRef.ReferenceTypeId()} {childRef.BrowseName()} {childRef.TypeDefinition()} {childRef.NodeClass()} {childRef.GetType()}")
                End If
            Next

            Return childNodes

        Catch ex As Exception
            Console.WriteLine($"❌ Browse child nodes failed: {ex.Message}")
            Return childNodes
        Finally
            If testSession IsNot Nothing Then
                Try
                    testSession.Close()
                    testSession.Dispose()
                Catch
                End Try
            End If
        End Try
    End Function

    ' === Disconnect manual ===
    Public Shared Async Function Disconnect() As Task
            Try
                If IsConnected Then
                    Await _session.CloseAsync()
                    Console.WriteLine("🔌 OPC UA disconnected.")
                End If
            Catch ex As Exception
                Console.WriteLine($"⚠️ Disconnect error: {ex.Message}")
            End Try
        End Function
End Class