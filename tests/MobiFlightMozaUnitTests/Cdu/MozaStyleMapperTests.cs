using MobiFlightMoza.Cdu;

namespace MobiFlightMoza.Cdu.Tests
{
    [TestClass]
    public class MozaStyleMapperTests
    {
        #region ToColorCode

        [TestMethod]
        [DataRow('w', (byte)7)]
        [DataRow('c', (byte)1)]
        [DataRow('r', (byte)2)]
        [DataRow('y', (byte)3)]
        [DataRow('g', (byte)4)]
        [DataRow('m', (byte)5)]
        [DataRow('a', (byte)6)]
        [DataRow('e', (byte)8)]
        public void ToColorCode_KnownLetter_ReturnsDocumentedCode(char letter, byte expected)
        {
            Assert.AreEqual(expected, MozaStyleMapper.ToColorCode(letter));
        }

        [TestMethod]
        public void ToColorCode_Blue_ApproximatesToCyan()
        {
            Assert.AreEqual((byte)1, MozaStyleMapper.ToColorCode('o'));
        }

        [TestMethod]
        public void ToColorCode_Khaki_ApproximatesToAmber()
        {
            Assert.AreEqual((byte)6, MozaStyleMapper.ToColorCode('k'));
        }

        [TestMethod]
        public void ToColorCode_UnknownLetter_FallsBackToWhite()
        {
            Assert.AreEqual(MozaStyleMapper.DefaultColorCode, MozaStyleMapper.ToColorCode('z'));
        }

        #endregion

        #region ToStyleByte

        [TestMethod]
        public void ToStyleByte_LargeWhite_Is0x87()
        {
            var cell = new CduCell('A', 'w', isSmall: false, isInverted: false);
            Assert.AreEqual((byte)0x87, MozaStyleMapper.ToStyleByte(cell));
        }

        [TestMethod]
        public void ToStyleByte_SmallRed_ClearsBit7()
        {
            var cell = new CduCell('A', 'r', isSmall: true, isInverted: false);
            Assert.AreEqual((byte)0x02, MozaStyleMapper.ToStyleByte(cell));
        }

        #endregion
    }
}
