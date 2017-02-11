// *** SendandReceive ***

// This example expands the previous SendandReceive example. The PC will now send multiple float values
// and wait for a response from the Arduino. 
// It adds a demonstration of how to:
// - Send multiple parameters, and wait for response
// - Receive multiple parameters
// - Add logging events on data that has been sent or received

using System;
using CommandMessenger;
using CommandMessenger.TransportLayer;

namespace SendAndReceiveArguments
{
    // This is the list of recognized commands. These can be commands that can either be sent or received. 
    // In order to receive, attach a callback function to these events
    enum Command
    {
        Acknowledge,
        Error,
        FloatAddition,
        FloatAdditionResult, 
    };

    public class SendAndReceiveArguments
    {
        public bool RunLoop { get; set; }
        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;
        
        // ------------------ M A I N  ----------------------

        // Setup function
        public void Setup()
        {
            // Create Serial Port object
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = "COM3", BaudRate = 115200, DataTerminalReady=true } // object initializer
            };

            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(_serialTransport);

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();

            // Attach to NewLinesReceived for logging purposes
            _cmdMessenger.NewLineReceived += NewLineReceived;

            // Attach to NewLineSent for logging purposes
            _cmdMessenger.NewLineSent += NewLineSent;                       

            // Start listening
            _cmdMessenger.StartListening();
        }

        // Loop function
        public void Loop()
        {
            // Create command FloatAddition, which will wait for a return command FloatAdditionResult
            var command = new SendCommand((int)Command.FloatAddition, (int)Command.FloatAdditionResult, 1000);

            // Add 2 float command arguments
            var a = 3.14F;
            var b = 2.71F;
            command.AddArgument(a);
            command.AddArgument(b);

            // Send command
            var floatAdditionResultCommand = _cmdMessenger.SendCommand(command);

            // Check if received a (valid) response
            if (floatAdditionResultCommand.Ok)
            {
                // Read returned argument
                var sum = floatAdditionResultCommand.ReadFloatArg();
                var diff = floatAdditionResultCommand.ReadFloatArg();

                // Compare with sum of sent values
                var errorSum  = sum  - (a + b);
                var errorDiff = diff - (a - b);

                Console.WriteLine("Received sum {0}, difference of {1}", sum, diff);
                Console.WriteLine("with errors {0} and {1}, respectively", errorSum, errorDiff);
            }
            else
                Console.WriteLine("No response!");

            // Stop running loop
            RunLoop = false;
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

            // Pause before stop
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }

        /// Attach command call backs. 
        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);
            _cmdMessenger.Attach((int)Command.Acknowledge, OnAcknowledge);
            _cmdMessenger.Attach((int)Command.Error, OnError);
        }

        // ------------------  C A L L B A C K S ---------------------

        // Called when a received command has no attached function.
        void OnUnknownCommand(ReceivedCommand arguments)
        {
            Console.WriteLine("Command without attached callback received");
        }

        // Callback function that prints that the Arduino has acknowledged
        void OnAcknowledge(ReceivedCommand arguments)
        {
            Console.WriteLine(" Arduino is ready");
        }

        // Callback function that prints that the Arduino has experienced an error
        void OnError(ReceivedCommand arguments)
        {
            Console.WriteLine(" Arduino has experienced an error");
        }

        // Log received line to console
        private void NewLineReceived(object sender, EventArgs e)
        {
            Console.WriteLine(" Received > " + _cmdMessenger.CurrentReceivedLine);
        }

        // Log sent line to console
        private void NewLineSent(object sender, EventArgs e)
        {
            Console.WriteLine(" Sent > " + _cmdMessenger.CurrentSentLine);
        }
    }
}
