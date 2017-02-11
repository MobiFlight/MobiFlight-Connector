using System;
using System.Threading;
using CommandMessenger;

namespace CommandMessengerTests
{

    enum DataType : int
    {
        Bool,
        Int16,
        Int32,
        Float,
        FloatSci,
        Double,
        DoubleSci,
        Char,
        String,
        BBool,
        BInt16,
        BInt32,
        BFloat,
        BDouble,
        BChar,
        EscString,
        
    };

    public class ClearTextData 
    {        
        private CmdMessenger _cmdMessenger;
        readonly Enumerator _command;
        private systemSettings _systemSettings;

        public ClearTextData(systemSettings systemSettings, Enumerator command)
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
                    "ValuePing"          , //  Send value
                    "ValuePong"          , //  Receive value
                });
        }

        private void AttachCommandCallBacks()
        {
        }

        // ------------------ Test functions -------------------------

        public void RunTests()
        {
            // Wait a bit before starting the test
            Thread.Sleep(1000);

            Common.StartTestSet("Clear text data");
            SetUpConnection();
            TestSendBoolData();
            TestSendInt16Data();
            TestSendInt32Data();
            TestSendFloatData();
            TestSendFloatSciData();
            TestSendDoubleSciData();
            CloseConnection();
            Common.EndTestSet();
        }

        public void SetUpConnection()
        {
            _cmdMessenger = Common.Connect(_systemSettings);
            AttachCommandCallBacks();
            if (!Common.Connected)
            {
                Common.TestNotOk("Not connected after opening connection");
            }
           
        }

        public void CloseConnection()
        {
            Common.Disconnect();

        }

        public void TestSendBoolData()
        {
            Common.StartTest("Ping-pong of random plain-text bool values");
            // Try a lot of random numbers
            for (var i = 0; i < 100; i++)
            {
                ValuePingPongBool(Random.RandomizeBool());
            }
            Common.EndTest();
        }

        public void TestSendInt16Data()
        {
            Common.StartTest("Ping-pong of random Int16 values");
            // Try a lot of random numbers
            for (var i = 0; i < 100; i++)
            {
                ValuePingPongInt16(Random.RandomizeInt16(Int16.MinValue, Int16.MaxValue), 0);
            }
            Common.EndTest();
        }

        public void TestSendInt32Data()
        {
            Common.StartTest("Ping-pong of random Int32 values");
            // Try a lot of random numbers
            for (var i = 0; i < 100; i++)
            {
                ValuePingPongInt32(Random.RandomizeInt32(Int32.MinValue, Int32.MaxValue), 0);
            }
            Common.EndTest();
        }

        public void TestSendFloatData()
        {
            // UInt32.MaxValue is the maximum range of the normal print float implementation
            const float stepsize = (float)UInt32.MaxValue / 100;
            Common.StartTest("Ping-pong of increasing float values");
            // Try a lot of random numbers
            for (var i = 0; i < 100; i++)
            {
                // Bigger values than this go wrong, due to truncation
                ValuePingPongFloat(i * stepsize );
            }
            Common.EndTest();
            Common.StartTest("Ping-pong of random float values");
            for (var i = 0; i < 100; i++)
            {
                // Bigger values than this go wrong, due to truncation
                ValuePingPongFloat(Random.RandomizeFloat(-UInt32.MaxValue, UInt32.MaxValue));
            }
            Common.EndTest();
        }

        public void TestSendFloatSciData()
        {
            const float stepsize = (float)float.MaxValue/100.0f;
            Common.StartTest("Ping-pong of increasing float values, returned in scientific format");
            // Try a lot of random numbers
            for (var i = 0; i < 100; i++)
            {
                // Bigger values than this go wrong, due to truncation
                ValuePingPongFloatSci(i * stepsize, (float)0.05);
            }
            Common.EndTest();
            Common.StartTest("Ping-pong of random float values, returned in scientific format");
            for (var i = 0; i < 100; i++)
            {
                ValuePingPongFloatSci(Random.RandomizeFloat(-float.MaxValue / 100.0f, float.MaxValue / 100.0f), 0.05f);
            }
            Common.EndTest();
        }

        public void TestSendDoubleSciData()
        {
            var range = (_systemSettings.BoardType==BoardType.Bit32)?double.MaxValue:float.MaxValue;
            var stepsize = range/100;
            Common.StartTest("Ping-pong of increasing double values, returned in scientific format");
            // Try a lot of random numbers
            for (var i = 0; i < 100; i++)
            {
                ValuePingPongDoubleSci(i * stepsize, 0.05f);
            }
            Common.EndTest();
            Common.StartTest("Ping-pong of random double values, returned in scientific format");
            for (var i = 0; i < 100; i++)
            {
                // Bigger values than this go wrong, due to truncation
                ValuePingPongDoubleSci(Random.RandomizeDouble(-range, range), 0.05f);
            }
            Common.EndTest();
        }

        private void ValuePingPongInt16(Int16 value, Int16 accuracy)
        {
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.Int16);
            pingCommand.AddArgument(value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok) Common.TestNotOk("No response on ValuePing command");

            var result = pongCommand.ReadInt16Arg();

            var difference = Math.Abs(result - value);

            if (difference <= accuracy)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
        }

        private void ValuePingPongInt32(Int32 value, Int32 accuracy)
        {
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.Int32);
            pingCommand.AddArgument((Int32)value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }

            var result = pongCommand.ReadInt32Arg();

            var difference = Math.Abs(result - value);
            if (difference <= accuracy)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
        }

        private void ValuePingPongBool(bool value)
        {
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.Bool);
            pingCommand.AddArgument(value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }

            var result = pongCommand.ReadBoolArg();

            if (result == value)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
        }

        private void ValuePingPongFloat(float value)
        {
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.Float);
            pingCommand.AddArgument(value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }

            var result = pongCommand.ReadFloatArg();
            var difference = Math.Abs(result - value);
            
            var accuracy = Math.Abs(value * 2e-7);

            if (difference <= accuracy)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
        }

        private void ValuePingPongFloatSci(float value, float accuracy)
        {
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.FloatSci);
            pingCommand.AddArgument(value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }

            var result = pongCommand.ReadFloatArg();

            var difference = RelativeError(value, result);
            if (difference <= accuracy)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
        }

        private void ValuePingPongDoubleSci(double value, double accuracy)
        {
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.DoubleSci);
            pingCommand.AddArgument(value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }

            var result = pongCommand.ReadDoubleArg();

            var difference = RelativeError(value, result);
            
            if (difference <= accuracy)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
        }

        private static double RelativeError(double value, double result)
        {
            var difference = (Math.Abs(result) > double.Epsilon) ? Math.Abs(result - value) / result : Math.Abs(result - value);
            if (Double.IsNaN(difference)) difference = 0;
            return difference;
        }
    }
}
