#Region "CmdMessenger - MIT - (c) 2014 Thijs Elenbaas."
'
'  CmdMessenger - library that provides command based messaging
'
'  Permission is hereby granted, free of charge, to any person obtaining
'  a copy of this software and associated documentation files (the
'  "Software"), to deal in the Software without restriction, including
'  without limitation the rights to use, copy, modify, merge, publish,
'  distribute, sublicense, and/or sell copies of the Software, and to
'  permit persons to whom the Software is furnished to do so, subject to
'  the following conditions:
'
'  The above copyright notice and this permission notice shall be
'  included in all copies or substantial portions of the Software.
'
'  Copyright 2014 - Thijs Elenbaas
'
#End Region


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports InTheHand.Net
Imports InTheHand.Net.Bluetooth
Imports InTheHand.Net.Sockets

' todo: User added common PINs and per-device PINs

Namespace CommandMessenger.Bluetooth
	''' <summary>
	''' Class for storing last succesful connection
	''' </summary>
	<Serializable> _
	Public Class BluetoothConnectionManagerSettings
		Private privateBluetoothAddress As BluetoothAddress
		Public Property BluetoothAddress() As BluetoothAddress
			Get
				Return privateBluetoothAddress
			End Get
			Set(ByVal value As BluetoothAddress)
				privateBluetoothAddress = value
			End Set
		End Property
		Private privateStoredDevicePins As Dictionary
		Public Property StoredDevicePins() As Dictionary(Of BluetoothAddress, String)
			Get
				Return privateStoredDevicePins
			End Get
			Set(ByVal value As Dictionary)
				privateStoredDevicePins = value
			End Set
		End Property

		Public Sub New()
			StoredDevicePins = New Dictionary(Of BluetoothAddress, String)()
		End Sub
	End Class

	''' <summary>
	''' Connection manager for Bluetooth devices
	''' </summary>
	Public Class BluetoothConnectionManager
		Inherits ConnectionManager

		Private privateDevicePins As Dictionary
		Public Property DevicePins() As Dictionary(Of String, String)
			Get
				Return privateDevicePins
			End Get
			Set(ByVal value As Dictionary)
				privateDevicePins = value
			End Set
		End Property

		Private privateGeneralPins As List
		Public Property GeneralPins() As List(Of String)
			Get
				Return privateGeneralPins
			End Get
			Set(ByVal value As List)
				privateGeneralPins = value
			End Set
		End Property

'TODO: INSTANT VB TODO TASK: Generic properties are not available in VB.NET:
'ORIGINAL LINE: private static readonly List(Of string) CommonDevicePins = New List(Of string)
		Private Shared ReadOnly List CommonDevicePins = Property List() As New(Of String)
				"0000", "1111", "1234",
		End Property

		Private Enum ScanType
			None
			Quick
			Thorough
		End Enum

		Private _bluetoothConnectionManagerSettings As BluetoothConnectionManagerSettings
		Private ReadOnly _bluetoothConnectionStorer As IBluetoothConnectionStorer
		Private ReadOnly _bluetoothTransport As BluetoothTransport
		Private _scanType As ScanType

		' The control to invoke the callback on
		Private ReadOnly _tryConnectionLock As Object = New Object()
		Private ReadOnly _deviceList As List(Of BluetoothDeviceInfo)
		Private _prevDeviceList As List(Of BluetoothDeviceInfo)

		''' <summary>
		''' Connection manager for Bluetooth devices
		''' </summary>
'TODO: INSTANT VB TODO TASK: Assignments within expressions are not supported in VB.NET
'ORIGINAL LINE: public BluetoothConnectionManager(BluetoothTransport bluetoothTransport, CmdMessenger cmdMessenger, int watchdogCommandId = 0, string uniqueDeviceId = Nothing, IBluetoothConnectionStorer bluetoothConnectionStorer = Nothing)
		Public Sub New(ByVal bluetoothTransport As BluetoothTransport, ByVal cmdMessenger As CmdMessenger, Integer watchdogCommandId = ByVal 0 As , String uniqueDeviceId = ByVal [Nothing] As , IBluetoothConnectionStorer bluetoothConnectionStorer = ByVal [Nothing] As )
			MyBase.New(cmdMessenger, watchdogCommandId, uniqueDeviceId)
			If bluetoothTransport Is [Nothing] Then
				Throw New ArgumentNullException("bluetoothTransport", "Transport is null.")
			End If

			_bluetoothTransport = bluetoothTransport

			_bluetoothConnectionManagerSettings = New BluetoothConnectionManagerSettings()
			_bluetoothConnectionStorer = bluetoothConnectionStorer
			PersistentSettings = (_bluetoothConnectionStorer IsNot [Nothing])
			ReadSettings()

			_deviceList = New List(Of BluetoothDeviceInfo)()
			_prevDeviceList = New List(Of BluetoothDeviceInfo)()

			DevicePins = New Dictionary(Of String, String)()
			GeneralPins = New List(Of String)()
		End Sub

		'Try to connect using current connections settings and trigger event if succesful
		Protected Overrides Sub DoWorkConnect()
			Const timeOut As Integer = 1000
			Dim activeConnection = False

			Try
				activeConnection = TryConnection(timeOut)
			Catch
			End Try

			If activeConnection Then
				ConnectionFoundEvent()
			End If
		End Sub

		' Perform scan to find connected systems
		Protected Overrides Sub DoWorkScan()
			If Thread.CurrentThread.Name Is Nothing Then
				Thread.CurrentThread.Name = "BluetoothConnectionManager"
			End If
			Dim activeConnection = False

			' Starting scan
			If _scanType = ScanType.None Then
				_scanType = ScanType.Quick
			End If

			Select Case _scanType
				Case ScanType.Quick
					Try
						activeConnection = QuickScan()
					Catch
					End Try
					_scanType = ScanType.Thorough
				Case ScanType.Thorough
					Try
						activeConnection = ThoroughScan()
					Catch
					End Try
					_scanType = ScanType.Quick
			End Select

			' Trigger event when a connection was made
			If activeConnection Then
				ConnectionFoundEvent()
			End If
		End Sub

		Private Sub QuickScanDevices()
			' Fast
			_prevDeviceList = _deviceList
			_deviceList.Clear()
			_deviceList.AddRange(_bluetoothTransport.BluetoothClient.DiscoverDevices(255, True, True, False, False))
		End Sub

		Private Sub ThorougScanForDevices()
			' Slow
			_deviceList.Clear()
			_deviceList.AddRange(_bluetoothTransport.BluetoothClient.DiscoverDevices(65536, True, True, True, True))
		End Sub

		Private Function PairDevice(ByVal device As BluetoothDeviceInfo) As Boolean
			If device.Authenticated Then
				Return True
			End If
			Log(2, "Trying to pair device " & device.DeviceName & " (" & device.DeviceAddress & ") ")

			' Check if PIN  for this device has been injected in ConnectionManager  
			Dim adress As String = device.DeviceAddress.ToString()

			Dim matchedDevicePin = FindPin(adress)
			If matchedDevicePin IsNot Nothing Then

				Log(3, "Trying known key for device " & device.DeviceName)
				If BluetoothSecurity.PairRequest(device.DeviceAddress, matchedDevicePin) Then
					Log(2, "Pairing device " & device.DeviceName & " succesful! ")
					Return True
				End If
				' When trying PINS, you really need to wait in between
				Thread.Sleep(1000)
			End If

			' Check if PIN has been previously found and stored
			If _bluetoothConnectionManagerSettings.StoredDevicePins.ContainsKey(device.DeviceAddress) Then
				Log(3, "Trying stored key for device " & device.DeviceName)
				If BluetoothSecurity.PairRequest(device.DeviceAddress, _bluetoothConnectionManagerSettings.StoredDevicePins(device.DeviceAddress)) Then
					Log(2, "Pairing device " & device.DeviceName & " succesful! ")
					Return True
				End If
				' When trying PINS, you really need to wait in between
				Thread.Sleep(1000)
			End If

			' loop through general pins PIN numbers that have been injected to see if they pair
			For Each devicePin As String In GeneralPins

				Log(3, "Trying known general pin " & devicePin & " for device " & device.DeviceName)
				Dim isPaired = BluetoothSecurity.PairRequest(device.DeviceAddress, devicePin)
				If isPaired Then
					_bluetoothConnectionManagerSettings.StoredDevicePins(device.DeviceAddress) = devicePin
					Log(2, "Pairing device " & device.DeviceName & " succesful! ")
					Return True
				End If
				' When trying PINS, you really need to wait in between
				Thread.Sleep(1000)
			Next devicePin

			' loop through common PIN numbers to see if they pair
			For Each devicePin As String In CommonDevicePins
				Log(3, "Trying common pin " & devicePin & " for device " & device.DeviceName)
				Dim isPaired = BluetoothSecurity.PairRequest(device.DeviceAddress, devicePin)
				If isPaired Then
					_bluetoothConnectionManagerSettings.StoredDevicePins(device.DeviceAddress) = devicePin
					StoreSettings()
					Log(2, "Pairing device " & device.DeviceName & " succesful! ")
					Return True
				End If
				' When trying PINS, you really need to wait in between
				Thread.Sleep(1000)
			Next devicePin

			Log(2, "Pairing device " & device.DeviceName & " unsuccesfull ")
			Return True
		End Function

		Private Function FindPin(ByVal adress As String) As String
			Return (From devicePin In DevicePins _
			        Where BluetoothUtils.StripBluetoothAdress(devicePin.Key) = adress _
			        Select devicePin.Value).FirstOrDefault()
		End Function

		Private Function TryConnection(ByVal bluetoothAddress As BluetoothAddress, ByVal timeOut As Integer) As Boolean
			If bluetoothAddress Is Nothing Then
				Return False
			End If
			' Find
			For Each bluetoothDeviceInfo In _deviceList
				If bluetoothDeviceInfo.DeviceAddress Is bluetoothAddress Then
					Return TryConnection(bluetoothDeviceInfo, timeOut)
				End If
			Next bluetoothDeviceInfo
			Return False
		End Function

		Private Function TryConnection(ByVal bluetoothDeviceInfo As BluetoothDeviceInfo, ByVal timeOut As Integer) As Boolean
			' Try specific settings
			_bluetoothTransport.CurrentBluetoothDeviceInfo = bluetoothDeviceInfo
			Return TryConnection(timeOut)
		End Function

		Private Function TryConnection(ByVal timeOut As Integer) As Boolean
			SyncLock _tryConnectionLock
				' Check if an (old) connection exists
				If _bluetoothTransport.CurrentBluetoothDeviceInfo Is Nothing Then
					Return False
				End If

				Connected = False
				Log(1, "Trying Bluetooth device " & _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceName)
				If _bluetoothTransport.Connect() Then
					Log(3, "Connected with Bluetooth device " & _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceName & ", requesting response")

					Dim status As DeviceStatus = ArduinoAvailable(timeOut, 5)
					Connected = (status = DeviceStatus.Available)

					If Connected Then
						Log(1, "Connected with Bluetooth device " & _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceName)
						StoreSettings()
					Else
						Log(3, "Connected with Bluetooth device " & _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceName & ", received no response")
					End If
					Return Connected
				Else
					Log(3, "No connection made with Bluetooth device " & _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceName)
				End If
				Return False
			End SyncLock
		End Function

		Protected Overrides Sub StartScan()
			MyBase.StartScan()

			If ConnectionManagerState = ConnectionManagerState.Scan Then
				_scanType = ScanType.None
			End If
		End Sub

		Private Function QuickScan() As Boolean
			Log(3, "Performing quick scan")
			Const longTimeOut As Integer = 1000
			Const shortTimeOut As Integer = 1000

			' First try if currentConnection is open or can be opened
			If TryConnection(longTimeOut) Then
				Return True
			End If

			' Do a quick rescan of all devices in range
			QuickScanDevices()

			If PersistentSettings Then
				' Then try if last stored connection can be opened
				Log(3, "Trying last stored connection")
				If TryConnection(_bluetoothConnectionManagerSettings.BluetoothAddress, longTimeOut) Then
					Return True
				End If
			End If

			' Then see if new devices have been added to the list 
			If NewDevicesScan() Then
				Return True
			End If

			For Each device In _deviceList

				Thread.Sleep(100) ' Bluetooth devices seem to work more reliably with some waits
				Log(1, "Trying Device " & device.DeviceName & " (" & device.DeviceAddress & ") ")
				If TryConnection(device, shortTimeOut) Then
					Return True
				End If
			Next device

			Return False
		End Function

		Private Function ThoroughScan() As Boolean
			Log(3, "Performing thorough scan")
			Const longTimeOut As Integer = 1000
			Const shortTimeOut As Integer = 1000

			' First try if currentConnection is open or can be opened
			If TryConnection(longTimeOut) Then
				Return True
			End If

			' Do a quick rescan of all devices in range
			ThorougScanForDevices()

			' Then try if last stored connection can be opened
			Log(3, "Trying last stored connection")
			If TryConnection(_bluetoothConnectionManagerSettings.BluetoothAddress, longTimeOut) Then
				Return True
			End If

			' Then see if new devices have been added to the list 
			If NewDevicesScan() Then
				Return True
			End If

			For Each device In _deviceList
				Thread.Sleep(100) ' Bluetooth devices seem to work more reliably with some waits
				If PairDevice(device) Then
					Log(1, "Trying Device " & device.DeviceName & " (" & device.DeviceAddress & ") ")
					If TryConnection(device, shortTimeOut) Then
						Return True
					End If
				End If
			Next device
			Return False
		End Function

		Private Function NewDevicesScan() As Boolean
			Const shortTimeOut As Integer = 200

			' Then see if port list has changed
			Dim newDevices = NewDevicesInList()
			If newDevices.Count = 0 Then
				Return False
			End If

			Log(1, "Trying new devices")

			For Each device In newDevices
				If TryConnection(device, shortTimeOut) Then
					Return True
				End If
				Thread.Sleep(100)
			Next device
			Return False
		End Function

		Private Function NewDevicesInList() As List(Of BluetoothDeviceInfo)
			Return (From device In _deviceList _
			        From prevdevice In _prevDeviceList _
			        Where device.DeviceAddress IsNot prevdevice.DeviceAddress _
			        Select device).ToList()
		End Function

		Protected Overrides Sub StoreSettings()
			If (Not PersistentSettings) Then
				Return
			End If
			_bluetoothConnectionManagerSettings.BluetoothAddress = _bluetoothTransport.CurrentBluetoothDeviceInfo.DeviceAddress

			_bluetoothConnectionStorer.StoreSettings(_bluetoothConnectionManagerSettings)
		End Sub

		Protected Overrides NotOverridable Sub ReadSettings()
			If (Not PersistentSettings) Then
				Return
			End If
			_bluetoothConnectionManagerSettings = _bluetoothConnectionStorer.RetrieveSettings()
		End Sub
	End Class
End Namespace
