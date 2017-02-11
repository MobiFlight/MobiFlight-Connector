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
Imports System.Threading

Namespace CommandMessenger
	''' <summary> Queue of received commands.  </summary>
	Friend Class SendCommandQueue
		Inherits CommandQueue
		Private ReadOnly _sender As Sender
		Public Event NewLineSent As NewLineEvent.NewLineHandler
		Private ReadOnly _sendBufferMaxLength As Integer = 62
		Private _sendBuffer As String = String.Empty
		Private _commandCount As Integer = 0

		Private privateMaxQueueLength As UInteger
		Public Property MaxQueueLength() As UInteger
			Get
				Return privateMaxQueueLength
			End Get
			Set(ByVal value As UInteger)
				privateMaxQueueLength = value
			End Set
		End Property


		''' <summary> send command queue constructor. </summary>
		''' <param name="disposeStack"> DisposeStack. </param>
		''' <param name="cmdMessenger"> The command messenger. </param>
		''' <param name="sender">Object that does the actual sending of the command</param>
		''' <param name="sendBufferMaxLength">Length of the send buffer</param>
		Public Sub New(ByVal disposeStack As DisposeStack, ByVal cmdMessenger As CmdMessenger, ByVal sender As Sender, ByVal sendBufferMaxLength As Integer)
			MyBase.New(disposeStack, cmdMessenger)
			MaxQueueLength = 5000
			QueueThread.Name = "SendCommandQueue"
			_sender = sender
			_sendBufferMaxLength = sendBufferMaxLength
			' _queueSpeed.Name = "SendCommandQueue";            
		End Sub

		''' <summary> Process the queue. </summary>
		Protected Overrides Sub ProcessQueue()
			' ProcessQueue is triggered by ProcessQueueLoop if a new item has been queued
			SendCommandsFromQueue()
		End Sub

		''' <summary> Sends the commands from queue. All commands will be combined until either
		''' 		   the SendBufferMaxLength  has been reached or if a command requires an acknowledge
		''' 		   </summary>
		Private Sub SendCommandsFromQueue()
			_commandCount = 0
			_sendBuffer = String.Empty
			Dim eventCommandStrategy As CommandStrategy = Nothing

			' while maximum buffer string is not reached, and command in queue, AND    
			Do While _sendBuffer.Length < _sendBufferMaxLength AndAlso Queue.Count > 0
				SyncLock Queue
					Dim commandStrategy = If((Not IsEmpty), Queue.Peek(), Nothing)
					If commandStrategy IsNot Nothing Then
						If commandStrategy.Command IsNot Nothing Then
							Dim sendCommand = CType(commandStrategy.Command, AddressOf SendCommand)

							If sendCommand.ReqAc Then
								If _commandCount > 0 Then
									Exit Do
								End If
								SendSingleCommandFromQueue(commandStrategy)
							Else
								eventCommandStrategy = commandStrategy
								AddToCommandsString(commandStrategy)
							End If
						End If
					End If
				End SyncLock
				' event callback outside lock for performance
				If eventCommandStrategy IsNot Nothing Then
					RaiseEvent NewLineSent(Me, New NewLineEvent.NewLineArgs(eventCommandStrategy.Command))
					eventCommandStrategy = Nothing
				End If
			Loop

			' Now check if a command string has been filled
			If _sendBuffer.Length > 0 Then
				_sender.ExecuteSendString(_sendBuffer, SendQueue.InFrontQueue)
			End If
		End Sub

		''' <summary> Sends a float command from the queue. </summary>
		''' <param name="commandStrategy"> The command strategy to send. </param>
		Private Sub SendSingleCommandFromQueue(ByVal commandStrategy As CommandStrategy)
			' Dequeue
			SyncLock Queue
				commandStrategy.DeQueue()
				' Process all generic dequeue strategies
				For Each generalStrategy In GeneralStrategies
					generalStrategy.OnDequeue()
				Next generalStrategy
			End SyncLock
			' Send command
			If commandStrategy IsNot Nothing AndAlso commandStrategy.Command IsNot Nothing Then
				_sender.ExecuteSendCommand(CType(commandStrategy.Command, AddressOf SendCommand), SendQueue.InFrontQueue)
			End If
		End Sub

		''' <summary> Adds a commandStrategy to the commands string.  </summary>
		''' <param name="commandStrategy"> The command strategy to add. </param>
		Private Sub AddToCommandsString(ByVal commandStrategy As CommandStrategy)
			' Dequeue
			SyncLock Queue
				commandStrategy.DeQueue()
				' Process all generic dequeue strategies
				For Each generalStrategy In GeneralStrategies
					generalStrategy.OnDequeue()
				Next generalStrategy
			End SyncLock
			' Add command
			If commandStrategy IsNot Nothing AndAlso commandStrategy.Command IsNot Nothing Then
					_commandCount += 1
					_sendBuffer &= commandStrategy.Command.CommandString()
					If Command.PrintLfCr Then
						_sendBuffer &= Environment.NewLine
					End If
			End If

		End Sub

		''' <summary> Sends a command. Note that the command is put at the front of the queue </summary>
		''' <param name="sendCommand"> The command to sent. </param>
		Public Sub SendCommand(ByVal sendCommand As SendCommand)
			' Add command to front of queue
			QueueCommand(New TopCommandStrategy(sendCommand))
		End Sub

		''' <summary> Queue the send command. </summary>
		''' <param name="sendCommand"> The command to sent. </param>
		Public Sub QueueCommand(ByVal sendCommand As SendCommand)
			QueueCommand(New CommandStrategy(sendCommand))
		End Sub

		''' <summary> Queue the send command wrapped in a command strategy. </summary>
		''' <param name="commandStrategy"> The command strategy. </param>
		Public Overrides Sub QueueCommand(ByVal commandStrategy As CommandStrategy)
			Do While Queue.Count > MaxQueueLength
				Thread.Yield()
			Loop
			SyncLock Queue
				' Process commandStrategy enqueue associated with command
				commandStrategy.CommandQueue = Queue
				commandStrategy.ThreadRunState = ThreadRunState

				commandStrategy.Enqueue()

				' Process all generic enqueue strategies
				For Each generalStrategy In GeneralStrategies
					generalStrategy.OnEnqueue()
				Next generalStrategy
			End SyncLock
			ItemPutOnQueueSignal.Set()
		End Sub
	End Class
End Namespace
