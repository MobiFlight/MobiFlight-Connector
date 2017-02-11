Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Ports
Imports System.Linq
Imports System.Reflection
Imports System.Text.RegularExpressions

Namespace CommandMessenger.Serialport
	''' <summary>
	''' Utility methods for serial communication handling.
	''' </summary>
	Public NotInheritable Class SerialUtils
		Private Shared _isMonoRuntime As Boolean = (Type.GetType("Mono.Runtime") IsNot Nothing)

		''' <summary>
		''' Commonly used baud rates.
		''' </summary>
		Private Sub New()
		End Sub
		Public Shared ReadOnly Property CommonBaudRates() As Integer()
			Get
				Return new Integer() { 115200, 57600, 9600 }
			End Get
		End Property

		''' <summary> Queries if a given port exists. </summary>
		''' <returns> true if it succeeds, false if it fails. </returns>
		Public Shared Function PortExists(ByVal serialPortName As String) As Boolean
			If _isMonoRuntime Then
				Return File.Exists(serialPortName)
			Else
				Return SerialPort.GetPortNames().Contains(serialPortName)
			End If
		End Function

		''' <summary>
		''' Retrieve available serial ports.
		''' </summary>
		''' <returns>Array of serial port names.</returns>
		Public Shared Function GetPortNames() As String()
'            *
'             * Under Mono SerialPort.GetPortNames() returns /dev/ttyS* devices,
'             * but Arduino is detected as ttyACM* or ttyUSB*
'             * 
			If _isMonoRuntime Then
				Dim searchPattern = New Regex("ttyACM.+|ttyUSB.+")
				Return Directory.GetFiles("/dev").Where(Function(f) searchPattern.IsMatch(f)).ToArray()
			Else
				Return SerialPort.GetPortNames()
			End If
		End Function

		''' <summary> 
		''' Retrieves the possible baud rates for the provided serial port. Windows ONLY.
		''' </summary>
		''' <returns>List of supported baud rates.</returns>
		Public Shared Function GetSupportedBaudRates(ByVal serialPortName As String) As Integer()
			Try
				Dim serialPort = New SerialPort(serialPortName)
				serialPort.Open()
				If serialPort.IsOpen Then
					Dim fieldInfo = serialPort.BaseStream.GetType().GetField("commProp", BindingFlags.Instance Or BindingFlags.NonPublic)
					If fieldInfo IsNot Nothing Then
						Dim p As Object = fieldInfo.GetValue(serialPort.BaseStream)
						Dim fieldInfoValue = p.GetType().GetField("dwSettableBaud", BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.Public)
						If fieldInfoValue IsNot Nothing Then
							Dim dwSettableBaud = CInt(Fix(fieldInfoValue.GetValue(p)))
							serialPort.Close()

							Return BaudRateMaskToActualRates(dwSettableBaud).ToArray()
						End If
					End If
				End If
			Catch
				' Ignore.
			End Try

			' Can't determine possible baud rates, will use all possible values
			Return BaudRateMaskToActualRates(Integer.MaxValue).ToArray()
		End Function

		''' <summary>
		''' Get the range of possible baud rates for serial port.
		''' </summary>
		''' <param name="possibleBaudRates">dwSettableBaud parameter from the COMMPROP Structure</param>
		''' <returns>List of bad rates</returns>
		Private Shared Function BaudRateMaskToActualRates(ByVal possibleBaudRates As Integer) As List(Of Integer)
'TODO: INSTANT VB TODO TASK: There is no equivalent to #pragma directives in VB.NET:
'			#pragma warning disable 219
			' ReSharper disable InconsistentNaming
			Const BAUD_075 As Integer = &H00000001
			Const BAUD_110 As Integer = &H00000002
			Const BAUD_150 As Integer = &H00000008
			Const BAUD_300 As Integer = &H00000010
			Const BAUD_600 As Integer = &H00000020
			Const BAUD_1200 As Integer = &H00000040
			Const BAUD_1800 As Integer = &H00000080
			Const BAUD_2400 As Integer = &H00000100
			Const BAUD_4800 As Integer = &H00000200
			Const BAUD_7200 As Integer = &H00000400
			Const BAUD_9600 As Integer = &H00000800
			Const BAUD_14400 As Integer = &H00001000
			Const BAUD_19200 As Integer = &H00002000
			Const BAUD_38400 As Integer = &H00004000
			Const BAUD_56K As Integer = &H00008000
			Const BAUD_57600 As Integer = &H00040000
			Const BAUD_115200 As Integer = &H00020000
			Const BAUD_128K As Integer = &H00010000
'TODO: INSTANT VB TODO TASK: There is no equivalent to #pragma directives in VB.NET:
'			#pragma warning restore 219

			Dim baudRateCollection = New List(Of Integer)()

			' We start with the most common baudrates:
			If (possibleBaudRates And BAUD_115200) > 0 Then
				baudRateCollection.Add(115200) ' Maxspeed Arduino Uno, Mega, with AT8u2 USB
			End If
			If (possibleBaudRates And BAUD_9600) > 0 Then
				baudRateCollection.Add(9600) ' Often default speed
			End If
			If (possibleBaudRates And BAUD_57600) > 0 Then
				baudRateCollection.Add(57600) ' Maxspeed Arduino Duemilanove, FTDI Serial
			End If

			' After that going from fastest to slowest baudrates:
			If (possibleBaudRates And BAUD_128K) > 0 Then
				baudRateCollection.Add(128000)
			End If
			If (possibleBaudRates And BAUD_56K) > 0 Then
				baudRateCollection.Add(56000)
			End If
			If (possibleBaudRates And BAUD_38400) > 0 Then
				baudRateCollection.Add(38400)
			End If
			If (possibleBaudRates And BAUD_19200) > 0 Then
				baudRateCollection.Add(19200)
			End If
			If (possibleBaudRates And BAUD_14400) > 0 Then
				baudRateCollection.Add(14400)
			End If
			If (possibleBaudRates And BAUD_7200) > 0 Then
				baudRateCollection.Add(7200)
			End If
			If (possibleBaudRates And BAUD_4800) > 0 Then
				baudRateCollection.Add(4800)
			End If
			If (possibleBaudRates And BAUD_2400) > 0 Then
				baudRateCollection.Add(2400)
			End If
			If (possibleBaudRates And BAUD_1800) > 0 Then
				baudRateCollection.Add(1800)
			End If
			If (possibleBaudRates And BAUD_1200) > 0 Then
				baudRateCollection.Add(1200)
			End If
			If (possibleBaudRates And BAUD_600) > 0 Then
				baudRateCollection.Add(600)
			End If
			If (possibleBaudRates And BAUD_300) > 0 Then
				baudRateCollection.Add(300)
			End If

			' Skip old and slow rates.
'            if ((possibleBaudRates & BAUD_150) > 0)
'                baudRateCollection.Add(150);
'            if ((possibleBaudRates & BAUD_110) > 0)
'                baudRateCollection.Add(110);
'            if ((possibleBaudRates & BAUD_075) > 0)
'                baudRateCollection.Add(75);

			Return baudRateCollection
		End Function
	End Class
End Namespace
