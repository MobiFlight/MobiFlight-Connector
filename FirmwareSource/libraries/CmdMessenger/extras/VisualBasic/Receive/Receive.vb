' This 1st example will make the PC toggle the integrated led on the arduino board. 
' It demonstrates how to:
' - Define commands
' - Set up a serial connection
' - Send a command with a parameter to the Arduino

Imports System
Imports System.Threading
Imports CommandMessenger
Imports CommandMessenger.Transport.Serial

' This is the list of recognized commands. These can be commands that can either be sent or received. 
' In order to receive, attach a callback function to these events
' 
' Default commands
' Note that commands work both directions:
' - All commands can be sent
' - Commands that have callbacks attached can be received
' 
' This means that both sides should have an identical command list:
' one side can either send it or receive it (sometimes both)

' Commands
Friend Enum CommandIDs
    SetLed ' Command to request led to be set in specific state
End Enum

Public Class Receive
    Public Property RunLoop As Boolean
    Private _serialTransport As SerialTransport
    Private _cmdMessenger As CmdMessenger
    Private _ledState As Boolean

    ' Setup function
    Public Sub Setup()
        _ledState = False

        ' Create Serial Port object
        _serialTransport = New SerialTransport()
        _serialTransport.CurrentSerialSettings.PortName = "COM6" ' Set com port
        _serialTransport.CurrentSerialSettings.BaudRate = 115200 ' Set baud rate
        _serialTransport.CurrentSerialSettings.DtrEnable = False ' For some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.

        ' Initialize the command messenger with the Serial Port transport layer
        _cmdMessenger = New CmdMessenger(_serialTransport, BoardType.Bit16)


        ' Attach the callbacks to the Command Messenger
        AttachCommandCallBacks()

        ' Start listening
        _cmdMessenger.Connect()
    End Sub

    ' Loop function
    Public Sub [Loop]()
        ' Create command
        Dim command = New SendCommand(CommandIDs.SetLed, _ledState)

        ' Send command
        _cmdMessenger.SendCommand(command)

        Console.Write("Turning led ")
        Console.WriteLine(If(_ledState, "on", "off"))

        ' Wait for 1 second and repeat
        Thread.Sleep(1000)
        _ledState = Not _ledState ' Toggle led state
    End Sub

    ' Exit function
    Public Sub [Exit]()
        ' We will never exit the application
    End Sub

    ''' Attach command call backs. 
    Private Sub AttachCommandCallBacks()
        ' No callbacks are currently needed
    End Sub
End Class