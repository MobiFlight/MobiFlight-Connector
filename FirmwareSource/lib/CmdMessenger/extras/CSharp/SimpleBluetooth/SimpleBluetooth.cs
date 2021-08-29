// *** SimpleBluetoothSimpleWatchdog ***

// This example shows the usage of the watchdog for communication over Bluetooth, tested with the well known JY-MCU HC-05 and HC-06
//
// To help get you started, have a look at:
//    - http://www.instructables.com/id/Cheap-2-Way-Bluetooth-Connection-Between-Arduino-a/step4/Set-up-your-PC-for-serial-Bluetooth-communication/
//    - http://homepages.ihug.com.au/~npyner/Arduino/GUIDE_2BT.pdf
//    basically for JY-MCU HC-05 and HC-06 you only have to make sure that
//     1) the device is connected using a voltage divider
//     2) the serial speed set in your script is the same as the Bluetooth speed (by default 9600)
//     So, don't worry about discovery and pairing, CmdMessenger will do that for you.
//     
//      On Arduino side, use the same SimpleWatchdog.ino script as the previous example, but make sure the speed is set to 9600
//
// - Use bluetooth connection
// - Use auto scanning and connecting
// - Use watchdog 

using System;
using CommandMessenger;
using CommandMessenger.Transport;
using CommandMessenger.Transport.Bluetooth;

namespace SimpleBluetooth
{
    class SimpleWatchdog
    {
        enum Command
        {
            Identify,           // Command to identify device
            TurnLedOn,          // Command to request led to be turned on
        };

        public bool RunLoop { get; set; }

        // Most of the time you want to be sure you are connecting with the correct device.        
        private const string CommunicationIdentifier = "BFAF4176-766E-436A-ADF2-96133C02B03C";
        
        // You could also check for the first device that has the correct (sketch) application and version running
        //private const string CommunicationIdentifier = "SimpleWatchdog__1_0_1";
        
        private static ITransport _transport;
        private static CmdMessenger _cmdMessenger;
        private static ConnectionManager _connectionManager;

        // Setup function
        public void Setup()
        {
            // Let's show all bluetooth devices
            ShowBluetoothInfo();

            // Now let us set Bluetooth transport
            _transport = new BluetoothTransport()
            {
                // If you know your bluetooth device and you have already paired
                // you can directly connect to you Bluetooth Device by adress adress.
                // Under windows you can find the adresss at:
                //    Control Panel >> All Control Panel Items >> Devices and Printers
                //    Right-click on device >> properties >> Unique id
                CurrentBluetoothDeviceInfo = BluetoothUtils.DeviceByAdress("20:13:07:26:10:08")
            };
                                                         
            // Initialize the command messenger with the Serial Port transport layer
            // Set if it is communicating with a 16- or 32-bit Arduino board
            _cmdMessenger = new CmdMessenger(_transport)
            {
                PrintLfCr = false // Do not print newLine at end of command, to reduce data being sent
            };

            // The Connection manager is capable or storing connection settings, in order to reconnect more quickly  
            // the next time the application is run. You can determine yourself where and how to store the settings
            // by supplying a class, that implements ISerialConnectionStorer. For convenience, CmdMessenger provides
            //  simple binary file storage functionality
            var bluetoothConnectionStorer = new BluetoothConnectionStorer("BluetoothConnectionManagerSettings.cfg");

            // It is easier to let the BluetoothConnectionManager connection for you.
            // It will:
            //  - Auto discover Bluetooth devices
            //  - If not yet paired, try to pair using the default Bluetooth passwords
            //  - See if the device responds with the correct CommunicationIdentifier
            _connectionManager = new BluetoothConnectionManager(
                _transport as BluetoothTransport, 
                _cmdMessenger,
                (int) Command.Identify, 
                CommunicationIdentifier,
                bluetoothConnectionStorer)
            {
                // Enable watchdog functionality.
                WatchdogEnabled = true,

                // You can add PIN codes for specific devices
                DevicePins =
                {
                    {"01:02:03:04:05:06","6666"},
                    {"01:02:03:04:05:07","7777"},
                },

                // You can also add PIN code to try on all unpaired devices
                // (the following PINs are tried by default: 0000, 1111, 1234 )
                GeneralPins =
                {
                    "8888",
                }
            };

            // Show all connection progress on command line             
            _connectionManager.Progress += (sender, eventArgs) =>
            {
                if (eventArgs.Level <= 3) Console.WriteLine(eventArgs.Description);
            };

            // If connection found, tell the arduino to turn the (internal) led on
            _connectionManager.ConnectionFound += (sender, eventArgs) =>
            {
                // Create command
                var command = new SendCommand((int)Command.TurnLedOn);
                // Send command
                _cmdMessenger.SendCommand(command);
            };

            // Finally - activate connection manager
            _connectionManager.StartConnectionManager();
        }

        // Show Bluetooth information
        private static void ShowBluetoothInfo()
        {
            // Show  adress of local primary bluetooth device 
            Console.WriteLine("Adress of the connected (primary) bluetooth device:");
            BluetoothUtils.PrintLocalAddress();
            Console.WriteLine("");

            //Show all paired bluetooth devices
            Console.WriteLine("All paired bluetooth devices:");
            BluetoothUtils.PrintPairedDevices();
            Console.WriteLine("");

            //Show all bluetooth devices, paired and unpaired. 
            // Note that this takes a lot of time!
            Console.WriteLine("All Bluetooth devices, paired and unpaired:");
            BluetoothUtils.PrintAllDevices();
            Console.WriteLine("");          

            // Show Virtual serial ports associated with Bluetooth devices
            // Note that CmdMessenger does not need these and will bypass them
            Console.WriteLine("Virtual serial ports associated with Bluetooth devices:");
            BluetoothUtils.PrintSerialPorts();
            Console.WriteLine("");
        }

        // Loop function
        public void Loop()
        {
            //Wait for key
            Console.ReadKey();            
            // Stop loop
            RunLoop = false;  
        }

        // Exit function
        public void Exit()
        {
            _connectionManager.Dispose();
            _cmdMessenger.Disconnect();
            _cmdMessenger.Dispose();
            _transport.Dispose();
        }

    }
}
