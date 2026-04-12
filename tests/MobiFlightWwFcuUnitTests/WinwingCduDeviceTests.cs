using MobiFlightWwFcu;
using Newtonsoft.Json.Linq;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinwingCduDeviceTests
    {
        [TestMethod]
        [DataRow('a', 0x21, 0x00)]
        [DataRow('w', 0x42, 0x00)]
        [DataRow('c', 0x63, 0x00)]
        [DataRow('g', 0x84, 0x00)]
        [DataRow('m', 0xa5, 0x00)]
        [DataRow('r', 0xc6, 0x00)]
        [DataRow('y', 0xe7, 0x00)]
        [DataRow('o', 0x08, 0x01)]
        [DataRow('e', 0x29, 0x01)]
        [DataRow('k', 0x4a, 0x01)]
        public void GetFormatByts_Large_Expected(
            char color,
            int lowByte,
            int highByte)
        {
            var token = JToken.Parse($"[\"A\", \"{color}\", 0]");
            var (actualLowByte, actualHighByte) = WinwingCduDevice.GetFormatBytes(token, out _);

            Assert.AreEqual(lowByte, actualLowByte);
            Assert.AreEqual(highByte, actualHighByte);
        }


        [TestMethod]
        [DataRow('a', 0x8c, 0x01)]
        [DataRow('w', 0xad, 0x01)]
        [DataRow('c', 0xce, 0x01)]
        [DataRow('g', 0xef, 0x01)]
        [DataRow('m', 0x10, 0x02)]
        [DataRow('r', 0x31, 0x02)]
        [DataRow('y', 0x52, 0x02)]
        [DataRow('o', 0x73, 0x02)]
        [DataRow('e', 0x94, 0x02)]
        [DataRow('k', 0xb5, 0x02)]
        public void GetFormatByts_Small_Expected(
            char color,
            int lowByte,
            int highByte)
        {
            var token = JToken.Parse($"[\"A\", \"{color}\", 1]");
            var (actualLowByte, actualHighByte) = WinwingCduDevice.GetFormatBytes(token, out _);

            Assert.AreEqual(lowByte, (int)actualLowByte);
            Assert.AreEqual(highByte, actualHighByte);
        }
    }
}
