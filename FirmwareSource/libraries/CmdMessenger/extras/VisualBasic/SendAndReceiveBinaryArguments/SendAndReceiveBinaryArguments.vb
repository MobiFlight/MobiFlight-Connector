' *** SendandReceiveBinaryArguments ***

' This example expands the previous SendandReceiveArguments example. The PC will 
'  send and receive multiple Binary values, demonstrating that this is more compact and faster. Since the output is not human readable any more, 
'  the logging is disabled and the NewLines are removed
'
' It adds a demonstration of how to:
' - Receive multiple binary parameters,
' - Send multiple binary parameters
' - Callback events being handled while the main program waits
' - How to calculate milliseconds, similar to Arduino function Millis()

Imports System
Imports System.Threading
Imports CommandMessenger
Imports CommandMessenger.Transport.Serial
Imports Microsoft.VisualBasic
' This is the list of recognized commands. These can be commands that can either be sent or received. 
' In order to receive, attach a callback function to these events
Friend Enum CommandIDs
    RequestPlainTextFloatSeries ' Command Request to send series in plain text
    ReceivePlainTextFloatSeries ' Command to send an item in plain text
    RequestBinaryFloatSeries ' Command Request to send series in binary form
    ReceiveBinaryFloatSeries ' Command to send an item in binary form
End Enum

Public Class SendAndReceiveBinaryArguments
    Public Property RunLoop As Boolean
    Private _serialTransport As SerialTransport
    Private _cmdMessenger As CmdMessenger
    Private _receivedItemsCount As Integer ' Counter of number of plain text items received
    Private _receivedBytesCount As Integer ' Counter of number of plain text bytes received
    Private _beginTime As Long ' Start time, 1st item of sequence received
    Private _endTime As Long ' End time, last item of sequence received
    Private _receivePlainTextFloatSeriesFinished As Boolean ' Indicates if plain text float series has been fully received
    Private _receiveBinaryFloatSeriesFinished As Boolean ' Indicates if binary float series has been fully received
    Private Const SeriesLength As UInt16 = 2000 ' Number of items we like to receive from the Arduino
    Private Const SeriesBase As Single = 1111111.13F ' Base of values to return: SeriesBase * (0..SeriesLength-1)

    ' ------------------ M A I N  ----------------------

    ' Setup function
    Public Sub Setup()
        ' Create Serial Port transport object
        _serialTransport = New SerialTransport With {
            .CurrentSerialSettings = New SerialSettings With {
                .PortName = "COM15",
                .BaudRate = 115200,
                .DtrEnable = False
                }
            }
        ' Initialize the command messenger with the Serial Port transport layer
     	' Set if it is communicating with a 16- or 32-bit Arduino board
        _cmdMessenger = New CmdMessenger(_serialTransport, BoardType.Bit16)

        ' Attach the callbacks to the Command Messenger
        AttachCommandCallBacks()

        ' Start listening
        _cmdMessenger.Connect()

        _receivedItemsCount = 0
        _receivedBytesCount = 0

		' Clear queues 
		_cmdMessenger.ClearReceiveQueue()
		_cmdMessenger.ClearSendQueue()

			Thread.Sleep(100)
        ' Send command requesting a series of 100 float values send in plain text form
        Dim commandPlainText = New SendCommand(CommandIDs.RequestPlainTextFloatSeries)
        commandPlainText.AddArgument(SeriesLength)
        commandPlainText.AddArgument(SeriesBase)
        ' Send command 
        _cmdMessenger.SendCommand(commandPlainText)

        ' Now wait until all values have arrived
        Do While Not _receivePlainTextFloatSeriesFinished
            Thread.Sleep(100)
        Loop
        ' Clear queues 
        _cmdMessenger.ClearReceiveQueue()
        _cmdMessenger.ClearSendQueue()

        _receivedItemsCount = 0
        _receivedBytesCount = 0
        ' Send command requesting a series of 100 float values send in binary form
        Dim commandBinary = New SendCommand(CommandIDs.RequestBinaryFloatSeries)
        commandBinary.AddBinArgument(SeriesLength)
        commandBinary.AddBinArgument(CSng(SeriesBase))

        ' Send command 
        _cmdMessenger.SendCommand(commandBinary)

        ' Now wait until all values have arrived
        Do While Not _receiveBinaryFloatSeriesFinished
			Thread.Sleep(100)
        Loop
    End Sub

    ' Loop function
    Public Sub [Loop]()
        RunLoop = False
    End Sub

    ' Exit function
    Public Sub [Exit]()
        ' Stop listening
        _cmdMessenger.Disconnect()

        ' Dispose Command Messenger
        _cmdMessenger.Dispose()

        ' Dispose Serial Port object
        _serialTransport.Dispose()

        ' Pause before stop
        Console.WriteLine("Press any key to stop...")
        Console.ReadKey()
    End Sub

    ''' Attach command call backs. 
    Private Sub AttachCommandCallBacks()
        _cmdMessenger.Attach(AddressOf OnUnknownCommand)
        _cmdMessenger.Attach(CommandIDs.ReceivePlainTextFloatSeries, AddressOf OnReceivePlainTextFloatSeries)
        _cmdMessenger.Attach(CommandIDs.ReceiveBinaryFloatSeries, AddressOf OnReceiveBinaryFloatSeries)
    End Sub

    ' ------------------  C A L L B A C K S ---------------------

    ' Called when a received command has no attached function.
    Private Sub OnUnknownCommand(ByVal arguments As ReceivedCommand)
        Console.WriteLine("Command without attached callback received")
    End Sub


    ' Callback function To receive the plain text float series from the Arduino
    Private Sub OnReceivePlainTextFloatSeries(ByVal arguments As ReceivedCommand)
        _receivedBytesCount += CountBytesInCommand(arguments, True)

			Dim count = arguments.ReadInt16Arg()
			Dim receivedValue = arguments.ReadFloatArg()


			If count <> _receivedItemsCount Then
				Console.WriteLine("Values not matching: received {0} expected {1}", count, _receivedItemsCount)
			End If

        If _receivedItemsCount Mod (SeriesLength / 10) = 0 Then
            Console.WriteLine("Received value: {0}", receivedValue)
        End If
        If _receivedItemsCount = 0 Then
            ' Received first value, start stopwatch
            _beginTime = Millis
        ElseIf count = SeriesLength - 1 Then
            ' Received all values, stop stopwatch
            _endTime = Millis
            Dim deltaTime = (_endTime - _beginTime)
            Console.WriteLine("{0} milliseconds per {1} items = is {2} ms/item, {3} Hz", deltaTime, SeriesLength, CSng(deltaTime) / CSng(SeriesLength), CSng(1000) * SeriesLength / CSng(deltaTime))
            Console.WriteLine("{0} milliseconds per {1} bytes = is {2} ms/byte,  {3} bytes/sec, {4} bps", deltaTime, _receivedBytesCount, CSng(deltaTime) / CSng(_receivedBytesCount), CSng(1000) * _receivedBytesCount / CSng(deltaTime), CSng(8) * 1000 * _receivedBytesCount / CSng(deltaTime))
            _receivePlainTextFloatSeriesFinished = True
        End If
        _receivedItemsCount += 1
    End Sub

    Private Function CountBytesInCommand(ByVal command As CommandMessenger.Command, ByVal printLfCr As Boolean) As Integer
        Dim bytes = command.CommandString().Length ' Command + command separator
        If printLfCr Then ' Add bytes for carriage return ('\r') and /or a newline ('\n')
            bytes += Environment.NewLine.Length
        End If
        Return bytes
    End Function

    ' Callback function To receive the binary float series from the Arduino
    Private Sub OnReceiveBinaryFloatSeries(ByVal arguments As ReceivedCommand)
			Dim count = arguments.ReadBinUInt16Arg()
			Dim receivedValue = arguments.ReadBinFloatArg()
        _receivedBytesCount += CountBytesInCommand(arguments, False)

        If _receivedItemsCount Mod (SeriesLength / 10) = 0 Then
            Console.WriteLine("Received value: {0}", receivedValue)
        End If
        If _receivedItemsCount = 0 Then
            ' Received first value, start stopwatch
            _beginTime = Millis
        ElseIf count = SeriesLength - 1 Then
            ' Received all values, stop stopwatch
            _endTime = Millis
            Dim deltaTime = (_endTime - _beginTime)
            Console.WriteLine("{0} milliseconds per {1} items = is {2} ms/item, {3} Hz", deltaTime, SeriesLength, CSng(deltaTime) / CSng(SeriesLength), CSng(1000) * SeriesLength / CSng(deltaTime))
            Console.WriteLine("{0} milliseconds per {1} bytes = is {2} ms/byte,  {3} bytes/sec, {4} bps", deltaTime, _receivedBytesCount, CSng(deltaTime) / CSng(_receivedBytesCount), CSng(1000) * _receivedBytesCount / CSng(deltaTime), CSng(8) * 1000 * _receivedBytesCount / CSng(deltaTime))
            _receiveBinaryFloatSeriesFinished = True
        End If
        _receivedItemsCount += 1
    End Sub

    ' Return Milliseconds since 1970
    Public Shared ReadOnly Property Millis() As Long
        Get
            Return CLng(Fix((DateTime.Now.ToUniversalTime() - New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds))
        End Get
    End Property
End Class