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
Imports CommandMessenger.TransportLayer

Namespace CommandMessenger
	' This class will trigger the main thread when a specific command is received on the ReceiveCommandQueue thread
	' this is used when synchronously waiting for an acknowledge command in BlockedTillReply

	Public Class ReceivedCommandSignal
		Public Enum WaitState
			TimeOut
			Normal
		End Enum

		Private ReadOnly _key As Object = New Object()
		Private _waitingForCommand As Boolean
		Private _waitingForCommandProcessed As Boolean
		Private _cmdIdToMatch As Nullable(Of Integer)
		Private _sendQueueState As SendQueue
		Private _receivedCommand As ReceivedCommand


		''' <summary>
		''' start blocked (waiting for signal)
		''' </summary>
		Public Sub New()
			SyncLock _key
				_waitingForCommand = True
				_waitingForCommandProcessed = False
				Monitor.Pulse(_key)
			End SyncLock
		End Sub

		''' <summary>
		''' start blocked or signalled. 
		''' </summary>
		''' <param name="set">If true, first Wait will directly continue</param>
		Public Sub New(ByVal [set] As Boolean)
			SyncLock _key
				_waitingForCommand = Not [set]
				_waitingForCommandProcessed = False
				Monitor.Pulse(_key)
			End SyncLock
		End Sub

		''' <summary>
		''' Wait function. 
		''' </summary>
		''' <param name="timeOut">time-out in ms</param>
		''' <param name="cmdId"></param>
		''' <param name="sendQueueState"></param>
		''' <returns></returns>
		Public Function WaitForCmd(ByVal timeOut As Integer, ByVal cmdId As Integer, ByVal sendQueueState As SendQueue) As ReceivedCommand
			SyncLock _key
				' Todo: this makes sure ProcessCommand is not waiting anymore
				' this sometimes seems to happen but should not
				_waitingForCommandProcessed = False
				Monitor.Pulse(_key)

				_cmdIdToMatch = cmdId
				_sendQueueState = sendQueueState

				Logger.LogLine("Waiting for Command")

				' Wait under conditions
				Dim noTimeOut = True
				Do While noTimeOut AndAlso _waitingForCommand
					noTimeOut = Monitor.Wait(_key, timeOut)
				Loop

				' Block Wait for next entry
				_waitingForCommand = True

				' Signal to continue listening for next command
				_waitingForCommandProcessed = False

				If _receivedCommand Is Nothing Then

				Else
					Logger.LogLine("Command " & _receivedCommand.CmdId & "was received in main thread")
				End If
				Monitor.Pulse(_key)

				' Reset CmdId to check on
				_cmdIdToMatch = Nothing

				' Return whether the Wait function was quit because of an Set event or timeout
				Return If(noTimeOut, _receivedCommand, Nothing)
			End SyncLock
		End Function


		''' <summary>
		''' Process command. See if it needs to be send to the main thread (false) or be used in queue (true)
		''' </summary>
		Public Function ProcessCommand(ByVal receivedCommand As ReceivedCommand) As Boolean
			SyncLock _key
				_receivedCommand = receivedCommand
				Dim receivedCmdId = _receivedCommand.CmdId

				' If main thread is not waiting for any command (not waiting for acknowlegde)
				If Not _cmdIdToMatch.HasValue Then
					Throw New Exception("should not happen")
					'return true;
				Else
					If _cmdIdToMatch.Equals(receivedCmdId) Then
						' Commands match! Sent a signal
						_waitingForCommand = False
						Monitor.Pulse(_key)
						Logger.LogLine("Send command " & receivedCommand.CmdId & " to main thread")

						' Wait for response
						Do While _waitingForCommandProcessed
							' Todo: timeout seems to be needed, otherwise sometimes a block occurred. Should not happen                            
							 Monitor.Wait(_key,100)
							 Logger.LogLine("Command " & receivedCommand.CmdId & " was send to main thread")
						Loop

						' Block Wait for next entry
						_waitingForCommandProcessed = True

						' Main thread wants this command
						Return False
					Else
						'Commands do not match, so depending of SendQueue state dump or put on queue
						Return (_sendQueueState <> SendQueue.ClearQueue)
					End If
				End If
			End SyncLock
		End Function
	End Class
End Namespace
