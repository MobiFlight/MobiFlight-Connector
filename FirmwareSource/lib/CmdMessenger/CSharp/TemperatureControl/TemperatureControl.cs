// *** TemperatureControl ***

// This example expands the previous ArduinoController example. The PC will now send a start command to the Arduino,
// and wait for a response from the Arduino. The Arduino will start sending temperature data and the heater steering
// value data which the PC will plot in a chart. With a slider we can set the goal temperature, which will make the
// PID software on the controller adjust the setting of the heater.
// 
// This example shows how to design a responsive performance UI that sends and receives commands
// - send queued commands
// - add queue strategies

using System;
using CommandMessenger;
using CommandMessenger.TransportLayer;

using System.Threading;
namespace DataLogging
{
    enum Command
    {
        Acknowledge,        // Command to acknowledge a received command
        Error,              // Command to message that an error has occurred
        StartLogging,       // Command to turn on data logging
        StopLogging,        // Command to turn off data logging
        PlotDataPoint,      // Command to request plotting a data point
        SetGoalTemperature, // Command to set the goal temperature       
    };

    public class TemperatureControl
    {
        // This class (kind of) contains presentation logic, and domain model.
        // ChartForm.cs contains the view components 
        private SerialTransport   _serialTransport;
        private CmdMessenger      _cmdMessenger;
        private ChartForm         _chartForm;
        private double            _goalTemperature;
        // ------------------ MAIN  ----------------------

        /// <summary> Gets or sets the goal temperature. </summary>
        /// <value> The goal temperature. </value>
        public double GoalTemperature
        {
            get { return _goalTemperature; }
            set
            {
                if (_goalTemperature != value)
                {
                    _goalTemperature = value;
                    SetGoalTemperature(_goalTemperature);
                    if (GoalTemperatureChanged!=null) GoalTemperatureChanged();
                }
            }
        }

        public Action GoalTemperatureChanged;   // Action that is called when the goal temperature has changed

        // Setup function
        public void Setup(ChartForm chartForm)
        {           
            // getting the chart control on top of the chart form.
            _chartForm = chartForm;
            
            // Set up chart
            _chartForm.SetupChart();

            // Connect slider to GoalTemperatureChanged
            GoalTemperatureChanged += new Action(() => _chartForm.GoalTemperatureTrackBarScroll(null, null));

            // Create Serial Port object
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = "COM6", BaudRate = 115200 } // object initializer
            };

            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(_serialTransport);

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
            _cmdMessenger.NewLineSent += NewLineSent;                       

            // Start listening
            _cmdMessenger.StartListening();

            // Send command to start sending data
            var command = new SendCommand((int)Command.StartLogging);

            // Send command
            _cmdMessenger.SendCommand(command);

            // Wait for a little bit and clear the receive queue
            Thread.Sleep(250);
            _cmdMessenger.ClearReceiveQueue();

            // Set initial goal temperature
            GoalTemperature = 25;            
        }

        // Exit function
        public void Exit()
        {
            // Stop listening
            _cmdMessenger.StopListening();           

            // Dispose Command Messenger
            _cmdMessenger.Dispose();

            // Dispose Serial Port object
            _serialTransport.Dispose();
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
            Console.WriteLine(@"Command without attached callback received");
        }

        // Callback function that prints that the Arduino has acknowledged
        void OnAcknowledge(ReceivedCommand arguments)
        {
            Console.WriteLine(@" Arduino is ready");
        }

        // Callback function that prints that the Arduino has experienced an error
        void OnError(ReceivedCommand arguments)
        {
            Console.WriteLine(@"Arduino has experienced an error");
        }

        // Callback function that plots a data point for the current temperature, the goal temperature,
        // the heater steer value and the Pulse Width Modulated (PWM) value.
        private void OnPlotDataPoint(ReceivedCommand arguments)
        {             
            var time        = arguments.ReadFloatArg();
            var currTemp    = arguments.ReadFloatArg();
            var goalTemp    = arguments.ReadFloatArg();
            var heaterValue = arguments.ReadFloatArg();
            var heaterPwm   = arguments.ReadBoolArg();

            _chartForm.UpdateGraph(time, currTemp, goalTemp, heaterValue, heaterPwm);
        }

        // Log received line to console
        private void NewLineReceived(object sender, EventArgs e)
        {
            Console.WriteLine(@" Received > " + _cmdMessenger.CurrentReceivedLine);
        }

        // Log sent line to console
        private void NewLineSent(object sender, EventArgs e)
        {
            Console.WriteLine(@" Sent > " + _cmdMessenger.CurrentSentLine);
        }

        // Set the goal temperature on the embedded controller
        public void SetGoalTemperature(double goalTemperature) 
        {
            _goalTemperature = goalTemperature;

            // Create command to start sending data
            var command = new SendCommand((int)Command.SetGoalTemperature, _goalTemperature);

            // Collapse this command if needed using CollapseCommandStrategy
            // This strategy will avoid duplicates of this command on the queue: if a SetGoalTemperature command is
            // already on the queue when a new one is added, it will be replaced at its current queue-position. 
            // Otherwise the command will be added to the back of the queue. 
            // 
            // This will make sure that when the slider raises a lot of events that each set a new goal temperature, the 
            // controller will not start lagging.
            _cmdMessenger.QueueCommand(new CollapseCommandStrategy(command));
        }

        // Signal the embedded controller to start sending temperature data.
        public void StartAcquisition()
        {
            // Send command to start sending data
            var command = new SendCommand((int)Command.StartLogging,(int)Command.Acknowledge,5);

            // Wait for an acknowledgment that data is being sent. Clear both the receive queue until the acknowledgment is received
            var receivedCommand = _cmdMessenger.SendCommand(command, ClearQueue.ClearReceivedQueue);
            if (!receivedCommand.Ok)
            {
                Console.WriteLine(@" Failure > no OK received from controller");
            }
        }

        // Signal the embedded controller to stop sending temperature data.
        public void StopAcquisition()
        {
            // Send command to stop sending data
            var command = new SendCommand((int)Command.StopLogging, (int)Command.Acknowledge, 5);

            // Wait for an acknowledgment that data is being sent. Clear both the send and receive queue until the acknowledgment is received
            var receivedCommand = _cmdMessenger.SendCommand(command, ClearQueue.ClearSendAndReceivedQueue);
            if (!receivedCommand.Ok)
            {
                Console.WriteLine(@" Failure > no OK received from controller");
            }
        }
    }
}
