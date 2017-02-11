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
Imports System.IO.Ports

Namespace CommandMessenger.Serialport
	''' <summary>
	''' Class containing serial port configuration
	''' </summary>
	Public Class SerialSettings
		#Region "Properties"

		''' <summary>
		''' The port to use (for example: COM1 or /dev/ttyACM1).
		''' </summary>
		Private privatePortName As String
		Public Property PortName() As String
			Get
				Return privatePortName
			End Get
			Set(ByVal value As String)
				privatePortName = value
			End Set
		End Property

		''' <summary>
		''' Port baud rate.
		''' </summary>
		Private privateBaudRate As Integer
		Public Property BaudRate() As Integer
			Get
				Return privateBaudRate
			End Get
			Set(ByVal value As Integer)
				privateBaudRate = value
			End Set
		End Property

		''' <summary>
		''' One of the Parity values.
		''' </summary>
		Private privateParity As Parity
		Public Property Parity() As Parity
			Get
				Return privateParity
			End Get
			Set(ByVal value As Parity)
				privateParity = value
			End Set
		End Property

		''' <summary>
		''' The data bits value.
		''' </summary>
		Private privateDataBits As Integer
		Public Property DataBits() As Integer
			Get
				Return privateDataBits
			End Get
			Set(ByVal value As Integer)
				privateDataBits = value
			End Set
		End Property

		''' <summary>
		''' One of the StopBits values.
		''' </summary>
		Private privateStopBits As StopBits
		Public Property StopBits() As StopBits
			Get
				Return privateStopBits
			End Get
			Set(ByVal value As StopBits)
				privateStopBits = value
			End Set
		End Property

		''' <summary>
		''' Set Data Terminal Ready.
		''' </summary>
		Private privateDtrEnable As Boolean
		Public Property DtrEnable() As Boolean
			Get
				Return privateDtrEnable
			End Get
			Set(ByVal value As Boolean)
				privateDtrEnable = value
			End Set
		End Property

		''' <summary>
		''' Timeout for read and write operations to serial port.
		''' </summary>
		Private privateTimeout As Integer
		Public Property Timeout() As Integer
			Get
				Return privateTimeout
			End Get
			Set(ByVal value As Integer)
				privateTimeout = value
			End Set
		End Property

		#End Region

		Public Sub New()
			StopBits = StopBits.One
			DataBits = 8
			Parity = Parity.None
			BaudRate = 9600
			PortName = String.Empty
			Timeout = 500 ' 500ms is default value for SerialPort
		End Sub

		''' <summary>
		''' Check is serial settings configured properly.
		''' </summary>
		''' <returns></returns>
		Public Function IsValid() As Boolean
			Return (Not String.IsNullOrEmpty(PortName)) AndAlso BaudRate > 0
		End Function
	End Class
End Namespace
