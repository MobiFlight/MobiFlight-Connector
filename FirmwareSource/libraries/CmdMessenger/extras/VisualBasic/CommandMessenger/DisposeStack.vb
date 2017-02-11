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

Namespace CommandMessenger
	''' <summary>
	''' The dispose stack takes manages disposal of objects that are pushed onto the stack.
	''' When the stack is disposed all objects are disposed (in reversed order). 
	''' </summary>
	Public NotInheritable Class DisposeStack
		Implements IDisposable
		Private ReadOnly _disposables As List(Of IDisposable) = New List(Of IDisposable)()

		''' <summary>
		''' Pushes a disposable object under the DisposeStack.
		''' </summary>
		''' <typeparam name="T">Type of object pushed</typeparam>
		''' <param name="newObject">The object pushed under the stack</param>
		''' <returns>Returns the pushed object</returns>
		Public Function PushFront(Of T As IDisposable)(ByVal newObject As T) As T
			_disposables.Insert(0, newObject)
			Return newObject
		End Function

		''' <summary>
		''' Pushes a disposable object under the DisposeStack.
		''' </summary>
		''' <typeparam name="T">Type of object pushed</typeparam>
		''' <param name="newObject">The object pushed on the stack</param>
		''' <returns>Returns the pushed object</returns>
		Public Function Push(Of T As IDisposable)(ByVal newObject As T) As T
			_disposables.Add(newObject)
			Return newObject
		End Function

		''' <summary>
		''' Push an arbitrary number of disposable objects onto the stack in one call
		''' </summary>
		''' <param name="objects">The disposable objects</param>
		Public Sub Push(ParamArray ByVal objects() As IDisposable)
			For Each d As IDisposable In objects
				_disposables.Add(d)
			Next d
		End Sub

		''' <summary>
		''' Dispose all items within the dispose stack.
		''' </summary>
		Public Sub Dispose() Implements IDisposable.Dispose
			For i As Integer = _disposables.Count - 1 To 0 Step -1
				_disposables(i).Dispose()
			Next i

			_disposables.Clear()
		End Sub
	End Class
End Namespace
