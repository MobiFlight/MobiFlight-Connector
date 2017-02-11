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
Imports System.Runtime.InteropServices

Namespace CommandMessenger
	''' <summary> Helper object to convert structures to byte arrays and vice versa.  </summary>
	Friend Class StructSerializer
		''' <summary> Convert an object to a byte array. </summary>
		''' <param name="obj"> The object. </param>
		''' <returns> The byte array. </returns>
		Public Shared Function ObjectToByteArray(ByVal obj As Object) As Byte()
			Dim length = Marshal.SizeOf(obj)
			Dim byteArray = New Byte(length - 1){}
			Dim arrayPointer = Marshal.AllocHGlobal(length)
			Marshal.StructureToPtr(obj, arrayPointer, True)
			Marshal.Copy(arrayPointer, byteArray, 0, length)
			Marshal.FreeHGlobal(arrayPointer)
			Return byteArray
		End Function

		''' <summary> Convert an byte array to an object. </summary>
		''' <param name="bytearray"> The bytearray. </param>
		''' <param name="obj">       [in,out] The object. </param>
		Public Shared Sub ByteArrayToObject(ByVal bytearray() As Byte, ByRef obj As Object)
			Dim length = Marshal.SizeOf(obj)
			Dim arrayPointer = Marshal.AllocHGlobal(length)
			Marshal.Copy(bytearray, 0, arrayPointer, length)
			obj = Marshal.PtrToStructure(arrayPointer, obj.GetType())
			Marshal.FreeHGlobal(arrayPointer)
		End Sub
	End Class
End Namespace
