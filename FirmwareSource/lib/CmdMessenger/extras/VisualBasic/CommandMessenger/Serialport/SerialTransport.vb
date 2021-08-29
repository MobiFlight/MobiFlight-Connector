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
Imports System.IO.Ports
Imports System.Threading
Imports CommandMessenger.TransportLayer
Imports System.IO

Namespace CommandMessenger.Serialport
	''' <summary>Fas
	''' Manager for serial port data
	''' </summary>
	Public Class SerialTransport
		Inherits DisposableObject
		Implements ITransport
		Private Enum ThreadRunStates
			Start
			[Stop]
			Abort
		End Enum

		Private Const BufferMax As Integer = 4096

		Private _queueThread As Thread
		Private _threadRunState As ThreadRunStates
		Private ReadOnly _threadRunStateLock As Object = New Object()
		Private ReadOnly _serialReadWriteLock As Object = New Object()
		Private ReadOnly _readLock As Object = New Object()
		Private ReadOnly _readBuffer(BufferMax - 1) As Byte
		Private _bufferFilled As Integer

		''' <summary> Gets or sets the run state of the thread. </summary>
		''' <value> The thread run state. </value>
		Private Property ThreadRunState() As ThreadRunStates
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

		#Region "Fields"

		Private _serialPort As SerialPort ' The serial port
		Private _currentSerialSettings As New SerialSettings() ' The current serial settings
		Public Event NewDataReceived As EventHandler Implements ITransport.NewDataReceived ' Event queue for all listeners interested in NewLinesReceived events.

		#End Region

		#Region "Properties"

		''' <summary> Gets or sets the current serial port settings. </summary>
		''' <value> The current serial settings. </value>
		Public Property CurrentSerialSettings() As SerialSettings
			Get
				Return _currentSerialSettings
			End Get
			Set(ByVal value As SerialSettings)
				_currentSerialSettings = value
			End Set
		End Property

		#End Region

		#Region "Methods"

		''' <summary> Initializes this object. </summary>
		Private Sub Initialize()
			' Create queue thread and wait for it to start
			_queueThread = New Thread(AddressOf ProcessQueue) With {.Priority = ThreadPriority.Normal, .Name = "SerialTransport"}
			ThreadRunState = ThreadRunStates.Start
			_queueThread.Start()
			Do While Not _queueThread.IsAlive
				Thread.Sleep(50)
			Loop
		End Sub

		Private Sub ProcessQueue()
			Do While ThreadRunState <> ThreadRunStates.Abort
				Poll(ThreadRunState)
			Loop
		End Sub

		''' <summary>
		''' Start Listening
		''' </summary>
		Public Sub StartListening() Implements ITransport.StartListening
			ThreadRunState = ThreadRunStates.Start
		End Sub

		''' <summary>
		''' Stop Listening
		''' </summary>
		Public Sub StopListening() Implements ITransport.StopListening
			ThreadRunState = ThreadRunStates.Stop
		End Sub

		Private Sub Poll(ByVal threadRunState As ThreadRunStates)
			Dim bytes = UpdateBuffer()
			If threadRunState = ThreadRunStates.Start Then
				If bytes > 0 AndAlso NewDataReceived IsNot Nothing Then
					NewDataReceived(Me, Nothing)
				End If
			End If
		End Sub

		Public Sub Poll() Implements ITransport.Poll
			Poll(ThreadRunStates.Start)
		End Sub

		''' <summary> Connects to a serial port defined through the current settings. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Connect() As Boolean Implements ITransport.Connect
			If (Not _currentSerialSettings.IsValid()) Then
				Throw New InvalidOperationException("Unable to open connection - serial settings invalid.")
			End If

			' Closing serial port if it is open
			Close()

			' Setting serial port settings
			_serialPort = New SerialPort(_currentSerialSettings.PortName, _currentSerialSettings.BaudRate, _currentSerialSettings.Parity, _currentSerialSettings.DataBits, _currentSerialSettings.StopBits) With {.DtrEnable = _currentSerialSettings.DtrEnable, .WriteTimeout = _currentSerialSettings.Timeout, .ReadTimeout = _currentSerialSettings.Timeout}

			' Subscribe to event and open serial port for data
			ThreadRunState = ThreadRunStates.Start
			Return Open()
		End Function

		''' <summary> Query if the serial port is open. </summary>
		''' <returns> true if open, false if not. </returns>
		Public Function IsConnected() As Boolean Implements ITransport.IsConnected
			Try
				Return _serialPort IsNot Nothing AndAlso PortExists() AndAlso _serialPort.IsOpen
			Catch
				Return False
			End Try
		End Function

		''' <summary> Stops listening to the serial port. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Disconnect() As Boolean Implements ITransport.Disconnect
			ThreadRunState = ThreadRunStates.Stop
			Dim state = Close()
			Return state
		End Function

		''' <summary> Writes a parameter to the serial port. </summary>
		''' <param name="buffer"> The buffer to write. </param>
		Public Sub Write(ByVal buffer() As Byte) Implements ITransport.Write
			Try
				If IsConnected() Then
					SyncLock _serialReadWriteLock
						_serialPort.Write(buffer, 0, buffer.Length)
					End SyncLock
				End If
			Catch
			End Try
		End Sub

		''' <summary> Reads the serial buffer into the string buffer. </summary>
		Public Function Read() As Byte() Implements ITransport.Read
			If IsConnected() Then
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
			'return IsOpen()? _serialPort.BytesToRead:0;
			Return _bufferFilled
		End Function

		''' <summary> Opens the serial port. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Private Function Open() As Boolean
			If _serialPort IsNot Nothing AndAlso PortExists() AndAlso (Not _serialPort.IsOpen) Then
				Try
					_serialPort.Open()
					_serialPort.DiscardInBuffer()
					_serialPort.DiscardOutBuffer()
					Return _serialPort.IsOpen
				Catch
					Return False
				End Try
			End If

			Return True
		End Function

		''' <summary> Closes the serial port. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Private Function Close() As Boolean
			Try
				If _serialPort Is Nothing OrElse (Not PortExists()) Then
					Return False
				End If
				If (Not _serialPort.IsOpen) Then
					Return True
				End If
				_serialPort.Close()
				Return True
			Catch
				Return False
			End Try
		End Function

		''' <summary> Queries if a current port exists. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Private Function PortExists() As Boolean
			Return SerialUtils.PortExists(_serialPort.PortName)
		End Function

		Private Function UpdateBuffer() As Integer
			If IsConnected() Then
				Try
					SyncLock _readLock
						Dim nbrDataRead = _serialPort.Read(_readBuffer, _bufferFilled, (BufferMax - _bufferFilled))
						_bufferFilled += nbrDataRead
					End SyncLock
					Return _bufferFilled
				Catch e1 As IOException
					' IO exception (seems to happen every so much time)
				Catch e2 As TimeoutException
					' Timeout (this is expected)
				End Try
			Else
				' In case of no connection 
				' Sleep a bit otherwise CPU load will go through roof
				Thread.Sleep(25)
			End If
			Return 0
		End Function

		''' <summary> Kills this object. </summary>
		Private Sub Kill()
			' Signal thread to stop
			ThreadRunState = ThreadRunStates.Abort

			'Wait for thread to die
			Join(1200)
			If _queueThread.IsAlive Then
				_queueThread.Abort()
			End If

			' Releasing serial port 
			Close()
			If _serialPort IsNot Nothing Then
				_serialPort.Dispose()
				_serialPort = Nothing
			End If

		End Sub

		Private Function Join(ByVal millisecondsTimeout As Integer) As Boolean
			If (Not _queueThread.IsAlive) Then
				Return True
			End If
			Return _queueThread.Join(millisecondsTimeout)
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