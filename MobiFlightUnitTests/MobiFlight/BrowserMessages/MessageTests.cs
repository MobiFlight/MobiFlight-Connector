using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.BrowserMessages.Tests
{
    [TestClass()]
    public class MessageTests
    {
        [TestMethod()]
        public void MessageTest()
        {
            // Constructor tests
            Message<string> message = new Message<string>();
            Assert.IsNull(message.key);
            Assert.IsNull(message.payload);

            message = new Message<string>("key", "payload");
            Assert.AreEqual("key", message.key);
            Assert.AreEqual("payload", message.payload);

            message = new Message<string>("payload");
            Assert.AreEqual("String", message.key);
            Assert.AreEqual("payload", message.payload);
        }
    }
}