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

Namespace CommandMessenger
	Public Class NewLineEvent
		Public Delegate Sub NewLineHandler(ByVal sender As Object, ByVal e As NewLineArgs)

		Public Class NewLineArgs
			Inherits EventArgs
			Private privateCommand As Command
			Public Property Command() As Command
				Get
					Return privateCommand
				End Get
				Private Set(ByVal value As Command)
					privateCommand = value
				End Set
			End Property
			Public Sub New(ByVal command As Command)
				Command = command
			End Sub
		End Class
	End Class
End Namespace
