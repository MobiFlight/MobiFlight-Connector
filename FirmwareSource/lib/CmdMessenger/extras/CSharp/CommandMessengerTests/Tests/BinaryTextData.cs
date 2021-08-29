using System;
using System.Threading;
using CommandMessenger;


namespace CommandMessengerTests
{
    public class BinaryTextData 
    {        
        private CmdMessenger _cmdMessenger;
        readonly Enumerator _command;
        private readonly systemSettings _systemSettings;

        public BinaryTextData(systemSettings systemSettings, Enumerator command)
        {
            _systemSettings = systemSettings;
            _command = command;
            DefineCommands();            
        }


        // ------------------ Command Callbacks -------------------------
        private void DefineCommands()
        {
        }

        private void AttachCommandCallBacks()
        {
        }

        // ------------------ Test functions -------------------------

        public void RunTests()
        {
            // Wait a bit before starting the test
            Thread.Sleep(1000);

            Common.StartTestSet("Clear binary data");
            SetUpConnection();
            TestSendBoolData();
            TestSendEscStringData();
            TestSendBinInt16Data();
            TestSendBinInt32Data();
            TestSendBinFloatData();
            TestSendBinDoubleData();
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
            Common.StartTest("Ping-pong of random binary bool values");
            // Try a lot of random numbers
            for (var i = 0; i < 100; i++)
            {
                ValuePingPongBinBool(Random.RandomizeBool());
            }
            Common.EndTest();
        }

        public void TestSendBinInt16Data()
        {
            Common.StartTest("Ping-pong of random binary Int16 values");
            // Try a lot of random numbers
            for (var i = 0; i < 100; i++)
            {
                ValuePingPongBinInt16(Random.RandomizeInt16(Int16.MinValue, Int16.MaxValue), 0);
            }
            Common.EndTest();
        }

        public void TestSendBinInt32Data()
        {
            Common.StartTest("Ping-pong of random binary Int32 values");
            // Try a lot of random numbers
            for (var i = 0; i < 100; i++)
            {
                ValuePingPongBinInt32(Random.RandomizeInt32(Int32.MinValue, Int32.MaxValue), 0);
            }
            Common.EndTest();
        }

        private void TestSendBinFloatData()
        {
            Common.StartTest("Ping-pong of handpicked binary float values");            

            // Try some typical numbers
            ValuePingPongBinFloat(0.0F) ;
            ValuePingPongBinFloat(1.0F);
            ValuePingPongBinFloat(15.0F);
            ValuePingPongBinFloat(65535.0F);

            ValuePingPongBinFloat(0.00390625F);
            ValuePingPongBinFloat(0.00000000023283064365386962890625F);
            Common.EndTest();


            //Craft difficult floating point values, using all special characters.
            //These should all be handled correctly by escaping

            Common.StartTest("Ping-pong of floating point values, using all special characters");
            for (int a = 0; a < 5; a++)
            {
                for (int b = 0; b < 5; b++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        for (int d = 0; d < 5; d++)
                        {
                            var charA = IntToSpecialChar(a);
                            var charB = IntToSpecialChar(b);
                            var charC = IntToSpecialChar(c);
                            var charD = IntToSpecialChar(d);
                            ValuePingPongBinFloat(CreateFloat(new[] { charA, charB, charC, charD }));
                        }
                    }
                }
            }
            Common.EndTest();

            Common.StartTest("Ping-pong of random binary float values"); 
            // Try a lot of random numbers
            for (int i = 0; i < 1000; i++)
            {
                ValuePingPongBinFloat(Random.RandomizeFloat(-float.MaxValue / 100.0f, float.MaxValue / 100.0f));
            }
            Common.EndTest();
        }

        public void TestSendBinDoubleData()
        {
            var range = (_systemSettings.BoardType == BoardType.Bit32) ? double.MaxValue : float.MaxValue;
            var stepsize = range / 100;
            Common.StartTest("Ping-pong of increasing binary double values");
            // Try a lot of random numbers
            for (var i = 0; i < 100; i++)
            {
                ValuePingPongBinDouble(i * stepsize);
            }
            Common.EndTest();
            Common.StartTest("Ping-pong of random binary double values");
            for (var i = 0; i < 100; i++)
            {
                // Bigger values than this go wrong, due to truncation
                ValuePingPongBinDouble(Random.RandomizeDouble(-range, range));
            }
            Common.EndTest();
        }

        private void TestSendEscStringData()
        {
            Common.StartTest("Echo strings");
            ValuePingPongEscString("abcdefghijklmnopqrstuvwxyz"); // No special characters, but escaped
            ValuePingPongEscString("abcde,fghijklmnopqrs,tuvwxyz"); //  escaped parameter separators
            ValuePingPongEscString("abcde,fghijklmnopqrs,tuvwxyz,"); //  escaped parameter separators at end
            ValuePingPongEscString("abc,defghij/klmnop//qr;stuvwxyz/"); // escaped escape char at end
            ValuePingPongEscString("abc,defghij/klmnop//qr;stuvwxyz//"); // double escaped escape  char at end
            Common.EndTest();
        }

        private void ValuePingPongBinBool(bool value)
        {
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.BBool);
            pingCommand.AddBinArgument(value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }

            var result = pongCommand.ReadBinBoolArg();
            if (result == value)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
        }

        private void ValuePingPongBinInt16(Int16 value, Int16 accuracy)
        {
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.BInt16);
            pingCommand.AddBinArgument(value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok) Common.TestNotOk("No response on ValuePing command");

            var result = pongCommand.ReadBinInt16Arg();

            var difference = Math.Abs(result - value);
            if (difference <= accuracy)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);

        }

        private void ValuePingPongBinInt32(Int32 value, Int32 accuracy)
        {
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.BInt32);
            pingCommand.AddBinArgument((Int32)value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }

            var result = pongCommand.ReadBinInt32Arg();

            var difference = Math.Abs(result - value);
            if (difference <= accuracy)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
        }

        private void ValuePingPongBinFloat(float value)
        {
            const float accuracy = float.Epsilon;
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.BFloat);
            pingCommand.AddBinArgument(value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }

            var result = pongCommand.ReadBinFloatArg();

            var difference = Math.Abs(result - value);
            if (difference <= accuracy)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
        }

        private void ValuePingPongBinDouble(double value)
        {
            
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((Int16)DataType.BDouble);
            pingCommand.AddBinArgument(value);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }

            var result = pongCommand.ReadBinDoubleArg();

            var difference = Math.Abs(result - value);

            // 
            // For 16bit, because of double-float-float-double casting a small error is introduced
            var accuracy = (_systemSettings.BoardType == BoardType.Bit32) ? double.Epsilon : Math.Abs(value * 1e-6);

            if (difference <= accuracy)
                Common.TestOk("Value as expected");
            else
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
        }

        private void ValuePingPongEscString(string value)
        {
            var pingCommand = new SendCommand(_command["ValuePing"], _command["ValuePong"], 1000);
            pingCommand.AddArgument((int)DataType.EscString);
            pingCommand.AddBinArgument(value); // Adding a string as binary command will escape it
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }

            var result = pongCommand.ReadBinStringArg();
            if (value == result)
            {
                Common.TestOk("Value as expected");
            }
            else
            {
                Common.TestNotOk("unexpected value received: " + result + " instead of " + value);
            }
        }

        // Utility functions
        private char IntToSpecialChar(int i)
        {
            switch (i)
            {
                case 0:
                    return ';'; // End of line
                case 1:
                    return ','; // End of parameter
                case 3:
                    return '/'; // Escaping next char
                case 4:
                    return '\0'; // End of byte array
                default:
                    return 'a'; // Normal character

            }
        }
        
        float CreateFloat(char[] chars)
        {
            var bytes = BinaryConverter.CharsToBytes(chars); 
            return BitConverter.ToSingle(bytes, 0);
        } 
    }
}
