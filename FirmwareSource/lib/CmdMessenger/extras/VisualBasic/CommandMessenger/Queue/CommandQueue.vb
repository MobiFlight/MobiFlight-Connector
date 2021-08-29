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
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading

Namespace CommandMessenger
	' Command queue base object. 
	Public Class CommandQueue
		Inherits DisposableObject
		Protected ReadOnly QueueThread As Thread
		Protected ReadOnly Queue As ListQueue(Of CommandStrategy) = New ListQueue(Of CommandStrategy)() ' Buffer for commands
		Protected ReadOnly GeneralStrategies As List(Of GeneralStrategy) = New List(Of GeneralStrategy)() ' Buffer for command independent strategies
		Protected ReadOnly CmdMessenger As CmdMessenger
		Private _threadRunState As ThreadRunStates
		Protected ReadOnly ItemPutOnQueueSignal As EventWaiter
		Protected ThreadRunStateLock As Object = New Object()

		''' <summary> Run state of thread running the queue.  </summary>
		Public Enum ThreadRunStates
			Start
			Started
			[Stop]
			Stopped
			Abort
		End Enum

		''' <summary> Gets or sets the run state of the thread . </summary>
		''' <value> The thread run state. </value>
		Public Property ThreadRunState() As ThreadRunStates
			Set(ByVal value As ThreadRunStates)
				SyncLock ThreadRunStateLock
					_threadRunState = value
					' Give a signal to indicate that process loop needs to run
					ItemPutOnQueueSignal.Set()
				End SyncLock
			End Set
			Get
				Dim result = ThreadRunStates.Start
				SyncLock ThreadRunStateLock
					result = _threadRunState
				End SyncLock
				Return result
			End Get
		End Property

		''' <summary> Gets or sets the run state of the thread . </summary>
		''' <value> The thread run state. </value>
		Private privateRunningThreadRunState As ThreadRunStates
		Protected Property RunningThreadRunState() As ThreadRunStates
			Get
				Return privateRunningThreadRunState
			End Get
			Set(ByVal value As ThreadRunStates)
				privateRunningThreadRunState = value
			End Set
		End Property

		''' <summary>Gets count of records in queue.</summary>
		Public ReadOnly Property Count() As Integer
			Get
				Return Queue.Count
			End Get
		End Property

		''' <summary>Gets is queue is empty. NOT THREAD-SAFE.</summary>
		Public ReadOnly Property IsEmpty() As Boolean
			Get
				Return Not Queue.Any()
			End Get
		End Property

		''' <summary> command queue constructor. </summary>
		''' <param name="disposeStack"> DisposeStack. </param>
		''' <param name="cmdMessenger"> The command messenger. </param>
		Public Sub New(ByVal disposeStack As DisposeStack, ByVal cmdMessenger As CmdMessenger)
			CmdMessenger = cmdMessenger
			disposeStack.Push(Me)

			ItemPutOnQueueSignal = New EventWaiter(True)

			' Create queue thread and wait for it to start
			QueueThread = New Thread(AddressOf ProcessQueueLoop) With {.Priority = ThreadPriority.Normal}
			QueueThread.Start()

			' Wait for thread to properly initialize
			Do While (Not QueueThread.IsAlive) AndAlso QueueThread.ThreadState <> ThreadState.Running
				Thread.Sleep(25)
			Loop
		End Sub

		Public Sub WaitForThreadRunStateSet()
			'  Now wait for state change
			SpinWait.SpinUntil(Function() RunningThreadRunState = ThreadRunState)
		End Sub

		Private Sub ProcessQueueLoop()
			Do While ThreadRunState <> ThreadRunStates.Abort
				Dim empty As Boolean
				SyncLock Queue
					empty = IsEmpty
				End SyncLock
				If empty Then
					ItemPutOnQueueSignal.WaitOne(1000)
				End If

				' Process queue unless stopped
				If ThreadRunState = ThreadRunStates.Start Then
					ProcessQueue()
				Else
					'Thread.SpinWait(100000);
					Thread.Yield()
					'Thread.Sleep(100);
				End If
				' Update real run state
				RunningThreadRunState = ThreadRunState
			Loop
		End Sub

		''' <summary> Process the queue. </summary>
		Protected Overridable Sub ProcessQueue()
		End Sub

		''' <summary> Clears the queue. </summary>
		Public Sub Clear()
			SyncLock Queue
				Queue.Clear()
			End SyncLock
		End Sub

		''' <summary> Queue the command wrapped in a command strategy. </summary>
		''' <param name="commandStrategy"> The command strategy. </param>
		Public Overridable Sub QueueCommand(ByVal commandStrategy As CommandStrategy)
		End Sub

		''' <summary> Adds a general strategy. This strategy is applied to all queued and dequeued commands.  </summary>
		''' <param name="generalStrategy"> The general strategy. </param>
		Public Sub AddGeneralStrategy(ByVal generalStrategy As GeneralStrategy)
			' Give strategy access to queue
			generalStrategy.CommandQueue = Queue
			' Add to general strategy list
			GeneralStrategies.Add(generalStrategy)
		End Sub

		''' <summary> Kills this object. </summary>
		Public Sub Kill()
			ThreadRunState = ThreadRunStates.Abort
			ItemPutOnQueueSignal.KeepOpen()
			'Wait for thread to die
			Join(2000)
			If QueueThread.IsAlive Then
				QueueThread.Abort()
			End If
		End Sub

		''' <summary> Joins the thread. </summary>
		''' <param name="millisecondsTimeout"> The milliseconds timeout. </param>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Join(ByVal millisecondsTimeout As Integer) As Boolean
			If QueueThread.IsAlive = False Then
				Return True
			End If
			Return QueueThread.Join(TimeSpan.FromMilliseconds(millisecondsTimeout))
		End Function

		' Dispose
		''' <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
		''' <param name="disposing"> true if resources should be disposed, false if not. </param>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				' Stop polling
				Kill()
			End If
			MyBase.Dispose(disposing)
		End Sub

	End Class
End Namespace
