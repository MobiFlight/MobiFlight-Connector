using CommandMessenger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlightUnitTests.mock.CommandMessenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class MobiFlightCustomDeviceTests
    {
        private void WaitForQueueUpdate()
        {
            var task = Task.Run(() =>
            {
                Thread.Sleep(100);
            });
            task.Wait();
        }

        [TestMethod()]
        public void DisplayTest()
        {
            byte CommandId = (byte)MobiFlightModule.Command.SetCustomDevice;
            string value = "12345678";
            var messageType = "3";

            var module = new MobiFlightCustomDevice()
            {
                DeviceNumber = 0
            };
            var mockTransport = new MockTransport();
            var DataExpected = "";
            module.CmdMessenger = new CmdMessenger(mockTransport);
            module.CmdMessenger.Connect();

            /// TEST CASES
            mockTransport.Clear();
            module.Display(messageType, value);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{module.DeviceNumber},{messageType},{value};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "First write should always send command.");
        }

        [TestMethod()]
        [Ignore]
        public void StopTest()
        {
            Assert.Fail();
        }
    }
}