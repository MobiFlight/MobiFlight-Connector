// This 1st example will make the PC toggle the integrated led on the arduino board. 
// It demonstrates how to:
// - Define commands
// - Set up a serial connection
// - Send a command with a parameter to the Arduino


using System;
using System.Threading;
using CommandMessenger;
using CommandMessenger.Serialport;
using CommandMessenger.TransportLayer;

namespace Receive
{
    
    // This is the list of recognized commands. These can be commands that can either be sent or received. 
    // In order to receive, attach a callback function to these events
    // 
    // Default commands
    // Note that commands work both directions:
    // - All commands can be sent
    // - Commands that have callbacks attached can be received
    // 
    // This means that both sides should have an identical command list:
    // one side can either send it or receive it (sometimes both)
 
    // Commands
    enum Command
    {
        SetLed, // Command to request led to be set in specific state
    };

    public class Receive
    {
        public bool RunLoop { get; set; }
        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;
        private bool _ledState;

        // Setup function
        public void Setup()
        {
            _ledState = false;

            // Create Serial Port object
            _serialTransport = new SerialTransport();
            _serialTransport.CurrentSerialSettings.PortName = "COM6";    // Set com port
            _serialTransport.CurrentSerialSettings.BaudRate = 115200;     // Set baud rate
            _serialTransport.CurrentSerialSettings.DtrEnable = false;     // For some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
            
            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(_serialTransport);

            // Tell CmdMessenger if it is communicating with a 16 or 32 bit Arduino board
            _cmdMessenger.BoardType = BoardType.Bit16;
            
            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();
            
            // Start listening
            _cmdMessenger.Connect();                                
        }

        // Loop function
        public void Loop()
        {
            // Create command
            var command = new SendCommand((int)Command.SetLed,_ledState);               

            // Send command
            _cmdMessenger.SendCommand(command);

            Console.Write("Turning led ");
            Console.WriteLine(_ledState?"on":"off");

            // Wait for 1 second and repeat
            Thread.Sleep(1000);
            _ledState = !_ledState;   // Toggle led state            
        }

        // Exit function
        public void Exit()
        {
            // We will never exit the application
        }

        /// Attach command call backs. 
        private void AttachCommandCallBacks()
        {
            // No callbacks are currently needed
        }
    }
}
