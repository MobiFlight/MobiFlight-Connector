using Hid.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ExtensionsTests
    {
        [TestMethod()]
        public void ReverseTest()
        {
            byte value = 1;
            Assert.AreEqual(128, value.Reverse(), $"The value was not reversed properly.");

            value = 128;
            Assert.AreEqual(1, value.Reverse(), $"The value was not reversed properly.");

            value = 0b11110000;
            Assert.AreEqual(0b00001111, value.Reverse(), $"The value was not reversed properly.");

            value = 0b10110111;
            Assert.AreEqual(0b11101101, value.Reverse(), $"The value was not reversed properly.");
        }

        [TestMethod()]
        public void GenerateSimpleHashTest()
        {
            string xTouch = "X-TOUCH MINI";
            int xTouchHash = xTouch.GenerateSimpleHash();
            Assert.AreEqual(954907131, xTouchHash, "Failed to generate correct hash from string.");

            string empty = "";
            int emptyHash = empty.GenerateSimpleHash();
            Assert.AreEqual(0, emptyHash, "Failed to generate correct hash from string.");

            string shortString = "a";
            int shortStringHash = shortString.GenerateSimpleHash();
            Assert.AreEqual(97, shortStringHash, "Failed to generate correct hash from string.");

            string longString = "This is a rather long string to hash. Let's do some magic. The result will be fantastic.";
            int longStringHash = longString.GenerateSimpleHash();
            Assert.AreEqual(1057832466, longStringHash, "Failed to generate correct hash from string.");

            string japanese = "日本にほんでは";
            int japaneseHash = japanese.GenerateSimpleHash();
            Assert.AreEqual(1550985689, japaneseHash, "Failed to generate correct hash from string.");

            string german = "Müßiggang";
            int germanHash = german.GenerateSimpleHash();
            Assert.AreEqual(1480680476, germanHash, "Failed to generate correct hash from string.");

            string fail = "My fail";
            int failHash = fail.GenerateSimpleHash();
            Assert.AreNotEqual(0, failHash, "Hash of 0 should not be the result.");
        }

        [TestMethod()]
        public void AreEqualTest()
        {
            string s1 = null;
            string s2 = null;
            Assert.IsTrue(s1.AreEqual(s2));

            s1 = "Hello";
            Assert.IsFalse(s1.AreEqual(s2));
            s2 = "World!";
            Assert.IsFalse(s1.AreEqual(s2));
            s2 = "Hello";
            Assert.IsTrue(s1.AreEqual(s2));
        }

        [TestMethod()]
        public void CreateSerialFromPathTest()
        {
            // Test with typical HID device path format
            var mockDevice = new Mock<IHidDevice>();
            mockDevice.Setup(d => d.DeviceId).Returns(@"\\?\hid#vid_a316&pid_c787#9&3a962385&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}");
            var result = mockDevice.Object.CreateSerialFromPath();
            Assert.IsNotNull(result);
            Assert.AreEqual("4d1e55b2-f16f-11cf-88cb-001111000030", result);

            // Test with another valid UUID
            mockDevice.Setup(d => d.DeviceId).Returns(@"\\?\hid#vid_1234&pid_5678#9&3a962385&0&0000#{12345678-abcd-1234-5678-123456789abc}");
            result = mockDevice.Object.CreateSerialFromPath();
            Assert.IsNotNull(result);
            Assert.AreEqual("12345678-abcd-1234-5678-123456789abc", result);

            // Test with Windows style path separators
            mockDevice.Setup(d => d.DeviceId).Returns(@"\\?\hid#vid_2468&pid_1357#6&12ab34cd&0&0000#{a1b2c3d4-e5f6-7890-abcd-ef1234567890}");
            result = mockDevice.Object.CreateSerialFromPath();
            Assert.IsNotNull(result);
            Assert.AreEqual("a1b2c3d4-e5f6-7890-abcd-ef1234567890", result);
        }

        [TestMethod()]
        public void CreateSerialFromPathTest_NullDevice()
        {
            // Test with null device
            IHidDevice nullDevice = null;
            var result = nullDevice.CreateSerialFromPath();
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void CreateSerialFromPathTest_NullDeviceId()
        {
            // Test with null device ID
            var mockDevice = new Mock<IHidDevice>();
            mockDevice.Setup(d => d.DeviceId).Returns((string)null);
            var result = mockDevice.Object.CreateSerialFromPath();
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void CreateSerialFromPathTest_InvalidFormat_NoUuid()
        {
            // Test with HID device path that doesn't contain a UUID in braces
            var mockDevice = new Mock<IHidDevice>();
            mockDevice.Setup(d => d.DeviceId).Returns(@"\\?\hid#vid_a316&pid_c787#9&3a962385&0&0000#invalid");
            var result = mockDevice.Object.CreateSerialFromPath();
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void CreateSerialFromPathTest_InvalidFormat_TooShort()
        {
            // Test with HID device path that's too short to contain a valid UUID
            var mockDevice = new Mock<IHidDevice>();
            mockDevice.Setup(d => d.DeviceId).Returns(@"{short}");
            var result = mockDevice.Object.CreateSerialFromPath();
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void CreateSerialFromPathTest_JoystickSerialIntegration()
        {
            // Test that when combined with Joystick.SerialPrefix, it's correctly identified as a joystick serial
            var mockDevice = new Mock<IHidDevice>();
            mockDevice.Setup(d => d.DeviceId).Returns(@"\\?\hid#vid_a316&pid_c787#9&3a962385&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}");
            var uuid = mockDevice.Object.CreateSerialFromPath();
            var joystickSerial = $"{Joystick.SerialPrefix}{uuid}";

            Assert.IsTrue(SerialNumber.IsJoystickSerial(joystickSerial));
            Assert.IsFalse(SerialNumber.IsMobiFlightSerial(joystickSerial));
            Assert.IsFalse(SerialNumber.IsMidiBoardSerial(joystickSerial));
            Assert.IsFalse(SerialNumber.IsArcazeSerial(joystickSerial));
        }
    }
}