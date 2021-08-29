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
Imports System.Diagnostics
Imports System.Threading
Imports System.Windows.Forms
Imports CommandMessenger.TransportLayer
Imports ThreadState = System.Diagnostics.ThreadState

Namespace CommandMessenger
	Public Enum SendQueue
		[Default]
		InFrontQueue
		AtEndQueue
		WaitForEmptyQueue
		ClearQueue
	End Enum

	Public Enum ReceiveQueue
		[Default]
		LeaveQueue
		WaitForEmptyQueue
		ClearQueue
	End Enum

	Public Enum UseQueue
		UseQueue
		BypassQueue
	End Enum

	Public Enum BoardType
		Bit16
		Bit32
	End Enum

	''' <summary> Command messenger main class  </summary>
	Public Class CmdMessenger
		Inherits DisposableObject
		Public Event NewLineReceived As NewLineEvent.NewLineHandler ' Event handler for new lines received
		Public Event NewLineSent As NewLineEvent.NewLineHandler ' Event handler for a new line received

		Private _communicationManager As CommunicationManager ' The communication manager
		Private _sender As Sender ' The command sender

		Private _fieldSeparator As Char ' The field separator
		Private _commandSeparator As Char ' The command separator
		Private _printLfCr As Boolean ' Add Linefeed + CarriageReturn
		Private _boardType As BoardType
		Private _defaultCallback As MessengerCallbackFunction ' The default callback
		Private _callbackList As Dictionary(Of Integer, MessengerCallbackFunction) ' List of callbacks

		Private _sendCommandQueue As SendCommandQueue ' The queue of commands to be sent
		Private _receiveCommandQueue As ReceiveCommandQueue ' The queue of commands to be processed


		''' <summary> Definition of the messenger callback function. </summary>
		''' <param name="receivedCommand"> The received command. </param>
		Public Delegate Sub MessengerCallbackFunction(ByVal receivedCommand As ReceivedCommand)

		''' <summary> Embedded Processor type. Needed to translate variables between sides. </summary>
		''' <value> The current received line. </value>
		Public Property BoardType() As BoardType
			Get
				Return _boardType
			End Get
			Set(ByVal value As BoardType)
				_boardType = value
				Command.BoardType = _boardType
			End Set
		End Property

		''' <summary> Gets or sets a whether to print a line feed carriage return after each command. </summary>
		''' <value> true if print line feed carriage return, false if not. </value>
		Public Property PrintLfCr() As Boolean
			Get
				Return _printLfCr
			End Get
			Set(ByVal value As Boolean)
				_printLfCr = value
				Command.PrintLfCr = _printLfCr
				_sender.PrintLfCr = _printLfCr
			End Set
		End Property

		''' <summary> Gets or sets the current received command line. </summary>
		''' <value> The current received line. </value>
		Private privateCurrentReceivedLine As String
		Public Property CurrentReceivedLine() As String
			Get
				Return privateCurrentReceivedLine
			End Get
			Private Set(ByVal value As String)
				privateCurrentReceivedLine = value
			End Set
		End Property

		' The control to invoke the callback on
		Friend ControlToInvokeOn As Control

		''' <summary> Constructor. </summary>
		''' <param name="transport"> The transport layer. </param>
		Public Sub New(ByVal transport As ITransport)
			Init(transport, ","c, ";"c, "/"c, 60)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="transport"> The transport layer. </param>
		''' <param name="sendBufferMaxLength"> The maximum size of the send buffer</param>
		Public Sub New(ByVal transport As ITransport, ByVal sendBufferMaxLength As Integer)
			Init(transport, ","c, ";"c, "/"c, sendBufferMaxLength)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="transport"> The transport layer. </param>
		''' <param name="fieldSeparator"> The field separator. </param>
		Public Sub New(ByVal transport As ITransport, ByVal fieldSeparator As Char)
			Init(transport, fieldSeparator, ";"c, "/"c, 60)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="transport"> The transport layer. </param>
		''' <param name="fieldSeparator"> The field separator. </param>
		''' <param name="sendBufferMaxLength"> The maximum size of the send buffer</param>
		Public Sub New(ByVal transport As ITransport, ByVal fieldSeparator As Char, ByVal sendBufferMaxLength As Integer)
			Init(transport, fieldSeparator, ";"c, "/"c, sendBufferMaxLength)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="transport">   The transport layer. </param>
		''' <param name="fieldSeparator">   The field separator. </param>
		''' <param name="commandSeparator"> The command separator. </param>
		Public Sub New(ByVal transport As ITransport, ByVal fieldSeparator As Char, ByVal commandSeparator As Char)
			Init(transport, fieldSeparator, commandSeparator, commandSeparator, 60)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="transport">   The transport layer. </param>
		''' <param name="fieldSeparator">   The field separator. </param>
		''' <param name="commandSeparator"> The command separator. </param>
		''' <param name="escapeCharacter">  The escape character. </param>
		''' <param name="sendBufferMaxLength"> The maximum size of the send buffer</param>
		Public Sub New(ByVal transport As ITransport, ByVal fieldSeparator As Char, ByVal commandSeparator As Char, ByVal escapeCharacter As Char, ByVal sendBufferMaxLength As Integer)
			Init(transport, fieldSeparator, commandSeparator, escapeCharacter, sendBufferMaxLength)
		End Sub

		''' <summary> Initializes this object. </summary>
		''' <param name="transport">   The transport layer. </param>
		''' <param name="fieldSeparator">   The field separator. </param>
		''' <param name="commandSeparator"> The command separator. </param>
		''' <param name="escapeCharacter">  The escape character. </param>
		''' <param name="sendBufferMaxLength"> The maximum size of the send buffer</param>
		Private Sub Init(ByVal transport As ITransport, ByVal fieldSeparator As Char, ByVal commandSeparator As Char, ByVal escapeCharacter As Char, ByVal sendBufferMaxLength As Integer)
			ControlToInvokeOn = Nothing

			'Logger.Open(@"sendCommands.txt");
			Logger.DirectFlush = True

			_receiveCommandQueue = New ReceiveCommandQueue(DisposeStack, Me)
			_communicationManager = New CommunicationManager(DisposeStack, transport, _receiveCommandQueue, commandSeparator, fieldSeparator, escapeCharacter)
			_sender = New Sender(_communicationManager, _receiveCommandQueue)
			_sendCommandQueue = New SendCommandQueue(DisposeStack, Me, _sender, sendBufferMaxLength)

			AddHandler _receiveCommandQueue.NewLineReceived, Function(o, e) InvokeNewLineEvent(NewLineReceived, e)
			AddHandler _sendCommandQueue.NewLineSent, Function(o, e) InvokeNewLineEvent(NewLineSent, e)

			_fieldSeparator = fieldSeparator
			_commandSeparator = commandSeparator
			PrintLfCr = False

			Command.FieldSeparator = _fieldSeparator
			Command.CommandSeparator = _commandSeparator
			Command.PrintLfCr = PrintLfCr

			Escaping.EscapeChars(_fieldSeparator, _commandSeparator, escapeCharacter)
			_callbackList = New Dictionary(Of Integer, MessengerCallbackFunction)()
			'CurrentSentLine = "";
			CurrentReceivedLine = ""
		End Sub

		Public Sub SetSingleCore()
			Dim proc = Process.GetCurrentProcess()
			For Each pt As ProcessThread In proc.Threads
				If pt.ThreadState <> ThreadState.Terminated Then
					Try
						pt.IdealProcessor = 0
						pt.ProcessorAffinity = CType(1, IntPtr)
					Catch e1 As Exception
					End Try

				End If
			Next pt
		End Sub
		''' <summary> Sets a control to invoke on. </summary>
		''' <param name="controlToInvokeOn"> The control to invoke on. </param>
		Public Sub SetControlToInvokeOn(ByVal controlToInvokeOn As Control)
			Me.ControlToInvokeOn = controlToInvokeOn
		End Sub

		''' <summary>  Stop listening and end serial port connection. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Disconnect() As Boolean
			Return _communicationManager.Disconnect()
		End Function

		''' <summary> Starts serial port connection and start listening. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Connect() As Boolean
			If _communicationManager.Connect() Then
				' Timestamp of this command is same as time stamp of serial line
				'LastReceivedCommandTimeStamp = _communicationManager.LastLineTimeStamp;
				Return True
			End If
			Return False
		End Function

		''' <summary> Attaches default callback for unsupported commands. </summary>
		''' <param name="newFunction"> The callback function. </param>
		Public Sub Attach(ByVal newFunction As MessengerCallbackFunction)
			_defaultCallback = newFunction
		End Sub

		''' <summary> Attaches default callback for certain Message ID. </summary>
		''' <param name="messageId">   Command ID. </param>
		''' <param name="newFunction"> The callback function. </param>
		Public Sub Attach(ByVal messageId As Integer, ByVal newFunction As MessengerCallbackFunction)
			_callbackList(messageId) = newFunction
		End Sub

		''' <summary> Gets or sets the time stamp of the last command line received. </summary>
		''' <value> The last line time stamp. </value>
		Public ReadOnly Property LastReceivedCommandTimeStamp() As Long
			Get
				Return _communicationManager.LastLineTimeStamp
			End Get
		End Property

		''' <summary> Handle message. </summary>
		''' <param name="receivedCommand"> The received command. </param>
		Public Sub HandleMessage(ByVal receivedCommand As ReceivedCommand)
			CurrentReceivedLine = receivedCommand.RawString

			Dim callback As MessengerCallbackFunction = Nothing
			If receivedCommand.Ok Then
				If _callbackList.ContainsKey(receivedCommand.CmdId) Then
					callback = _callbackList(receivedCommand.CmdId)
				Else
					If _defaultCallback IsNot Nothing Then
						callback = _defaultCallback
					End If
				End If
			Else
				' Empty command
				receivedCommand = New ReceivedCommand()
			End If
			InvokeCallBack(callback, receivedCommand)
		End Sub

		''' <summary> Sends a command. 
		''' 		  If no command acknowledge is requested, the command will be send asynchronously: it will be put on the top of the send queue
		'''  		  If a  command acknowledge is requested, the command will be send synchronously:  the program will block until the acknowledge command 
		'''  		  has been received or the timeout has expired. </summary>
		''' <param name="sendCommand_Renamed"> The command to sent. </param>
'INSTANT VB NOTE: The parameter sendCommand was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Function SendCommand(ByVal sendCommand_Renamed As SendCommand) As ReceivedCommand
			Return SendCommand(sendCommand_Renamed, SendQueue.InFrontQueue, ReceiveQueue.Default)
		End Function

		''' <summary> Sends a command. 
		''' 		  If no command acknowledge is requested, the command will be send asynchronously: it will be put on the top of the send queue
		'''  		  If a  command acknowledge is requested, the command will be send synchronously:  the program will block until the acknowledge command 
		'''  		  has been received or the timeout has expired.
		'''  		  Based on ClearQueueState, the send- and receive-queues are left intact or are cleared</summary>
		''' <param name="sendCommand_Renamed">       The command to sent. </param>
		''' <param name="sendQueueState">    Property to optionally clear/wait the send queue</param>
		''' <param name="receiveQueueState"> Property to optionally clear/wait the send queue</param>
		''' <returns> A received command. The received command will only be valid if the ReqAc of the command is true. </returns>
'INSTANT VB NOTE: The parameter sendCommand was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Function SendCommand(ByVal sendCommand_Renamed As SendCommand, ByVal sendQueueState As SendQueue, ByVal receiveQueueState As ReceiveQueue) As ReceivedCommand
			Return SendCommand(sendCommand_Renamed, sendQueueState, receiveQueueState, UseQueue.UseQueue)
		End Function

		''' <summary> Sends a command. 
		''' 		  If no command acknowledge is requested, the command will be send asynchronously: it will be put on the top of the send queue
		'''  		  If a  command acknowledge is requested, the command will be send synchronously:  the program will block until the acknowledge command 
		'''  		  has been received or the timeout has expired.
		'''  		  Based on ClearQueueState, the send- and receive-queues are left intact or are cleared</summary>
		''' <param name="sendCommand_Renamed">       The command to sent. </param>
		''' <param name="sendQueueState">    Property to optionally clear/wait the send queue</param>
		''' <param name="receiveQueueState"> Property to optionally clear/wait the send queue</param>
		''' <param name="useQueue">          Property to optionally bypass the queue</param>
		''' <returns> A received command. The received command will only be valid if the ReqAc of the command is true. </returns>
'INSTANT VB NOTE: The parameter sendCommand was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Function SendCommand(ByVal sendCommand_Renamed As SendCommand, ByVal sendQueueState As SendQueue, ByVal receiveQueueState As ReceiveQueue, ByVal useQueue As UseQueue) As ReceivedCommand
			Dim synchronizedSend = (sendCommand_Renamed.ReqAc OrElse useQueue = UseQueue.BypassQueue)

			' When waiting for an acknowledge, it is typically best to wait for the ReceiveQueue to be empty
			' This is thus the default state
			If sendCommand_Renamed.ReqAc AndAlso receiveQueueState = ReceiveQueue.Default Then
				receiveQueueState = ReceiveQueue.WaitForEmptyQueue
			End If

			If sendQueueState = SendQueue.ClearQueue Then
				' Clear receive queue
				_receiveCommandQueue.Clear()
			End If

			If receiveQueueState = ReceiveQueue.ClearQueue Then
				' Clear send queue
				_sendCommandQueue.Clear()
			End If

			' If synchronized sending, the only way to get command at end of queue is by waiting
			If sendQueueState = SendQueue.WaitForEmptyQueue OrElse (synchronizedSend AndAlso sendQueueState = SendQueue.AtEndQueue) Then
				Do While _sendCommandQueue.Count > 0
					Thread.Sleep(1)
				Loop
			End If

			If receiveQueueState = ReceiveQueue.WaitForEmptyQueue Then
				Do While _receiveCommandQueue.Count > 0
					Thread.Sleep(1)
				Loop
			End If

			If synchronizedSend Then
				Return SendCommandSync(sendCommand_Renamed, sendQueueState)
			End If

			If sendQueueState <> SendQueue.AtEndQueue Then
				' Put command at top of command queue
				_sendCommandQueue.SendCommand(sendCommand_Renamed)
			Else
				' Put command at bottom of command queue
				_sendCommandQueue.QueueCommand(sendCommand_Renamed)
			End If
			Return New ReceivedCommand()
		End Function

		''' <summary> Synchronized send a command. </summary>
		''' <param name="sendCommand">    The command to sent. </param>
		''' <param name="sendQueueState"> Property to optionally clear/wait the send queue. </param>
		''' <returns> . </returns>
		Public Function SendCommandSync(ByVal sendCommand As SendCommand, ByVal sendQueueState As SendQueue) As ReceivedCommand
			' Directly call execute command
			Dim resultSendCommand = _sender.ExecuteSendCommand(sendCommand, sendQueueState)
			InvokeNewLineEvent(NewLineSentEvent, New NewLineEvent.NewLineArgs(sendCommand))
			Return resultSendCommand
		End Function

		''' <summary> Put the command at the back of the sent queue.</summary>
		''' <param name="sendCommand"> The command to sent. </param>
		Public Sub QueueCommand(ByVal sendCommand As SendCommand)
			_sendCommandQueue.QueueCommand(sendCommand)
		End Sub

		''' <summary> Put  a command wrapped in a strategy at the back of the sent queue.</summary>
		''' <param name="commandStrategy"> The command strategy. </param>
		Public Sub QueueCommand(ByVal commandStrategy As CommandStrategy)
			_sendCommandQueue.QueueCommand(commandStrategy)
		End Sub

		''' <summary> Adds a general command strategy to the receive queue. This will be executed on every enqueued and dequeued command.  </summary>
		''' <param name="generalStrategy"> The general strategy for the receive queue. </param>
		Public Sub AddReceiveCommandStrategy(ByVal generalStrategy As GeneralStrategy)
			_receiveCommandQueue.AddGeneralStrategy(generalStrategy)
		End Sub

		''' <summary> Adds a general command strategy to the send queue. This will be executed on every enqueued and dequeued command.  </summary>
		''' <param name="generalStrategy"> The general strategy for the send queue. </param>
		Public Sub AddSendCommandStrategy(ByVal generalStrategy As GeneralStrategy)
			_sendCommandQueue.AddGeneralStrategy(generalStrategy)
		End Sub

		''' <summary> Clears the receive queue. </summary>
		Public Sub ClearReceiveQueue()
			_receiveCommandQueue.Clear()
		End Sub

		''' <summary> Clears the send queue. </summary>
		Public Sub ClearSendQueue()
			_sendCommandQueue.Clear()
		End Sub

		''' <summary> Helper function to Invoke or directly call event. </summary>
		''' <param name="newLineHandler"> The event handler. </param>
		''' <param name="newLineArgs"></param>
		Private Sub InvokeNewLineEvent(ByVal newLineHandler As NewLineEvent.NewLineHandler, ByVal newLineArgs As NewLineEvent.NewLineArgs)
			Try
				If newLineHandler Is Nothing OrElse (ControlToInvokeOn IsNot Nothing AndAlso ControlToInvokeOn.IsDisposed) Then
					Return
				End If

				If ControlToInvokeOn IsNot Nothing AndAlso ControlToInvokeOn.InvokeRequired Then
					'Asynchronously call on UI thread
					ControlToInvokeOn.Invoke(CType(Function() newLineHandler(Me, newLineArgs), MethodInvoker))
				Else
					'Directly call
					newLineHandler(Me, newLineArgs)
				End If
			Catch e1 As Exception
			End Try
		End Sub

		''' <summary> Helper function to Invoke or directly call callback function. </summary>
		''' <param name="messengerCallbackFunction"> The messenger callback function. </param>
		''' <param name="command">                   The command. </param>
		Private Sub InvokeCallBack(ByVal messengerCallbackFunction As MessengerCallbackFunction, ByVal command As ReceivedCommand)
			Try
				If messengerCallbackFunction Is Nothing OrElse (ControlToInvokeOn IsNot Nothing AndAlso ControlToInvokeOn.IsDisposed) Then
					Return
				End If

				If ControlToInvokeOn IsNot Nothing AndAlso ControlToInvokeOn.InvokeRequired Then
					'Asynchronously call on UI thread
					ControlToInvokeOn.Invoke(New MessengerCallbackFunction(AddressOf messengerCallbackFunction), CObj(command))
				Else
					'Directly call
					messengerCallbackFunction(command)
				End If
			Catch e1 As Exception
			End Try
		End Sub

		''' <summary> Finaliser. </summary>
		Protected Overrides Sub Finalize()
			ControlToInvokeOn = Nothing
			_receiveCommandQueue.ThreadRunState = CommandQueue.ThreadRunStates.Abort
			_sendCommandQueue.ThreadRunState = CommandQueue.ThreadRunStates.Abort
		End Sub


		''' <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
		''' <param name="disposing"> true if resources should be disposed, false if not. </param>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				ControlToInvokeOn = Nothing
				_receiveCommandQueue.ThreadRunState = CommandQueue.ThreadRunStates.Abort
				_sendCommandQueue.ThreadRunState = CommandQueue.ThreadRunStates.Abort
			End If
			MyBase.Dispose(disposing)
		End Sub
	End Class
End Namespace