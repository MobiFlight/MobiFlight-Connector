using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class SerialNumberTests
    {
        [TestMethod()]
        public void ExtractSerialTest()
        {
            var serial = "GMA345/ SN-b44-4c5";
            var result = SerialNumber.ExtractSerial(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("SN-b44-4c5", result);

            serial = "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.ExtractSerial(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("JS-b0875190-3b89-11ed-8007-444553540000", result);

            serial = "Arcaze/ 000393600000";
            result = SerialNumber.ExtractSerial(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("000393600000", result);

            serial = "MFG Crosswind V2/3 / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.ExtractSerial(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("JS-b0875190-3b89-11ed-8007-444553540000", result);
        }

        [TestMethod()]
        public void ExtractDeviceNameTest()
        {
            var serial = "GMA345/ SN-b44-4c5";
            var result = SerialNumber.ExtractDeviceName(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("GMA345", result);

            serial = "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.ExtractDeviceName(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("Bravo Throttle Quadrant", result);

            serial = "Arcaze v5.36/ 000393600000";
            result = SerialNumber.ExtractDeviceName(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("Arcaze v5.36", result);

            serial = "MFG Crosswind V2/3 / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.ExtractDeviceName(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("MFG Crosswind V2/3", result);
        }

        [TestMethod()]
        public void ExtractPrefixTest() {
            var serial = "GMA345/ SN-b44-4c5";
            var result = SerialNumber.ExtractPrefix(serial);
            Assert.AreEqual(MobiFlightModule.SerialPrefix, result);

            serial = "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.ExtractPrefix(serial);
            Assert.AreEqual(Joystick.SerialPrefix, result);

            serial = "My MidiDevice/ MI-123456";
            result = SerialNumber.ExtractPrefix(serial);
            Assert.AreEqual(MidiBoard.SerialPrefix, result);

            serial = "Arcaze v5.36/ 000393600000";
            result = SerialNumber.ExtractPrefix(serial);
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void IsMobiFlightSerialTest()
        {
            var serial = "GMA345/ SN-b44-4c5";
            var result = SerialNumber.IsMobiFlightSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsTrue(result);

            serial = "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.IsMobiFlightSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsFalse(result);

            serial = "Arcaze v5.36/ 000393600000";
            result = SerialNumber.IsMobiFlightSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsJoystickSerialTest()
        {
            var serial = "GMA345/ SN-b44-4c5";
            var result = SerialNumber.IsJoystickSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsFalse(result);

            serial = "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.IsJoystickSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsTrue(result);

            serial = "Arcaze v5.36/ 000393600000";
            result = SerialNumber.IsJoystickSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsArcazeSerialTest()
        {
            var serial = "GMA345/ SN-b44-4c5";
            var result = SerialNumber.IsArcazeSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsFalse(result);

            serial = "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.IsArcazeSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsFalse(result);

            serial = "Arcaze v5.36/ 000393600000";
            result = SerialNumber.IsArcazeSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsMidiBoardSerialTest()
        {
            var serial = "GMA345/ SN-b44-4c5";
            var result = SerialNumber.IsMidiBoardSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsFalse(result);

            serial = "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.IsMidiBoardSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsFalse(result);

            serial = "Arcaze v5.36/ 000393600000";
            result = SerialNumber.IsMidiBoardSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsFalse(result);

            serial = "My MidiDevice/ MI-123456";
            result = SerialNumber.IsMidiBoardSerial(SerialNumber.ExtractSerial(serial));
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ToController_ValidModuleSerial_ParsesCorrectly()
        {
            // Arrange
            var moduleSerial = "ProtoBoard-v2/ SN-5FC-1CF";

            // Act
            var controller = SerialNumber.CreateControllerFromFullSerial(moduleSerial);

            // Assert
            Assert.IsNotNull(controller);
            Assert.AreEqual("ProtoBoard-v2", controller.Name);
            Assert.AreEqual("SN-5FC-1CF", controller.Serial);
        }

        [TestMethod()]
        public void ToController_EmptyString_ReturnsEmptyController()
        {
            // Arrange
            var moduleSerial = "";

            // Act
            var controller = SerialNumber.CreateControllerFromFullSerial(moduleSerial);

            // Assert
            Assert.IsNotNull(controller);
            Assert.AreEqual("", controller.Name);
            Assert.AreEqual("", controller.Serial);
        }

        [TestMethod()]
        public void ToController_NullString_ReturnsEmptyController()
        {
            // Arrange
            string moduleSerial = null;

            // Act
            var controller = SerialNumber.CreateControllerFromFullSerial(moduleSerial);

            // Assert
            Assert.IsNotNull(controller);
            Assert.AreEqual("", controller.Name);
            Assert.AreEqual("", controller.Serial);
        }

        [TestMethod()]
        public void ToFullSerial_ValidController_FormatsCorrectly()
        {
            // Arrange
            var controller = new Controller();
            controller.Name = "ProtoBoard-v2";
            controller.Serial = "SN-5FC-1CF";

            // Act
            var moduleSerial = SerialNumber.ToFullSerial(controller);

            // Assert
            Assert.AreEqual("ProtoBoard-v2/ SN-5FC-1CF", moduleSerial);
        }

        [TestMethod()]
        public void ToFullSerial_JoystickController_UsesSpaceSlashSpace()
        {
            // Arrange
            var controller = new Controller();
            controller.Name = "Joystick X";
            controller.Serial = "JS-123456";

            // Act
            var moduleSerial = SerialNumber.ToFullSerial(controller);

            // Assert
            Assert.AreEqual("Joystick X / JS-123456", moduleSerial);
        }

        [TestMethod()]
        public void ToFullSerial_MidiController_UsesSpaceSlashSpace()
        {
            // Arrange
            var controller = new Controller();
            controller.Name = "MIDI Device";
            controller.Serial = "MI-789012";

            // Act
            var moduleSerial = SerialNumber.ToFullSerial(controller);

            // Assert
            Assert.AreEqual("MIDI Device / MI-789012", moduleSerial);
        }

        [TestMethod()]
        public void ToFullSerial_EmptyController_ReturnsEmptyString()
        {
            // Arrange
            var controller = new Controller();

            // Act
            var moduleSerial = SerialNumber.ToFullSerial(controller);

            // Assert
            Assert.AreEqual("", moduleSerial);
        }

            // Act
            var moduleSerial = SerialNumber.ToFullSerial(controller);

            // Assert
            Assert.AreEqual("", moduleSerial);
        }

        [TestMethod()]
        public void ToFullSerial_NullController_ReturnsEmptyString()
        {
            // Arrange
            Controller controller = null;

            // Act
            var moduleSerial = SerialNumber.ToFullSerial(controller);

            // Assert
            Assert.AreEqual("", moduleSerial);
        }

        [TestMethod()]
        public void RoundTrip_ControllerConversion_PreservesValue()
        {
            // Arrange
            var originalModuleSerial = "ProtoBoard-v2/ SN-5FC-1CF";

            // Act
            var controller = SerialNumber.CreateControllerFromFullSerial(originalModuleSerial);
            var resultModuleSerial = SerialNumber.ToFullSerial(controller);

            // Assert
            Assert.AreEqual(originalModuleSerial, resultModuleSerial);
        }

        [TestMethod()]
        public void RoundTrip_JoystickControllerConversion_PreservesValue()
        {
            // Arrange
            var originalModuleSerial = "Joystick X / JS-123456";

            // Act
            var controller = SerialNumber.CreateControllerFromFullSerial(originalModuleSerial);
            var resultModuleSerial = SerialNumber.ToFullSerial(controller);

            // Assert
            Assert.AreEqual(originalModuleSerial, resultModuleSerial);
        }

        [TestMethod()]
        public void RoundTrip_MidiControllerConversion_PreservesValue()
        {
            // Arrange
            var originalModuleSerial = "MIDI Device / MI-789012";

            // Act
            var controller = SerialNumber.CreateControllerFromFullSerial(originalModuleSerial);
            var resultModuleSerial = SerialNumber.ToFullSerial(controller);

            // Assert
            Assert.AreEqual(originalModuleSerial, resultModuleSerial);
        }
    }
}