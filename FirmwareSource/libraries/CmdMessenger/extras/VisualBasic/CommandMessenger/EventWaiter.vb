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
Imports System.Net.Configuration
Imports System.Threading

Namespace CommandMessenger
	' Functionality comparable to AutoResetEvent (http://www.albahari.com/threading/part2.aspx#_AutoResetEvent)
	' but implemented using the monitor class: not inter processs, but ought to be more efficient.
	Public Class EventWaiter
		Public Enum WaitState
			KeepOpen
			KeepBlocked
			TimedOut
			Normal
		End Enum

		Private ReadOnly _key As Object = New Object()
		Private _block As Boolean
		Private _waitState As WaitState = WaitState.Normal

		''' <summary>
		''' start blocked (waiting for signal)
		''' </summary>
		Public Sub New()
			SyncLock _key
				_block = True
				Monitor.Pulse(_key)
			End SyncLock
		End Sub

		''' <summary>
		''' start blocked or signalled. 
		''' </summary>
		''' <param name="set">If true, first Wait will directly continue</param>
		Public Sub New(ByVal [set] As Boolean)
			SyncLock _key
				_block = Not [set]
				Monitor.Pulse(_key)
			End SyncLock
		End Sub

		''' <summary>
		''' Wait function. Blocks until signal is set or time-out
		''' </summary>
		''' <param name="timeOut">time-out in ms</param>
		''' <returns></returns>
		Public Function WaitOne(ByVal timeOut As Integer) As WaitState
			SyncLock _key
				' Check if quit has been raised before the wait function is entered
				If _waitState = WaitState.KeepOpen Then
					Return _waitState
				End If

				' Check if signal has already been raised before the wait function is entered                
				If (Not _block) Then
					' If so, reset event for next time and exit wait loop
					_block = True
					Return WaitState.Normal
				End If

				' Wait under conditions
				Dim noTimeOut As Boolean = True


				Do While IsBlocked(_block,noTimeOut,_waitState)
					noTimeOut = Monitor.Wait(_key, timeOut)
				Loop
				' Block Wait for next entry
				_block = True

				' Check if quit signal has already been raised after wait                
				If _waitState = WaitState.KeepOpen Then
					Return _waitState
				End If

				' Check if quit signal has already been raised after wait                
				If _waitState = WaitState.KeepBlocked Then
					Throw New Exception("Blocked state unexpected")
				End If

				' Return whether the Wait function was quit because of an Set event or timeout
				Return If(noTimeOut, WaitState.Normal, WaitState.TimedOut)
			End SyncLock
		End Function

		Private Function IsBlocked(ByVal block As Boolean, ByVal noTimeOut As Boolean, ByVal waitState As WaitState) As Boolean
			Select Case waitState
				Case WaitState.KeepBlocked
					Return True
				Case WaitState.KeepOpen
					Return False
				Case Else
					Return (noTimeOut AndAlso block)
			End Select
		End Function

		''' <summary>
		''' Sets signal, will unblock thread in Wait function
		''' </summary>
		Public Sub [Set]()
			SyncLock _key
				_block = False
				Monitor.Pulse(_key)
			End SyncLock
		End Sub

		''' <summary>
		''' Resets signal, will block threads entering Wait function
		''' </summary>
		Public Sub Reset()
			SyncLock _key
				_block = True
			End SyncLock
		End Sub

		''' <summary>
		''' KeepOpen. Unblocks thread in Wait function and exits
		' will not block again until Normal is called
		''' </summary>
		Public Sub KeepOpen()
			SyncLock _key
				_block = False
				'_quit = true;
				_waitState = WaitState.KeepOpen
				Monitor.Pulse(_key)
			End SyncLock
		End Sub

		''' <summary>
		''' KeepBlocked. Blocks thread in Wait function and exits
		' will not unblock until Normal is called
		''' </summary>
		Public Sub KeepBlocked()
			SyncLock _key
				_block = False
				'_quit = true;
				_waitState = WaitState.KeepBlocked
				Monitor.Pulse(_key)
			End SyncLock
		End Sub

		''' <summary>
		''' Resumes functionallity
		''' </summary>
		''' <param name="set">If true, first Wait will directly continue</param>
		Public Sub Normal(ByVal [set] As Boolean)
			SyncLock _key
				_block = Not [set]
				_waitState = WaitState.Normal
				Monitor.Pulse(_key)
			End SyncLock
		End Sub
	End Class
End Namespace
