#Region "CmdMessenger - MIT - (c) 2013 Thijs Elenbaas."
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
'  Copyright 2013 - Thijs Elenbaas
'
#End Region


Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Net.Sockets
Imports System.Threading
Imports CommandMessenger.TransportLayer
Imports InTheHand.Net
Imports InTheHand.Net.Bluetooth
Imports InTheHand.Net.Sockets

Namespace CommandMessenger.Bluetooth
	Public Enum ThreadRunStates
		Start
		[Stop]
		Abort
	End Enum

	''' <summary>
	''' Manager for Bluetooth connection
	''' </summary>
	Public Class BluetoothTransport
		Inherits DisposableObject
		Implements ITransport
		Private _stream As NetworkStream
		'private readonly QueueSpeed _queueSpeed = new QueueSpeed(4,10);
		Private _queueThread As Thread
		Private _threadRunState As ThreadRunStates
		Private ReadOnly _threadRunStateLock As Object = New Object()
		Private ReadOnly _readLock As Object = New Object()
		Private ReadOnly _writeLock As Object = New Object()
		Private Const BufferMax As Integer = 4096
		Private ReadOnly _readBuffer(BufferMax - 1) As Byte
		Private _bufferFilled As Integer

		''' <summary> Gets or sets the run state of the thread . </summary>
		''' <value> The thread run state. </value>
		Public Property ThreadRunState() As ThreadRunStates
			Set(ByVal value As ThreadRunStates)
				SyncLock _threadRunStateLock
					_threadRunState = value
				End SyncLock
			End Set
			Get
				Dim result As ThreadRunStates
				SyncLock _threadRunStateLock
					result = _threadRunState
				End SyncLock
				Return result
			End Get
		End Property

		''' <summary> Default constructor. </summary>
		Public Sub New()
			Initialize()
		End Sub

		Protected Overrides Sub Finalize()
			Kill()
		End Sub

		''' <summary> Initializes this object. </summary>
		Public Sub Initialize()
			' _queueSpeed.Name = "Bluetooth";

			_queueThread = New Thread(AddressOf ProcessQueue) With {.Priority = ThreadPriority.Normal, .Name = "Bluetooth"}
			ThreadRunState = ThreadRunStates.Start
			_queueThread.Start()
			Do While Not _queueThread.IsAlive
				Thread.Sleep(25)
			Loop
		End Sub

		#Region "Fields"

		Public Event NewDataReceived As EventHandler Implements ITransport.NewDataReceived ' Event queue for all listeners interested in NewLinesReceived events.

		#End Region

		#Region "Properties"

		''' <summary> Gets or sets the current serial port settings. </summary>
		''' <value> The current serial settings. </value>
		Private privateCurrentBluetoothDeviceInfo As BluetoothDeviceInfo
		Public Property CurrentBluetoothDeviceInfo() As BluetoothDeviceInfo
			Get
				Return privateCurrentBluetoothDeviceInfo
			End Get
			Set(ByVal value As BluetoothDeviceInfo)
				privateCurrentBluetoothDeviceInfo = value
			End Set
		End Property

		Public ReadOnly Property BluetoothClient() As BluetoothClient
			Get
				Return BluetoothUtils.LocalClient
			End Get
		End Property


		#End Region

		#Region "Methods"

		Protected Sub ProcessQueue()
			' Endless loop
			Do While ThreadRunState <> ThreadRunStates.Abort
				Poll(ThreadRunState)
			Loop
		End Sub

		Public Sub StartListening() Implements ITransport.StartListening
			ThreadRunState = ThreadRunStates.Start
		End Sub

		Public Sub StopListening() Implements ITransport.StopListening
			ThreadRunState = ThreadRunStates.Stop
		End Sub

		Private Sub Poll(ByVal threadRunState As ThreadRunStates)
			Dim bytes = UpdateBuffer()
			If threadRunState = ThreadRunStates.Start Then
				If bytes > 0 Then
					' Send an event
					If NewDataReceived IsNot Nothing Then
						NewDataReceived(Me, Nothing)
					End If
					' Signal so that processes waiting on this continue
				End If
			End If
		End Sub

		''' <summary> Polls for bluetooth device for data. </summary>
		Public Sub Poll() Implements ITransport.Poll
			Poll(ThreadRunStates.Start)
		End Sub

		''' <summary> Connects to a serial port defined through the current settings. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Connect() As Boolean Implements ITransport.Connect
			' Closing serial port if it is open
			_stream = Nothing

			' set pin of device to connect with            
			' check if device is paired
			'CurrentBluetoothDeviceInfo.Refresh();
			Try
				If (Not CurrentBluetoothDeviceInfo.Authenticated) Then
					'Console.WriteLine("Not authenticated");
					Return False
				End If

				If BluetoothClient.Connected Then
					'Console.WriteLine("Previously connected, setting up new connection");
					BluetoothUtils.UpdateClient()
				End If

				' synchronous connection method
				BluetoothClient.Connect(CurrentBluetoothDeviceInfo.DeviceAddress, BluetoothService.SerialPort)

				If (Not Open()) Then
					Console.WriteLine("Stream not opened")
					Return False
				End If

				' Subscribe to event and open serial port for data
				ThreadRunState = ThreadRunStates.Start
				Return True
			Catch e1 As SocketException
				'Console.WriteLine("Socket exception while trying to connect");
				Return False
			Catch e2 As InvalidOperationException
				BluetoothUtils.UpdateClient()
				Return False
			End Try
		End Function

		''' <summary> Opens the serial port. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Open() As Boolean
			If (Not BluetoothClient.Connected) Then
				Return False
			End If
			_stream = BluetoothClient.GetStream()
			_stream.ReadTimeout = 2000
			_stream.WriteTimeout = 1000
			Return (True)
		End Function

		Public Function IsConnected() As Boolean Implements ITransport.IsConnected
			' note: this does not always work. Perhaps do a scan
			Return BluetoothClient.Connected
		End Function

		Public Function IsOpen() As Boolean
			' note: this does not always work. Perhaps do a scan
			Return IsConnected() AndAlso (_stream IsNot Nothing)
		End Function


		''' <summary> Closes the Bluetooth stream port. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Close() As Boolean
			' No closing needed
			If _stream Is Nothing Then
				Return True
			End If
			_stream.Close()
			_stream = Nothing
			Return True
		End Function

		''' <summary> Disconnect the bluetooth stream. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Disconnect() As Boolean Implements ITransport.Disconnect
			ThreadRunState = ThreadRunStates.Stop
			Dim state = Close()
			Return state
		End Function

		''' <summary> Writes a byte array to the bluetooth stream. </summary>
		''' <param name="buffer"> The buffer to write. </param>
		Public Sub Write(ByVal buffer() As Byte) Implements ITransport.Write
			Try
				If IsOpen() Then
					SyncLock _writeLock
						_stream.Write(buffer,0,buffer.Length)
					End SyncLock
				End If
			Catch
			End Try
		End Sub

		''' <summary> Retrieves the address of the local bluetooth radio. </summary>
		''' <returns> The address of the local bluetooth radio. </returns>
		Public Function RetreiveLocalBluetoothAddress() As BluetoothAddress
			Dim primaryRadio = BluetoothRadio.PrimaryRadio
			If primaryRadio Is Nothing Then
				Return Nothing
			End If
			Return primaryRadio.LocalAddress
		End Function

		Private Function UpdateBuffer() As Integer
			If IsOpen() Then
				Try
					SyncLock _readLock
						'if (_stream.DataAvailable)
							Dim nbrDataRead = _stream.Read(_readBuffer, _bufferFilled, (BufferMax - _bufferFilled))
							_bufferFilled += nbrDataRead
					End SyncLock
					Return _bufferFilled
				Catch e1 As IOException
					' Timeout (expected)
				End Try
			Else
				' In case of no connection 
				' Sleep a bit otherwise CPU load will go through roof
				Thread.Sleep(25)
			End If
			Return 0
		End Function

		''' <summary> Reads the serial buffer into the string buffer. </summary>
		Public Function Read() As Byte() Implements ITransport.Read
			If IsOpen() Then
				Dim buffer() As Byte
				SyncLock _readLock
					buffer = New Byte(_bufferFilled - 1){}
					Array.Copy(_readBuffer, buffer, _bufferFilled)
					_bufferFilled = 0
				End SyncLock
				Return buffer
			End If
			Return New Byte(){}
		End Function

		''' <summary> Gets the bytes in buffer. </summary>
		''' <returns> Bytes in buffer </returns>
		Public Function BytesInBuffer() As Integer Implements ITransport.BytesInBuffer
			Return If(IsOpen(), _bufferFilled, 0)
		End Function

		''' <summary> Kills this object. </summary>
		Public Sub Kill()
			' Signal thread to abort
			ThreadRunState = ThreadRunStates.Abort

			'Wait for thread to die
			Join(500)
			If _queueThread.IsAlive Then
				_queueThread.Abort()
			End If

			' Releasing stream
			If IsOpen() Then
				Close()
			End If

			' component is used to manage device discovery
			'_localComponent.Dispose();

			' client is used to manage connections
			'_localClient.Dispose();
		End Sub

		''' <summary> Joins the thread. </summary>
		''' <param name="millisecondsTimeout"> The milliseconds timeout. </param>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Join(ByVal millisecondsTimeout As Integer) As Boolean
			If _queueThread.IsAlive = False Then
				Return True
			End If
			Return _queueThread.Join(TimeSpan.FromMilliseconds(millisecondsTimeout))
		End Function

		' Dispose
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				Kill()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#End Region
	End Class
End Namespace