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
Imports System.ComponentModel
Imports System.Threading
Imports System.Windows.Forms

Namespace CommandMessenger
	Public Class ConnectionManagerProgressEventArgs
		Inherits EventArgs
		Private privateLevel As Integer
		Public Property Level() As Integer
			Get
				Return privateLevel
			End Get
			Set(ByVal value As Integer)
				privateLevel = value
			End Set
		End Property
		Private privateDescription As String
		Public Property Description() As String
			Get
				Return privateDescription
			End Get
			Set(ByVal value As String)
				privateDescription = value
			End Set
		End Property
	End Class

	Public Enum ConnectionManagerState
		Scan
		Connect
		Watchdog
		Wait
		[Stop]
	End Enum

	Public Enum DeviceStatus
		NotAvailable
		Available
		IdentityMismatch
	End Enum

	Public MustInherit Class ConnectionManager
		Implements IDisposable
		Protected ReadOnly CmdMessenger As CmdMessenger
		Protected ConnectionManagerState As ConnectionManagerState

		Public Event ConnectionTimeout As EventHandler
		Public Event ConnectionFound As EventHandler
		Public Event Progress As EventHandler(Of ConnectionManagerProgressEventArgs)

		Private ReadOnly _workerThread As BackgroundWorker
		Private ReadOnly _identifyCommandId As Integer
		Private ReadOnly _uniqueDeviceId As String

		Private _lastCheckTime As Long
		Private _nextTimeOutCheck As Long
		Private _watchdogTries As UInteger
		Private _watchdogEnabled As Boolean

		''' <summary>
		''' Is connection manager currently connected to device.
		''' </summary>
		Private privateConnected As Boolean
		Public Property Connected() As Boolean
			Get
				Return privateConnected
			End Get
			Protected Set(ByVal value As Boolean)
				privateConnected = value
			End Set
		End Property

		Private privateWatchdogTimeout As Integer
		Public Property WatchdogTimeout() As Integer
			Get
				Return privateWatchdogTimeout
			End Get
			Set(ByVal value As Integer)
				privateWatchdogTimeout = value
			End Set
		End Property
		Private privateWatchdogRetryTimeout As Integer
		Public Property WatchdogRetryTimeout() As Integer
			Get
				Return privateWatchdogRetryTimeout
			End Get
			Set(ByVal value As Integer)
				privateWatchdogRetryTimeout = value
			End Set
		End Property
		Private privateWatchdogTries As UInteger
		Public Property WatchdogTries() As UInteger
			Get
				Return privateWatchdogTries
			End Get
			Set(ByVal value As UInteger)
				privateWatchdogTries = value
			End Set
		End Property

		Friend ReadOnly Property ControlToInvokeOn() As Control
			Get
				Return CmdMessenger.ControlToInvokeOn
			End Get
		End Property

		''' <summary>
		''' Enables or disables connection watchdog functionality using identify command and unique device id.
		''' </summary>
		Public Property WatchdogEnabled() As Boolean
			Get
				Return _watchdogEnabled
			End Get
			Set(ByVal value As Boolean)
				If value AndAlso String.IsNullOrEmpty(_uniqueDeviceId) Then
					Throw New InvalidOperationException("Watchdog can't be enabled without Unique Device ID.")
				End If
				_watchdogEnabled = value
			End Set
		End Property

		''' <summary>
		''' Enables or disables device scanning. 
		''' When disabled, connection manager will try to open connection to the device configured in the setting.
		''' - For SerialConnection this means scanning for (virtual) serial ports, 
		''' - For BluetoothConnection this means scanning for a device on RFCOMM level
		''' </summary>
		Private privateDeviceScanEnabled As Boolean
		Public Property DeviceScanEnabled() As Boolean
			Get
				Return privateDeviceScanEnabled
			End Get
			Set(ByVal value As Boolean)
				privateDeviceScanEnabled = value
			End Set
		End Property

		''' <summary>
		''' Enables or disables storing of last connection configuration in persistent file.
		''' </summary>
		Private privatePersistentSettings As Boolean
		Public Property PersistentSettings() As Boolean
			Get
				Return privatePersistentSettings
			End Get
			Friend Set(ByVal value As Boolean)
				privatePersistentSettings = value
			End Set
		End Property

'TODO: INSTANT VB TODO TASK: Assignments within expressions are not supported in VB.NET
'ORIGINAL LINE: protected ConnectionManager(CmdMessenger cmdMessenger, int identifyCommandId = 0, string uniqueDeviceId = Nothing)
		Protected Sub New(ByVal cmdMessenger As CmdMessenger, Integer identifyCommandId = ByVal 0 As , String uniqueDeviceId = ByVal [Nothing] As )
			If cmdMessenger Is [Nothing] Then
				Throw New ArgumentNullException("cmdMessenger", "Command Messenger is null.")
			End If

			_identifyCommandId = identifyCommandId
			_uniqueDeviceId = uniqueDeviceId

			WatchdogTimeout = 3000
			WatchdogRetryTimeout = 1500
			WatchdogTries = 3
			WatchdogEnabled = False

			PersistentSettings = False
			DeviceScanEnabled = True

			CmdMessenger = cmdMessenger

			ConnectionManagerState = ConnectionManagerState.Stop

			_workerThread = New BackgroundWorker With {.WorkerSupportsCancellation = True, .WorkerReportsProgress = False}

			If (Not String.IsNullOrEmpty(uniqueDeviceId)) Then
				CmdMessenger.Attach(identifyCommandId, AddressOf OnIdentifyResponse)
			End If
		End Sub

		''' <summary>
		''' Start connection manager.
		''' </summary>
		Public Overridable Function StartConnectionManager() As Boolean
			If ConnectionManagerState = ConnectionManagerState.Stop Then
				ConnectionManagerState = ConnectionManagerState.Wait
				If (Not _workerThread.IsBusy) Then
					AddHandler _workerThread.DoWork, AddressOf WorkerThreadDoWork
					' Start the asynchronous operation.
					_workerThread.RunWorkerAsync()

					If DeviceScanEnabled Then
						StartScan()
					Else
						StartConnect()
					End If

					Return True
				End If
			End If

			Return False
		End Function

		''' <summary>
		''' Stop connection manager.
		''' </summary>
		Public Overridable Sub StopConnectionManager()
			If ConnectionManagerState <> ConnectionManagerState.Stop Then
				ConnectionManagerState = ConnectionManagerState.Stop

				If _workerThread.WorkerSupportsCancellation Then
					' Cancel the asynchronous operation.
					_workerThread.CancelAsync()
				End If

				RemoveHandler _workerThread.DoWork, AddressOf WorkerThreadDoWork
			End If
		End Sub

		Protected Overridable Sub ConnectionFoundEvent()
			ConnectionManagerState = ConnectionManagerState.Wait

			If WatchdogEnabled Then
				StartWatchDog()
			End If

			InvokeEvent(AddressOf ConnectionFoundEvent)
		End Sub

		Protected Overridable Sub ConnectionTimeoutEvent()
			ConnectionManagerState = ConnectionManagerState.Wait

			Disconnect()

			InvokeEvent(AddressOf ConnectionTimeoutEvent)

			If WatchdogEnabled Then
				StopWatchDog()

				If DeviceScanEnabled Then
					StartScan()
				Else
					StartConnect()
				End If
			End If
		End Sub

		Protected Overridable Sub InvokeEvent(ByVal eventHandler As EventHandler)
			Try
				If eventHandler Is Nothing OrElse (ControlToInvokeOn IsNot Nothing AndAlso ControlToInvokeOn.IsDisposed) Then
					Return
				End If
				If ControlToInvokeOn IsNot Nothing AndAlso ControlToInvokeOn.InvokeRequired Then
					'Asynchronously call on UI thread
					ControlToInvokeOn.BeginInvoke(CType(Function() eventHandler(Me, Nothing), MethodInvoker))
					Thread.Yield()
				Else
					'Directly call
					eventHandler(Me, Nothing)
				End If
			Catch e1 As Exception
			End Try
		End Sub

		Protected Overridable Sub InvokeEvent(Of TEventHandlerArguments As EventArgs)(ByVal eventHandler As EventHandler(Of TEventHandlerArguments), ByVal eventHandlerArguments As TEventHandlerArguments)
			Try
				If eventHandler Is Nothing OrElse (ControlToInvokeOn IsNot Nothing AndAlso ControlToInvokeOn.IsDisposed) Then
					Return
				End If
				If ControlToInvokeOn IsNot Nothing AndAlso ControlToInvokeOn.InvokeRequired Then
					'Asynchronously call on UI thread
					ControlToInvokeOn.BeginInvoke(CType(Function() eventHandler(Me, eventHandlerArguments), MethodInvoker))
					Thread.Yield()
				Else
					'Directly call
					eventHandler(Me, eventHandlerArguments)
				End If
			Catch e1 As Exception
			End Try
		End Sub

		Protected Overridable Sub Log(ByVal level As Integer, ByVal logMessage As String)
			Dim args = New ConnectionManagerProgressEventArgs With {.Level = level, .Description = logMessage}
			InvokeEvent(ProgressEvent, args)
		End Sub

		Protected Overridable Sub OnIdentifyResponse(ByVal responseCommand As ReceivedCommand)
			If responseCommand.Ok AndAlso (Not String.IsNullOrEmpty(_uniqueDeviceId)) Then
				ValidateDeviceUniqueId(responseCommand)
			End If
		End Sub

		Private Sub WorkerThreadDoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
			If Thread.CurrentThread.Name Is Nothing Then
				Thread.CurrentThread.Name = "SerialConnectionManager"
			End If

			Do While ConnectionManagerState <> ConnectionManagerState.Stop
				' Check if thread is being canceled
				Dim worker = TryCast(sender, BackgroundWorker)
				If worker IsNot Nothing AndAlso worker.CancellationPending Then
					Exit Do
				End If

				' Switch between waiting, device scanning and watchdog 
				Select Case ConnectionManagerState
					Case ConnectionManagerState.Scan
						DoWorkScan()
					Case ConnectionManagerState.Connect
						DoWorkConnect()
					Case ConnectionManagerState.Watchdog
						DoWorkWatchdog()
				End Select

				' Sleep a bit before checking again. If not present, the connection manager will 
				' consume a lot of CPU resources while waiting
				Thread.Sleep(100)
			Loop
		End Sub

		''' <summary>
		'''  Check if Arduino is available
		''' </summary>
		''' <param name="timeOut">Timout for waiting on response</param>
		''' <returns>Check result.</returns>
		Public Function ArduinoAvailable(ByVal timeOut As Integer) As DeviceStatus
			Dim challengeCommand = New SendCommand(_identifyCommandId, _identifyCommandId, timeOut)
			Dim responseCommand = CmdMessenger.SendCommand(challengeCommand, SendQueue.InFrontQueue, ReceiveQueue.Default, UseQueue.BypassQueue)

			If responseCommand.Ok AndAlso (Not String.IsNullOrEmpty(_uniqueDeviceId)) Then
				Return If(ValidateDeviceUniqueId(responseCommand), DeviceStatus.Available, DeviceStatus.IdentityMismatch)
			End If

			Return If(responseCommand.Ok, DeviceStatus.Available, DeviceStatus.NotAvailable)
		End Function

		''' <summary>
		'''  Check if Arduino is available
		''' </summary>
		''' <param name="timeOut">Timout for waiting on response</param>
		''' <param name="tries">Number of tries</param>
		''' <returns>Check result.</returns>
		Public Function ArduinoAvailable(ByVal timeOut As Integer, ByVal tries As Integer) As DeviceStatus
			For i = 1 To tries
				Log(3, "Polling Arduino, try # " & i)

				Dim status As DeviceStatus = ArduinoAvailable(timeOut)
				If status = DeviceStatus.Available OrElse status = DeviceStatus.IdentityMismatch Then
					Return status
				End If
			Next i
			Return DeviceStatus.NotAvailable
		End Function

		Protected Overridable Function ValidateDeviceUniqueId(ByVal responseCommand As ReceivedCommand) As Boolean
			Dim valid As Boolean = _uniqueDeviceId = responseCommand.ReadStringArg()
			If (Not valid) Then
				Log(3, "Invalid device response. Device ID mismatch.")
			End If

			Return valid
		End Function

		'Try to connect using current connections settings
		Protected MustOverride Sub DoWorkConnect()

		' Perform scan to find connected systems
		Protected MustOverride Sub DoWorkScan()

		Protected Overridable Sub DoWorkWatchdog()
			Dim lastLineTimeStamp = CmdMessenger.LastReceivedCommandTimeStamp
			Dim currentTimeStamp = TimeUtils.Millis

			' If timeout has not elapsed, wait till next watch time
			If currentTimeStamp < _nextTimeOutCheck Then
				Return
			End If

			' if a command has been received recently, set next check time
			If lastLineTimeStamp >= _lastCheckTime Then
				Log(3, "Successful watchdog response.")
				_lastCheckTime = currentTimeStamp
				_nextTimeOutCheck = _lastCheckTime + WatchdogTimeout
				_watchdogTries = 0
				Return
			End If

			' Apparently, other side has not reacted in time
			' If too many tries, notify and stop
			If _watchdogTries >= WatchdogTries Then
				Log(2, "Watchdog received no response after final try #" & WatchdogTries)
				_watchdogTries = 0
				ConnectionManagerState = ConnectionManagerState.Wait
				ConnectionTimeoutEvent()
				Return
			End If

			' We'll try another time
			' We queue the command in order to not be intrusive, but put it in front to get a quick answer
			CmdMessenger.SendCommand(New SendCommand(_identifyCommandId), SendQueue.InFrontQueue, ReceiveQueue.Default)
			_watchdogTries += 1

			_lastCheckTime = currentTimeStamp
			_nextTimeOutCheck = _lastCheckTime + WatchdogRetryTimeout
			Log(3,If(_watchdogTries = 1, "Watchdog detected no communication for " & WatchdogTimeout/1000.0 & "s, asking for response", "Watchdog received no response, performing try #" & _watchdogTries))
		End Sub

		''' <summary>
		''' Disconnect from Arduino
		''' </summary>
		''' <returns>true if sucessfully disconnected</returns>
		Public Function Disconnect() As Boolean
			If Connected Then
				Connected = False
				Return CmdMessenger.Disconnect()
			End If

			Return True
		End Function

		''' <summary>
		''' Start watchdog. Will check if connection gets interrupted
		''' </summary>
		Protected Overridable Sub StartWatchDog()
			If ConnectionManagerState <> ConnectionManagerState.Watchdog AndAlso Connected Then
				Log(1, "Starting Watchdog.")
				_lastCheckTime = TimeUtils.Millis
				_nextTimeOutCheck = _lastCheckTime + WatchdogTimeout
				_watchdogTries = 0

				ConnectionManagerState = ConnectionManagerState.Watchdog
			End If
		End Sub

		''' <summary>
		''' Stop watchdog.
		''' </summary>
		Protected Overridable Sub StopWatchDog()
			If ConnectionManagerState = ConnectionManagerState.Watchdog Then
				Log(1, "Stopping Watchdog.")
				ConnectionManagerState = ConnectionManagerState.Wait
			End If
		End Sub

		''' <summary>
		''' Start scanning for devices
		''' </summary>
		Protected Overridable Sub StartScan()
			If ConnectionManagerState <> ConnectionManagerState.Scan AndAlso (Not Connected) Then
				Log(1, "Starting device scan.")
				ConnectionManagerState = ConnectionManagerState.Scan
			End If
		End Sub

		''' <summary>
		''' Stop scanning for devices
		''' </summary>
		Protected Overridable Sub StopScan()
			If ConnectionManagerState = ConnectionManagerState.Scan Then
				Log(1, "Stopping device scan.")
				ConnectionManagerState = ConnectionManagerState.Wait
			End If
		End Sub

		''' <summary>
		''' Start connect to device
		''' </summary>
		Protected Overridable Sub StartConnect()
			If ConnectionManagerState <> ConnectionManagerState.Connect AndAlso (Not Connected) Then
				Log(1, "Start connecting to device.")
				ConnectionManagerState = ConnectionManagerState.Connect
			End If
		End Sub

		''' <summary>
		''' Stop connect to device
		''' </summary>
		Protected Overridable Sub StopConnect()
			If ConnectionManagerState = ConnectionManagerState.Connect Then
				Log(1, "Stop connecting to device.")
				ConnectionManagerState = ConnectionManagerState.Wait
			End If
		End Sub

		Protected Overridable Sub StoreSettings()
		End Sub

		Protected Overridable Sub ReadSettings()
		End Sub

		' Dispose 
		Public Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
		End Sub

		' Dispose
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				StopConnectionManager()
			End If
		End Sub
	End Class
End Namespace


