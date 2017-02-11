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
	Public Class DisposableObject
		Implements IDisposable
		Protected DisposeStack As New DisposeStack()
		Protected IsDisposed As Boolean = False

		Public Overridable Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub

		''' <summary>
		''' Remove all references and remove children
		''' </summary>
		''' <param name="disposing">If true, cleanup</param>
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If (Not IsDisposed) Then
				If disposing Then
					DisposeStack.Dispose()
					DisposeStack = Nothing
					IsDisposed = True
				End If
			End If
		End Sub
	End Class


End Namespace
