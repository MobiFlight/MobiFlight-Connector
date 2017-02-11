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
Imports System.Text

Namespace CommandMessenger
	Public Class BinaryConverter
		Private Shared _stringEncoder As Encoding = Encoding.GetEncoding("ISO-8859-1") ' The string encoder

		''' <summary> Sets the string encoder. </summary>
		''' <value> The string encoder. </value>
		Public WriteOnly Property StringEncoder() As Encoding
			Set(ByVal value As Encoding)
				_stringEncoder = value
			End Set
		End Property

		'**** from binary value to string ***

		''' <summary> Convert a float into a string representation. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> A string representation of this object. </returns>
		Public Shared Overloads Function ToString(ByVal value As Single) As String
			Try
				Dim byteArray() As Byte = BitConverter.GetBytes(value)
				Return BytesToEscapedString(byteArray)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Convert a Double into a string representation. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> A string representation of this object. </returns>
		Public Shared Overloads Function ToString(ByVal value As Double) As String
			Try
				Dim byteArray() As Byte = BitConverter.GetBytes(value)
				Return BytesToEscapedString(byteArray)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Convert an int into a string representation. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> A string representation of this object. </returns>
		Public Shared Overloads Function ToString(ByVal value As Integer) As String
			Try
				Dim byteArray() As Byte = BitConverter.GetBytes(value)
				Return BytesToEscapedString(byteArray)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Convert an unsigned int into a string representation. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> A string representation of this object. </returns>
		Public Shared Overloads Function ToString(ByVal value As UInteger) As String
			Try
				Dim byteArray() As Byte = BitConverter.GetBytes(value)
				Return BytesToEscapedString(byteArray)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Convert a short into a string representation. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> A string representation of this object. </returns>
		Public Shared Overloads Function ToString(ByVal value As Short) As String
			Try
				Dim byteArray() As Byte = BitConverter.GetBytes(value)
				Return BytesToEscapedString(byteArray)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Convert an unsigned an unsigned short into a string representation. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> A string representation of this object. </returns>
		Public Shared Overloads Function ToString(ByVal value As UShort) As String
			Try
				Dim byteArray() As Byte = BitConverter.GetBytes(value)
				Return BytesToEscapedString(byteArray)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Convert a byte into a string representation. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> A string representation of this object. </returns>
		Public Shared Overloads Function ToString(ByVal value As Byte) As String
			Try
				Return BytesToEscapedString(New Byte() {value})
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function


		'**** from string to binary value ***

		''' <summary> Converts a string to a float. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> Input string as a float? </returns>
		Public Shared Function ToFloat(ByVal value As String) As Nullable(Of Single)
			Try
				Dim bytes() As Byte = EscapedStringToBytes(value)
				If bytes.Length < 4 Then
					Return Nothing
				End If
				Return BitConverter.ToSingle(bytes, 0)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Converts a string representation to a double. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> Input string as a Double? </returns>
		Public Shared Function ToDouble(ByVal value As String) As Nullable(Of Double)
			Try
				Dim bytes() As Byte = EscapedStringToBytes(value)
				Return BitConverter.ToDouble(bytes, 0)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Converts a string representation to an int 32. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> This object as an Int32? </returns>
		Public Shared Function ToInt32(ByVal value As String) As Nullable(Of Int32)
			Try
				Dim bytes() As Byte = EscapedStringToBytes(value)
				Return BitConverter.ToInt32(bytes, 0)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Converts a string representation to a u int 32. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> Input string as a UInt32? </returns>
		Public Shared Function ToUInt32(ByVal value As String) As Nullable(Of UInt32)
			Try
				Dim bytes() As Byte = EscapedStringToBytes(value)
				Return BitConverter.ToUInt32(bytes, 0)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Converts a string representation to a u int 16. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> Input string as a UInt16? </returns>
		Public Shared Function ToUInt16(ByVal value As String) As Nullable(Of UInt16)
			Try
				Dim bytes() As Byte = EscapedStringToBytes(value)
				Return BitConverter.ToUInt16(bytes, 0)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Converts a string representation to an int 16. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> This object as an Int16? </returns>
		Public Shared Function ToInt16(ByVal value As String) As Nullable(Of Int16)
			Try
				Dim bytes() As Byte = EscapedStringToBytes(value)
				Return BitConverter.ToInt16(bytes, 0)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Converts a string representation to a byte. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> Input string as a byte? </returns>
		Public Shared Function ToByte(ByVal value As String) As Nullable(Of Byte)
			Try
				Dim bytes() As Byte = EscapedStringToBytes(value)
				Return bytes(0)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		'**** conversion functions ***

		''' <summary> Converts a byte array to escaped string. </summary>
		''' <param name="byteArray"> Array of bytes. </param>
		''' <returns> input value as an escaped string. </returns>
		Private Shared Function BytesToEscapedString(ByVal byteArray() As Byte) As String
			Try
				Dim stringValue As String = _stringEncoder.GetString(byteArray)
				Return Escaping.Escape(stringValue)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Converts an escaped string to a bytes array. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> input value as an escaped string. </returns>
		Public Shared Function EscapedStringToBytes(ByVal value As String) As Byte()
			Try
				Dim unEscapedValue As String = Escaping.Unescape(value)
				Return _stringEncoder.GetBytes(unEscapedValue)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary> Converts a string to a bytes array. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> input value as a byte array. </returns>
		Public Shared Function StringToBytes(ByVal value As String) As Byte()
			Return _stringEncoder.GetBytes(value)
		End Function

		''' <summary> Converts a char array to a bytes array. </summary>
		''' <param name="value"> The value to be converted. </param>
		''' <returns> input value as a byte array. </returns>
		Public Shared Function CharsToBytes(ByVal value() As Char) As Byte()
			Return _stringEncoder.GetBytes(value)
		End Function

	End Class
End Namespace