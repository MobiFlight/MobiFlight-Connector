Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices

Namespace CommandMessenger
	Public Class ConsoleUtils
		Public Shared ConsoleClose As EventHandler = AddressOf AnonymousMethod1

		Shared Sub New()
			_handler = AddressOf ConsoleEventCallback
			SetConsoleCtrlHandler(_handler, True)
		End Sub
		Private Sub AnonymousMethod1(ByVal sender As Object, ByVal e As System.EventArgs)
		End Sub

		Private Shared Function ConsoleEventCallback(ByVal eventType As Integer) As Boolean
			If eventType = 2 Then
				ConsoleClose(Nothing, EventArgs.Empty)
			End If
			ConsoleClose= Nothing
			_handler = Nothing
			Return False
		End Function

		Private Shared _handler As ConsoleEventDelegate ' Keeps it from getting garbage collected

		Private Delegate Function ConsoleEventDelegate(ByVal eventType As Integer) As Boolean
		<DllImport("kernel32.dll", SetLastError := True)> _
		Private Shared Function SetConsoleCtrlHandler(ByVal callback As ConsoleEventDelegate, ByVal add As Boolean) As Boolean
		End Function

	End Class
End Namespace
