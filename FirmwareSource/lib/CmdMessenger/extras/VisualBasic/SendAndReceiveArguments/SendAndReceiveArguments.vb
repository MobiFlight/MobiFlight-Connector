' *** SendandReceive ***

' This example expands the previous SendandReceive example. The PC will now send multiple float values
' and wait for a response from the Arduino. 
' It adds a demonstration of how to:
' - Send multiple parameters, and wait for response
' - Receive multiple parameters
' - Add logging events on data that has been sent or received

Imports System
Imports CommandMessenger
Imports CommandMessenger.Transport.Serial
' This is the list of recognized commands. These can be commands that can either be sent or received. 
' In order to receive, attach a callback function to these events
Friend Enum CommandIDs
    Acknowledge
    ErrorEncountered
    FloatAddition
    FloatAdditionResult
End Enum

Public Class SendAndReceiveArguments
    Public Property RunLoop As Boolean
    Private _serialTransport As SerialTransport
    Private _cmdMessenger As CmdMessenger

    ' ------------------ M A I N  ----------------------

    ' Setup function
    Public Sub Setup()

        ' Create Serial Port transport object
        _serialTransport = New SerialTransport With {
            .CurrentSerialSettings = New SerialSettings With {
                .PortName = "COM6",
                .BaudRate = 115200,
                .DtrEnable = False
                }
            }
        ' Initialize the command messenger with the Serial Port transport layer

        _cmdMessenger = New CmdMessenger(_serialTransport, BoardType.Bit16)

        ' Attach the callbacks to the Command Messenger
        AttachCommandCallBacks()

        ' Attach to NewLinesReceived for logging purposes
        AddHandler _cmdMessenger.NewLineReceived, AddressOf NewLineReceived

        ' Attach to NewLineSent for logging purposes
        AddHandler _cmdMessenger.NewLineSent, AddressOf NewLineSent

        ' Start listening
        _cmdMessenger.Connect()
    End Sub

    ' Loop function
    Public Sub [Loop]()
        ' Create command FloatAddition, which will wait for a return command FloatAdditionResult
        Dim command = New SendCommand(CommandIDs.FloatAddition, CommandIDs.FloatAdditionResult, 1000)

        ' Add 2 float command arguments
        Const a As Single = 3.14F
        Const b As Single = 2.71F
        command.AddArgument(a)
        command.AddArgument(b)

        ' Send command
        Dim floatAdditionResultCommand = _cmdMessenger.SendCommand(command)

        ' Check if received a (valid) response
        If floatAdditionResultCommand.Ok Then
            ' Read returned argument
            Dim sum = floatAdditionResultCommand.ReadFloatArg()
            Dim diff = floatAdditionResultCommand.ReadFloatArg()

            ' Compare with sum of sent values
            Dim errorSum = sum - (a + b)
            Dim errorDiff = diff - (a - b)

            Console.WriteLine("Received sum {0}, difference of {1}", sum, diff)
            Console.WriteLine("with errors {0} and {1}, respectively", errorSum, errorDiff)

            If errorDiff < 0.000001 AndAlso errorSum < 0.000001 Then
                Console.WriteLine("Seems to be correct!")
            Else
                Console.WriteLine("Does not seem to be correct!")
            End If
        Else
            Console.WriteLine("No response!")
        End If

        ' Stop running loop
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
        _cmdMessenger.Attach(CommandIDs.Acknowledge, AddressOf OnAcknowledge)
        _cmdMessenger.Attach(CommandIDs.ErrorEncountered, AddressOf OnError)
    End Sub

    ' ------------------  C A L L B A C K S ---------------------

    ' Called when a received command has no attached function.
    Private Sub OnUnknownCommand(ByVal arguments As ReceivedCommand)
        Console.WriteLine("Command without attached callback received")
    End Sub

    ' Callback function that prints that the Arduino has acknowledged
    Private Sub OnAcknowledge(ByVal arguments As ReceivedCommand)
        Console.WriteLine(" Arduino is ready")
    End Sub

    ' Callback function that prints that the Arduino has experienced an error
    Private Sub OnError(ByVal arguments As ReceivedCommand)
        Console.WriteLine(" Arduino has experienced an error")
    End Sub

    ' Log received line to console
    Private Sub NewLineReceived(ByVal sender As Object, ByVal e As CommandEventArgs)
        Console.WriteLine("Received > " & e.Command.CommandString())
    End Sub

    ' Log sent line to console
    Private Sub NewLineSent(ByVal sender As Object, ByVal e As CommandEventArgs)
        Console.WriteLine("Sent > " & e.Command.CommandString())
    End Sub
End Class