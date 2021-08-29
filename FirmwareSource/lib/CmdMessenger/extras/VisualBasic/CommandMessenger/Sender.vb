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
Imports System.Threading

Namespace CommandMessenger
	Public Class Sender
		Private ReadOnly _communicationManager As CommunicationManager
		Private ReadOnly _receiveCommandQueue As ReceiveCommandQueue
		Private ReadOnly _sendCommandDataLock As New Object() ' The process serial data lock

		''' <summary> Gets or sets the current received command. </summary>
		''' <value> The current received command. </value>
		Private privateCurrentReceivedCommand As ReceivedCommand
		Public Property CurrentReceivedCommand() As ReceivedCommand
			Get
				Return privateCurrentReceivedCommand
			End Get
			Private Set(ByVal value As ReceivedCommand)
				privateCurrentReceivedCommand = value
			End Set
		End Property

		''' <summary> Gets or sets a whether to print a line feed carriage return after each command. </summary>
		''' <value> true if print line feed carriage return, false if not. </value>
		Private privatePrintLfCr As Boolean
		Public Property PrintLfCr() As Boolean
			Get
				Return privatePrintLfCr
			End Get
			Set(ByVal value As Boolean)
				privatePrintLfCr = value
			End Set
		End Property

		Public Sub New(ByVal communicationManager As CommunicationManager, ByVal receiveCommandQueue As ReceiveCommandQueue)
			_communicationManager = communicationManager
			_receiveCommandQueue = receiveCommandQueue
		End Sub

		''' <summary> Directly executes the send command operation. </summary>
		''' <param name="sendCommand">    The command to sent. </param>
		''' <param name="sendQueueState"> Property to optionally clear the send and receive queues. </param>
		''' <returns> A received command. The received command will only be valid if the ReqAc of the command is true. </returns>
		Public Function ExecuteSendCommand(ByVal sendCommand As SendCommand, ByVal sendQueueState As SendQueue) As ReceivedCommand
			' Disable listening, all callbacks are disabled until after command was sent

			Dim ackCommand As ReceivedCommand
			SyncLock _sendCommandDataLock
				If PrintLfCr Then
					_communicationManager.WriteLine(sendCommand.CommandString())
				Else
					_communicationManager.Write(sendCommand.CommandString())
				End If
				ackCommand = If(sendCommand.ReqAc, BlockedTillReply(sendCommand.AckCmdId, sendCommand.Timeout, sendQueueState), New ReceivedCommand())
			End SyncLock
			Return ackCommand
		End Function

		''' <summary> Directly executes the send string operation. </summary>
		''' <param name="commandsString"> The string to sent. </param>
		''' <param name="sendQueueState"> Property to optionally clear the send and receive queues. </param>
		''' <returns> The received command is added for compatibility. It will not yield a response. </returns>
		Public Function ExecuteSendString(ByVal commandsString As String, ByVal sendQueueState As SendQueue) As ReceivedCommand
			SyncLock _sendCommandDataLock
				If PrintLfCr Then
					_communicationManager.WriteLine(commandsString)
				Else
					_communicationManager.Write(commandsString)
				End If
			End SyncLock
			Return New ReceivedCommand()
		End Function

		Private Function BlockedTillReply(ByVal ackCmdId As Integer, ByVal timeout As Integer, ByVal sendQueueState As SendQueue) As ReceivedCommand
			' Start direct processing. This will block the processQueue thread
			_receiveCommandQueue.DirectProcessing()

			' Wait for matching command
		   Dim acknowledgeCommand = If(((_receiveCommandQueue.ReceivedCommandSignal.WaitForCmd(10000, ackCmdId, sendQueueState)) <> Nothing), _receiveCommandQueue.ReceivedCommandSignal.WaitForCmd(10000, ackCmdId, sendQueueState), New ReceivedCommand())

		   ' Return to queued processing. This will unblock the processQueue thread
			_receiveCommandQueue.QueuedProcessing()

			' return acknowledgeCommand
			Return acknowledgeCommand
		End Function

		''' <summary> Listen to the receive queue and check for a specific acknowledge command. </summary>
		''' <param name="ackCmdId">        acknowledgement command ID. </param>
		''' <param name="sendQueueState"> Property to optionally clear the send and receive queues. </param>
		''' <returns> The first received command that matches the command ID. </returns>
		Private Function CheckForAcknowledge(ByVal ackCmdId As Integer, ByVal sendQueueState As SendQueue) As ReceivedCommand
			' Read command from received queue
			CurrentReceivedCommand = _receiveCommandQueue.DequeueCommand()
			If CurrentReceivedCommand IsNot Nothing Then
				' Check if received command is valid
				If (Not CurrentReceivedCommand.Ok) Then
					Return CurrentReceivedCommand
				End If

				' If valid, check if is same as command we are waiting for
				If CurrentReceivedCommand.CmdId = ackCmdId Then
					' This is command we are waiting for, so return
					Return CurrentReceivedCommand
				End If

				' This is not command we are waiting for
				If sendQueueState <> SendQueue.ClearQueue Then
					' Add to queue for later processing
					_receiveCommandQueue.QueueCommand(CurrentReceivedCommand)
				End If
			End If
			' Return not Ok received command
			Return New ReceivedCommand()
		End Function

	End Class
End Namespace
