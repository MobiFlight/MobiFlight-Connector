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
Namespace CommandMessenger


	''' <summary> Queue of received commands.  </summary>
	Public Class ReceiveCommandQueue
		Inherits CommandQueue
		Private _directProcessing As Boolean = False

		Public Event NewLineReceived As NewLineEvent.NewLineHandler

		Private privateReceivedCommandSignal As ReceivedCommandSignal
		Public Property ReceivedCommandSignal() As ReceivedCommandSignal
			Get
				Return privateReceivedCommandSignal
			End Get
			Private Set(ByVal value As ReceivedCommandSignal)
				privateReceivedCommandSignal = value
			End Set
		End Property

		'private readonly QueueSpeed _queueSpeed = new QueueSpeed(0.5,5);

		''' <summary> Receive command queue constructor. </summary>
		''' <param name="disposeStack"> DisposeStack. </param>
		''' <param name="cmdMessenger"> The command messenger. </param>
		Public Sub New(ByVal disposeStack As DisposeStack, ByVal cmdMessenger As CmdMessenger)
			MyBase.New(disposeStack, cmdMessenger)
			disposeStack.Push(Me)
			ReceivedCommandSignal = New ReceivedCommandSignal()
			QueueThread.Name = "ReceiveCommandQueue"
		   ' _queueSpeed.Name = "ReceiveCommandQueue";
		End Sub

		Public Sub DirectProcessing()
			' Disable processing queue
			ItemPutOnQueueSignal.KeepBlocked()
			_directProcessing = True
		End Sub

		Public Sub QueuedProcessing()
			' Enable processing queue
			ItemPutOnQueueSignal.Normal(True)
			_directProcessing = False
		End Sub


		''' <summary> Dequeue the received command. </summary>
		''' <returns> The received command. </returns>
		Public Function DequeueCommand() As ReceivedCommand
			Dim receivedCommand As ReceivedCommand = Nothing
			SyncLock Queue
				If (Not IsEmpty) Then
					For Each generalStrategy In GeneralStrategies
						generalStrategy.OnDequeue()
					Next generalStrategy
					Dim commandStrategy = Queue.Dequeue()
					receivedCommand = CType(commandStrategy.Command, ReceivedCommand)
				End If
			End SyncLock
			Return receivedCommand
		End Function

		''' <summary> Process the queue. </summary>
		Protected Overrides Sub ProcessQueue()
			Dim dequeueCommand = Me.DequeueCommand()
			If dequeueCommand IsNot Nothing Then
				CmdMessenger.HandleMessage(dequeueCommand)
			End If
		End Sub

		''' <summary> Queue the received command. </summary>
		''' <param name="receivedCommand"> The received command. </param>
		Public Sub QueueCommand(ByVal receivedCommand As ReceivedCommand)
			QueueCommand(New CommandStrategy(receivedCommand))
		End Sub

		''' <summary> Queue the command wrapped in a command strategy. </summary>
		''' <param name="commandStrategy"> The command strategy. </param>
		Public Overrides Sub QueueCommand(ByVal commandStrategy As CommandStrategy)
			' See if we should redirect the command to the live thread for synchronous processing
			' or put on the queue
			If _directProcessing Then
				' Directly send this command to waiting thread
				Dim addToQueue = ReceivedCommandSignal.ProcessCommand(CType(commandStrategy.Command, ReceivedCommand))
				' check if the item needs to be added to the queue for later processing. If not return directly
				If (Not addToQueue) Then
					Return
				End If
			End If

			' Put it on the queue
			SyncLock Queue
				' Process all generic enqueue strategies
				Queue.Enqueue(commandStrategy)
				For Each generalStrategy In GeneralStrategies
					generalStrategy.OnEnqueue()
				Next generalStrategy
			End SyncLock

			' If queue-ing, give a signal to queue processor to indicate that a new item has been queued
			If (Not _directProcessing) Then
				ItemPutOnQueueSignal.Set()
				RaiseEvent NewLineReceived(Me, New NewLineEvent.NewLineArgs(commandStrategy.Command))
			End If
		End Sub
	End Class
End Namespace
