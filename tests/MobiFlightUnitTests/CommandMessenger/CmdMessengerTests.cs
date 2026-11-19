using CommandMessenger;
using CommandMessenger.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobiFlight.Tests.CommandMessenger
{
    [TestClass]
    public class CmdMessengerTests
    {
        private const int GetInfoCommand = 9;
        private const int InfoCommand = 10;
        private const int ReplyTimeoutMs = 200;

        private const string InfoReply = "10,MobiFlight Mega,Nano,SN-G5E-45E,3.1.4,3.1.4;\r\n";

        #region ClearReceiveBuffers Tests

        [TestMethod]
        public void ClearReceiveBuffers_ShouldDropIncompleteMessage_SoTheNextReplyIsRecognized()
        {
            // Arrange
            // A message cut off before its ';' is left over, e.g. from before the port was opened.
            var transport = new FeedableTransport(InfoReply);
            var messenger = CreateMessenger(transport);
            transport.Feed("7,Butt");

            // Act
            messenger.ClearReceiveBuffers();
            var reply = messenger.SendCommand(new SendCommand(GetInfoCommand, InfoCommand, ReplyTimeoutMs));

            // Assert
            Assert.IsTrue(reply.Ok, "The Info reply should be recognized after the leftover bytes were dropped.");
            Assert.AreEqual(InfoCommand, reply.CmdId);
            Assert.AreEqual("MobiFlight Mega", reply.ReadStringArg());
        }

        [TestMethod]
        public void ClearReceiveBuffers_ShouldNotAffectTheNextReply_WhenBuffersAreEmpty()
        {
            // Arrange
            var transport = new FeedableTransport(InfoReply);
            var messenger = CreateMessenger(transport);

            // Act
            messenger.ClearReceiveBuffers();
            var reply = messenger.SendCommand(new SendCommand(GetInfoCommand, InfoCommand, ReplyTimeoutMs));

            // Assert
            Assert.IsTrue(reply.Ok, "The Info reply should be recognized.");
            Assert.AreEqual(InfoCommand, reply.CmdId);
        }

        #endregion

        private static CmdMessenger CreateMessenger(ITransport transport)
        {
            var messenger = new CmdMessenger(transport, BoardType.Bit16, ',', ';', '\\', 90);
            messenger.Connect();
            return messenger;
        }

        /// <summary>
        /// Test double for the serial port. Feed() delivers bytes that arrive unprompted;
        /// each write is answered with the next scripted reply.
        /// </summary>
        private class FeedableTransport : ITransport
        {
            private readonly Queue<string> _replies;
            private string _pending = string.Empty;

            public event EventHandler DataReceived;

            public FeedableTransport(params string[] replies)
            {
                _replies = new Queue<string>(replies);
            }

            public void Feed(string data)
            {
                _pending += data;
                DataReceived?.Invoke(this, EventArgs.Empty);
            }

            public bool Connect() => true;

            public bool Disconnect() => true;

            public bool IsConnected() => true;

            public void Dispose()
            {
            }

            public byte[] Read()
            {
                var data = Encoding.ASCII.GetBytes(_pending);
                _pending = string.Empty;
                return data;
            }

            public void Write(byte[] buffer)
            {
                if (_replies.Count == 0) return;

                // CmdMessenger registers the reply it waits for before writing,
                // so delivering right away does not depend on thread scheduling.
                Feed(_replies.Dequeue());
            }
        }
    }
}
