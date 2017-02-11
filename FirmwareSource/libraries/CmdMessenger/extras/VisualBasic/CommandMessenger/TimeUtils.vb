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
	''' <summary>Class to get a timestamp </summary>
	Public NotInheritable Class TimeUtils
		Public Shared ReadOnly Jan1St1970 As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) ' 1 January 1970

		''' <summary> Gets the milliseconds since 1 Jan 1970. </summary>
		''' <value> The milliseconds since 1 Jan 1970. </value>
		Private Sub New()
		End Sub
		Public Shared ReadOnly Property Millis() As Long
			Get
				Return CLng(Fix((DateTime.Now.ToUniversalTime() - Jan1St1970).TotalMilliseconds))
			End Get
		End Property

		''' <summary> Gets the seconds since 1 Jan 1970. </summary>
		''' <value> The seconds since 1 Jan 1970. </value>
		Public Shared ReadOnly Property Seconds() As Long
			Get
				Return CLng(Fix((DateTime.Now.ToUniversalTime() - Jan1St1970).TotalSeconds))
			End Get
		End Property

		' Returns if it has been more than interval (in ms) ago. Used for periodic actions
		Public Shared Function HasExpired(ByRef prevTime As Long, ByVal interval As Long) As Boolean
			Dim millis = TimeUtils.Millis
			If millis - prevTime > interval Then
				prevTime = millis
				Return True
			End If
			Return False
		End Function
	End Class
End Namespace