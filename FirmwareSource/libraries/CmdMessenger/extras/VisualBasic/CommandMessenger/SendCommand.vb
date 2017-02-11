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
	Public Class SendCommand
		Inherits Command
		''' <summary> Indicates if we want to wait for an acknowledge command. </summary>
		''' <value> true if request acknowledge, false if not. </value>
		Private privateReqAc As Boolean
		Public Property ReqAc() As Boolean
			Get
				Return privateReqAc
			End Get
			Set(ByVal value As Boolean)
				privateReqAc = value
			End Set
		End Property

		''' <summary> Gets or sets the acknowledge command ID. </summary>
		''' <value> the acknowledge command ID. </value>
		Private privateAckCmdId As Integer
		Public Property AckCmdId() As Integer
			Get
				Return privateAckCmdId
			End Get
			Set(ByVal value As Integer)
				privateAckCmdId = value
			End Set
		End Property

		''' <summary> Gets or sets the time we want to wait for the acknowledge command. </summary>
		''' <value> The timeout on waiting for an acknowledge</value>
		Private privateTimeout As Integer
		Public Property Timeout() As Integer
			Get
				Return privateTimeout
			End Get
			Set(ByVal value As Integer)
				privateTimeout = value
			End Set
		End Property

		''' <summary> Constructor. </summary>
		''' <param name="cmdId"> the command ID. </param>
		Public Sub New(ByVal cmdId As Integer)
			Init(cmdId, False, 0, 0)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As String)
			Init(cmdId, False, 0, 0)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">     Command ID </param>
		''' <param name="arguments"> The arguments. </param>
		Public Sub New(ByVal cmdId As Integer, ByVal arguments() As String)
			Init(cmdId, False, 0, 0)
			AddArguments(arguments)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As Single)
			Init(cmdId, False, 0, 0)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As Double)
			Init(cmdId, False, 0, 0)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As UInt16)
			Init(cmdId, False, 0, 0)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As Int16)
			Init(cmdId, False, 0, 0)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As UInt32)
			Init(cmdId, False, 0, 0)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As Int32)
			Init(cmdId, False, 0, 0)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As Boolean)
			Init(cmdId, False, 0, 0)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="ackCmdId"> Acknowledge command ID. </param>
		''' <param name="timeout">  The timeout on waiting for an acknowledge</param>
		Public Sub New(ByVal cmdId As Integer, ByVal ackCmdId As Integer, ByVal timeout As Integer)
			Init(cmdId, True, ackCmdId, timeout)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		''' <param name="ackCmdId"> Acknowledge command ID. </param>
		''' <param name="timeout">  The timeout on waiting for an acknowledge</param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As String, ByVal ackCmdId As Integer, ByVal timeout As Integer)
			Init(cmdId, True, ackCmdId, timeout)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">     Command ID </param>
		''' <param name="arguments"> The arguments. </param>
		''' <param name="ackCmdId">  Acknowledge command ID. </param>
		''' <param name="timeout">   The timeout on waiting for an acknowledge</param>
		Public Sub New(ByVal cmdId As Integer, ByVal arguments() As String, ByVal ackCmdId As Integer, ByVal timeout As Integer)
			Init(cmdId, True, ackCmdId, timeout)
			AddArguments(arguments)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		''' <param name="ackCmdId"> Acknowledge command ID. </param>
		''' <param name="timeout">  The timeout on waiting for an acknowledge</param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As Single, ByVal ackCmdId As Integer, ByVal timeout As Integer)
			Init(cmdId, True, ackCmdId, timeout)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		''' <param name="ackCmdId"> Acknowledge command ID. </param>
		''' <param name="timeout">  The timeout on waiting for an acknowledge</param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As Double, ByVal ackCmdId As Integer, ByVal timeout As Integer)
			Init(cmdId, True, ackCmdId, timeout)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		''' <param name="ackCmdId"> Acknowledge command ID. </param>
		''' <param name="timeout">  The timeout on waiting for an acknowledge</param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As Int16, ByVal ackCmdId As Integer, ByVal timeout As Integer)
			Init(cmdId, True, ackCmdId, timeout)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		''' <param name="ackCmdId"> Acknowledge command ID. </param>
		''' <param name="timeout">  The timeout on waiting for an acknowledge</param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As UInt16, ByVal ackCmdId As Integer, ByVal timeout As Integer)
			Init(cmdId, True, ackCmdId, timeout)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		''' <param name="ackCmdId"> Acknowledge command ID. </param>
		''' <param name="timeout">  The timeout on waiting for an acknowledge</param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As Int32, ByVal ackCmdId As Integer, ByVal timeout As Integer)
			Init(cmdId, True, ackCmdId, timeout)
			AddArgument(argument)
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="argument"> The argument. </param>
		''' <param name="ackCmdId"> Acknowledge command ID. </param>
		''' <param name="timeout">  The timeout on waiting for an acknowledge</param>
		Public Sub New(ByVal cmdId As Integer, ByVal argument As UInt32, ByVal ackCmdId As Integer, ByVal timeout As Integer)
			Init(cmdId, True, ackCmdId, timeout)
			AddArgument(argument)
		End Sub

		''' <summary> Initializes this object. </summary>
		''' <param name="cmdId">    Command ID </param>
		''' <param name="reqAc">    true to request ac. </param>
		''' <param name="ackCmdId"> Acknowledge command ID. </param>
		''' <param name="timeout">  The timeout on waiting for an acknowledge</param>
		Private Sub Init(ByVal cmdId As Integer, ByVal reqAc As Boolean, ByVal ackCmdId As Integer, ByVal timeout As Integer)
			Me.ReqAc = reqAc
			Me.CmdId = cmdId
			Me.AckCmdId = ackCmdId
			Me.Timeout = timeout
			_arguments = New List(Of String)()
		End Sub

		' ***** String based **** /

		''' <summary> Adds a command argument.  </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddArgument(ByVal argument As String)
			If argument IsNot Nothing Then
				_arguments.Add(argument)
			End If
		End Sub

		''' <summary> Adds command arguments.  </summary>
		''' <param name="arguments"> The arguments. </param>
		Public Sub AddArguments(ByVal arguments() As String)
			If arguments IsNot Nothing Then
				_arguments.AddRange(arguments)
			End If
		End Sub



		''' <summary> Adds a command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddArgument(ByVal argument As Single)
			_arguments.Add(argument.ToString("R",CultureInfo.InvariantCulture))
		End Sub

		''' <summary> Adds a command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddArgument(ByVal argument As Double)
			If BoardType = BoardType.Bit16 Then
				' Not completely sure if this is needed for plain text sending.
				Dim floatArg = CSng(argument)
				_arguments.Add(floatArg.ToString("R",CultureInfo.InvariantCulture))
			Else
				_arguments.Add(argument.ToString("R",CultureInfo.InvariantCulture))
			End If
		End Sub

		''' <summary> Adds a command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddArgument(ByVal argument As Int16)
			_arguments.Add(argument.ToString(CultureInfo.InvariantCulture))
		End Sub

		''' <summary> Adds a command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddArgument(ByVal argument As UInt16)
			_arguments.Add(argument.ToString(CultureInfo.InvariantCulture))
		End Sub

		''' <summary> Adds a command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddArgument(ByVal argument As Int32)
			' Make sure the other side can read this: on a 16 processor, read as Long
			_arguments.Add(argument.ToString(CultureInfo.InvariantCulture))
		End Sub

		''' <summary> Adds a command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddArgument(ByVal argument As UInt32)
			' Make sure the other side can read this: on a 16 processor, read as Long
			_arguments.Add(argument.ToString(CultureInfo.InvariantCulture))
		End Sub

		''' <summary> Adds a command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddArgument(ByVal argument As Boolean)
			AddArgument(CShort(Fix(If(argument, 1, 0))))
		End Sub

		' ***** Binary **** /

		''' <summary> Adds a binary command argument.  </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddBinArgument(ByVal argument As String)
			_arguments.Add(Escaping.Escape(argument))
		End Sub

		''' <summary> Adds a binary command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddBinArgument(ByVal argument As Single)
			_arguments.Add(BinaryConverter.ToString(argument))
		End Sub

		''' <summary> Adds a binary command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddBinArgument(ByVal argument As Double)
			_arguments.Add(If(BoardType = BoardType.Bit16, BinaryConverter.ToString(CSng(argument)), BinaryConverter.ToString(argument)))
		End Sub

		''' <summary> Adds a binary command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddBinArgument(ByVal argument As Int16)
			_arguments.Add(BinaryConverter.ToString(argument))
		End Sub

		''' <summary> Adds a binary command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddBinArgument(ByVal argument As UInt16)
			_arguments.Add(BinaryConverter.ToString(argument))
		End Sub

		''' <summary> Adds a binary command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddBinArgument(ByVal argument As Int32)
			_arguments.Add(BinaryConverter.ToString(argument))
		End Sub

		''' <summary> Adds a binary command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddBinArgument(ByVal argument As UInt32)
			_arguments.Add(BinaryConverter.ToString(argument))
		End Sub

		''' <summary> Adds a binary command argument. </summary>
		''' <param name="argument"> The argument. </param>
		Public Sub AddBinArgument(ByVal argument As Boolean)
			_arguments.Add(BinaryConverter.ToString(If(argument, CByte(1), CByte(0))))
		End Sub
	End Class
End Namespace
