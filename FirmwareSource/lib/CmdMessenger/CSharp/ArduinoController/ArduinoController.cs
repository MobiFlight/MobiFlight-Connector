// *** ArduinoController ***

// This example expands the SendandReceiveArguments example. The PC will now sends commands to the Arduino when the trackbar 
// is pulled. Every TrackBarChanged events will queue a message to the Arduino to set the blink speed of the 
// internal / pin 13 LED
// 
// This example shows how to :
// - use in combination with WinForms
// - use in combination with ZedGraph
// - send queued commands
// - Use the CollapseCommandStrategy

using System;
using CommandMessenger;
using CommandMessenger.TransportLayer;

namespace ArduinoController
{
    enum Command
    {
        Acknowledge,            // Command to acknowledge a received command
        Error,                  // Command to message that an error has occurred
        SetLed,                 // Command to turn led ON or OFF
        SetLedFrequency,        // Command to set led blink frequency
    };

    public class ArduinoController
    {
        // This class (kind of) contains presentation logic, and domain model.
        // ChartForm.cs contains the view components 

        private SerialTransport   _serialTransport;
        private CmdMessenger      _cmdMessenger;
        private ControllerForm    _controllerForm;

        // ------------------ MAIN  ----------------------

        // Setup function
        public void Setup(ControllerForm controllerForm)
        {
            // storing the controller form for later reference
            _controllerForm = controllerForm;
            
            // Create Serial Port object
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = "COM6", BaudRate = 115200 } // object initializer
            };

            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(_serialTransport);

            // Tell CmdMessenger to "Invoke" commands on the thread running the WinForms UI
            _cmdMessenger.SetControlToInvokeOn(_controllerForm);

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();

            // Attach to NewLinesReceived for logging purposes
            _cmdMessenger.NewLineReceived += NewLineReceived;

            // Attach to NewLineSent for logging purposes
            _cmdMessenger.NewLineSent += NewLineSent;                       

            // Start listening
            _cmdMessenger.StartListening();

            _controllerForm.SetLedState(true);
            _controllerForm.SetFrequency(2);
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

        // Sent command to change led blinking frequency
        public void SetLedFrequency(double ledFrequency)
        {
            // Create command to start sending data
            var command = new SendCommand((int)Command.SetLedFrequency,ledFrequency);

            // Put the command on the queue and wrap it in a collapse command strategy
            // This strategy will avoid duplicates of this certain command on the queue: if a SetLedFrequency command is
            // already on the queue when a new one is added, it will be replaced at its current queue-position. 
            // Otherwise the command will be added to the back of the queue. 
            // 
            // This will make sure that when the slider raises a lot of events that each send a new blink frequency, the 
            // embedded controller will not start lagging.
            _cmdMessenger.QueueCommand(new CollapseCommandStrategy(command));
        }


        // Sent command to change led on/of state
        public void SetLedState(bool ledState)
        {
            // Create command to start sending data
            var command = new SendCommand((int)Command.SetLed, ledState);

            // Send command
            _cmdMessenger.SendCommand(new SendCommand((int)Command.SetLed, ledState));         
        }
    }
}
