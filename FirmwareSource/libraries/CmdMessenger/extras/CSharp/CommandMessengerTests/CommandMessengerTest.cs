// *** CommandMessengerTest ***

// This project runs unit tests on several parts on the mayor parts of the CmdMessenger library
// Note that the primary function is not to serve as an example, so the code may be less documented 
// and clean than the example projects. 

//Check bluetooth connection
//Merge with other tree

using System;
using System.IO.Ports;
using CommandMessenger;
using CommandMessenger.Transport.Bluetooth;
using CommandMessenger.Transport.Serial;

namespace CommandMessengerTests
{
 
    public class CommandMessengerTest
    {        
        private SetupConnection _setupConnection;
        private Acknowledge _acknowledge;
        private ClearTextData _clearTextData;
        private BinaryTextData _binaryTextData;
        private TransferSpeed _transferSpeed;
        private MultipleArguments _multipleArguments;

        public CommandMessengerTest()
        {
            // Set up board & transport mode
            var teensy31 = new systemSettings()
            {
                Description = @"Teensy 3.1",
                MinReceiveSpeed     = 2000000,         // Bits per second    
                MinSendSpeed        = 1250000,         // Bits per second                                       
                MinDirectSendSpeed  = 45000,           // Bits per second                     
                BoardType           = BoardType.Bit32, // 32-bit architecture, needed from binary value conversion
                sendBufferMaxLength = 512,             // Maximum send buffer size, optimally buffer size is similar to embedded controller
                Transport = new SerialTransport
                    {
                        CurrentSerialSettings = new SerialSettings()
                            {
                                PortName = "COM15",    // Can be different!
                                BaudRate = 115200,     // Bits per second
                                DataBits = 8,          // Data bits
                                Parity = Parity.None,  // Bit parity
                                DtrEnable = false,     // Some boards need to send this to enabled                                    
                            },
                            
                    }
            };

            var arduinoNano = new systemSettings()
            {
                Description = @"Arduino Nano /w AT mega328",
                MinReceiveSpeed     = 82000,              // Bits per second 
                MinSendSpeed        = 85000,              // Bits per second                                      
                MinDirectSendSpeed  = 52000,              // Bits per second                
                BoardType           = BoardType.Bit16,    // 16-bit architecture, needed from binary value conversion
                sendBufferMaxLength = 60,                 // Maximum send buffer size, optimally buffer size is similar to embedded controller
                Transport = new SerialTransport
                {
                    CurrentSerialSettings = new SerialSettings()
                    {
                        PortName  = "COM16",                  // Can be different!
                        BaudRate  = 115200,                  // Bits per second
                        DataBits  = 8,                       // Data bits
                        Parity    = Parity.None,             // Bit parity
                        DtrEnable = false,                   // Some boards need to send this to enabled                                    
                    },

                }
            };


            var arduinoNanoBluetooth = new systemSettings()
            {
                Description         = @"Arduino Nano /w AT mega328 - Bluetooth",
                MinReceiveSpeed     = 6500,                         // Bits per second 
                MinSendSpeed        = 7400,                         // Bits per second                                      
                MinDirectSendSpeed  = 8000,                         // Bits per second                
                BoardType           = BoardType.Bit16,              // 16-bit architecture, needed from binary value conversion
                sendBufferMaxLength = 60,                           // Maximum send buffer size, optimally buffer size is similar to embedded controller
                Transport           = new BluetoothTransport()
                {
                    CurrentBluetoothDeviceInfo = BluetoothUtils.DeviceByAdress("20:13:07:26:10:08"),
                    LazyReconnect              = true,              // Only reconnect if really necessary
                }
            };

            var arduinoLeonardoOrProMicro = new systemSettings()
            {
                Description         = @"Arduino Leonardo or Sparkfun ProMicro /w AT mega32u4",
                MinReceiveSpeed     = 82000,               // Bits per second 
                MinSendSpeed        = 90000,               // Bits per second                                      
                MinDirectSendSpeed  = 52000,               // Bits per second                
                BoardType           = BoardType.Bit16,     // 16-bit architecture, needed from binary value conversion
                sendBufferMaxLength = 60,                  // Maximum send buffer size, optimally buffer size is similar to embedded controller
                Transport           = new SerialTransport
                {
                    CurrentSerialSettings = new SerialSettings()
                    {
                        PortName  = "COM13",                 // Can be different!
                        BaudRate  = 115200,                  // Bits per second
                        DataBits  = 8,                       // Data bits
                        Parity    = Parity.None,             // Bit parity
                        DtrEnable = true,                    // Some boards need to send this to enabled                                    
                    },

                }
            };

            // Set up Command enumerators
            var command = DefineCommands();

            // Initialize tests, CHANGE "DEVICE" VARIABLE TO YOUR DEVICE!
            var device = teensy31;
            InitializeTests(device, command);

            // Open log file for testing 
            Common.OpenTestLogFile(@"TestLogFile.txt");
            
            // Run tests
            RunTests();

            Common.CloseTestLogFile();
        }


        private static Enumerator DefineCommands()
        {
            var command = new Enumerator();
            // Set up default commands
            command.Add(new[]
                {
                    "CommError", // Command reports serial port comm error (only works for some comm errors)
                    "kComment", // Command to sent comment in argument
                });
            return command;
        }

        private void InitializeTests(systemSettings systemSettings, Enumerator command)
        {
            _setupConnection   = new SetupConnection(systemSettings, command);
            _acknowledge       = new Acknowledge(systemSettings, command);
            _clearTextData     = new ClearTextData(systemSettings, command);
            _binaryTextData    = new BinaryTextData(systemSettings, command);
            _multipleArguments = new MultipleArguments(systemSettings, command);
            _transferSpeed     = new TransferSpeed(systemSettings, command);

        }

        private void RunTests()
        {

            //Todo: implement autoconnect tests

            // Test opening and closing connection
            _setupConnection.RunTests();
            
            // Test acknowledgment both on PC side and embedded side
            _acknowledge.RunTests();

            //// Test all plain text formats
            _clearTextData.RunTests();

            //// Test all binary formats
            _binaryTextData.RunTests();            
            
            //// Test sending multiple arguments
            _multipleArguments.RunTests();

            //// Test large series for completeness (2-way)
            //// todo
            
            //// Test speed
            _transferSpeed.RunTests();

            //// Test load
            //// todo
            
            //// Test Strategies
            //// todo
            
            // Summary of tests
            Common.TestSummary();

            // Exit application
            Exit();            
        }


        public void Exit()
        {
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
            Environment.Exit(0);
        }

    }
}
