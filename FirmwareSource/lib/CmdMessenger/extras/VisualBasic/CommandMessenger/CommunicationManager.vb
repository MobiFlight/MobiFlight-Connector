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
Imports System.Text
Imports System.Threading
Imports CommandMessenger.TransportLayer

Namespace CommandMessenger
	''' <summary>
	''' Manager for data over transport layer. 
	''' </summary>
	Public Class CommunicationManager
		Inherits DisposableObject
		Private _transport As ITransport
		Private ReadOnly _stringEncoder As Encoding = Encoding.GetEncoding("ISO-8859-1") ' The string encoder
		Private _buffer As String = String.Empty ' The buffer

		Private _receiveCommandQueue As ReceiveCommandQueue
		Private _isEscaped As IsEscaped ' The is escaped
		Private _fieldSeparator As Char ' The field separator
		Private _commandSeparator As Char ' The command separator
		Private _escapeCharacter As Char ' The escape character
		Private ReadOnly _parseLinesLock As Object = New Object()

		''' <summary> Default constructor. </summary>
		''' /// <param name="disposeStack"> The DisposeStack</param>
		''' <param name="transport"> The Transport Layer</param>
		''' <param name="receiveCommandQueue"></param>
		Public Sub New(ByVal disposeStack As DisposeStack, ByVal transport As ITransport, ByVal receiveCommandQueue As ReceiveCommandQueue)
			Initialize(disposeStack,transport, receiveCommandQueue, ";"c, ","c, "/"c)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="receiveCommandQueue"></param>
		''' <param name="commandSeparator">    The End-Of-Line separator. </param>
		''' <param name="fieldSeparator"></param>
		''' <param name="escapeCharacter"> The escape character. </param>
		''' <param name="disposeStack"> The DisposeStack</param>
		''' <param name="transport"> The Transport Layer</param>
		Public Sub New(ByVal disposeStack As DisposeStack, ByVal transport As ITransport, ByVal receiveCommandQueue As ReceiveCommandQueue, ByVal commandSeparator As Char, ByVal fieldSeparator As Char, ByVal escapeCharacter As Char)
			Initialize(disposeStack, transport, receiveCommandQueue, commandSeparator, fieldSeparator, escapeCharacter)
		End Sub

		''' <summary> Finaliser. </summary>
		Protected Overrides Sub Finalize()
			Dispose(False)
		End Sub

		''' <summary> Initializes this object. </summary>
		''' <param name="receiveCommandQueue"></param>
		''' <param name="commandSeparator">    The End-Of-Line separator. </param>
		''' <param name="fieldSeparator"></param>
		''' <param name="escapeCharacter"> The escape character. </param>
		''' <param name="disposeStack"> The DisposeStack</param>
		''' /// <param name="transport"> The Transport Layer</param>
		Private Sub Initialize(ByVal disposeStack As DisposeStack, ByVal transport As ITransport, ByVal receiveCommandQueue As ReceiveCommandQueue, ByVal commandSeparator As Char, ByVal fieldSeparator As Char, ByVal escapeCharacter As Char)
			disposeStack.Push(Me)
			_transport = transport
			_receiveCommandQueue = receiveCommandQueue
			AddHandler _transport.NewDataReceived, AddressOf NewDataReceived
			_commandSeparator = commandSeparator
			_fieldSeparator = fieldSeparator
			_escapeCharacter = escapeCharacter

			_isEscaped = New IsEscaped()
		End Sub

		#Region "Fields"

		''' <summary> Gets or sets the time stamp of the last received line. </summary>
		''' <value> time stamp of the last received line. </value>
		Private privateLastLineTimeStamp As Long
		Public Property LastLineTimeStamp() As Long
			Get
				Return privateLastLineTimeStamp
			End Get
			Set(ByVal value As Long)
				privateLastLineTimeStamp = value
			End Set
		End Property

		#End Region

		#Region "Properties"


		#End Region

		#Region "Event handlers"

		''' <summary> transport layer data received. </summary>
		Private Sub NewDataReceived(ByVal o As Object, ByVal e As EventArgs)
			ParseLines()
		End Sub

		#End Region

		#Region "Methods"

		''' <summary> Connects to a transport layer defined through the current settings. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Connect() As Boolean
			Return _transport.Connect()
		End Function

		''' <summary> Stops listening to the transport layer </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Disconnect() As Boolean
			Return _transport.Disconnect()
		End Function

		''' <summary> Starts polling. </summary>
		Public Sub StartPolling()
			_transport.StartListening()
		End Sub

		''' <summary> Stop polling. </summary>
		Public Sub StopPolling()
			_transport.StopListening()
		End Sub

		''' <summary> Writes a string to the transport layer. </summary>
		''' <param name="value"> The string to write. </param>
		Public Sub WriteLine(ByVal value As String)
			Dim writeBytes() As Byte = _stringEncoder.GetBytes(value & ControlChars.Lf)
			_transport.Write(writeBytes)
		End Sub

		''' <summary> Writes a parameter to the transport layer followed by a NewLine. </summary>
		''' <typeparam name="T"> Generic type parameter. </typeparam>
		''' <param name="value"> The value. </param>
		Public Sub WriteLine(Of T)(ByVal value As T)
			Dim writeString = value.ToString()
			Dim writeBytes() As Byte = _stringEncoder.GetBytes(writeString + ControlChars.Lf)
			_transport.Write(writeBytes)
		End Sub

		''' <summary> Writes a parameter to the transport layer. </summary>
		''' <typeparam name="T"> Generic type parameter. </typeparam>
		''' <param name="value"> The value. </param>
		Public Sub Write(Of T)(ByVal value As T)
			Dim writeString = value.ToString()
			Dim writeBytes() As Byte = _stringEncoder.GetBytes(writeString)
			_transport.Write(writeBytes)
		End Sub

		Public Sub UpdateTransportBuffer()
			_transport.Poll()
		End Sub

		''' <summary> Reads the transport buffer into the string buffer. </summary>
		Private Sub ReadInBuffer()
			Dim data = _transport.Read()
			_buffer &= _stringEncoder.GetString(data)
		End Sub

		Private Sub ParseLines()
			SyncLock _parseLinesLock
				ReadInBuffer()

				Do
					Dim currentLine As String = ParseLine()
					If String.IsNullOrEmpty(currentLine) Then
						Exit Do
					End If

					LastLineTimeStamp = TimeUtils.Millis
					ProcessLine(currentLine)
				Loop While True
			End SyncLock
		End Sub

		''' <summary> Processes the byte message and add to queue. </summary>
		Public Sub ProcessLine(ByVal line As String)
			' Read line from raw buffer and make command
			Dim currentReceivedCommand = ParseMessage(line)
			currentReceivedCommand.RawString = line
			' Set time stamp
			currentReceivedCommand.TimeStamp = LastLineTimeStamp
			' And put on queue
			_receiveCommandQueue.QueueCommand(currentReceivedCommand)
		End Sub

		''' <summary> Parse message. </summary>
		''' <param name="line"> The received command line. </param>
		''' <returns> The received command. </returns>
		Private Function ParseMessage(ByVal line As String) As ReceivedCommand
			' Trim and clean line
			Dim cleanedLine = line.Trim(ControlChars.Cr, ControlChars.Lf)
			cleanedLine = Escaping.Remove(cleanedLine, _commandSeparator, _escapeCharacter)

			Return New ReceivedCommand(Escaping.Split(cleanedLine, _fieldSeparator, _escapeCharacter, StringSplitOptions.RemoveEmptyEntries))
		End Function

		''' <summary> Reads a float line from the buffer, if complete. </summary>
		''' <returns> Whether a complete line was present in the buffer. </returns>
		Private Function ParseLine() As String
			If (Not String.IsNullOrEmpty(_buffer)) Then
				' Check if an End-Of-Line is present in the string, and split on first
				'var i = _buffer.IndexOf(CommandSeparator);
				Dim i = FindNextEol()
				If i >= 0 AndAlso i < _buffer.Length Then
					Dim line = _buffer.Substring(0, i + 1)
					If (Not String.IsNullOrEmpty(line)) Then
						_buffer = _buffer.Substring(i + 1)
						Return line
					End If
					_buffer = _buffer.Substring(i + 1)
					Return String.Empty
				End If
			End If
			Return String.Empty
		End Function

		''' <summary> Searches for the next End-Of-Line. </summary>
		''' <returns> The the location in the string of the next End-Of-Line. </returns>
		Private Function FindNextEol() As Integer
			Dim pos As Integer = 0
			Do While pos < _buffer.Length
				Dim escaped = _isEscaped.EscapedChar(_buffer.Chars(pos))
				If _buffer.Chars(pos) = _commandSeparator AndAlso (Not escaped) Then
					Return pos
				End If
				pos += 1
			Loop
			Return pos
		End Function

		' Dispose
		''' <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
		''' <param name="disposing"> true if resources should be disposed, false if not. </param>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				' Stop polling
				RemoveHandler _transport.NewDataReceived, AddressOf NewDataReceived
			End If
			MyBase.Dispose(disposing)
		End Sub

		#End Region
	End Class
End Namespace