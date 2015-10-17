// *** SendandReceive ***

// This example expands the previous Receive example. The Arduino will now send back a status.
// It adds a demonstration of how to:
// - Handle received commands that do not have a function attached
// - Receive a command with a parameter from the Arduino

using System;
using System.Threading;
using CommandMessenger;
using CommandMessenger.Bluetooth;
using CommandMessenger.Serialport;

namespace SendAndReceive
{
    // This is the list of recognized commands. These can be commands that can either be sent or received. 
    // In order to receive, attach a callback function to these events
    enum Command
    {
        SetLed, 
        Status, 
    };

    public class SendAndReceive
    {
        public bool RunLoop { get; set; }
        private SerialTransport _serialTransport;
        private BluetoothTransport _bluetoothTransport;
        private CmdMessenger _cmdMessenger;
        private bool _ledState;
        private int _count;

        // Setup function
        public void Setup()
        {
            _ledState = false;

            BluetoothInfo();

            // Create Bluetooth transport object
            _bluetoothTransport = new BluetoothTransport
            {
                //CurrentBluetoothDeviceInfo = BluetoothUtils.DeviceByAdress("98-D3-31-B0-FB-B5")
                CurrentBluetoothDeviceInfo = BluetoothUtils.DeviceByAdress("20:13:07:26:10:08")
                
            };

            // Create Serial Port transport object
            // Note that for some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = "COM16", BaudRate = 9600, DtrEnable = false } // object initializer
            };

            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(_bluetoothTransport) 
                {
                    BoardType = BoardType.Bit16 // Set if it is communicating with a 16- or 32-bit Arduino board
                };

            // Tell CmdMessenger if it is communicating with a 16 or 32 bit Arduino board

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();
            
            // Start listening
            _cmdMessenger.Connect();                                
        }

        private static void BluetoothInfo()
        {
            // Show  adress of local primary bluetooth device 
            Console.WriteLine("Adress of local primary bluetooth device:");
            BluetoothUtils.PrintLocalAddress();
            Console.WriteLine("");

            //Show all paired bluetooth devices
            Console.WriteLine("All paired bluetooth devices:");
            BluetoothUtils.PrintPairedDevices();
            Console.WriteLine("");

            // Show Virtual serial ports associated with Bluetooth devices
            Console.WriteLine("Virtual serial ports associated with Bluetooth devices:");
            BluetoothUtils.PrintSerialPorts();
            Console.WriteLine("");
        }

        // Loop function
        public void Loop()
        {
            _count++;

            // Create command
            var command = new SendCommand((int)Command.SetLed,_ledState);               

            // Send command
            _cmdMessenger.SendCommand(command);

            // Wait for 1 second and repeat
            Thread.Sleep(1000);
            _ledState = !_ledState;                                        // Toggle led state  

            if (_count > 100) RunLoop = false;                             // Stop loop after 100 rounds
        }

        // Exit function
        public void Exit()
        {
            
            if (_cmdMessenger != null)
            {
                // Stop listening
                _cmdMessenger.Disconnect();
                // Dispose Command Messenger
                _cmdMessenger.Dispose();
            }
           

            // Dispose Serial Port object
            if (_serialTransport != null) _serialTransport.Dispose();
            if (_bluetoothTransport != null) _bluetoothTransport.Dispose();            
        }

        /// Attach command call backs. 
        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);
            _cmdMessenger.Attach((int)Command.Status, OnStatus);
        }

        /// Executes when an unknown command has been received.
        void OnUnknownCommand(ReceivedCommand arguments)
        {
            Console.WriteLine("Command without attached callback received");
        }

        // Callback function that prints the Arduino status to the console
        void OnStatus(ReceivedCommand arguments)
        {
            Console.Write("Arduino status: ");
            Console.WriteLine(arguments.ReadStringArg());
        }
    }
}
