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
	''' <summary> Collapse command strategy. 
	''' 		  The purpose of the strategy is to avoid duplicates of a certain command on the queue
	''' 		  to avoid lagging </summary>
	Public Class CollapseCommandStrategy
		Inherits CommandStrategy
		''' <summary>  Collapse strategy. </summary>
		''' <param name="command"> The command that will be collapsed on the queue. </param>
		Public Sub New(ByVal command As Command)
			MyBase.New(command)
		End Sub

		''' <summary> Add command (strategy) to command queue. </summary>
		Public Overrides Sub Enqueue()
			' find if there already is a command with the same CmdId
			Dim index = CommandQueue.FindIndex(Function(strategy) strategy.Command.CmdId = Command.CmdId)
			If index < 0 Then
				' if not, add to the back of the queue
				CommandQueue.Enqueue(Me)
			Else
				' if on the queue, replace with new command
				CommandQueue(index) = Me
			End If
		End Sub
	End Class
End Namespace
