namespace MobiFlightMoza.Tests
{
    [TestClass]
    public class MozaScreenControlTests
    {
        // Connect() actually locating and opening a real port needs hardware and isn't
        // covered here, mirroring how HidReportReceiverTests fakes the Stream but never
        // exercises SwitchPanel's own HID device enumeration. What's testable without
        // hardware is every guard around it - all covered below.

        [TestMethod]
        public void Connect_HardwareIdsNotConfigured_ReturnsFalseWithoutRaisingError()
        {
            // Arrange - VID/PID are still placeholders (R-VIDPID); this is an expected
            // state, not a failure worth surfacing to the user.
            var control = new MozaScreenControl();
            bool errorRaised = false;
            control.ErrorMessageCreated += _ => errorRaised = true;
            // Act
            bool connected = control.Connect();
            // Assert
            Assert.IsFalse(connected);
            Assert.IsFalse(errorRaised);
        }

        [TestMethod]
        public void SubmitScreenData_BeforeConnect_DoesNotThrow()
        {
            // Arrange
            var control = new MozaScreenControl();
            // Act & Assert
            control.SubmitScreenData("{\"Target\":\"Display\",\"Data\":[]}");
        }

        [TestMethod]
        public void Stop_BeforeConnect_DoesNotThrow()
        {
            new MozaScreenControl().Stop();
        }

        [TestMethod]
        public void Shutdown_BeforeConnect_DoesNotThrow()
        {
            new MozaScreenControl().Shutdown();
        }

        [TestMethod]
        public void Shutdown_TwiceInARow_DoesNotThrow()
        {
            var control = new MozaScreenControl();
            control.Shutdown();
            control.Shutdown();
        }
    }
}
