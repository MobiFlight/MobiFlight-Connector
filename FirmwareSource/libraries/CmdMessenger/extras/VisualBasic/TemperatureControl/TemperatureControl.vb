' *** TemperatureControl ***

' This example example is where all previously described features come together in one full application
' and wait for a response from the Arduino. The Arduino will start sending temperature data and the heater steering
' value data which the PC will plot in a chart. With a slider we can set the goal temperature, which will make the
' PID software on the controller adjust the setting of the heater.
' 
' This example shows how to design a responsive performance UI that sends and receives commands
' - Send queued commands
' - Manipulate the send and receive queue
' - Add queue strategies
' - Use serial or bluetooth connection
' - Use auto scanning and connecting
' - Use the autoconnect and watchdog 

Imports System
Imports System.Threading
Imports CommandMessenger
Imports CommandMessenger.Queue
Imports CommandMessenger.Transport
Imports CommandMessenger.Transport.Serial
Imports CommandMessenger.Transport.Bluetooth

Friend Enum CommandIds
    Identify           ' Command to identify device
    Acknowledge        ' Command to acknowledge a received command
    ErrorEncountered   ' Command to message that an error has occurred
    StartLogging       ' Command to turn on data logging
    StopLogging        ' Command to turn off data logging
    PlotDataPoint      ' Command to request plotting a data point
    SetGoalTemperature ' Command to set the goal temperature
    SetStartTime       ' Command to set the new start time for the logger
End Enum

Friend Enum TransportMode
    Serial ' Serial port connection (over USB)
    Bluetooth ' Bluetooth connection
End Enum

Public Class TemperatureControl
    Private Const UniqueDeviceId As String = "77FAEDD5-FAC8-46BD-875E-5E9B6D44F85C"

    ' This class (kind of) contains presentation logic, and domain model.
    ' ChartForm.cs contains the view components 
    Private _transport As ITransport
    Private _cmdMessenger As CmdMessenger
    Private _connectionManager As ConnectionManager
    Private _chartForm As ChartForm
    Private _goalTemperature As Double

    ' ------------------ MAIN  ----------------------

    Public Property AcquisitionStarted As Boolean
    Public Property AcceptData As Boolean

    ''' <summary> Gets or sets the goal temperature. </summary>
    ''' <value> The goal temperature. </value>
    Public Property GoalTemperature() As Double
        Get
            Return _goalTemperature
        End Get
        Set(ByVal value As Double)
            If Math.Abs(_goalTemperature - value) > Single.Epsilon Then
                _goalTemperature = value
                SetGoalTemperature(_goalTemperature)
                RaiseEvent GoalTemperatureChanged()
            End If
        End Set
    End Property

    Public Event GoalTemperatureChanged As Action ' Action that is called when the goal temperature has changed
    Private _startTime As Long


    ' Setup function
    Public Sub Setup(ByVal chartForm As ChartForm)
        ' Choose which transport mode you want to use:
        ' 1. Serial port. This can be a real serial port but is usually a virtual serial port over USB. 
        '                 It can also be a virtual serial port over Bluetooth, but the direct bluetooth works better
        ' 2. Bluetooth    This bypasses the Bluetooth virtual serial port, and instead communicates over the RFCOMM layer       

        Dim transportMode As TransportMode
        transportMode = transportMode.Serial
        'transportMode = transportMode.Bluetooth

        ' getting the chart control on top of the chart form.
        _chartForm = chartForm

        ' Set up chart
        _chartForm.SetupChart()

        ' Connect slider to GoalTemperatureChanged
        AddHandler GoalTemperatureChanged, Sub() _chartForm.GoalTemperatureTrackBarScroll(Nothing, Nothing)

        ' Set up transport 
        If transportMode = transportMode.Bluetooth Then
            _transport = New BluetoothTransport()
            ' We do not need to set the device: it will be found by the connection manager
        Else
            _transport = New SerialTransport With {
                .CurrentSerialSettings = New SerialSettings With {
                    .DtrEnable = False
                    }
                }

        End If
        ' We do not need to set serial port and baud rate: it will be found by the connection manager                                                           

        ' Initialize the command messenger with one of the two transport layers
        _cmdMessenger = New CmdMessenger(_transport, BoardType.Bit16) With {.PrintLfCr = False}

        ' Tell CmdMessenger to "Invoke" commands on the thread running the WinForms UI
        _cmdMessenger.ControlToInvokeOn = chartForm

        ' Set command strategy to continuously to remove all commands on the receive queue that 
        ' are older than 1 sec. This makes sure that if data logging comes in faster that it can 
        ' be plotted, the graph will not start lagging
        _cmdMessenger.AddReceiveCommandStrategy(New StaleGeneralStrategy(1000))

        ' Attach the callbacks to the Command Messenger
        AttachCommandCallBacks()

        ' Attach to NewLinesReceived for logging purposes
        AddHandler _cmdMessenger.NewLineReceived, AddressOf NewLineReceived

        ' Attach to NewLineSent for logging purposes
        AddHandler _cmdMessenger.NewLineSent, AddressOf NewLineSent

        ' Set up connection manager, corresponding to the transportMode
        If transportMode = transportMode.Bluetooth Then
            _connectionManager = New BluetoothConnectionManager((TryCast(_transport, BluetoothTransport)), _cmdMessenger, CommandIds.Identify, UniqueDeviceId)
        Else
            _connectionManager = New SerialConnectionManager((TryCast(_transport, SerialTransport)), _cmdMessenger, CommandIds.Identify, UniqueDeviceId)
        End If

        ' Enable watchdog functionality.
        _connectionManager.WatchdogEnabled = True

        ' Event when the connection manager finds a connection
        AddHandler _connectionManager.ConnectionFound, AddressOf ConnectionFound

        ' Event when the connection manager watchdog notices that the connection is gone
        AddHandler _connectionManager.ConnectionTimeout, AddressOf ConnectionTimeout

        ' Event notifying on scanning process
        AddHandler _connectionManager.Progress, AddressOf LogProgress

        ' Initialize the application
        InitializeTemperatureControl()

        ' Start scanning for ports/devices
        _connectionManager.StartConnectionManager()
    End Sub

    Private Sub InitializeTemperatureControl()
        _startTime = TimeUtils.Millis

        ' Set initial goal temperature
        GoalTemperature = 25
        AcquisitionStarted = False
        AcceptData = False
        _chartForm.SetDisConnected()
    End Sub

    ' Exit function
    Public Sub [Exit]()
        ' Disconnect ConnectionManager
        RemoveHandler _connectionManager.Progress, AddressOf LogProgress
        RemoveHandler _connectionManager.ConnectionTimeout, AddressOf ConnectionTimeout
        RemoveHandler _connectionManager.ConnectionFound, AddressOf ConnectionFound

        ' Dispose ConnectionManager
        _connectionManager.Dispose()

        ' Disconnect Command Messenger
        _cmdMessenger.Disconnect()

        ' Dispose Command Messenger
        _cmdMessenger.Dispose()

        ' Dispose transport layer
        _transport.Dispose()
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
    Private Sub OnUnknownCommand(ByVal arguments As ReceivedCommand)
        _chartForm.LogMessage("Command without attached callback received")
    End Sub

    ' Callback function that prints that the Arduino has acknowledged
    Private Sub OnAcknowledge(ByVal arguments As ReceivedCommand)
        _chartForm.LogMessage("Arduino acknowledged")
    End Sub

    ' Callback function that prints that the Arduino has experienced an error
    Private Sub OnError(ByVal arguments As ReceivedCommand)
        _chartForm.LogMessage("Arduino has experienced an error")
    End Sub

    ' Callback function that plots a data point for the current temperature, the goal temperature,
    ' the heater steer value and the Pulse Width Modulated (PWM) value.
    Private Sub OnPlotDataPoint(ByVal arguments As ReceivedCommand)
        ' Plot data if we are accepting data
        If (Not AcceptData) Then
            Return
        End If

        ' Get all arguments from plot data point command
        Dim time = arguments.ReadBinFloatArg()
        time = (TimeUtils.Millis - _startTime) / 1000.0F
        Dim currTemp = arguments.ReadBinFloatArg()
        Dim goalTemp = arguments.ReadBinFloatArg()
        Dim heaterValue = arguments.ReadBinFloatArg()
        Dim heaterPwm = arguments.ReadBinBoolArg()

        ' Update chart with new data point;
        _chartForm.UpdateGraph(time, currTemp, goalTemp, heaterValue, heaterPwm)
    End Sub

    ' Log received line to console
    Private Sub NewLineReceived(ByVal sender As Object, ByVal e As CommandEventArgs)
        _chartForm.LogMessage("Received > " & e.Command.CommandString())
    End Sub

    ' Log sent line to console
    Private Sub NewLineSent(ByVal sender As Object, ByVal e As CommandEventArgs)
        _chartForm.LogMessage("Sent > " & e.Command.CommandString())
    End Sub

    ' Log connection manager progress to status bar
    Private Sub LogProgress(ByVal sender As Object, ByVal e As ConnectionManagerProgressEventArgs)
        If e.Level <= 2 Then
            _chartForm.SetStatus(e.Description)
        End If
        _chartForm.LogMessage(e.Description)
    End Sub

    Private Sub ConnectionTimeout(ByVal sender As Object, ByVal e As EventArgs)
        ' Connection time-out!
        ' Disable UI ..                 
        _chartForm.SetStatus("Connection timeout, attempting to reconnect")
        _chartForm.SetDisConnected()
    End Sub

    Private Sub ConnectionFound(ByVal sender As Object, ByVal e As EventArgs)
        'We have been connected! 

        ' Make sure we do not receive data until we are ready
        AcceptData = False

        ' Enable UI
        _chartForm.SetConnected()

        ' Send command to set goal Temperature
        SetGoalTemperature(_goalTemperature)

        ' Restart acquisition if needed 
        If AcquisitionStarted Then
            StartAcquisition()
        Else
            StopAcquisition()
        End If
        AcceptData = True

        ' Yield time slice in order to get UI updated
        Thread.Yield()
    End Sub

    ' Set the goal temperature on the embedded controller
    Public Sub SetGoalTemperature(ByVal goalTemp As Double)
        _goalTemperature = goalTemp

        ' Create command to start sending data
        Dim command = New SendCommand(CommandIds.SetGoalTemperature)
        command.AddBinArgument(_goalTemperature)

        ' Collapse this command if needed using CollapseCommandStrategy
        ' This strategy will avoid duplicates of this command on the queue: if a SetGoalTemperature command is
        ' already on the queue when a new one is added, it will be replaced at its current queue-position. 
        ' Otherwise the command will be added to the back of the queue. 
        ' 
        ' This will make sure that when the slider raises a lot of events that each set a new goal temperature, the 
        ' controller will not start lagging.
        _chartForm.LogMessage("Queue command - SetGoalTemperature")
        _cmdMessenger.QueueCommand(New CollapseCommandStrategy(command))
    End Sub

    ' Set the start time on the embedded controller
    Public Sub SetStartTime(ByVal startTime As Single)
        Dim command = New SendCommand(CommandIds.SetStartTime, CommandIds.Acknowledge, 500)
        command.AddBinArgument(CSng(startTime))

        ' We place this command at the front of the queue in order to receive correctly timestamped data as soon as possible
        ' Meanwhile, the data in the receivedQueue is cleared as these will contain the wrong timestamp
        _cmdMessenger.SendCommand(command, SendQueue.ClearQueue, ReceiveQueue.ClearQueue, UseQueue.BypassQueue)
    End Sub

    ' Signal the embedded controller to start sending temperature data.
    Public Function StartAcquisition() As Boolean
        ' Send command to start sending data
        Dim command = New SendCommand(CommandIds.StartLogging, CommandIds.Acknowledge, 500)

        ' Wait for an acknowledgment that data is being sent. Clear both the receive queue until the acknowledgment is received
        _chartForm.LogMessage("Send command - Start acquisition")
        Dim receivedCommand = _cmdMessenger.SendCommand(command, SendQueue.ClearQueue, ReceiveQueue.ClearQueue)
        If receivedCommand.Ok Then
            AcquisitionStarted = True
        Else
            _chartForm.LogMessage(" Failure > no OK received from controller")
        End If
        Return receivedCommand.Ok
    End Function

    ' Signal the embedded controller to stop sending temperature data.
    Public Function StopAcquisition() As Boolean
        ' Send command to stop sending data
        Dim command = New SendCommand(CommandIds.StopLogging, CommandIds.Acknowledge, 2500)

        ' Wait for an acknowledgment that data is being sent. Clear both the send and receive queue until the acknowledgment is received
        _chartForm.LogMessage("Send command - Stop acquisition")
        Dim receivedCommand = _cmdMessenger.SendCommand(command, SendQueue.ClearQueue, ReceiveQueue.ClearQueue)
        If receivedCommand.Ok Then
            AcquisitionStarted = False
        Else
            _chartForm.LogMessage(" Failure > no OK received from controller")
        End If
        Return receivedCommand.Ok
    End Function
End Class