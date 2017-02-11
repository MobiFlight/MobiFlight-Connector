// *** DataLogging ***

// This example expands the previous SendandReceiveArguments example. The PC will now send a start command to the Arduino,
// and wait for a response from the Arduino. The Arduino will start sending analog data which the PC will plot in a chart
// This example shows how to :
// - use in combination with WinForms
// - use in combination with ZedGraph

using System;
using CommandMessenger;
using CommandMessenger.Queue;
using CommandMessenger.Transport.Serial;

using System.Threading;
namespace DataLogging
{
    enum Command
    {
        Acknowledge,
        Error,
        StartLogging,
        PlotDataPoint,
    };

    public class DataLogging
    {
        // This class (kind of) contains presentation logic, and domain model.
        // ChartForm.cs contains the view components 
        private SerialTransport   _serialTransport;
        private CmdMessenger      _cmdMessenger;
        private ChartForm         _chartForm;
        
        // ------------------ MAIN  ----------------------

        // Setup function
        public void Setup(ChartForm chartForm)
        {
           
            // getting the chart control on top of the chart form.
            _chartForm = chartForm;
            
            // Set up chart
            _chartForm.SetupChart();

            // Create Serial Port object
            // Note that for some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = "COM6", BaudRate = 115200, DtrEnable = false } // object initializer
            };

            // Initialize the command messenger with the Serial Port transport layer
            // Set if it is communicating with a 16- or 32-bit Arduino board
            _cmdMessenger = new CmdMessenger(_serialTransport, BoardType.Bit16);

            // Tell CmdMessenger to "Invoke" commands on the thread running the WinForms UI
            _cmdMessenger.ControlToInvokeOn = chartForm;

            // Set Received command strategy that removes commands that are older than 1 sec
            _cmdMessenger.AddReceiveCommandStrategy(new StaleGeneralStrategy(1000));

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();

            // Attach to NewLinesReceived for logging purposes
            _cmdMessenger.NewLineReceived += NewLineReceived;

            // Attach to NewLineSent for logging purposes
            _cmdMessenger.NewLineSent += NewLineSent;                       

            // Start listening
            _cmdMessenger.Connect();

            // Send command to start sending data
            var command = new SendCommand((int)Command.StartLogging);

            // Send command
            _cmdMessenger.SendCommand(command);
        }


        // Exit function
        public void Exit()
        {
            // Stop listening
            _cmdMessenger.Disconnect();
           
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

        // Callback function that plots a data point for ADC 1 and ADC 2
        private void OnPlotDataPoint(ReceivedCommand arguments)
        {
            var time    = arguments.ReadFloatArg();
            var analog1 = arguments.ReadFloatArg();
            var analog2 = arguments.ReadFloatArg();
            _chartForm.UpdateGraph(time, analog1, analog2);
        }

        // Log received line to console
        private void NewLineReceived(object sender, CommandEventArgs e)
        {
            Console.WriteLine(@"Received > " + e.Command.CommandString());
        }

        // Log sent line to console
        private void NewLineSent(object sender, CommandEventArgs e)
        {
            Console.WriteLine(@"Sent > " + e.Command.CommandString());
        }




    }
}
