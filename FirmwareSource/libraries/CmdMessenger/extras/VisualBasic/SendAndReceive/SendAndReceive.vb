' *** SendandReceive ***

' This example expands the previous Receive example. The Arduino will now send back a status.
' It adds a demonstration of how to:
' - Handle received commands that do not have a function attached
' - Receive a command with a parameter from the Arduino

Imports System
Imports System.Threading
Imports CommandMessenger
Imports CommandMessenger.Transport.Serial
' This is the list of recognized commands. These can be commands that can either be sent or received. 
' In order to receive, attach a callback function to these events
Friend Enum CommandIDs
    SetLed
    Status
End Enum

Public Class SendAndReceive
    Public Property RunLoop As Boolean
    Private _serialTransport As SerialTransport
    Private _cmdMessenger As CmdMessenger
    Private _ledState As Boolean
    Private _count As Integer

    ' Setup function
    Public Sub Setup()
        _ledState = False

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

        ' Tell CmdMessenger if it is communicating with a 16 or 32 bit Arduino board

        ' Attach the callbacks to the Command Messenger
        AttachCommandCallBacks()

        ' Start listening
        _cmdMessenger.Connect()
    End Sub


    ' Loop function
    Public Sub [Loop]()
        _count += 1

        ' Create command
        Dim command = New SendCommand(CommandIDs.SetLed, _ledState)

        ' Send command
        _cmdMessenger.SendCommand(command)

        ' Wait for 1 second and repeat
        Thread.Sleep(1000)
        _ledState = Not _ledState ' Toggle led state

        If _count > 100 Then ' Stop loop after 100 rounds
            RunLoop = False
        End If
    End Sub

    ' Exit function
    Public Sub [Exit]()

        If _cmdMessenger IsNot Nothing Then
            ' Stop listening
            _cmdMessenger.Disconnect()
            ' Dispose Command Messenger
            _cmdMessenger.Dispose()
        End If


        ' Dispose Serial Port object
        If _serialTransport IsNot Nothing Then
            _serialTransport.Dispose()
        End If

    End Sub

    ''' Attach command call backs. 
    Private Sub AttachCommandCallBacks()
        _cmdMessenger.Attach(AddressOf OnUnknownCommand)
        _cmdMessenger.Attach(CommandIDs.Status, AddressOf OnStatus)
    End Sub

    ''' Executes when an unknown command has been received.
    Private Sub OnUnknownCommand(ByVal arguments As ReceivedCommand)
        Console.WriteLine("Command without attached callback received")
    End Sub

    ' Callback function that prints the Arduino status to the console
    Private Sub OnStatus(ByVal arguments As ReceivedCommand)
        Console.Write("Arduino status: ")
        Console.WriteLine(arguments.ReadStringArg())
    End Sub
End Class