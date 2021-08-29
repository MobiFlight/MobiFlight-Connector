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

Namespace CommandMessenger.Serialport

	''' <summary>
	''' Class for storing last succesfull connection
	''' </summary>
	<Serializable()> _
	Public Class SerialConnectionManagerSettings
		Private privatePort As String
		Public Property Port() As String
			Get
				Return privatePort
			End Get
			Set(ByVal value As String)
				privatePort = value
			End Set
		End Property
		Private privateBaudRate As Integer
		Public Property BaudRate() As Integer
			Get
				Return privateBaudRate
			End Get
			Set(ByVal value As Integer)
				privateBaudRate = value
			End Set
		End Property
	End Class

	''' <summary>
	''' Connection manager for serial port connection
	''' </summary>
	Public Class SerialConnectionManager
		Inherits ConnectionManager
		Private Enum ScanType
			None
			Quick
			Thorough
		End Enum

		Private _serialConnectionManagerSettings As SerialConnectionManagerSettings
		Private ReadOnly _serialConnectionStorer As ISerialConnectionStorer
		Private ReadOnly _serialTransport As SerialTransport

		Private _scanType As ScanType = ScanType.None

		' The control to invoke the callback on
		Private ReadOnly _tryConnectionLock As Object = New Object()

		''' <summary>
		''' Available serial port names in system.
		''' </summary>
		Private privateAvailableSerialPorts As String()
		Public Property AvailableSerialPorts() As String()
			Get
				Return privateAvailableSerialPorts
			End Get
			Private Set(ByVal value As String())
				privateAvailableSerialPorts = value
			End Set
		End Property

		''' <summary>
		''' In scan mode allow to try different baud rates besides that is configured in SerialSettings.
		''' </summary>
		Private privateDeviceScanBaudRateSelection As Boolean
		Public Property DeviceScanBaudRateSelection() As Boolean
			Get
				Return privateDeviceScanBaudRateSelection
			End Get
			Set(ByVal value As Boolean)
				privateDeviceScanBaudRateSelection = value
			End Set
		End Property

		''' <summary>
		''' Connection manager for serial port connection
		''' </summary
'TODO: INSTANT VB TODO TASK: Assignments within expressions are not supported in VB.NET
'ORIGINAL LINE: public SerialConnectionManager(SerialTransport serialTransport, CmdMessenger cmdMessenger, int watchdogCommandId = 0, string uniqueDeviceId = Nothing, ISerialConnectionStorer serialConnectionStorer = Nothing)
		Public Sub New(ByVal serialTransport As SerialTransport, ByVal cmdMessenger As CmdMessenger, Integer watchdogCommandId = ByVal 0 As , String uniqueDeviceId = ByVal [Nothing] As , ISerialConnectionStorer serialConnectionStorer = ByVal [Nothing] As )
			MyBase.New(cmdMessenger, watchdogCommandId, uniqueDeviceId)

			If serialTransport Is [Nothing] Then
				Throw New ArgumentNullException("serialTransport", "Transport is null.")
			End If

			_serialTransport = serialTransport
			_serialConnectionStorer = serialConnectionStorer
			PersistentSettings = (_serialConnectionStorer IsNot [Nothing])

			DeviceScanBaudRateSelection = True

			UpdateAvailablePorts()

			_serialConnectionManagerSettings = New SerialConnectionManagerSettings()
			ReadSettings()
		End Sub

		''' <summary>
		''' Try connection 
		''' </summary>
		''' <returns>Result</returns>
'TODO: INSTANT VB TODO TASK: Assignments within expressions are not supported in VB.NET
'ORIGINAL LINE: private DeviceStatus TryConnection(string portName = Nothing, int baudRate = int.MinValue)
		Private Function TryConnection(String portName = ByVal [Nothing] As , Integer baudRate = ByVal Integer.MinValue As ) As DeviceStatus
			SyncLock _tryConnectionLock
				' Save current port and baud rate
				Dim oldPortName As String = _serialTransport.CurrentSerialSettings.PortName
				Dim oldBaudRate As Integer = _serialTransport.CurrentSerialSettings.BaudRate

				' Update serial settings with new port and baud rate.
				If portName IsNot [Nothing] Then
					_serialTransport.CurrentSerialSettings.PortName = portName
				End If
				If baudRate IsNot Integer.MinValue Then
					_serialTransport.CurrentSerialSettings.BaudRate = baudRate
				End If

				If (Not _serialTransport.CurrentSerialSettings.IsValid()) Then
					' Restore back previous settings if newly provided was invalid.
					_serialTransport.CurrentSerialSettings.PortName = oldPortName
					_serialTransport.CurrentSerialSettings.BaudRate = oldBaudRate

					Return DeviceStatus.NotAvailable
				End If

				Connected = False

				If _serialTransport.Connect() Then
					Log(1, "Trying serial port " & _serialTransport.CurrentSerialSettings.PortName & " at " & _serialTransport.CurrentSerialSettings.BaudRate & " bauds.")

					' Calculate optimal timeout for command. It should be not less than Serial Port timeout. Lets add additional 250ms.
					Dim optimalTimeout As Integer = _serialTransport.CurrentSerialSettings.Timeout + 250
					Dim status As DeviceStatus = ArduinoAvailable(optimalTimeout)

					Connected = (status = DeviceStatus.Available)

					If Connected Then
						Log(1, "Connected to serial port " & _serialTransport.CurrentSerialSettings.PortName & " at " & _serialTransport.CurrentSerialSettings.BaudRate & " bauds.")
						StoreSettings()
					End If
					Return status
				End If

				Return DeviceStatus.NotAvailable
			End SyncLock
		End Function

		Protected Overrides Sub StartScan()
			MyBase.StartScan()

			If ConnectionManagerState = ConnectionManagerState.Scan Then
				UpdateAvailablePorts()
				_scanType = ScanType.None
			End If
		End Sub

		'Try to connect using current connections settings and trigger event if succesful
		Protected Overrides Sub DoWorkConnect()
			Dim activeConnection = False

			Try
				activeConnection = TryConnection() = DeviceStatus.Available
			Catch
			End Try

			If activeConnection Then
				ConnectionFoundEvent()
			End If
		End Sub

		' Perform scan to find connected systems
		Protected Overrides Sub DoWorkScan()
			' First try if currentConnection is open or can be opened
			Dim activeConnection = False

			Select Case _scanType
				Case ScanType.None
					Try
						activeConnection = TryConnection() = DeviceStatus.Available
					Catch
					End Try
					_scanType = ScanType.Quick
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

		Private Function QuickScan() As Boolean
			Log(3, "Performing quick scan.")

			If PersistentSettings Then
				' Then try if last stored connection can be opened
				Log(3, "Trying last stored connection.")
				If TryConnection(_serialConnectionManagerSettings.Port, _serialConnectionManagerSettings.BaudRate) = DeviceStatus.Available Then
					Return True
				End If
			End If

			' Quickly run through most used baud rates
			Dim commonBaudRates = If(DeviceScanBaudRateSelection, SerialUtils.CommonBaudRates, new Integer()){ _serialTransport.CurrentSerialSettings.BaudRate }
			For Each portName In AvailableSerialPorts
				' Get baud rates collection
				Dim baudRateCollection = If(DeviceScanBaudRateSelection, SerialUtils.GetSupportedBaudRates(portName), New Integer()){ _serialTransport.CurrentSerialSettings.BaudRate }

				Dim baudRates = commonBaudRates.Where(baudRateCollection.Contains).ToList()
				If baudRates.Any() Then
					Log(1, "Trying serial port " & portName & " using " & baudRateCollection.Length & " baud rate(s).")

					'  Now loop through baud rate collection
					For Each commonBaudRate In baudRates
						' Stop scanning if state was changed
						If ConnectionManagerState <> ConnectionManagerState.Scan Then
							Return False
						End If

						Dim status As DeviceStatus = TryConnection(portName, commonBaudRate)
						If status = DeviceStatus.Available Then
							Return True
						End If
						If status = DeviceStatus.IdentityMismatch Then ' break the loop and continue to next port.
							Exit For
						End If
					Next commonBaudRate
				End If

				' If port list has changed, interrupt scan and test new ports first
				If NewPortScan() Then
					Return True
				End If
			Next portName

			If (Not AvailableSerialPorts.Any()) Then
				' Need to check for new ports if current ports list is empty
				If NewPortScan() Then
					Return True
				End If

				' Add small delay to reduce of Quick->Thorough->Quick->Thorough scan attempts - 400ms here + 100ms in main loop = ~500ms
				Thread.Sleep(400)
			End If

			Return False
		End Function

		Private Function ThoroughScan() As Boolean
			Log(1, "Performing thorough scan.")

			' Then try if last stored connection can be opened
			If PersistentSettings AndAlso TryConnection(_serialConnectionManagerSettings.Port, _serialConnectionManagerSettings.BaudRate) = DeviceStatus.Available Then
				Return True
			End If

			' Slowly walk through 
			For Each portName In AvailableSerialPorts
				' Get baud rates collection
				Dim baudRateCollection = If(DeviceScanBaudRateSelection, SerialUtils.GetSupportedBaudRates(portName), New Integer()){ _serialTransport.CurrentSerialSettings.BaudRate }

				'  Now loop through baud rate collection
				If baudRateCollection.Any() Then
					Log(1, "Trying serial port " & portName & " using " & baudRateCollection.Length & " baud rate(s).")

					For Each baudRate In baudRateCollection
						' Stop scanning if state was changed
						If ConnectionManagerState <> ConnectionManagerState.Scan Then
							Return False
						End If

						Dim status As DeviceStatus = TryConnection(portName, baudRate)
						If status = DeviceStatus.Available Then
							Return True
						End If
						If status = DeviceStatus.IdentityMismatch Then ' break the loop and continue to next port.
							Exit For
						End If
					Next baudRate
				End If

				' If port list has changed, interrupt scan and test new ports first
				If NewPortScan() Then
					Return True
				End If
			Next portName

			If (Not AvailableSerialPorts.Any()) Then
				' Need to check for new ports if current ports list is empty
				If NewPortScan() Then
					Return True
				End If

				' Add small delay to reduce of Quick->Thorough->Quick->Thorough scan attempts - 400ms here + 100ms in main loop = ~500ms
				Thread.Sleep(400)
			End If

			Return False
		End Function

		Private Function NewPortScan() As Boolean
			' Then see if port list has changed
			Dim newPorts = NewPortInList()
			If (Not newPorts.Any()) Then
				Return False
			End If

			'TODO: 4s - practical delay for Leonardo board, probably for other boards will be different. Need to investigate more on this.
			Const waitTime As Integer = 4000
			Log(1, "New port(s) " & String.Join(",", newPorts) & " detected, wait for " & (waitTime / 1000.0) & "s before attempt to connect.")

			' Wait a bit before new port will be available then try to connect
			Thread.Sleep(waitTime)

			' Quickly run through most used ports
			Dim commonBaudRates = If(DeviceScanBaudRateSelection, SerialUtils.CommonBaudRates, New Integer()){ _serialTransport.CurrentSerialSettings.BaudRate }

			For Each portName In newPorts
				' Get baud rates collection
				Dim baudRateCollection = If(DeviceScanBaudRateSelection, SerialUtils.GetSupportedBaudRates(portName), New Integer()){ _serialTransport.CurrentSerialSettings.BaudRate }

				' First add commonBaudRates available
				Dim sortedBaudRates = commonBaudRates.Where(baudRateCollection.Contains).ToList()
				' Then add other BaudRates 
				sortedBaudRates.AddRange(baudRateCollection.Where(Function(baudRate) (Not commonBaudRates.Contains(baudRate))))

				For Each currentBaudRate In sortedBaudRates
					' Stop scanning if state was changed
					If ConnectionManagerState <> ConnectionManagerState.Scan Then
						Return False
					End If

					Dim status As DeviceStatus = TryConnection(portName, currentBaudRate)
					If status = DeviceStatus.Available Then
						Return True
					End If
					If status = DeviceStatus.IdentityMismatch Then ' break the loop and continue to next port.
						Exit For
					End If
				Next currentBaudRate
			Next portName

			Return False
		End Function

		Private Sub UpdateAvailablePorts()
			AvailableSerialPorts = SerialUtils.GetPortNames()
		End Sub

		Private Function NewPortInList() As List(Of String)
			Dim currentPorts = SerialUtils.GetPortNames()
			Dim newPorts = currentPorts.Except(AvailableSerialPorts).ToList()

			' Actualize ports collection
			AvailableSerialPorts = currentPorts

			Return newPorts
		End Function

		Protected Overrides Sub StoreSettings()
			If (Not PersistentSettings) Then
				Return
			End If

			_serialConnectionManagerSettings.Port = _serialTransport.CurrentSerialSettings.PortName
			_serialConnectionManagerSettings.BaudRate = _serialTransport.CurrentSerialSettings.BaudRate

			_serialConnectionStorer.StoreSettings(_serialConnectionManagerSettings)
		End Sub

		Protected Overrides Sub ReadSettings()
			If (Not PersistentSettings) Then
				Return
			End If
			_serialConnectionManagerSettings = _serialConnectionStorer.RetrieveSettings()
		End Sub
	End Class
End Namespace
