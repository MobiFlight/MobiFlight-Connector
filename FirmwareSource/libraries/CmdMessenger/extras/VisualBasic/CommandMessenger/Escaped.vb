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
Imports System.Linq

Namespace CommandMessenger
	''' <summary> Class for bookkeeping which characters in the stream are escaped. </summary>
	Public Class IsEscaped
		Private _lastChar As Char = ControlChars.NullChar ' The last character

		' Returns if the character is escaped
		' Note create new instance for every independent string

		''' <summary>Returns if the character is escaped.
		''' 		 Note create new instance for every independent string </summary>
		''' <param name="currChar"> The currebt character. </param>
		''' <returns> true if the character is escaped, false if not. </returns>
		Public Function EscapedChar(ByVal currChar As Char) As Boolean
			Dim escaped As Boolean = (_lastChar = Escaping.EscapeCharacter)
			_lastChar = currChar

			' special case: the escape char has been escaped: 
			If _lastChar = Escaping.EscapeCharacter AndAlso escaped Then
				_lastChar = ControlChars.NullChar
			End If
			Return escaped
		End Function
	End Class

	''' <summary> Utility class providing escaping functions </summary>
	Public NotInheritable Class Escaping
		' Remove all occurrences of removeChar unless it is escaped by escapeChar

		Private Shared _fieldSeparator As Char = ","c ' The field separator
		Private Shared _commandSeparator As Char = ";"c ' The command separator
		Private Shared _escapeCharacter As Char = "/"c ' The escape character

		''' <summary> Gets the escape character. </summary>
		''' <value> The escape character. </value>
		Private Sub New()
		End Sub
		Public Shared ReadOnly Property EscapeCharacter() As Char
			Get
				Return _escapeCharacter
			End Get
		End Property

		''' <summary> Sets custom escape characters. </summary>
		''' <param name="fieldSeparator">   The field separator. </param>
		''' <param name="commandSeparator"> The command separator. </param>
		''' <param name="escapeCharacter">  The escape character. </param>
		Public Shared Sub EscapeChars(ByVal fieldSeparator As Char, ByVal commandSeparator As Char, ByVal escapeCharacter As Char)
			_fieldSeparator = fieldSeparator
			_commandSeparator = commandSeparator
			_escapeCharacter = escapeCharacter
		End Sub

		''' <summary> Removes all occurences of a specific character unless escaped. </summary>
		''' <param name="input">      The input. </param>
		''' <param name="removeChar"> The  character to remove. </param>
		''' <param name="escapeChar"> The escape character. </param>
		''' <returns> The string with all removeChars removed. </returns>
		Public Shared Function Remove(ByVal input As String, ByVal removeChar As Char, ByVal escapeChar As Char) As String
			Dim output = ""
			Dim escaped = New IsEscaped()
			For i = 0 To input.Length - 1
				Dim inputChar As Char = input.Chars(i)
				Dim isEscaped As Boolean = escaped.EscapedChar(inputChar)
				If inputChar <> removeChar OrElse isEscaped Then
					output += inputChar
				End If
			Next i
			Return output
		End Function

		' Split String on separator character unless it is escaped by escapeChar

		''' <summary> Splits. </summary>
		''' <param name="input">              The input. </param>
		''' <param name="separator">          The separator. </param>
		''' <param name="escapeCharacter">    The escape character. </param>
		''' <param name="stringSplitOptions"> Options for controlling the string split. </param>
		''' <returns> The split string. </returns>
		Public Shared Function Split(ByVal input As String, ByVal separator As Char, ByVal escapeCharacter As Char, ByVal stringSplitOptions As StringSplitOptions) As String()
			Dim word = ""
			Dim result = New List(Of String)()
			For i = 0 To input.Length - 1
				Dim t = input.Chars(i)
				If t Is separator Then
					result.Add(word)
					word = ""
				Else
					If t Is escapeCharacter Then
						word += t
						If i < input.Length - 1 Then
							i += 1
							t = input.Chars(i)
						End If
					End If
					word += t
				End If
			Next i
			result.Add(word)
			If stringSplitOptions = StringSplitOptions.RemoveEmptyEntries Then
				result.RemoveAll(Function(item) item = "")
			End If
			Return result.ToArray()
		End Function

		''' <summary> Escapes the input string. </summary>
		''' <param name="input"> The unescaped input string. </param>
		''' <returns> Escaped output string. </returns>
		Public Shared Function Escape(ByVal input As String) As String
			Dim escapeChars = New Object() { _escapeCharacter.ToString(CultureInfo.InvariantCulture), _fieldSeparator.ToString(CultureInfo.InvariantCulture), _commandSeparator.ToString(CultureInfo.InvariantCulture), Constants.vbNullChar }
			input = escapeChars.Aggregate(input, Function(current, escapeChar) current.Replace(escapeChar, _escapeCharacter + escapeChar))
			Return input
		End Function

		''' <summary> Unescapes the input string. </summary>
		''' <param name="input"> The escaped input string. </param>
		''' <returns> The unescaped output string. </returns>
		Public Shared Function Unescape(ByVal input As String) As String
			Dim output As String = ""
			' Move unescaped characters right
			For fromChar = 0 To input.Length - 1
				If input.Chars(fromChar) = _escapeCharacter Then
					fromChar += 1
				End If
				output &= input.Chars(fromChar)
			Next fromChar
			Return output
		End Function
	End Class
End Namespace