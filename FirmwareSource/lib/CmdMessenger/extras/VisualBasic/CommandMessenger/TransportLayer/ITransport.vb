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
Namespace CommandMessenger.TransportLayer
	''' <summary> Interface for transport layer.  </summary>
	Public Interface ITransport
	Inherits IDisposable
		Function BytesInBuffer() As Integer
		Function Read() As Byte()
		Sub Poll()
		Function Connect() As Boolean
		Function Disconnect() As Boolean
		Function IsConnected() As Boolean
		Sub StartListening()
		Sub StopListening()
		Sub Write(ByVal buffer() As Byte)
		Event NewDataReceived As EventHandler
	End Interface
End Namespace
