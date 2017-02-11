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
	''' <summary>  Top strategy. The command is added to the front of the queue</summary>
	Public Class TopCommandStrategy
		Inherits CommandStrategy
		''' <summary>  Top strategy. The command is added to the front of the queue</summary>
		''' <param name="command"> The command to add to the front of the queue. </param>
		Public Sub New(ByVal command As Command)
			MyBase.New(command)
		End Sub

		''' <summary> Add command (strategy) to command queue. </summary>
		Public Overrides Sub Enqueue()
			'Console.WriteLine("Enqueue {0}", CommandQueue.Count);
			CommandQueue.EnqueueFront(Me)
		End Sub
	End Class
End Namespace
