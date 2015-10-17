using System;
using CommandMessenger;


namespace CommandMessengerTests
{
    public class MultipleArguments 
    {        
        private CmdMessenger _cmdMessenger;
        readonly Enumerator _command;
        private readonly systemSettings _systemSettings;
        private readonly System.Random _randomNumber;

        public MultipleArguments(systemSettings systemSettings, Enumerator command)
        {
            _systemSettings = systemSettings;
            _command = command;
            DefineCommands();
            _randomNumber = new System.Random(DateTime.Now.Millisecond);               
        }

        // ------------------ Command Callbacks -------------------------
        private void DefineCommands()
        {
            _command.Add(new[]
                {
                    "MultiValuePing"          , //  Send value
                    "MultiValuePong"          , //  Receive value
                });
        }

        private void AttachCommandCallBacks()
        {
        }

        // ------------------ Test functions -------------------------

        public void RunTests()
        {

            Common.StartTestSet("Clear binary data");
            SetUpConnection();
            TestSendMultipleValues();
            CloseConnection();
            Common.EndTestSet();
        }

        public void SetUpConnection()
        {
            _cmdMessenger = Common.Connect(_systemSettings);
            AttachCommandCallBacks();
        }

        public void CloseConnection()
        {
            Common.Disconnect();
        }

        public void TestSendMultipleValues()
        {
            Common.StartTest("Ping-pong of a command with handpicked binary int16, int32, and double parameters");
            //_cmdMessenger.LogSendCommandsEnabled = true;
            ValuePingPongBinInt16Int32Double(
                -11776,
                -1279916419,
                -2.7844819605867E+38
                );
            //_cmdMessenger.LogSendCommandsEnabled = false;
            Common.EndTest();

            Common.StartTest("Ping-pong of a command with random binary int16, int32, and double parameters");
            for (var i = 0; i < 1000; i++)
            {
                // Bigger values than this go wrong, due to truncation
                ValuePingPongBinInt16Int32Double(
                    Random.RandomizeInt16(Int16.MinValue, Int16.MaxValue),
                    Random.RandomizeInt32(Int32.MinValue, Int32.MaxValue),
                    Random.RandomizeDouble(
                        (_systemSettings.BoardType == BoardType.Bit32 ? double.MinValue : float.MinValue), 
                        (_systemSettings.BoardType == BoardType.Bit32 ? double.MaxValue : float.MaxValue)
                        ));
            }
            Common.EndTest();
        }

        private void ValuePingPongBinInt16Int32Double(Int16 int16Value, Int32 int32Value, double doubleValue)
        {
            var pingCommand = new SendCommand(_command["MultiValuePing"], _command["MultiValuePong"], 1000);
            pingCommand.AddBinArgument(int16Value);
            pingCommand.AddBinArgument(int32Value);
            pingCommand.AddBinArgument(doubleValue);
            var pongCommand = _cmdMessenger.SendCommand(pingCommand);

            if (!pongCommand.Ok)
            {
                Common.TestNotOk("No response on ValuePing command");
                return;
            }
            var int16Result = pongCommand.ReadBinInt16Arg();
            var int32Result = pongCommand.ReadBinInt32Arg();
            var doubleResult = pongCommand.ReadBinDoubleArg();

            if (int16Result == int16Value)
                Common.TestOk("1st parameter value, Int16, as expected: " + int16Result);
            else
                Common.TestNotOk("unexpected 1st parameter value received: " + int16Result + " instead of " + int16Value);

            if (int32Result == int32Value)
                Common.TestOk("2nd parameter value, Int32, as expected: " + int32Result);
            else
                Common.TestNotOk("unexpected 2nd parameter value, Int32, received: " + int32Result + " instead of " + int32Value);

            // For 16bit, because of double-float-float-double casting a small error is introduced
            var accuracy = (_systemSettings.BoardType == BoardType.Bit32) ? double.Epsilon : Math.Abs(doubleValue * 1e-6);
            var difference = Math.Abs(doubleResult - doubleValue);
            if (difference <= accuracy)
                Common.TestOk("3rd parameter value, Double, as expected: " + doubleResult);
            else
                Common.TestNotOk("unexpected 3rd parameter value, Double, received: " + doubleResult + " instead of " + doubleValue);
        } 
    }
}
