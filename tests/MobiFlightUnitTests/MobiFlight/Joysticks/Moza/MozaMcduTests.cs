using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Joysticks.Cdu;
using MobiFlight.Joysticks.Cdu.Tests.Mocks;
using MobiFlight.Joysticks.Moza.Tests.Mocks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.Moza.Tests
{
    [TestClass]
    public class MozaMcduTests
    {
        private static (MozaMcdu Device, FakeMozaScreenControl ScreenControl, FakeCduWebsocketHub Hub) CreateDevice()
        {
            var screenControl = new FakeMozaScreenControl();
            var hub = new FakeCduWebsocketHub();
            var device = new MozaMcdu(new JoystickDefinition(), hub, screenControl);
            return (device, screenControl, hub);
        }

        private static (MozaMcdu Device, FakeMozaScreenControl ScreenControl, FakeCduWebsocketHub Hub) CreateConnectedDeviceWithLeds()
        {
            var screenControl = new FakeMozaScreenControl();
            var hub = new FakeCduWebsocketHub();
            var definition = new JoystickDefinition
            {
                Outputs = new List<JoystickOutput>
                {
                    new JoystickOutput { Id = "FM1.led", Label = "FM1", Byte = 0, Bit = 0 },
                    new JoystickOutput { Id = "FAIL.led", Label = "FAIL", Byte = 0, Bit = 5 },
                }
            };
            var device = new MozaMcdu(definition, hub, screenControl);
            device.Connect(IntPtr.Zero);
            // Connecting already resyncs the (all-off) initial LED state - isolate each
            // test's assertions from that.
            screenControl.LedCalls.Clear();
            return (device, screenControl, hub);
        }

        [TestMethod]
        public void Connect_ConnectsScreenControl()
        {
            var (device, screenControl, _) = CreateDevice();

            device.Connect(System.IntPtr.Zero);

            Assert.IsTrue(screenControl.ConnectCalled);
        }

        [TestMethod]
        public void CabinPositionResolved_RegistersResolvedPathWithHub()
        {
            var (_, screenControl, hub) = CreateDevice();

            screenControl.RaiseCabinPositionResolved(0);

            Assert.HasCount(1, hub.Registered);
            Assert.AreEqual("/winwing/cdu-captain", hub.Registered[0].Path);
        }

        [TestMethod]
        public void CabinPositionResolved_RaisedTwice_RegistersOnlyOnce()
        {
            var (_, screenControl, hub) = CreateDevice();

            screenControl.RaiseCabinPositionResolved(0);
            screenControl.RaiseCabinPositionResolved(0);

            Assert.HasCount(1, hub.Registered);
        }

        [TestMethod]
        public void OnCduData_ForwardsJsonToScreenControl()
        {
            var (device, screenControl, _) = CreateDevice();

            ((ICduDataConsumer)device).OnCduData("{ \"Target\": \"Display\", \"Data\": [] }");

            Assert.Contains("Display", screenControl.SubmittedPages[0]);
        }

        [TestMethod]
        public void Shutdown_UnregistersFromHub_WhenRegistered()
        {
            var (device, screenControl, hub) = CreateDevice();
            screenControl.RaiseCabinPositionResolved(1);

            device.Shutdown();

            Assert.HasCount(1, hub.Unregistered);
            Assert.AreEqual("/winwing/cdu-co-pilot", hub.Unregistered[0].Path);
        }

        [TestMethod]
        public void Shutdown_NeverRegistered_DoesNotUnregister()
        {
            var (device, _, hub) = CreateDevice();

            device.Shutdown();

            Assert.IsEmpty(hub.Unregistered);
        }

        [TestMethod]
        public void ShowUserMessage_SubmitsWellFormed336CellPage()
        {
            var (device, screenControl, _) = CreateDevice();

            device.ShowUserMessage(UserMessageCodes.PYTHON_NOT_READY);

            Assert.HasCount(1, screenControl.SubmittedPages);
            var json = JObject.Parse(screenControl.SubmittedPages[0]);
            Assert.AreEqual("Display", (string)json["Target"]);
            Assert.HasCount(24 * 14, (JArray)json["Data"]);
        }

        [TestMethod]
        public void UpdateOutputDeviceStates_UnknownOutputName_DoesNotThrowAndSendsNothing()
        {
            var (device, screenControl, _) = CreateDevice();
            device.SetOutputDeviceState("anything", 1);

            device.UpdateOutputDeviceStates();

            Assert.IsEmpty(screenControl.LedCalls);
        }

        [TestMethod]
        public void UpdateOutputDeviceStates_NoChangePending_DoesNotCallSetLed()
        {
            var (device, screenControl, _) = CreateConnectedDeviceWithLeds();

            device.UpdateOutputDeviceStates();

            Assert.IsEmpty(screenControl.LedCalls);
        }

        [TestMethod]
        public void SetOutputDeviceState_ThenUpdate_TurnsOnTheMatchingLed()
        {
            var (device, screenControl, _) = CreateConnectedDeviceWithLeds();

            device.SetOutputDeviceState("FAIL", 1);
            device.UpdateOutputDeviceStates();

            Assert.Contains(((byte)5, true), screenControl.LedCalls);
        }

        [TestMethod]
        public void SetOutputDeviceState_PushesEveryLightsCurrentState_NotJustTheChangedOne()
        {
            var (device, screenControl, _) = CreateConnectedDeviceWithLeds();

            device.SetOutputDeviceState("FAIL", 1);
            device.UpdateOutputDeviceStates();

            Assert.HasCount(2, screenControl.LedCalls);
            Assert.Contains(((byte)0, false), screenControl.LedCalls); // FM1 stays off
            Assert.Contains(((byte)5, true), screenControl.LedCalls);  // FAIL turned on
        }

        [TestMethod]
        public void Connect_ResyncsLedStateSetBeforeScreenControlConnected()
        {
            var screenControl = new FakeMozaScreenControl();
            var hub = new FakeCduWebsocketHub();
            var definition = new JoystickDefinition
            {
                Outputs = new List<JoystickOutput>
                {
                    new JoystickOutput { Id = "FAIL.led", Label = "FAIL", Byte = 0, Bit = 5 },
                }
            };
            var device = new MozaMcdu(definition, hub, screenControl);

            device.Connect(IntPtr.Zero);

            Assert.Contains(((byte)5, false), screenControl.LedCalls);
        }
    }
}
