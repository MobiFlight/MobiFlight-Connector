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
    public class MobiFlightLedModuleTests
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
        public void MobiFlightLedModuleTest()
        {
            var module = new MobiFlightLedModule();
            Assert.IsNotNull(module, "MobiFlightLedModule is null");
            Assert.AreEqual(1, module.SubModules, "Default SubModules are not correct.");
            Assert.AreEqual(15, module.Brightness, "Default Brightness is not correct.");
        }

        [TestMethod()]
        public void DisplayTest()
        {
            byte points = 0x00;
            byte mask = 0xFF;
            int ModuleIndex = 0;
            int SubModuleIndex = 0;
            byte CommandId = (byte) MobiFlightModule.Command.SetModule;

            string value = "12345678";

            var module = new MobiFlightLedModule();
            var mockTransport = new MockTransport();
            var DataExpected = "";
            module.CmdMessenger = new CmdMessenger(mockTransport);
            module.CmdMessenger.Connect();

            /// TEST CASES
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},{value},{points},{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "First write should always send command.");

            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();

            DataExpected = "";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Caching mechanism test failed, we are sending the same value again, nothing has changed. Value in Mock should be \"\"");

            module.ClearState();
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},{value},{points},{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, $"Caching mechanism test failed, sending same value again, after ClearState. Value in Mock should be \"{value}\"");

            // Values stays same, but points change
            points = 128;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},{value},{points},{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, $"Caching mechanism test failed, sending different points. Points in Mock should be \"{points}\"");

            points = 128;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, $"Caching mechanism test failed, sending same points again. Value in Mock should be \"\"");

            // Mask change test, requires value change too
            // Because if we only change the mask the values on the display will not change.
            // Bare in mind that digit mask is reversed :(
            mask = 254;
            value = "12345678";
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();

            DataExpected = $"";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, $"Caching mechanism test failed, sending different mask, but values haven't changed in masked area. Mask in Mock should be \"{mask}\"");

            mask = 254;
            value = "12345608";
            var expectedValue = "1234560";
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();

            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},{expectedValue},{points},{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, $"Sending same mask, and value changed in masked area. Mask in Mock should be \"{mask}\"");

            // #
            /// https://github.com/MobiFlight/MobiFlight-Connector/issues/961
            points = 0xFF;
            mask = 0xFF;
            ModuleIndex = 0;
            SubModuleIndex = 0;
            CommandId = (byte)MobiFlightModule.Command.SetModule;

            value = "123";
            points = 0;
            mask = 112;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},{value},{points},{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "First write should always send command.");

            value = "456";
            points = 4;
            mask = 14;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},{value},{points},{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Second write should always send command.");

            value = "123";
            points = 0;
            mask = 112;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = "";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Caching mechanism test failed, we are sending the same value again, nothing has changed. Value in Mock should be \"\"");

            value = "1234567.8";
            points = 0;
            mask = 255;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},12345678,2,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");


            value = "1.2345678";
            points = 0;
            mask = 255;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},12345678,128,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");

            value = "1.2.3.4.5.6.7.8.";
            points = 0;
            mask = 255;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},12345678,255,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");

            value = "1.234";
            points = 0;
            mask = 8 + 4 + 2 + 1;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},1234,8,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");

            value = "1.234";
            points = 0;
            mask = 128 + 64 + 32 + 16;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},1234,128,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");

            value = "1.2";
            points = 0;
            mask = 0 + 4 + 0 + 1;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},12,4,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");

            value = ".123";
            points = 0;
            mask = 8 + 4 + 2 + 1;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex}, 123,8,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");


            value = "........";
            points = 0;
            mask = 255;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},        ,255,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");

            value = "1..2";
            points = 0;
            mask = 0 + 4 + 2 + 1;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},1 2,6,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");

            // https://github.com/MobiFlight/MobiFlight-Connector/issues/1062
            value = "123.50";
            points = 0;
            mask = 16 + 8 + 4 + 2 + 1;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},12350,4,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");

            value = "118.805000305176";
            points = 0;
            mask = 255;
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},11880500,32,{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");


            value = "1234";
            points = 0;
            mask = 8 + 4 + 2 + 1;
            var reverseMask = 128 + 64 + 32 + 16;
            mockTransport.Clear();
            module.Display(0, value, points, mask, true);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},4321,0,{reverseMask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");

            module.ClearState();
            value = "123456";
            points = 0;
            mask = 8 + 4 + 2 + 1;
            reverseMask = 128 + 64 + 32 + 16;
            mockTransport.Clear();
            module.Display(0, value, points, mask, true);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},4321,0,{reverseMask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Decimal point not set correctly");
        }

        [TestMethod()]
        public void SetBrightnessTest()
        {
            int ModuleIndex = 0;
            int SubModuleIndex = 0;
            byte CommandId = (byte)MobiFlightModule.Command.SetModuleBrightness;

            string brightness = "15";

            var module = new MobiFlightLedModule();
            var mockTransport = new MockTransport();
            module.CmdMessenger = new CmdMessenger(mockTransport);
            module.CmdMessenger.Connect();

            /// TEST CASES
            mockTransport.Clear();
            module.SetBrightness(0, brightness);
            WaitForQueueUpdate();
            var DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},{brightness};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "First write should always send command.");

            mockTransport.Clear();
            module.SetBrightness(0, brightness);
            WaitForQueueUpdate();
            DataExpected = $"";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Caching mechanism test failed, we are sending the same value again, nothing has changed.");

            brightness = "1";
            mockTransport.Clear();
            module.SetBrightness(0, brightness);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},{brightness};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write after brigthness change should always send command.");

            mockTransport.Clear();
            module.ClearState();
            module.SetBrightness(0, brightness);
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},{brightness};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write after ClearState should always send command.");
        }

        [TestMethod()]
        public void StopTest()
        {
            byte points = 0xFF;
            byte mask = 0xFF;
            int ModuleIndex = 0;
            int SubModuleIndex = 0;
            byte CommandId = (byte)MobiFlightModule.Command.SetModule;

            string value = "12345678";

            var module = new MobiFlightLedModule();
            var mockTransport = new MockTransport();
            module.CmdMessenger = new CmdMessenger(mockTransport);
            module.CmdMessenger.Connect();

            /// TEST CASES
            mockTransport.Clear();
            module.Display(0, value, points, mask);
            WaitForQueueUpdate();
            var DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},{value},{points},{mask};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "First write should always send command.");

            mockTransport.Clear();
            module.Stop();
            WaitForQueueUpdate();
            DataExpected = $"{CommandId},{ModuleIndex},{SubModuleIndex},        ,0,255;";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "First write should always send command.");
        }

        [TestMethod()]
        [Ignore]
        public void ClearStateTest()
        {
            // this is tested with the Display Tests
        }
    }
}