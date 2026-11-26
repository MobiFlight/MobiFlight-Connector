using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Joysticks.Octavi.Tests
{
    internal class TestableOctavi : Octavi
    {
        public List<(string name, MobiFlightButton.InputEvent inputEvent)> SentEvents { get; } = [];
        public override string Name => "IFR1";
        public override string Serial => "JS-OCTAVI-TEST";
        public Action<string> OnSent { get; set; }
        public Action OnSendData { get; set; }

        public TestableOctavi(JoystickDefinition definition) : base(null, definition)
        {
            OnButtonPressed += (s, e) =>
            {
                SentEvents.Add((e.Device.Name, (MobiFlightButton.InputEvent)(int)e.Value));
                OnSent?.Invoke(e.Device.Name);
            };
            EnumerateDevices();
        }

        // no device to connect to
        public override void Update()
        {
        }

        protected override void SendData(byte[] data)
        {
            OnSendData?.Invoke();
        }

        public void Receive(OctaviReport.OctaviState context)
        {
            ProcessInputReport(new HidReport([0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, (byte)context]));
        }
    }

    [TestClass]
    public class OctaviTests
    {
        private static TestableOctavi CreateOctavi()
        {
            // The contexts with a shift mode, as in octavi.joystick.json: COM1, COM2, NAV1, NAV2, XPDR
            var definition = new JoystickDefinition
            {
                Inputs = new[] { 0, 1, 2, 3, 7 }
                    .Select(id => new JoystickInput { Id = id, Type = JoystickDeviceType.Button })
                    .ToList()
            };
            return new TestableOctavi(definition);
        }

        private static List<(string name, MobiFlightButton.InputEvent inputEvent)> Tapped(params string[] names)
        {
            return [.. names.SelectMany(name => new[] { (name, MobiFlightButton.InputEvent.PRESS), (name, MobiFlightButton.InputEvent.RELEASE) })];
        }

        #region Retrigger and stop

        [TestMethod]
        public void Retrigger_ShouldSendNothing_UntilTheNextReport()
        {
            // Arrange
            var octavi = CreateOctavi();
            octavi.Receive(OctaviReport.OctaviState.STATE_NAV1);
            octavi.SentEvents.Clear();

            // Act
            octavi.Retrigger();

            // Assert
            Assert.IsEmpty(octavi.SentEvents);
        }

        [TestMethod]
        public void Retrigger_ShouldSendOnlyTheContextWithTheNextReport()
        {
            // Arrange
            var octavi = CreateOctavi();
            octavi.Receive(OctaviReport.OctaviState.STATE_NAV1);
            octavi.Retrigger();
            octavi.SentEvents.Clear();

            // Act
            octavi.Receive(OctaviReport.OctaviState.STATE_NAV1);

            // Assert
            CollectionAssert.AreEqual(Tapped("Button_CONTEXT_NAV1"), octavi.SentEvents);
        }

        [TestMethod]
        public void Stop_ShouldSendTheContextWithTheNextReport()
        {
            // Arrange
            var octavi = CreateOctavi();
            octavi.Receive(OctaviReport.OctaviState.STATE_XPDR);
            octavi.Stop();
            octavi.SentEvents.Clear();

            // Act
            octavi.Receive(OctaviReport.OctaviState.STATE_XPDR);

            // Assert
            CollectionAssert.AreEqual(Tapped("Button_CONTEXT_XPDR"), octavi.SentEvents);
        }

        [TestMethod]
        public void Stop_ShouldSendTheContextWithTheNextReport_WhenClearingTheOutputsFails()
        {
            // Arrange
            var octavi = CreateOctavi();
            octavi.Receive(OctaviReport.OctaviState.STATE_COM2);
            octavi.OnSendData = () => throw new InvalidOperationException("device gone");
            Assert.ThrowsExactly<InvalidOperationException>(() => octavi.Stop());
            octavi.OnSendData = null;
            octavi.SentEvents.Clear();

            // Act
            octavi.Receive(OctaviReport.OctaviState.STATE_COM2);

            // Assert
            CollectionAssert.AreEqual(Tapped("Button_CONTEXT_COM2"), octavi.SentEvents);
        }

        [TestMethod]
        public void Retrigger_ShouldSendWithTheNextReport_WhenAnInputActionOfTheReportRetriggers()
        {
            // Arrange
            var octavi = CreateOctavi();
            octavi.Receive(OctaviReport.OctaviState.STATE_COM1);
            octavi.SentEvents.Clear();
            var retriggered = false;
            octavi.OnSent = name =>
            {
                if (name == "Button_UNSHIFT_SW" && !retriggered)
                {
                    retriggered = true;
                    octavi.Retrigger();
                }
            };

            // Act
            octavi.Receive(OctaviReport.OctaviState.STATE_AP);
            var reportEvents = octavi.SentEvents.ToList();
            octavi.SentEvents.Clear();
            octavi.Receive(OctaviReport.OctaviState.STATE_AP);

            // Assert
            CollectionAssert.AreEqual(Tapped("Button_UNSHIFT_SW", "Button_CONTEXT_AP"), reportEvents);
            CollectionAssert.AreEqual(Tapped("Button_CONTEXT_AP"), octavi.SentEvents);
        }

        #endregion
    }
}
