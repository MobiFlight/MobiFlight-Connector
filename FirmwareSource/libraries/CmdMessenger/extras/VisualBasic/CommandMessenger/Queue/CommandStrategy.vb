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
	''' <summary> Base command strategy.  </summary>
	Public Class CommandStrategy
		''' <summary> Base command strategy. </summary>
		''' <param name="command"> The command to be wrapped in a strategy. </param>
		Public Sub New(ByVal command As Command)
			Command = command
		End Sub

		''' <summary> Gets or sets the command queue. </summary>
		''' <value> A Queue of commands. </value>
		Private privateCommandQueue As ListQueue
		Public Property CommandQueue() As ListQueue(Of CommandStrategy)
			Get
				Return privateCommandQueue
			End Get
			Set(ByVal value As ListQueue)
				privateCommandQueue = value
			End Set
		End Property

		''' <summary> Gets or sets the run state of the thread. </summary>
		''' <value> The thread run state. </value>
		Private privateThreadRunState As CommandQueue.ThreadRunStates
		Public Property ThreadRunState() As CommandQueue.ThreadRunStates
			Get
				Return privateThreadRunState
			End Get
			Set(ByVal value As CommandQueue.ThreadRunStates)
				privateThreadRunState = value
			End Set
		End Property

		''' <summary> Gets or sets the command. </summary>
		''' <value> The command wrapped in the strategy. </value>
		Private privateCommand As Command
		Public Property Command() As Command
			Get
				Return privateCommand
			End Get
			Private Set(ByVal value As Command)
				privateCommand = value
			End Set
		End Property

		''' <summary> Add command (strategy) to command queue. </summary>
		Public Overridable Sub Enqueue()
			CommandQueue.Enqueue(Me)
		End Sub

		''' <summary> Remove this command (strategy) from command queue. </summary>
		Public Overridable Sub DeQueue()
			CommandQueue.Remove(Me)
		End Sub
	End Class
End Namespace
