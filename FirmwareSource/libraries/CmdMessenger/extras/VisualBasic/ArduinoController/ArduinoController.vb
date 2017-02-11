' *** ArduinoController ***

' This example expands the SendandReceiveArguments example. The PC will now sends commands to the Arduino when the trackbar 
' is pulled. Every TrackBarChanged events will queue a message to the Arduino to set the blink speed of the 
' internal / pin 13 LED
' 
' This example shows how to :
' - use in combination with WinForms
' - use in combination with ZedGraph
' - send queued commands
' - Use the CollapseCommandStrategy
Imports System
Imports CommandMessenger
Imports CommandMessenger.Queue
Imports CommandMessenger.Transport.Serial

Enum CommandIds
    Acknowledge       ' Command to acknowledge a received command
    ErrorEncountered  ' Command to message that an error has occurred
    SetLed            ' Command to turn led ON or OFF
    SetLedFrequency   ' Command to set led blink frequency
End Enum

Public Class ArduinoController
    ' This class (kind of) contains presentation logic, and domain model.
    ' ChartForm.cs contains the view components 

    Private _serialTransport As SerialTransport
    Private _cmdMessenger As CmdMessenger
    Private _controllerForm As ControllerForm

    ' ------------------ MAIN  ----------------------

    ' Setup function
    Public Sub Setup(controllerForm As ControllerForm)
        ' storing the controller form for later reference
        _controllerForm = controllerForm

        ' Create Serial Port object
        ' Note that for some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
        ' object initializer
        ' Create Serial Port transport object
        _serialTransport = New SerialTransport With {
            .CurrentSerialSettings = New SerialSettings With {
                .PortName = "COM6",
                .BaudRate = 115200,
                .DtrEnable = False
                }
            }

        ' Initialize the command messenger with the Serial Port transport layer
        ' Set if it is communicating with a 16- or 32-bit Arduino board
        _cmdMessenger = New CmdMessenger(_serialTransport, BoardType.Bit16)

        ' Tell CmdMessenger to "Invoke" commands on the thread running the WinForms UI
        _cmdMessenger.ControlToInvokeOn = controllerForm


        ' Attach the callbacks to the Command Messenger
        AttachCommandCallBacks()

        ' Attach to NewLinesReceived for logging purposes
        AddHandler _cmdMessenger.NewLineReceived, AddressOf NewLineReceived

        ' Attach to NewLineSent for logging purposes
        AddHandler _cmdMessenger.NewLineSent, AddressOf NewLineSent

        ' Start listening
        _cmdMessenger.Connect()

        _controllerForm.SetLedState(True)
        _controllerForm.SetFrequency(2)
    End Sub

    ' Exit function
    Public Sub [Exit]()
        ' Stop listening
        _cmdMessenger.Disconnect()

        ' Dispose Command Messenger
        _cmdMessenger.Dispose()

        ' Dispose Serial Port object
        _serialTransport.Dispose()
    End Sub

    ''' Attach command call backs. 
    Private Sub AttachCommandCallBacks()
        _cmdMessenger.Attach(AddressOf OnUnknownCommand)
        _cmdMessenger.Attach(CommandIds.Acknowledge, AddressOf OnAcknowledge)
        _cmdMessenger.Attach(CommandIds.ErrorEncountered, AddressOf OnError)
    End Sub

    ' ------------------  CALLBACKS ---------------------

    ' Called when a received command has no attached function.
    ' In a WinForm application, console output gets routed to the output panel of your IDE
    Private Sub OnUnknownCommand(arguments As ReceivedCommand)
        Console.WriteLine("Command without attached callback received")
    End Sub

    ' Callback function that prints that the Arduino has acknowledged
    Private Sub OnAcknowledge(arguments As ReceivedCommand)
        Console.WriteLine(" Arduino is ready")
    End Sub

    ' Callback function that prints that the Arduino has experienced an error
    Private Sub OnError(arguments As ReceivedCommand)
        Console.WriteLine("Arduino has experienced an error")
    End Sub

    ' Log received line to console
    Private Sub NewLineReceived(sender As Object, e As CommandEventArgs)
        Console.WriteLine("Received > " + e.Command.CommandString())
    End Sub

    ' Log sent line to console
    Private Sub NewLineSent(sender As Object, e As CommandEventArgs)
        Console.WriteLine("Sent > " + e.Command.CommandString())
    End Sub

    ' Sent command to change led blinking frequency
    Public Sub SetLedFrequency(ledFrequency As Double)
        ' Create command to start sending data
        Dim sendCommand = New SendCommand(CInt(CommandIds.SetLedFrequency), ledFrequency)

        ' Put the command on the queue and wrap it in a collapse command strategy
        ' This strategy will avoid duplicates of this certain command on the queue: if a SetLedFrequency command is
        ' already on the queue when a new one is added, it will be replaced at its current queue-position. 
        ' Otherwise the command will be added to the back of the queue. 
        ' 
        ' This will make sure that when the slider raises a lot of events that each send a new blink frequency, the 
        ' embedded controller will not start lagging.
        _cmdMessenger.QueueCommand(New CollapseCommandStrategy(sendCommand))
    End Sub


    ' Sent command to change led on/of state
    Public Sub SetLedState(ledState As Boolean)
        ' Create command to start sending data
        Dim sendCommand = New SendCommand(CommandIds.SetLed, ledState)

        ' Send command
        _cmdMessenger.SendCommand(New SendCommand(CommandIds.SetLed, ledState))
    End Sub
End Class