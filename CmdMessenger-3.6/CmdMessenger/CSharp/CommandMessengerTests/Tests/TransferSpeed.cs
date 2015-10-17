using System;
using System.Threading;
using CommandMessenger;

namespace CommandMessengerTests
{
    public class TransferSpeed 
    {        
        private CmdMessenger _cmdMessenger;
        readonly Enumerator _command;      
        public bool RunLoop { get; set; }
        private int _receivedItemsCount;                        // Counter of number of command items received
        private int _receivedBytesCount;                        // Counter of number of command bytes received
        long _beginTime;                                        // Start time, 1st item of sequence received 
        long _endTime;                                          // End time, last item of sequence received 
        private volatile bool _receiveSeriesFinished;           // Indicates if plain text float series has been fully received
        private volatile bool _sendSeriesFinished;
        private readonly systemSettings _systemSettings;
        const int SeriesLength = 10000;                         // Number of items we like to receive from the Arduino
        private const float SeriesBase = 1.00001F;              // Base of values to return: SeriesBase * (0..SeriesLength-1)
        private float _minimalBps;
        public TransferSpeed(systemSettings systemSettings, Enumerator command)
        {
            _systemSettings = systemSettings;
            _command = command;
            DefineCommands();
        }

        // ------------------ Command Callbacks -------------------------
        private void DefineCommands()
        {
            _command.Add(new[]
               {
                "RequestReset"                   ,   // Command Request reset
                "RequestResetAcknowledge"        ,   // Command to acknowledge reset
        
                "RequestSeries"    ,   // Command Request to send series in plain text
                "ReceiveSeries"    ,   // Command to send an item in plain text
                "DoneReceiveSeries",   // Command to tell that sending series in plain text is done
      
                "PrepareSendSeries",   // Command to tell other side to prepare for receiving a series of text float commands
                "SendSeries"       ,   // Command to send a series of text float commands
                "AckSendSeries"        // Command to acknowledge the send series of text float commands

             });
        }

        // ------------------ Command Callbacks -------------------------
        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(_command["RequestResetAcknowledge"], OnRequestResetAcknowledge);
            _cmdMessenger.Attach(_command["ReceiveSeries"], OnReceiveSeries);
            _cmdMessenger.Attach(_command["DoneReceiveSeries"], OnDoneReceiveSeries);
            _cmdMessenger.Attach(_command["AckSendSeries"], OnAckSendSeries);
        }


        // ------------------ Test functions -------------------------

        public void RunTests()
        {
            // Open Connection
            Common.StartTestSet("Benchmarking transfer speeds");
            SetUpConnection();

            // Stop logging commands as this may degrade performance
            Common.LogCommands(false);

            // Test acknowledgments
            //*** Benchmark 1: receive series of float data
            SetupReceiveSeries();

            //*** Benchmark 2: queued send series of float data
            SetupQueuedSendSeries();

            //*** Benchmark 3: direct send series of float data
            DirectSendSeries();

            // Start logging commands again
            Common.LogCommands(true);

            // Close connection
            CloseConnection();
            Common.EndTestSet();
        }

        public void SetUpConnection()
        {
            try
            {
                _cmdMessenger = Common.Connect(_systemSettings);
                AttachCommandCallBacks();
            }
            catch (Exception)
            {
            }
        }

        public void CloseConnection()
        {
            try
            {
                Common.Disconnect();
            }
            catch (Exception)
            {
            }
        }

        private void WaitAndClear()
        {
            var requestResetCommand = new SendCommand(_command["RequestReset"], _command["RequestResetAcknowledge"], 1000);
            var requestResetAcknowledge = _cmdMessenger.SendCommand(requestResetCommand, SendQueue.ClearQueue,ReceiveQueue.ClearQueue);

            Common.WriteLine(!requestResetAcknowledge.Ok ? "No Wait OK received" : "Wait received");
            // Wait another second to see if
            Thread.Sleep(1000);
            // Clear queues again to be very sure
            _cmdMessenger.ClearReceiveQueue();
            _cmdMessenger.ClearSendQueue();
        }


        // Called when a RequestResetAcknowledge comes in   
        private void OnRequestResetAcknowledge(ReceivedCommand receivedcommand)
        {
            // This function should not be called because OnRequestResetAcknowledge should
            // be handled by the requestResetAcknowledge synchronous command in WaitAndClear()
            // if it happens, we will do another WaitAndClear() and hope that works 
            WaitAndClear();
        }

        // *** Benchmark 1 ***
        private void SetupReceiveSeries()
        {
            Common.StartTest("Calculating speed in receiving series of float data");

            WaitAndClear();

            _receiveSeriesFinished = false;
            _receivedItemsCount = 0;
            _receivedBytesCount = 0;
            _minimalBps = _systemSettings.MinReceiveSpeed;

            // Send command requesting a series of 100 float values send in plain text form           
            var commandPlainText = new SendCommand(_command["RequestSeries"]);
            commandPlainText.AddArgument(SeriesLength);
            commandPlainText.AddArgument(SeriesBase);
            
            // Send command 
            _cmdMessenger.SendCommand(commandPlainText);

            // Now wait until all values have arrived
            while (!_receiveSeriesFinished)
            {
                Thread.Sleep(10);
            }
        }

        // Callback function To receive the plain text float series from the Arduino
        void OnReceiveSeries(ReceivedCommand arguments)
        {

            if (_receivedItemsCount % (SeriesLength / 10) == 0)
                Common.WriteLine(_receivedItemsCount+ " Received value: " + arguments.ReadFloatArg());
            if (_receivedItemsCount == 0)
            {
                // Received first value, start stopwatch
                _beginTime = Millis;
            }

            _receivedItemsCount++;
            _receivedBytesCount += CountBytesInCommand(arguments, false);

        }

        private void OnDoneReceiveSeries(ReceivedCommand receivedcommand)
        {
            float bps = CalcTransferSpeed();

            if (bps > _minimalBps)
            {
                Common.TestOk("Embedded system is receiving data as fast as expected. Measured: " + bps + " bps, expected " + _minimalBps);
            }
            else
            {
                Common.TestNotOk("Embedded system is receiving data not as fast as expected. Measured: " + bps + " bps, expected " + _minimalBps);
            }


            _receiveSeriesFinished = true;

            Common.EndTest();
        }

        private float CalcTransferSpeed()
        {
            Common.WriteLine("Benchmark results");
            // Received all values, stop stopwatch
            _endTime = Millis;
            var deltaTime = (_endTime - _beginTime);
            Common.WriteLine(
                deltaTime +
                " milliseconds per " +
                _receivedItemsCount +
                " items = is " +
                (float)deltaTime / (float)_receivedItemsCount +
                " ms/item, " +
                (float)1000 * _receivedItemsCount / (float)deltaTime +
                " Hz"
            );

            float bps = (float)8 * 1000 * _receivedBytesCount / (float)deltaTime;
            Common.WriteLine(
                deltaTime +
                " milliseconds per " +
                _receivedItemsCount +
                " bytes = is " +
                (float)deltaTime / (float)_receivedBytesCount +
                " ms/byte, " +
                (float)1000 * _receivedBytesCount / (float)deltaTime +
                " bytes/sec, " +
                bps +
                " bps"
            );
            return bps;
        }


        // *** Benchmark 2 ***
        // Setup queued send series
        private void SetupQueuedSendSeries()
        {
            Common.StartTest("Calculating speed in sending queued series of float data");
            WaitAndClear();

            _minimalBps = _systemSettings.MinSendSpeed;
            _sendSeriesFinished = false;
            var prepareSendSeries = new SendCommand(_command["PrepareSendSeries"]);
            prepareSendSeries.AddArgument(SeriesLength);
            _cmdMessenger.SendCommand(prepareSendSeries, SendQueue.WaitForEmptyQueue, ReceiveQueue.WaitForEmptyQueue);

            // Prepare
            _receivedBytesCount = 0;
            _cmdMessenger.PrintLfCr = true;
            _beginTime = Millis;

            // Now queue all commands
            for (var sendItemsCount = 0; sendItemsCount < SeriesLength; sendItemsCount++)
            {
                var sendSeries = new SendCommand(_command["SendSeries"]);
                sendSeries.AddArgument(sendItemsCount * SeriesBase);

                _receivedBytesCount += CountBytesInCommand(sendSeries, _cmdMessenger.PrintLfCr);

                _cmdMessenger.QueueCommand(sendSeries);
                if (sendItemsCount % (SeriesLength / 10) == 0)
                    Common.WriteLine("Send value: " + sendItemsCount * SeriesBase);
            }

            // Now wait until receiving party acknowledges that values have arrived
            while (!_sendSeriesFinished)
            {
                Thread.Sleep(10);
            }
        }

        private void OnAckSendSeries(ReceivedCommand receivedcommand)
        {
            float bps = CalcTransferSpeed();

            if (bps > _minimalBps)
            {
                Common.TestOk("Embedded system is receiving data as fast as expected. Measured: " + bps + " bps, expected " + _minimalBps);
            }
            else
            {
                Common.TestNotOk("Embedded system is receiving data not as fast as expected. Measured: " + bps + " bps, expected " + _minimalBps);
            }


            Common.EndTest();
            _sendSeriesFinished = true;
        }

        // *** Benchmark 3 ***
        private void DirectSendSeries()
        {
            Common.StartTest("Calculating speed in individually sending a series of float data");
            WaitAndClear();

            _minimalBps = _systemSettings.MinDirectSendSpeed; 
            _sendSeriesFinished = false;

            var prepareSendSeries = new SendCommand(_command["PrepareSendSeries"]);
            prepareSendSeries.AddArgument(SeriesLength);
            // We need to to send the prepareSendSeries by bypassing the queue or it might be sent after the directly send commands later on
            _cmdMessenger.SendCommand(prepareSendSeries, SendQueue.WaitForEmptyQueue, ReceiveQueue.WaitForEmptyQueue,UseQueue.BypassQueue);

            // Prepare
            _receivedBytesCount = 0;
            _cmdMessenger.PrintLfCr = true;
            _beginTime = Millis;
            
            // Now send all commands individually and bypass the queue
             for (var sendItemsCount = 0; sendItemsCount < SeriesLength; sendItemsCount++)
            {
                var sendSeries = new SendCommand(_command["SendSeries"]);
                sendSeries.AddArgument(sendItemsCount * SeriesBase);

                _receivedBytesCount += CountBytesInCommand(sendSeries, _cmdMessenger.PrintLfCr);

                _cmdMessenger.SendCommand(sendSeries, SendQueue.Default, ReceiveQueue.Default, UseQueue.BypassQueue);
     
                if (sendItemsCount%(SeriesLength/10) == 0)
                {
                    Common.WriteLine("Send value: " + sendItemsCount*SeriesBase);
                }
            }
            _endTime = Millis;
            // Now wait until receiving party acknowledges that values have arrived
            while (!_sendSeriesFinished)
            {
                Thread.Sleep(10);
            }
        }

        // Tools
        private static int CountBytesInCommand(Command command, bool printLfCr)
        {
            var bytes = command.CommandString().Length; // Command + command separator
            if (printLfCr) bytes += Environment.NewLine.Length; // Add  bytes for carriage return ('\r') and /or a newline  ('\n')
            return bytes;
        }

        // Return Milliseconds since 1970
        public static long Millis { get { return (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds); } }
    }
}
