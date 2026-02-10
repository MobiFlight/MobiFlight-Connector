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

            serial = "Test Serial";
            result = SerialNumber.ExtractSerial(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual(serial, result);
        }

        [TestMethod()]
        public void ExtractDeviceNameTest()
        {
            var serial = "GMA345/ SN-b44-4c5";
            var result = SerialNumber.ExtractControllerName(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("GMA345", result);

            serial = "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.ExtractControllerName(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("Bravo Throttle Quadrant", result);

            serial = "Arcaze v5.36/ 000393600000";
            result = SerialNumber.ExtractControllerName(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("Arcaze v5.36", result);

            serial = "MFG Crosswind V2/3 / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.ExtractControllerName(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("MFG Crosswind V2/3", result);
        }

        [TestMethod()]
        public void ExtractPrefixTest()
        {
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
        public void CreateControllerTest()
        {
            var serial = "GMA345/ SN-b44-4c5";
            var result = SerialNumber.CreateController(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("GMA345", result.Name);
            Assert.AreEqual("SN-b44-4c5", result.Serial);

            serial = "Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.CreateController(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("Bravo Throttle Quadrant", result.Name);
            Assert.AreEqual("JS-b0875190-3b89-11ed-8007-444553540000", result.Serial);

            serial = "Arcaze v5.36/ 000393600000";
            result = SerialNumber.CreateController(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("Arcaze v5.36", result.Name);
            Assert.AreEqual("000393600000", result.Serial);

            serial = "MFG Crosswind V2/3 / JS-b0875190-3b89-11ed-8007-444553540000";
            result = SerialNumber.CreateController(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("MFG Crosswind V2/3", result.Name);
            Assert.AreEqual("JS-b0875190-3b89-11ed-8007-444553540000", result.Serial);

            serial = "Test Serial";
            result = SerialNumber.CreateController(serial);
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.Name);
            Assert.AreEqual("Test Serial", result.Serial);
        }

        [TestMethod]
        public void BuildFullSerialTest()
        {
            var name = "GMA345";
            var serial = "SN-b44-4c5";
            var controller = new Controller() { Name = name, Serial = serial };
            var result = SerialNumber.BuildFullSerial(controller);
            Assert.AreEqual("GMA345/ SN-b44-4c5", result);
            
            name = "Bravo Throttle Quadrant";
            serial = "JS-b0875190-3b89-11ed-8007-444553540000";
            controller = new Controller() { Name = name, Serial = serial };
            result = SerialNumber.BuildFullSerial(controller);
            Assert.AreEqual("Bravo Throttle Quadrant / JS-b0875190-3b89-11ed-8007-444553540000", result);

            name = "Arcaze v5.36";
            serial = "000393600000";
            controller = new Controller() { Name = name, Serial = serial };
            result = SerialNumber.BuildFullSerial(controller);
            Assert.AreEqual("Arcaze v5.36/ 000393600000", result);

            name = "MFG Crosswind V2/3";
            serial = "JS-b0875190-3b89-11ed-8007-444553540000";
            controller = new Controller() { Name = name, Serial = serial };
            result = SerialNumber.BuildFullSerial(controller);
            Assert.AreEqual("MFG Crosswind V2/3 / JS-b0875190-3b89-11ed-8007-444553540000", result);

            name = "";
            serial = "Test Serial";
            controller = new Controller() { Name = name, Serial = serial };
            result = SerialNumber.BuildFullSerial(controller);
            Assert.AreEqual("Test Serial", result);

            name = "";
            serial = SerialNumber.NOT_SET;
            controller = new Controller() { Name = name, Serial = serial };
            result = SerialNumber.BuildFullSerial(controller);
            Assert.AreEqual(SerialNumber.NOT_SET, result);
        }
    }
}