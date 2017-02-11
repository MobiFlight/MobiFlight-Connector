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
Imports System.Collections.Generic

Namespace CommandMessenger
	''' <summary> Queue class.  </summary>
	''' <typeparam name="T"> Type of object to queue. </typeparam>
	Public Class ListQueue(Of T)
		Inherits List(Of T)
		''' <summary> Adds item to front of queue. </summary>
		''' <param name="item"> The item to queue. </param>
		Public Sub EnqueueFront(ByVal item As T)
			Insert(Count, item)
		End Sub

		''' <summary> Adds item to back of queue. </summary>
		''' <param name="item"> The item to queue. </param>
		Public Sub Enqueue(ByVal item As T)
			Add(item)
		End Sub

		''' <summary> fetches item from front of queue. </summary>
		''' <returns> The item to dequeue. </returns>
		Public Function Dequeue() As T
			Dim t = MyBase.Item(0)
			RemoveAt(0)
			Return t
		End Function

		''' <summary> look at item at front of queue without removing it from the queue. </summary>
		''' <returns> The item to peek at. </returns>
		Public Function Peek() As T
			Return MyBase.Item(0)
		End Function

	End Class
End Namespace
