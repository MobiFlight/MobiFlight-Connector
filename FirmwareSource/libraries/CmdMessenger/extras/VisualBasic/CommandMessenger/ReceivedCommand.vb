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
	''' <summary> A command received from CmdMessenger </summary>
	Public Class ReceivedCommand
		Inherits Command
		Private _parameter As Integer=-1 ' The parameter
		Private _dumped As Boolean = True ' true if parameter has been dumped

		''' <summary> Gets or sets the command input. </summary>
		''' <value> The raw string. </value>
		Private privateRawString As String
		Public Property RawString() As String
			Get
				Return privateRawString
			End Get
			Set(ByVal value As String)
				privateRawString = value
			End Set
		End Property

		''' <summary> Default constructor. </summary>
		Public Sub New()
		End Sub

		''' <summary> Constructor. </summary>
		''' <param name="rawArguments"> All command arguments, first one is command ID </param>
		Public Sub New(ByVal rawArguments() As String)
			Dim cmdId As Integer
			Me.CmdId = If((rawArguments IsNot Nothing AndAlso rawArguments.Length <>0 AndAlso Integer.TryParse(rawArguments(0), cmdId)), cmdId, -1)
			If Me.CmdId<0 Then
				Return
			End If
			If rawArguments.Length > 1 Then
				Dim array = New String(rawArguments.Length - 2){}
				Array.Copy(rawArguments, 1, array, 0, array.Length)
				_arguments.AddRange(array)
			End If
		End Sub

		''' <summary> Fetches the next argument. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function [Next]() As Boolean
			' If this parameter has already been read, see if there is another one
			If _dumped Then
				If _parameter < _arguments.Count-1 Then
					_parameter += 1
					_dumped = False
					Return True
				End If
				Return False
			End If
			Return True
		End Function

		''' <summary> returns if a next command is available </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Function Available() As Boolean
			Return [Next]()
		End Function

		' ***** String based **** /

		''' <summary> Reads the current argument as short value. </summary>
		''' <returns> The short value. </returns>
		Public Function ReadInt16Arg() As Int16
			If [Next]() Then
				Dim current As Int16
				If Int16.TryParse(_arguments(_parameter), current) Then
					_dumped = True
					Return current
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current argument as unsigned short value. </summary>
		''' <returns> The unsigned short value. </returns>
		Public Function ReadUInt16Arg() As UInt16
			If [Next]() Then
				Dim current As UInt16
				If UInt16.TryParse(_arguments(_parameter), current) Then
					_dumped = True
					Return current
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current argument as boolean value. </summary>
		''' <returns> The boolean value. </returns>
		Public Function ReadBoolArg() As Boolean
			Return (ReadInt32Arg() <> 0)
		End Function

		''' <summary> Reads the current argument as int value. </summary>
		''' <returns> The int value. </returns>
		Public Function ReadInt32Arg() As Int32
			If [Next]() Then
				Dim current As Int32
				If Int32.TryParse(_arguments(_parameter), current) Then
					_dumped = True
					Return current
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current argument as unsigned int value. </summary>
		''' <returns> The unsigned int value. </returns>
		Public Function ReadUInt32Arg() As UInt32
			If [Next]() Then
				Dim current As UInt32
				If UInt32.TryParse(_arguments(_parameter), current) Then
					_dumped = True
					Return current
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current argument as a float value. </summary>
		''' <returns> The float value. </returns>
		Public Function ReadFloatArg() As Single
			If [Next]() Then
				Dim current As Single
				If Single.TryParse(_arguments(_parameter), current) Then
					_dumped = True
					Return current
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current argument as a double value. </summary>
		''' <returns> The unsigned double value. </returns>
		Public Function ReadDoubleArg() As Double
			If [Next]() Then
				If BoardType = BoardType.Bit16 Then
					Dim current As Single
					If Single.TryParse(_arguments(_parameter), current) Then
						_dumped = True
						Return CType(current, Double)
					End If
				Else
					Dim current As Double
					If Double.TryParse(_arguments(_parameter), current) Then
						_dumped = True
						Return current
					End If
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current argument as a string value. </summary>
		''' <returns> The string value. </returns>
		Public Function ReadStringArg() As String
			If [Next]() Then
				If _arguments(_parameter) IsNot Nothing Then
					_dumped = True
					Return _arguments(_parameter)
				End If
			End If
			Return ""
		End Function

		' ***** Binary **** /

		''' <summary> Reads the current binary argument as a float value. </summary>
		''' <returns> The float value. </returns>
		Public Function ReadBinFloatArg() As Single
			If [Next]() Then
				Dim current = BinaryConverter.ToFloat(_arguments(_parameter))
				If current IsNot Nothing Then
					_dumped = True
					Return CSng(current)
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current binary argument as a double value. </summary>
		''' <returns> The double value. </returns>
		Public Function ReadBinDoubleArg() As Double
			If [Next]() Then
				If BoardType = BoardType.Bit16 Then
					Dim current = BinaryConverter.ToFloat(_arguments(_parameter))
					If current IsNot Nothing Then
						_dumped = True
						Return CDbl(current)
					End If
				Else
					Dim current = BinaryConverter.ToDouble(_arguments(_parameter))
					If current IsNot Nothing Then
						_dumped = True
						Return CDbl(current)
					End If
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current binary argument as a short value. </summary>
		''' <returns> The short value. </returns>
		Public Function ReadBinInt16Arg() As Int16
			If [Next]() Then
				Dim current = BinaryConverter.ToInt16(_arguments(_parameter))
				If current IsNot Nothing Then
					_dumped = True
					Return CShort(Fix(current))
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current binary argument as a unsigned short value. </summary>
		''' <returns> The unsigned short value. </returns>
		Public Function ReadBinUInt16Arg() As UInt16
			If [Next]() Then
				Dim current = BinaryConverter.ToUInt16(_arguments(_parameter))
				If current IsNot Nothing Then
					_dumped = True
					Return CUShort(current)
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current binary argument as a int value. </summary>
		''' <returns> The int32 value. </returns>
		Public Function ReadBinInt32Arg() As Int32
			If [Next]() Then
				Dim current = BinaryConverter.ToInt32(_arguments(_parameter))
				If current IsNot Nothing Then
					_dumped = True
					Return CInt(Fix(current))
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current binary argument as a unsigned int value. </summary>
		''' <returns> The unsigned int value. </returns>
		Public Function ReadBinUInt32Arg() As UInt32
			If [Next]() Then
				Dim current = BinaryConverter.ToUInt32(_arguments(_parameter))
				If current IsNot Nothing Then
					_dumped = True
					Return CUInt(current)
				End If
			End If
			Return 0
		End Function

		''' <summary> Reads the current binary argument as a string value. </summary>
		''' <returns> The string value. </returns>
		Public Function ReadBinStringArg() As String
			If [Next]() Then
				If _arguments(_parameter) IsNot Nothing Then
					_dumped = True
					Return Escaping.Unescape(_arguments(_parameter))
				End If
			End If
			Return ""
		End Function

		''' <summary> Reads the current binary argument as a boolean value. </summary>
		''' <returns> The boolean value. </returns>
		Public Function ReadBinBoolArg() As Boolean
			If [Next]() Then
				Dim current = BinaryConverter.ToByte(_arguments(_parameter))
				If current IsNot Nothing Then
					_dumped = True
					Return (current <> 0)
				End If
			End If
			Return False
		End Function
	End Class
End Namespace