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
	''' <summary> Stale strategy. Any command older than the time-out is removed from the queue</summary>
	Public Class StaleGeneralStrategy
		Inherits GeneralStrategy
		Private ReadOnly _commandTimeOut As Long

		''' <summary> Stale strategy. Any command older than the time-out is removed from the queue</summary>
		''' <param name="commandTimeOut"> The time-out for any commands on the queue. </param>
		Public Sub New(ByVal commandTimeOut As Long)
			_commandTimeOut = commandTimeOut
		End Sub

		''' <summary> Remove this command (strategy) from command queue. </summary>
		Public Overrides Sub OnDequeue()
			' Remove commands that have gone stale
			'Console.WriteLine("Before StaleStrategy {0}", CommandQueue.Count);
			Dim currentTime = TimeUtils.Millis
			' Work from oldest to newest
			Dim item = 0
			Do While item < CommandQueue.Count
				Dim age = currentTime - CommandQueue(item).Command.TimeStamp
				If age > _commandTimeOut AndAlso CommandQueue.Count > 1 Then
					CommandQueue.RemoveAt(item)
				Else
					' From here on commands are newer, so we can stop
					Exit Do
				End If
				item += 1
			Loop
			'Console.WriteLine("After StaleStrategy {0}", CommandQueue.Count);
		End Sub
	End Class
End Namespace
