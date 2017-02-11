' *** DataLogging ***

' This example expands the previous SendandReceiveArguments example. The PC will now send a start command to the Arduino,
' and wait for a response from the Arduino. The Arduino will start sending analog data which the PC will plot in a chart
' This example shows how to :
' - use in combination with WinForms
' - use in combination with ZedGraph
Imports System
Imports CommandMessenger
Imports CommandMessenger.Queue
Imports CommandMessenger.Transport.Serial


Enum CommandIds
    Acknowledge
    ErrorEncountered
    StartLogging
    PlotDataPoint
End Enum

Public Class DataLogging
    ' This class (kind of) contains presentation logic, and domain model.
    ' ChartForm.cs contains the view components 
    Private _serialTransport As SerialTransport
    Private _cmdMessenger As CmdMessenger
    Private _chartForm As ChartForm

    ' ------------------ MAIN  ----------------------

    ' Setup function
    Public Sub Setup(chartForm As ChartForm)

        ' getting the chart control on top of the chart form.
        _chartForm = chartForm

        ' Set up chart
        _chartForm.SetupChart()

        ' Create Serial Port object
        ' Note that for some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
        ' object initializer
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
        _cmdMessenger.ControlToInvokeOn = chartForm

        ' Set Received command strategy that removes commands that are older than 1 sec
        _cmdMessenger.AddReceiveCommandStrategy(New StaleGeneralStrategy(1000))

        ' Attach the callbacks to the Command Messenger
        AttachCommandCallBacks()

        ' Attach to NewLinesReceived for logging purposes
        AddHandler _cmdMessenger.NewLineReceived, AddressOf NewLineReceived

        ' Attach to NewLineSent for logging purposes
        AddHandler _cmdMessenger.NewLineSent, AddressOf NewLineSent

        ' Start listening
        _cmdMessenger.Connect()

        ' Send command to start sending data
        Dim commandStartLogging = New SendCommand(CommandIds.StartLogging)

        ' Send command
        _cmdMessenger.SendCommand(commandStartLogging)
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
        _cmdMessenger.Attach(CommandIds.PlotDataPoint, AddressOf OnPlotDataPoint)
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

    ' Callback function that plots a data point for ADC 1 and ADC 2
    Private Sub OnPlotDataPoint(arguments As ReceivedCommand)
        Dim time = arguments.ReadFloatArg()
        Dim analog1 = arguments.ReadFloatArg()
        Dim analog2 = arguments.ReadFloatArg()
        _chartForm.UpdateGraph(time, analog1, analog2)
    End Sub

    ' Log received line to console
    Private Sub NewLineReceived(sender As Object, e As CommandEventArgs)
        Console.WriteLine("Received > " + e.Command.CommandString())
    End Sub

    ' Log sent line to console
    Private Sub NewLineSent(sender As Object, e As CommandEventArgs)
        Console.WriteLine("Sent > " + e.Command.CommandString())
    End Sub

End Class