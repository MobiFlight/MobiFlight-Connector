// *** SendandReceiveBinaryArguments ***

// This example expands the previous SendandReceiveArguments example. The PC will 
//  send and receive multiple Binary values, demonstrating that this is more compact and faster. Since the output is not human readable any more, 
//  the logging is disabled and the NewLines are removed
//
// It adds a demonstration of how to:
// - Receive multiple binary parameters,
// - Send multiple binary parameters
// - Callback events being handled while the main program waits
// - How to calculate milliseconds, similar to Arduino function Millis()

using System;
using CommandMessenger;
using CommandMessenger.Serialport;
using CommandMessenger.TransportLayer;

namespace SendAndReceiveBinaryArguments
{
    // This is the list of recognized commands. These can be commands that can either be sent or received. 
    // In order to receive, attach a callback function to these events
    enum Command
    {
        RequestPlainTextFloatSeries , // Command Request to send series in plain text
        ReceivePlainTextFloatSeries , // Command to send an item in plain text
        RequestBinaryFloatSeries    , // Command Request to send series in binary form
        ReceiveBinaryFloatSeries    , // Command to send an item in binary form
    };

    public class SendAndReceiveBinaryArguments
    {
        public bool RunLoop { get; set; }
        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;
        private int _receivedItemsCount;                    // Counter of number of plain text items received
        private int _receivedBytesCount;               // Counter of number of plain text bytes received
        //private int _receivedBinaryCount;                       // Counter of number of binary items received
        long _beginTime;                                        // Start time, 1st item of sequence received 
        long _endTime;                                          // End time, last item of sequence received 
        private bool _receivePlainTextFloatSeriesFinished;      // Indicates if plain text float series has been fully received
        private bool _receiveBinaryFloatSeriesFinished;         // Indicates if binary float series has been fully received
        const int SeriesLength = 2000;                          // Number of items we like to receive from the Arduino
        private const float SeriesBase = 1111111.111111F;       // Base of values to return: SeriesBase * (0..SeriesLength-1)

        // ------------------ M A I N  ----------------------

        // Setup function
        public void Setup()
        {
            // Create Serial Port object
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = "COM6", BaudRate = 115200 } // object initializer
            };

            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(_serialTransport)
            {
                BoardType = BoardType.Bit16 // Set if it is communicating with a 16- or 32-bit Arduino board
            };

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();                

            // Start listening
            _cmdMessenger.Connect();

            _receivedItemsCount = 0;
            _receivedBytesCount = 0;
            
            // Send command requesting a series of 100 float values send in plain text form
            var commandPlainText = new SendCommand((int)Command.RequestPlainTextFloatSeries);
            commandPlainText.AddArgument(SeriesLength);
            commandPlainText.AddArgument(SeriesBase);
            // Send command 
            _cmdMessenger.SendCommand(commandPlainText);

            // Now wait until all values have arrived
            while (!_receivePlainTextFloatSeriesFinished) {}

            _receivedItemsCount = 0;
            _receivedBytesCount = 0;
            // Send command requesting a series of 100 float values send in binary form
            var commandBinary = new SendCommand((int)Command.RequestBinaryFloatSeries);
            commandBinary.AddBinArgument((UInt16)SeriesLength);
            commandBinary.AddBinArgument((float)SeriesBase); 
            
            // Send command 
            _cmdMessenger.SendCommand(commandBinary);

            // Now wait until all values have arrived
            while (!_receiveBinaryFloatSeriesFinished) { }
        }

        // Loop function
        public void Loop()
        {
            RunLoop = false;
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

            // Pause before stop
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }

        /// Attach command call backs. 
        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);
            _cmdMessenger.Attach((int)Command.ReceivePlainTextFloatSeries, OnReceivePlainTextFloatSeries);
            _cmdMessenger.Attach((int)Command.ReceiveBinaryFloatSeries, OnReceiveBinaryFloatSeries);
        }

        // ------------------  C A L L B A C K S ---------------------

        // Called when a received command has no attached function.
        void OnUnknownCommand(ReceivedCommand arguments)
        {
            Console.WriteLine("Command without attached callback received");
        }


        // Callback function To receive the plain text float series from the Arduino
        void OnReceivePlainTextFloatSeries(ReceivedCommand arguments)
        {
            _receivedBytesCount += CountBytesInCommand(arguments, true);


            if (_receivedItemsCount % (SeriesLength/10) == 0)
                Console.WriteLine("Received value: {0}",arguments.ReadFloatArg());
            if (_receivedItemsCount == 0)
            {
                // Received first value, start stopwatch
                _beginTime = Millis;
            }
            else if (_receivedItemsCount == SeriesLength - 1)
            {
                // Received all values, stop stopwatch
                _endTime = Millis;
                var deltaTime = (_endTime - _beginTime);
                Console.WriteLine("{0} milliseconds per {1} items = is {2} ms/item, {3} Hz",
                    deltaTime, 
                    SeriesLength, 
                    (float)deltaTime / (float)SeriesLength,
                    (float)1000 * SeriesLength / (float)deltaTime
                    );
                Console.WriteLine("{0} milliseconds per {1} bytes = is {2} ms/byte,  {3} bytes/sec, {4} bps",
                    deltaTime,
                    _receivedBytesCount,
                    (float)deltaTime / (float)_receivedBytesCount,
                    (float)1000 * _receivedBytesCount / (float)deltaTime,
                    (float)8 * 1000 * _receivedBytesCount / (float)deltaTime
                    );
                _receivePlainTextFloatSeriesFinished = true;
           }            
            _receivedItemsCount++;     
        }

        private int CountBytesInCommand(CommandMessenger.Command command, bool printLfCr)
        {
            var bytes = command.CommandString().Length; // Command + command separator
            //var bytes = _cmdMessenger.CommandToString(command).Length + 1; // Command + command separator
            if (printLfCr) bytes += Environment.NewLine.Length; // Add  bytes for carriage return ('\r') and /or a newline  ('\n')
            return bytes;
        }

        // Callback function To receive the binary float series from the Arduino
        void OnReceiveBinaryFloatSeries(ReceivedCommand arguments)
        {
            _receivedBytesCount += CountBytesInCommand(arguments, false);

            if (_receivedItemsCount % (SeriesLength / 10) == 0)
                    Console.WriteLine("Received value: {0}", arguments.ReadBinFloatArg());
            if (_receivedItemsCount == 0)
            {
                // Received first value, start stopwatch
                _beginTime = Millis;
            }
            else if (_receivedItemsCount == SeriesLength - 1)
            {
                // Received all values, stop stopwatch
                _endTime = Millis;
                var deltaTime = (_endTime - _beginTime);
                Console.WriteLine("{0} milliseconds per {1} items = is {2} ms/item, {3} Hz",
                    deltaTime,
                    SeriesLength,
                    (float)deltaTime / (float)SeriesLength,
                    (float)1000 * SeriesLength / (float)deltaTime
                    );
                Console.WriteLine("{0} milliseconds per {1} bytes = is {2} ms/byte,  {3} bytes/sec, {4} bps",
                    deltaTime,
                    _receivedBytesCount,
                    (float)deltaTime / (float)_receivedBytesCount,
                    (float)1000 * _receivedBytesCount / (float)deltaTime,
                    (float)8 * 1000 * _receivedBytesCount / (float)deltaTime
                    );
                _receiveBinaryFloatSeriesFinished = true;
            }
            _receivedItemsCount++;
        }

        // Return Milliseconds since 1970
        public static long Millis { get { return (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds); } }
    }
}
