using System;
using System.Threading;
using CommandMessenger;

namespace CommandMessengerTests
{

    public class SetupConnection
    {
        readonly Enumerator _command;
        private readonly systemSettings _systemSettings;
        private CmdMessenger _cmdMessenger;

        public SetupConnection(systemSettings systemSettings, Enumerator command)
        {
            _systemSettings = systemSettings;            
            _command = command;
            DefineCommands();
        }

        public void DefineCommands()
        {
            _command.Add(new[]
                {
                    "Ack",              // acknowledgment that cmd was received
                    "AreYouReady",      // Command asking if other side is ready
                    "Err"               // Reports incorrectly formatted cmd, or cmd not recognized
                } );
        }

        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(Common.OnUnknownCommand);
        }

        public void RunTests()
        {
            // Wait a bit before starting the test
            Thread.Sleep(1000);

            // Test opening and closing connection
            Common.StartTestSet("Opening connections");
            TestOpenConnection();
            TestSendCommand();
            TestCloseConnection();
            Common.EndTestSet();
        }

        public void TestOpenConnection()
        {
            Common.StartTest("Test opening connection");
            try
            {
                _cmdMessenger = Common.Connect(_systemSettings);           
                AttachCommandCallBacks();
                if (!Common.Connected)
                {
                    Common.TestNotOk("Not connected after opening connection");
                }                
            }
            catch (Exception)
            {
                Common.TestNotOk("Exception during opening connection");
                Common.EndTest();
                if (_cmdMessenger==null) Common.EndTestSet();
                return;
            }

            if (_systemSettings.Transport.IsConnected())
            {
                Common.TestOk("No issues during opening connection");
            }
            else
            {
                Common.TestNotOk("Not open after trying to open connection");                
            }

            Common.EndTest();
        }

        public void TestCloseConnection()
        {
            Common.StartTest("Test closing connection");
            try
            {                
                Common.Disconnect();
            }
            catch (Exception)
            {
                Common.TestNotOk("Exception during opening connection");
                Common.EndTest();
                return;
            }
            Console.WriteLine("No issues during closing of connection");

            if (_systemSettings.Transport.IsConnected())
            {
                Common.TestNotOk("Transport connection still open after disconnection");
            }
            else
            {
                Common.TestOk("Transport connection not open anymore after disconnection");
            }

            Common.EndTest();
        }

        // Test: send a command without acknowledgment needed
        public void TestSendCommand()
        {
            Common.StartTest("Test sending command");
            try
            {
                _cmdMessenger.SendCommand(new SendCommand(_command["AreYouReady"]));
            }
            catch (Exception e)
            {
                Common.TestNotOk("Exception during sending of command: " + e.Message);
                Common.EndTest();
                return;
            }
            Common.TestOk("No issues during sending command");
            Common.EndTest();
        }
    }
}
