// *** TemperatureControl ***

// This example expands the previous ArduinoController example. The PC will now send a start command to the Arduino,
// and wait for a response from the Arduino. The Arduino will start sending temperature data and the heater steering
// value data which the PC will plot in a chart. With a slider we can set the goal temperature, which will make the
// PID software on the controller adjust the setting of the heater.
// 
// This example shows how to design a responsive performance UI that sends and receives commands
// - Send queued commands
// - Manipulate the send and receive queue
// - Add queue strategies
// - Use bluetooth connection
// - Use auto scanning and connecting
// - Use watchdog 

using System;
using CommandMessenger;
using CommandMessenger.Serialport;
using CommandMessenger.TransportLayer;
using System.Threading;
using CommandMessenger.Bluetooth;
namespace DataLogging
{
    enum Command
    {
        RequestId,          // Command to request application ID
        SendId,             // Command to send application ID
        Acknowledge,        // Command to acknowledge a received command
        Error,              // Command to message that an error has occurred
        StartLogging,       // Command to turn on data logging
        StopLogging,        // Command to turn off data logging
        PlotDataPoint,      // Command to request plotting a data point
        SetGoalTemperature, // Command to set the goal temperature       
        SetStartTime,       // Command to set the new start time for the logger       
    };

    enum TransportMode
    {
        Serial,             // Serial port connection (over USB)
        Bluetooth,          // Bluetooth connection 
    };

    public class TemperatureControl
    {
        // This class (kind of) contains presentation logic, and domain model.
        // ChartForm.cs contains the view components 
        private ITransport            _transport;
        private CmdMessenger          _cmdMessenger;
        private ConnectionManager    _connectionManager;
        private ChartForm             _chartForm;
        //private float                 _startTime;
        private double                _goalTemperature;
        
        // ------------------ MAIN  ----------------------


        public bool AcquisitionStarted { get; set; }
        public bool AcceptData { get; set; }

        /// <summary> Gets or sets the goal temperature. </summary>
        /// <value> The goal temperature. </value>
        public double GoalTemperature
        {
            get { return _goalTemperature; }
            set
            {
                if (Math.Abs(_goalTemperature - value) > float.Epsilon)
                {
                    _goalTemperature = value;
                    SetGoalTemperature(_goalTemperature);
                    if (GoalTemperatureChanged!=null) GoalTemperatureChanged();
                }
            }
        }

        public Action GoalTemperatureChanged;   // Action that is called when the goal temperature has changed
        private long _startTime;
        

        // Setup function
        public void Setup(ChartForm chartForm)
        {
            // Choose which transport mode you want to use:
            // 1. Serial port. This can be a real serial port but is usually a virtual serial port over USB. 
            //                 It can also be a virtual serial port over Bluetooth, but the direct bluetooth works better
            // 2. Bluetooth    This bypasses the Bluetooth virtual serial port, but communicates over the RFCOMM layer                 
            var transportMode = TransportMode.Serial;
            
            // getting the chart control on top of the chart form.
            _chartForm = chartForm;
            
            // Set up chart
            _chartForm.SetupChart();

            // Connect slider to GoalTemperatureChanged
            GoalTemperatureChanged += () => _chartForm.GoalTemperatureTrackBarScroll(null, null);

            // Set up transport 
            if (transportMode == TransportMode.Bluetooth)
                _transport = new BluetoothTransport();
                    // We do not need to set the device: it will be found by the connection manager
            else
                _transport = new SerialTransport { CurrentSerialSettings = { DtrEnable = false } }; // some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.                        
                    // We do not need to set serial port and baud rate: it will be found by the connection manager                                                           

            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(_transport)
            {
                BoardType = BoardType.Bit16, // Set if it is communicating with a 16- or 32-bit Arduino board
                PrintLfCr = false            // Do not print newLine at end of command, to reduce data being sent
            };

            // Tell CmdMessenger to "Invoke" commands on the thread running the WinForms UI
            _cmdMessenger.SetControlToInvokeOn(chartForm);

            // Set command strategy to continuously to remove all commands on the receive queue that 
            // are older than 1 sec. This makes sure that if data logging comes in faster that it can 
            // be plotted, the graph will not start lagging
            _cmdMessenger.AddReceiveCommandStrategy(new StaleGeneralStrategy(1000));            

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();

            // Attach to NewLinesReceived for logging purposes
            _cmdMessenger.NewLineReceived += NewLineReceived;

            // Attach to NewLineSent for logging purposes
            _cmdMessenger.NewLineSent     += NewLineSent;                       

            // Set up connection manager 
            if (transportMode == TransportMode.Bluetooth)
                _connectionManager = new BluetoothConnectionManager((_transport as BluetoothTransport), _cmdMessenger, (int)Command.RequestId, (int)Command.SendId);
            else
                _connectionManager = new SerialConnectionManager   ((_transport as SerialTransport),    _cmdMessenger, (int)Command.RequestId, (int)Command.SendId);                    
            
            // Tell the Connection manager to "Invoke" commands on the thread running the WinForms UI
            _connectionManager.SetControlToInvokeOn(chartForm);

            // Event when the connection manager finds a connection
            _connectionManager.ConnectionFound += ConnectionFound;

            // Event when the connection manager watchdog notices that the connection is gone
            _connectionManager.ConnectionTimeout += ConnectionTimeout;
            
            // Event notifying on scanning process
            _connectionManager.Progress += LogProgress;

            // Initialize the application
            InitializeTemperatureControl(); 

            // Start scanning for ports/devices
            _connectionManager.StartScan();           
        }

        private void InitializeTemperatureControl()
        {
            _startTime = TimeUtils.Millis;

            // Set initial goal temperature
            GoalTemperature    = 25;
           // _startTime         = 0.0f;
            AcquisitionStarted = false;
            AcceptData         = false;
            _chartForm.SetDisConnected();
        }

        // Exit function
        public void Exit()
        {
            // Disconnect ConnectionManager
            _connectionManager.Progress          -= LogProgress;
            _connectionManager.ConnectionTimeout -= ConnectionTimeout;
            _connectionManager.ConnectionFound   -= ConnectionFound;

            // Dispose ConnectionManager
            _connectionManager.Dispose();

            // Disconnect Command Messenger
            _cmdMessenger.Disconnect();           

            // Dispose Command Messenger
            _cmdMessenger.Dispose();

            // Dispose transport layer
            _transport.Dispose();
        }

        /// Attach command call backs. 
        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);
            _cmdMessenger.Attach((int)Command.Acknowledge, OnAcknowledge);
            _cmdMessenger.Attach((int)Command.Error, OnError);
            _cmdMessenger.Attach((int)Command.PlotDataPoint, OnPlotDataPoint);
        }

        // ------------------  CALLBACKS ---------------------

        // Called when a received command has no attached function.
        // In a WinForm application, console output gets routed to the output panel of your IDE
        void OnUnknownCommand(ReceivedCommand arguments)
        {
            _chartForm.LogMessage(@"Command without attached callback received");
            //Console.WriteLine(@"Command without attached callback received");
        }

        // Callback function that prints that the Arduino has acknowledged
        void OnAcknowledge(ReceivedCommand arguments)
        {
            _chartForm.LogMessage(@"Arduino acknowledged");
            //Console.WriteLine(@" Arduino is ready");
        }

        // Callback function that prints that the Arduino has experienced an error
        void OnError(ReceivedCommand arguments)
        {
            _chartForm.LogMessage(@"Arduino has experienced an error");
            //Console.WriteLine(@"Arduino has experienced an error");
        }

        // Callback function that plots a data point for the current temperature, the goal temperature,
        // the heater steer value and the Pulse Width Modulated (PWM) value.
        private void OnPlotDataPoint(ReceivedCommand arguments)
        {   
            // Plot data if we are accepting data
            if (!AcceptData) return;

            // Get all arguments from plot data point command
            var time        = arguments.ReadBinFloatArg();
            time        = (TimeUtils.Millis-_startTime)/1000.0f;
            var currTemp    = arguments.ReadBinFloatArg();
            var goalTemp    = arguments.ReadBinFloatArg();
            var heaterValue = arguments.ReadBinFloatArg();
            var heaterPwm   = arguments.ReadBinBoolArg();

            // do not log data if times are out of sync
            //if (time<_startTime) return;

            // Update chart with new data point;
            _chartForm.UpdateGraph(time, currTemp, goalTemp, heaterValue, heaterPwm);

            // Update _startTime in case it needs to be resend after disconnection
            //_startTime = time;
        }

        // Log received line to console
        private void NewLineReceived(object sender, NewLineEvent.NewLineArgs e)
        {
            _chartForm.LogMessage(@"Received > " + e.Command.CommandString());
          //  Console.WriteLine(@"Received > " + e.Command.CommandString());
        }

        // Log sent line to console
        private void NewLineSent(object sender, NewLineEvent.NewLineArgs e)
        {
            _chartForm.LogMessage(@"Sent > " + e.Command.CommandString());
           // Console.WriteLine(@"Sent > " + e.Command.CommandString());
        }

        // Log connection manager progress to status bar
        void LogProgress(object sender, ConnectionManagerProgressEventArgs e)
        {
            if (e.Level <= 2) { _chartForm.SetStatus(e.Description); }
            _chartForm.LogMessage(e.Description);
           // Console.WriteLine(e.Level + @" :" + e.Description);
        }

        private void ConnectionTimeout(object sender, EventArgs e)
        {           
            // Connection time-out!
            // Disable UI ..                 
            _chartForm.SetStatus(@"Connection timeout, attempting to reconnect");           
            _chartForm.SetDisConnected();
            // and start scanning
            _connectionManager.StartScan();
        }

        private void ConnectionFound(object sender, EventArgs e)
        {
            //We have been connected! 

            // Make sure we do not receive data until we are ready
            AcceptData = false;
            
            // Enable UI
            _chartForm.SetConnected();
            
            // Send command to set goal Temperature
            SetGoalTemperature(_goalTemperature);

            // Restart acquisition if needed 
            if (AcquisitionStarted) StartAcquisition(); else StopAcquisition();
            AcceptData = true;
            // Start Watchdog
            _connectionManager.StartWatchDog();

            // Yield time slice in order to get UI updated
            Thread.Yield();
        }

        // Set the goal temperature on the embedded controller
        public void SetGoalTemperature(double goalTemperature) 
        {
            _goalTemperature = goalTemperature;

            // Create command to start sending data
             var command = new SendCommand((int)Command.SetGoalTemperature);
             command.AddBinArgument(_goalTemperature);

            // Collapse this command if needed using CollapseCommandStrategy
            // This strategy will avoid duplicates of this command on the queue: if a SetGoalTemperature command is
            // already on the queue when a new one is added, it will be replaced at its current queue-position. 
            // Otherwise the command will be added to the back of the queue. 
            // 
            // This will make sure that when the slider raises a lot of events that each set a new goal temperature, the 
            // controller will not start lagging.
             _chartForm.LogMessage(@"Queue command - SetGoalTemperature");
            _cmdMessenger.QueueCommand(new CollapseCommandStrategy(command));
        }

        // Set the start time on the embedded controller
        public void SetStartTime(float startTime)
        {
            var command = new SendCommand((int)Command.SetStartTime, (int)Command.Acknowledge,500);
            command.AddBinArgument((float)startTime);

            // We place this command at the front of the queue in order to receive correctly timestamped data as soon as possible
            // Meanwhile, the data in the receivedQueue is cleared as these will contain the wrong timestamp
            _cmdMessenger.SendCommand(command,SendQueue.ClearQueue, ReceiveQueue.ClearQueue, UseQueue.BypassQueue);
        }

        // Signal the embedded controller to start sending temperature data.
        public bool StartAcquisition()
        {
            // Send command to start sending data
            var command = new SendCommand((int)Command.StartLogging,(int)Command.Acknowledge,500);

            // Wait for an acknowledgment that data is being sent. Clear both the receive queue until the acknowledgment is received
            _chartForm.LogMessage(@"Send command - Start acquisition");
            var receivedCommand = _cmdMessenger.SendCommand(command, SendQueue.ClearQueue, ReceiveQueue.ClearQueue);
            if (receivedCommand.Ok)
            {
                AcquisitionStarted = true;
            }
            else
                _chartForm.LogMessage(@" Failure > no OK received from controller");
            return receivedCommand.Ok;
        }

        // Signal the embedded controller to stop sending temperature data.
        public bool StopAcquisition()
        {
            // Send command to stop sending data
            var command = new SendCommand((int)Command.StopLogging, (int)Command.Acknowledge, 2500);

            // Wait for an acknowledgment that data is being sent. Clear both the send and receive queue until the acknowledgment is received
            _chartForm.LogMessage(@"Send command - Stop acquisition");
            var receivedCommand = _cmdMessenger.SendCommand(command, SendQueue.ClearQueue, ReceiveQueue.ClearQueue);
            if (receivedCommand.Ok)
            {
                AcquisitionStarted = false;
            }
            else
                _chartForm.LogMessage(@" Failure > no OK received from controller");
            return receivedCommand.Ok;
        }
    }
}
