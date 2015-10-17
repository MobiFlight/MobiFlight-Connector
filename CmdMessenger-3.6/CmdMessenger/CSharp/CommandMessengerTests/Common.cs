#region CmdMessenger - MIT - (c) 2014 Thijs Elenbaas.
/*
  CmdMessenger - library that provides command based messaging

  Permission is hereby granted, free of charge, to any person obtaining
  a copy of this software and associated documentation files (the
  "Software"), to deal in the Software without restriction, including
  without limitation the rights to use, copy, modify, merge, publish,
  distribute, sublicense, and/or sell copies of the Software, and to
  permit persons to whom the Software is furnished to do so, subject to
  the following conditions:

  The above copyright notice and this permission notice shall be
  included in all copies or substantial portions of the Software.

  Copyright 2014 - Thijs Elenbaas
*/
#endregion
using System;
using CommandMessenger;
using CommandMessenger.Serialport;
using CommandMessenger.TransportLayer;
using System.IO;


namespace CommandMessengerTests
{
    public class Common
    {
        //private const string TestLogFile = "TestLogFile.txt";

        private const string IdentTp = @"      "; // indentation test part
        private const string IdentTt = @"   "; // indentation test 
        private const string IdentTs = @" "; // indentation test series
        private const string IdentSt = @"      ";
        private const string IdentWn = @"      ";
        public static CmdMessenger CmdMessenger { get; set; }
        public static SerialTransport SerialTransport { get; set; }

        private static bool _loggingCommands = false;
        private static bool _testStarted     = false;
        private static bool _testSetStarted  = false;

        private static string _testDescription    = "";
        private static string _testSetDescription = "";

        private static int _testElementFailCount = 0;
        private static int _testElementPassCount = 0;


        private static int _testFailCount = 0;
        private static int _testPassCount = 0;

        private static int _testSetFailCount = 0;
        private static int _testSetPassCount = 0;

        private static StreamWriter _streamWriter;


        public static CmdMessenger Connect(systemSettings systemSettings)
        {
            CmdMessenger = new CmdMessenger(systemSettings.Transport, systemSettings.sendBufferMaxLength) {BoardType = systemSettings.BoardType};
            // Attach to NewLineReceived and NewLineSent for logging purposes
            LogCommands(true);

            CmdMessenger.Connect();
            return CmdMessenger;
        }

        public static void LogCommands(bool logCommands)
        {
          if (logCommands && !_loggingCommands)
          {
              CmdMessenger.NewLineReceived += NewLineReceived;
              CmdMessenger.NewLineSent += NewLineSent;
              _loggingCommands = true;
          }
          else if (!logCommands && _loggingCommands)
          {
              // ReSharper disable DelegateSubtraction
              CmdMessenger.NewLineReceived -= NewLineReceived;
              CmdMessenger.NewLineSent -= NewLineSent;
              _loggingCommands = false;
              // ReSharper restore DelegateSubtraction
          }
        }

        public static void OpenTestLogFile(string testLogFile) 
        {
            _streamWriter = new StreamWriter(testLogFile);
        }


        public static void CloseTestLogFile() 
        {           
              _streamWriter.Close();
              _streamWriter = null;
        }

        public static void Disconnect()
        {
            LogCommands(false);
            CmdMessenger.Disconnect();
            CmdMessenger.Dispose();            
        }


        // Remove beeps
        public static string Silence(string input)
        {
            var output = input.Replace('\x0007', ' ');
            return output;
        }

        public static void StartTestSet(string testSetDescription)
        {
            if (_testSetStarted)
            {
                EndTestSet();
            }            
            _testSetDescription = testSetDescription;
            WriteLine(IdentTs + "*************************************");
            WriteLine(IdentTs + "*** Start test-set " + _testSetDescription + " ****");
            WriteLine(IdentTs + "*************************************");
            WriteLine();
            _testFailCount = 0;
            _testPassCount = 0;
            _testSetStarted = true;
        }


        public static void EndTestSet()
        {
            WriteLine(IdentTs + "*************************************");
            WriteLine(IdentTs + "*** End test-set " + _testSetDescription + " ****");
            WriteLine(IdentTs + "*************************************");
            WriteLine();
            WriteLine(IdentTs + "Tests passed: " + _testPassCount);
            WriteLine(IdentTs + "Tests failed: " + _testFailCount);
            WriteLine();
            if (_testFailCount > 0)
            {
                _testSetFailCount++;
            }
            else
            {
                _testSetPassCount++;
            }
            _testSetStarted = false;

        }

        public static void StartTest(string testDescription)
        {
            if (_testStarted)
            {
                EndTest();
            }
            _testDescription = testDescription;
            WriteLine(IdentTt + "*** Start test " + _testDescription + " ****");
            WriteLine();
            _testElementPassCount = 0;
            _testElementFailCount = 0;
            _testStarted = true;
        }

        public static void EndTest()
        {
            WriteLine(IdentTt + "*** End test " + _testDescription + " ****");
            WriteLine();
            if (_testElementPassCount + _testElementFailCount == 0)
            {
                WriteLine(IdentTt + "No tests done");
            }
            else
            {
                if (_testElementFailCount > 0)
                {
                    _testFailCount++;
                    WriteLine(IdentTt + "Test failed" );
                }
                else
                {
                    _testPassCount++;
                    WriteLine(IdentTt + "Test passed");
                } 
                if (_testElementPassCount + _testElementFailCount > 1)
                {
                    WriteLine(IdentTt + "Test parts passed: " + _testElementPassCount);
                    WriteLine(IdentTt + "Test parts failed: " + _testElementFailCount);
                }
                WriteLine();
            }

            _testStarted = false;

            if (_streamWriter != null) { _streamWriter.Flush(); }
        }


        public static void TestSummary()
        {
            WriteLine(IdentTs + "*** Test Summary ****");
            WriteLine();

            if (_testSetPassCount > 0 && _testSetFailCount > 0)
            {
                WriteLine(IdentTs + "Some test sets failed!! ");
            }
            else if (_testSetPassCount > 0 && _testSetFailCount == 0)
            {
                WriteLine(IdentTs + "All test sets passed!! ");
            }
            else if (_testSetPassCount == 0 && _testSetFailCount > 0)
            {
                WriteLine(IdentTs + "All test sets failed!! ");
            } if (_testSetPassCount == 0 && _testSetFailCount == 0)
            {
                WriteLine(IdentTs + "No tests done!! ");
            }

            WriteLine(IdentTs + "Test sets passed: " + _testSetPassCount);
            WriteLine(IdentTs + "Test sets failed: " + _testSetFailCount);

            if (_streamWriter != null) { _streamWriter.Flush(); }
        }

        public static void TestOk(string resultDescription)
        {
            WriteLine(IdentTp + "OK: " + resultDescription);
            _testElementPassCount++;
        }

        public static void TestNotOk(string resultDescription)
        {
            WriteLine(IdentTp + "Not OK: " + resultDescription);
            _testElementFailCount++;
        }

        public static void NewLineReceived(object sender, NewLineEvent.NewLineArgs e)
        {
            var message = e.Command.CommandString();
            //var message = CmdMessenger.CurrentReceivedLine;
            WriteLine(IdentSt + "Received > " + Silence(message));
        }

        public static void NewLineSent(object sender, NewLineEvent.NewLineArgs e)
        {
            //// Log data to text box
            var message = e.Command.CommandString();
            WriteLine(IdentSt + "Sent > " + Silence(message));
        }

        public static void OnUnknownCommand(ReceivedCommand arguments)
        {
            // In response to unknown commands and corrupt messages
            WriteLine(IdentWn + "Warn > Command without attached callback received");
        }

        public static void WriteLine()
        {
            WriteLine("");
        }

        public static void WriteLine(string message)
        {
            Console.WriteLine(message);

            if (_streamWriter!=null) {
                _streamWriter.WriteLine(message);
            }
        }

        public static void Write(string message)
        {
            Console.Write(message);
            if (_streamWriter!=null) {
                _streamWriter.Write(message);
            }
        }
    }
}
