Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Management
Imports System.Net.Sockets
Imports System.Runtime.InteropServices
Imports InTheHand.Net
Imports InTheHand.Net.Bluetooth
Imports InTheHand.Net.Sockets

Namespace CommandMessenger.Bluetooth
	Public Class BluetoothUtils

		Private privateLocalEndpoint As BluetoothEndPoint
		Public Shared Property LocalEndpoint() As BluetoothEndPoint
			Get
				Return privateLocalEndpoint
			End Get
			Private Set(ByVal value As BluetoothEndPoint)
				privateLocalEndpoint = value
			End Set
		End Property
		Private privateLocalClient As BluetoothClient
		Public Shared Property LocalClient() As BluetoothClient
			Get
				Return privateLocalClient
			End Get
			Private Set(ByVal value As BluetoothClient)
				privateLocalClient = value
			End Set
		End Property
		Private privatePrimaryRadio As BluetoothRadio
		Public Shared Property PrimaryRadio() As BluetoothRadio
			Get
				Return privatePrimaryRadio
			End Get
			Private Set(ByVal value As BluetoothRadio)
				privatePrimaryRadio = value
			End Set
		End Property
		Private Shared ReadOnly Guid As Guid = Guid.NewGuid()

		Private Shared ReadOnly DeviceList As List(Of BluetoothDeviceInfo)

'TODO: INSTANT VB TODO TASK: Generic properties are not available in VB.NET:
'ORIGINAL LINE: private static readonly List(Of string) CommonDevicePins = New List(Of string)
		Private Shared ReadOnly List CommonDevicePins = Property List() As New(Of String)
				"0000", "1111", "1234"
		End Property
		Private Shared _stream As NetworkStream

		Public Structure SerialPort
			Public Port As String
			Public DeviceId As String

		End Structure

		Shared Sub New()
			' Define common Pin codes for Bluetooth devices

			PrimaryRadio = BluetoothRadio.PrimaryRadio
			If PrimaryRadio Is Nothing Then
				'Console.WriteLine("No radio hardware or unsupported software stack");
				Return
			End If

			' Local bluetooth MAC address 
			Dim mac = PrimaryRadio.LocalAddress
			If mac Is Nothing Then
				'Console.WriteLine("No local Bluetooth MAC address found");
				Return
			End If
			DeviceList = New List(Of BluetoothDeviceInfo)()
			' mac is mac address of local bluetooth device
			'LocalEndpoint = new BluetoothEndPoint(mac, BluetoothService.SerialPort);
			LocalEndpoint = New BluetoothEndPoint(mac, Guid)
			' client is used to manage connections
			LocalClient = New BluetoothClient(LocalEndpoint)
			' component is used to manage device discovery
			'LocalComponent = new BluetoothComponent(LocalClient);
		End Sub

		Public Shared Function DeviceByAdress(ByVal address As String) As BluetoothDeviceInfo
			Return New BluetoothDeviceInfo(BluetoothAddress.Parse(address))
		End Function

		Public Shared Sub PrintPairedDevices()
			DeviceList.AddRange(LocalClient.DiscoverDevices(255, True, True, False, False))
			PrintDevices()
		End Sub

		Public Shared Sub PrintAllDevices()
			DeviceList.AddRange(LocalClient.DiscoverDevices(65536, True, True, True,True))
			PrintDevices()
		End Sub

		Private Shared Function LocalAddress() As BluetoothAddress
			If PrimaryRadio Is Nothing Then
				Console.WriteLine("No radio hardware or unsupported software stack")
				Return Nothing
			End If
			' Note that LocalAddress is null if the radio is powered-off.
			'Console.WriteLine("* Radio, address: {0:C}", primaryRadio.LocalAddress);
			Return PrimaryRadio.LocalAddress
		End Function

		Public Shared Sub UpdateClient()
			If LocalClient IsNot Nothing Then
				LocalClient.Close()
				LocalClient = New BluetoothClient(LocalEndpoint)
			End If
		End Sub

		Public Shared Function StripBluetoothAdress(ByVal bluetoothAdress As String) As String
			Dim charsToRemove = New String() {":", "-"}
			Return charsToRemove.Aggregate(bluetoothAdress, Function(current, c) current.Replace(c, String.Empty))
		End Function

		'const int IoctlBthDisconnectDevice = 0x41000c;
		'[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		'internal static extern bool DeviceIoControl(
		'IntPtr hDevice,
		'uint dwIoControlCode,
		'ref long inBuffer,
		'int nInBufferSize,
		'IntPtr outBuffer,
		'int nOutBufferSize,
		'out int pBytesReturned,
		'IntPtr lpOverlapped);
		'public static int Disconnect(BluetoothAddress address)
		'{
		'    //var primaryRadio = BluetoothRadio.PrimaryRadio;
		'    var handle = PrimaryRadio.Handle;
		'    //var btAddr = BluetoothAddress.Parse("00:1b:3d:0d:ac:31").ToInt64();
		'    var bluetoothAdress = address.ToInt64();
		'    var bytesReturned = 0;
		'    var success = DeviceIoControl(
		'        handle,
		'        IoctlBthDisconnectDevice,
		'        ref bluetoothAdress, 8,
		'        IntPtr.Zero, 
		'        0, 
		'        out bytesReturned, 
		'        IntPtr.Zero);

		'    return !success ? Marshal.GetLastWin32Error() : 0;
		'}

		'public static int Disconnect(BluetoothDeviceInfo device)
		'{
		'    return Disconnect(device.DeviceAddress);
		'}

		Public Shared Function PairDevice(ByVal device As BluetoothDeviceInfo) As Boolean
			If device.Authenticated Then
				Return True
			End If
			' loop through common PIN numbers to see if they pair
			For Each devicePin In CommonDevicePins
				Dim isPaired = BluetoothSecurity.PairRequest(device.DeviceAddress, devicePin)
				If isPaired Then
					Exit For
				End If
			Next devicePin

			device.Update()
			Return device.Authenticated
		End Function

		Public Shared Sub AutoPairDevices()
			' get a list of all paired devices
			Dim paired = LocalClient.DiscoverDevices(255, False, True, False, False)
			' check every discovered device if it is already paired 
			For Each device In DeviceList
				Dim isPaired = paired.Any(Function(t) device.Equals(t))

				' if the device is not paired, try to pair it
				If (Not isPaired) Then
					' loop through common PIN numbers to see if they pair
					For Each devicePin In CommonDevicePins
						isPaired = BluetoothSecurity.PairRequest(device.DeviceAddress, devicePin)
						If isPaired Then
							Exit For
						End If
					Next devicePin
				End If
			Next device
		End Sub


		Public Shared Sub ConnectDevice(ByVal device As BluetoothDeviceInfo, ByVal devicePin As String)
			' set pin of device to connect with
			If devicePin IsNot Nothing Then
				LocalClient.SetPin(devicePin)
			End If
			'device.SetServiceState(BluetoothService.SerialPort, false);
			'device.SetServiceState(BluetoothService.SerialPort, true);
			' check if device is paired
			If device.Authenticated Then
				' synchronous connection method
				LocalClient.Connect(device.DeviceAddress, BluetoothService.SerialPort)
				If LocalClient.Connected Then
					_stream = LocalClient.GetStream()
					_stream.ReadTimeout = 500
				End If

			End If
		End Sub

		Public Sub ConnectDevice(ByVal deviceId As Integer)

			Dim device = DeviceList(deviceId)
			ConnectDevice(device, Nothing)
		End Sub


		'public void Read()
		'{
		'    //keep connection open
		'    var buffer = new byte[2048];
		'    int bytesReceived = 0;
		'    bool listening = true;
		'    while (listening)
		'    {
		'        Thread.Yield();
		'        try
		'        {
		'            bytesReceived = _stream.Read(buffer, 0, 2048);
		'        }
		'        catch(Exception e)
		'        {
		'           Console.WriteLine("error during read: "+e.Message); 
		'        }

		'        if (bytesReceived > 0)
		'        {
		'            var stringReceived = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
		'            Console.Write(stringReceived);

		'        }
		'        else
		'        {
		'            QuickScanForDevices();
		'            // Do read to force connection check
		'            _stream.Read(buffer, 0, 0);
		'            if (!_localClient.Connected)
		'            {
		'                Console.WriteLine("Disconnected!");
		'            }
		'        }
		'    }
		'}

		Public Shared Function GetSerialPorts() As List(Of SerialPort)
			Dim portIdentifiers = New String() { "bthenum","btmodem","btport" }
			Dim portList = New List(Of SerialPort)()
			Const win32SerialPort As String = "Win32_SerialPort"
			Dim query = New SelectQuery(win32SerialPort)
			Dim managementObjectSearcher = New ManagementObjectSearcher(query)
			Dim portslist = managementObjectSearcher.Get()
			For Each port In portslist
				Dim managementObject = CType(port, ManagementObject)
				Dim deviceId = managementObject.GetPropertyValue("DeviceID").ToString()
				Dim pnpDeviceId = managementObject.GetPropertyValue("PNPDeviceID").ToString().ToLower()

				If portIdentifiers.Any(pnpDeviceId.Contains) Then
					portList.Add(New SerialPort With {.Port = deviceId, .DeviceId = pnpDeviceId})
				End If
			Next port
			Return portList
		End Function

		Public Shared Sub PrintSerialPorts()
			Dim portList = GetSerialPorts()
			For Each port In portList
				Console.WriteLine("Port: {0}, name: {1}", port.Port, port.DeviceId)
			Next port
		End Sub

		Public Shared Sub PrintLocalAddress()
			Dim localBluetoothAddress = LocalAddress()

			If localBluetoothAddress Is Nothing Then
				Return
			End If
			Console.WriteLine("{0:C}", localBluetoothAddress)
		End Sub

		Public Shared Sub PrintDevice(ByVal device As BluetoothDeviceInfo)
			' log and save all found devices

			Console.Write(device.DeviceName & " (" & device.DeviceAddress & "): Device is ")
			Console.Write(If(device.Remembered, "remembered", "not remembered"))
			Console.Write(If(device.Authenticated, ", paired", ", not paired"))
			Console.WriteLine(If(device.Connected, ", connected", ", not connected"))
		End Sub

		Private Shared Sub PrintDevices()
			' log and save all found devices
			For Each t In DeviceList
				PrintDevice(t)
			Next t
		End Sub

	End Class
End Namespace
