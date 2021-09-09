﻿// *** SendandReceiveBinaryArguments ***

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
        private int _receivedPlainTextCount;                    // Counter of number of plain text items received
        private int _receivedBinaryCount;                       // Counter of number of binary items received
        long _beginTime;                                        // Start time, 1st item of sequence received 
        long _endTime;                                          // End time, last item of sequence received 
        private bool _receivePlainTextFloatSeriesFinished;      // Indicates if plain text float series has been fully received
        private bool _receiveBinaryFloatSeriesFinished;         // Indicates if binary float series has been fully received
        const int SeriesLength = 1000;                          // Number of items we like to receive from the Arduino
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
            _cmdMessenger = new CmdMessenger(_serialTransport);

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();                

            // Start listening
            _cmdMessenger.StartListening();

            _receivedPlainTextCount = 0;
            
            // Send command requesting a series of 100 float values send in plain text
            var commandPlainText = new SendCommand((int)Command.RequestPlainTextFloatSeries);
            commandPlainText.AddArgument(SeriesLength);
            commandPlainText.AddArgument(SeriesBase);
            // Send command 
            _cmdMessenger.SendCommand(commandPlainText);

            // Now wait until all values have arrived
            while (!_receivePlainTextFloatSeriesFinished) {}

            // Send command requesting a series of 100 float values send in plain text
            var commandBinary = new SendCommand((int)Command.RequestBinaryFloatSeries);
            commandBinary.AddBinArgument((UInt16)SeriesLength);
            commandBinary.AddBinArgument((Single)SeriesBase); 
            
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
            if (_receivedPlainTextCount % (SeriesLength/10) == 0)
                Console.WriteLine("Received value: {0}",arguments.ReadFloatArg());
            if (_receivedPlainTextCount == 0)
            {
                // Received first value, start stopwatch
                _beginTime = Millis;
            }
            else if (_receivedPlainTextCount == SeriesLength - 1)
            {
                // Received all values, stop stopwatch
                _endTime = Millis;
                Console.WriteLine("{0} milliseconds per {1} items = is {2} ms/item", _endTime - _beginTime, SeriesLength, (_endTime - _beginTime) / (float)SeriesLength);
                _receivePlainTextFloatSeriesFinished = true;
           }            
            _receivedPlainTextCount++;     
        }

        // Callback function To receive the binary float series from the Arduino
        void OnReceiveBinaryFloatSeries(ReceivedCommand arguments)
        {
            if (_receivedBinaryCount % (SeriesLength / 10) == 0)
                    Console.WriteLine("Received value: {0}", arguments.ReadBinFloatArg());
            if (_receivedBinaryCount == 0)
            {
                // Received first value, start stopwatch
                _beginTime = Millis;
            }
            else if (_receivedBinaryCount == SeriesLength - 1)
            {
                // Received all values, stop stopwatch
                _endTime = Millis;
                Console.WriteLine("{0} milliseconds per {1} items = is {2} ms/item", _endTime - _beginTime, SeriesLength, (_endTime - _beginTime) / (float)SeriesLength);
                _receiveBinaryFloatSeriesFinished = true;
            }
            _receivedBinaryCount++;
        }

        // Return Milliseconds since 1970
        public static long Millis { get { return (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds); } }
    }
}
