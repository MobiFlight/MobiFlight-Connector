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
Imports System.Globalization

Namespace CommandMessenger
	''' <summary> A command to be send by CmdMessenger </summary>
	Public Class Command
		Private privateFieldSeparator As Char
		Public Shared Property FieldSeparator() As Char
			Get
				Return privateFieldSeparator
			End Get
			Set(ByVal value As Char)
				privateFieldSeparator = value
			End Set
		End Property
		Private privateCommandSeparator As Char
		Public Shared Property CommandSeparator() As Char
			Get
				Return privateCommandSeparator
			End Get
			Set(ByVal value As Char)
				privateCommandSeparator = value
			End Set
		End Property
		Private privatePrintLfCr As Boolean
		Public Shared Property PrintLfCr() As Boolean
			Get
				Return privatePrintLfCr
			End Get
			Set(ByVal value As Boolean)
				privatePrintLfCr = value
			End Set
		End Property
		Private privateBoardType As BoardType
		Public Shared Property BoardType() As BoardType
			Get
				Return privateBoardType
			End Get
			Set(ByVal value As BoardType)
				privateBoardType = value
			End Set
		End Property

		Protected _arguments As List(Of String) ' The argument list of the command, first one is the command ID

		''' <summary> Gets or sets the command ID. </summary>
		''' <value> The command ID. </value>
		Private privateCmdId As Integer
		Public Property CmdId() As Integer
			Get
				Return privateCmdId
			End Get
			Set(ByVal value As Integer)
				privateCmdId = value
			End Set
		End Property

		''' <summary> Gets the command arguments. </summary>
		''' <value> The arguments, first one is the command ID </value>
		Public ReadOnly Property Arguments() As String()
			Get
				Return _arguments.ToArray()
			End Get
		End Property

		''' <summary> Gets or sets the time stamp. </summary>
		''' <value> The time stamp. </value>
		Private privateTimeStamp As Long
		Public Property TimeStamp() As Long
			Get
				Return privateTimeStamp
			End Get
			Set(ByVal value As Long)
				privateTimeStamp = value
			End Set
		End Property

		''' <summary> Constructor. </summary>
		Public Sub New()
			CmdId = -1
			_arguments = New List(Of String)()
			TimeStamp = TimeUtils.Millis
		End Sub

		''' <summary> Returns whether this is a valid & filled command. </summary>
		''' <value> true if ok, false if not. </value>
		Public ReadOnly Property Ok() As Boolean
			Get
				Return (CmdId >= 0)
			End Get
		End Property

		Public Function CommandString() As String
'INSTANT VB NOTE: The local variable commandString was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim commandString_Renamed = CmdId.ToString(CultureInfo.InvariantCulture)

			For Each argument In Arguments
				commandString_Renamed += FieldSeparator + argument
			Next argument
			commandString_Renamed += CommandSeparator
			Return commandString_Renamed
		End Function

	End Class
End Namespace
