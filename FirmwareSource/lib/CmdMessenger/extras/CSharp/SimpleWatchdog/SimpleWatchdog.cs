// *** SimpleWatchdog ***

// This example shows the usage of the watchdog for communication over virtual serial port,
// 
// - Use bluetooth connection
// - Use auto scanning and connecting
// - Use watchdog 

using System;
using CommandMessenger;
using CommandMessenger.Transport;
using CommandMessenger.Transport.Serial;

namespace SimpleWatchdog
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
        // This can by done by checking for a specific Communication Identifier. 
        // You can make a unique identifier per device, 
        // see http://pragmateek.com/4-ways-to-generate-a-guid/         
        private const string CommunicationIdentifier = "BFAF4176-766E-436A-ADF2-96133C02B03C";
        
        // You could also check for the first device that has the correct (sketch) application and version running
        //private const string CommunicationIdentifier = "SimpleWatchdog__1_0_1";
        
        private static ITransport _transport;
        private static CmdMessenger _cmdMessenger;
        private static ConnectionManager _connectionManager;

        // Setup function
        public void Setup()
        {
            _transport = new SerialTransport {CurrentSerialSettings = {DtrEnable = false}};
            // some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.                        
            // We do not need to set serial port and baud rate: it will be found by the connection manager                                                           

            // Initialize the command messenger with the Serial Port transport layer
            // Set if it is communicating with a 16- or 32-bit Arduino board
            _cmdMessenger = new CmdMessenger(_transport, BoardType.Bit16)
            {
                PrintLfCr = false // Do not print newLine at end of command, to reduce data being sent
            };

            // The Connection manager is capable or storing connection settings, in order to reconnect more quickly  
            // the next time the application is run. You can determine yourself where and how to store the settings
            // by supplying a class, that implements ISerialConnectionStorer. For convenience, CmdMessenger provides
            //  simple binary file storage functionality
            var serialConnectionStorer = new SerialConnectionStorer("SerialConnectionManagerSettings.cfg");

            // We don't need to provide a handler for the Identify command - this is a job for Connection Manager.
            _connectionManager = new SerialConnectionManager(
                _transport as SerialTransport, 
                _cmdMessenger,
                (int) Command.Identify, 
                CommunicationIdentifier,
                serialConnectionStorer)
            {
                // Enable watchdog functionality.
                WatchdogEnabled = true,

                // Instead of scanning for the connected port, you can disable scanning and only try the port set in CurrentSerialSettings
                //DeviceScanEnabled = false
            };

            // Show all connection progress on command line             
            _connectionManager.Progress += (sender, eventArgs) =>
            {
                // If you want to reduce verbosity, you can only show events of level <=2 or ==1
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

            //You can also do something when the connection is lost
            _connectionManager.ConnectionTimeout += (sender, eventArgs) =>
            {
                //Do something
            };

            // Finally - activate connection manager
            _connectionManager.StartConnectionManager();
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
