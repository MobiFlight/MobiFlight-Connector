Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Text

Namespace CommandMessenger.TransportLayer
	Friend NotInheritable Class Logger
		Public Shared ReadOnly StringEncoder As Encoding = Encoding.GetEncoding("ISO-8859-1") ' The string encoder
		Private Shared _fileStream As FileStream


		Private Sub New()
		End Sub
		Shared Sub New()
			LogFileName = Nothing
			IsEnabled = True
		End Sub

		Private privateIsEnabled As Boolean
		Public Shared Property IsEnabled() As Boolean
			Get
				Return privateIsEnabled
			End Get
			Set(ByVal value As Boolean)
				privateIsEnabled = value
			End Set
		End Property
		Private privateIsOpen As Boolean
		Public Shared Property IsOpen() As Boolean
			Get
				Return privateIsOpen
			End Get
			Private Set(ByVal value As Boolean)
				privateIsOpen = value
			End Set
		End Property
		Private privateDirectFlush As Boolean
		Public Shared Property DirectFlush() As Boolean
			Get
				Return privateDirectFlush
			End Get
			Set(ByVal value As Boolean)
				privateDirectFlush = value
			End Set
		End Property

		''' <summary> Gets or sets the log file name. </summary>
		''' <value> The logfile name . </value>
		Private privateLogFileName As String
		Public Shared Property LogFileName() As String
			Get
				Return privateLogFileName
			End Get
			Private Set(ByVal value As String)
				privateLogFileName = value
			End Set
		End Property


		Public Shared Function Open() As Boolean
			Return Open(LogFileName)
		End Function

		Public Shared Function Open(ByVal logFileName As String) As Boolean
			If IsOpen AndAlso Logger.LogFileName = logFileName Then
				Return True
			End If

			Logger.LogFileName = logFileName
			If IsOpen Then
				Try
					_fileStream.Close()
				Catch e1 As Exception
				End Try
				IsOpen = False
			End If

			Try
				_fileStream = New FileStream(logFileName, FileMode.Create, FileAccess.ReadWrite)
			Catch e2 As Exception
				Return False
			End Try
			IsOpen = True
			Return True
		End Function

		Public Shared Sub Close()
			If (Not IsOpen) Then
				Return
			End If
			Try
				_fileStream.Close()
			Catch e1 As Exception
			End Try
			IsOpen = False
		End Sub

		Public Shared Sub Log(ByVal logString As String)
			If (Not IsEnabled) OrElse (Not IsOpen) Then
				Return
			End If
			Dim writeBytes = StringEncoder.GetBytes(logString)
			_fileStream.Write(writeBytes, 0, writeBytes.Length)
			If DirectFlush Then
				_fileStream.Flush()
			End If
		End Sub


		Public Shared Sub LogLine(ByVal logString As String)
			Log(logString & ControlChars.Lf)
		End Sub
	End Class
End Namespace
