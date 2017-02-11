using System;
using System.Threading;
using CommandMessenger;

namespace CommandMessengerTests
{
    public class Acknowledge 
    {        
        private CmdMessenger _cmdMessenger;
        readonly Enumerator _command;
        private bool _acknowledgementByEmbeddedFinished;
        private readonly systemSettings _systemSettings;


        public Acknowledge(systemSettings systemSettings, Enumerator command)
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
                    "AskUsIfReady", // Command asking other side to check if we acknowledge
                    "YouAreReady"   // Command to send to other side to tell them we received their acknowledgment                     
                });
        }

        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(_command["AreYouReady"], OnAreYouReadyCommand);
            _cmdMessenger.Attach(_command["YouAreReady"], OnYouAreReadyCommand);
        }


        // ------------------ Command Callbacks -------------------------

        private void OnAreYouReadyCommand(ReceivedCommand arguments)
        {
            // In response to AreYouReady ping. We send an ACK acknowledgment to say that we are ready
            _cmdMessenger.SendCommand(new SendCommand(_command["Ack"], "We are ready"));
        }

        private void OnYouAreReadyCommand(ReceivedCommand arguments)
        {
            // in response to YouAreReady message 
            TestSendCommandWithAcknowledgementByArduinoFinished(arguments);
        }

        // ------------------ Test functions -------------------------

        public void RunTests()
        {
            // Wait a bit before starting the test
            Thread.Sleep(1000);

            // Test opening and closing connection
            Common.StartTestSet("Waiting for acknowledgments");
            SetUpConnection();
            
            // Test acknowledgments
            TestSendCommandWithAcknowledgement();
            TestSendCommandWithAcknowledgementByArduino();
            WaitForAcknowledgementByEmbeddedFinished();
            TestSendCommandWithAcknowledgementAfterQueued();

            // Test closing connection
            CloseConnection();
            Common.EndTestSet();
        }

        public void SetUpConnection()
        {
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
                Common.TestNotOk("CmdMessenger application could not be created");
                Common.EndTestSet();
            }
            if (!_systemSettings.Transport.IsConnected())
            {
                Common.TestOk("No issues during opening connection");
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

        // Test: Send a test command with acknowledgment needed
        public void TestSendCommandWithAcknowledgement()
        {
            Common.StartTest("Test sending command and receiving acknowledgment");            
            var receivedCommand = _cmdMessenger.SendCommand(new SendCommand(_command["AreYouReady"], _command["Ack"], 1000)) ;
            if (receivedCommand.Ok)
            {
                Common.TestOk("Acknowledgment for command AreYouReady");                
            }
            else
            {
                Common.TestNotOk("No acknowledgment for command AreYouReady");
            }
            Common.EndTest();
        }


        public void TestSendCommandWithAcknowledgementAfterQueued()
        {
            Common.StartTest("Test sending command and receiving acknowledgment after larger queue");
            
            // Quickly sent a bunch of commands, that will be combined in a command string
            for (var i = 0; i < 100; i++)
            {
                _cmdMessenger.QueueCommand(new SendCommand(_command["AreYouReady"]));
            }

            // Now wait for an acknowledge, terminating the command string
            var receivedCommand = _cmdMessenger.SendCommand(new SendCommand(_command["AreYouReady"], _command["Ack"], 1000), SendQueue.Default, ReceiveQueue.WaitForEmptyQueue);
            if (receivedCommand.Ok)
            {
                Common.TestOk("Acknowledgment for command AreYouReady");                
            }
            else
            {
                Common.TestNotOk("No acknowledgment for command AreYouReady");
            }
            Common.EndTest();
        }        

        public void TestSendCommandWithAcknowledgementByArduino()
        {
            Common.StartTest("TestSendCommandWithAcknowledgementByArduino");
            //SendCommandAskUsIfReady();
            _acknowledgementByEmbeddedFinished = false;
            _cmdMessenger.SendCommand(new SendCommand(_command["AskUsIfReady"]));

            // We will exit here, but the test has just begun:
            // - Next the arduino will call us with AreYouReady command which will trigger OnAreYouReadyCommand() 
            // - After this the Command TestAckSendCommandArduinoFinish will be called by Arduino with results
        }

        public void TestSendCommandWithAcknowledgementByArduinoFinished(ReceivedCommand command)
        {
            var result = command.ReadBoolArg();
            if (!result)
            {
                Common.TestNotOk("Incorrect response");
            }
            _acknowledgementByEmbeddedFinished = true;
        }

        public void WaitForAcknowledgementByEmbeddedFinished()
        {
            for (var i = 0; i < 10; i++)
            {
                if (_acknowledgementByEmbeddedFinished)
                {
                    Common.TestOk("Received acknowledge from processor");
                    return;
                }
                System.Threading.Thread.Sleep(1000);
            }
            Common.TestNotOk("Received no acknowledge from  processor");
        }

    }
}
