namespace MobiFlightMoza.Cdu.Tests
{
    [TestClass]
    public class MozaCharacterMapperTests
    {
        [TestMethod]
        public void ToSupportedChar_BallotBox_MapsToWhiteSquare()
        {
            Assert.AreEqual('□', MozaCharacterMapper.ToSupportedChar('☐'));
        }

        [TestMethod]
        public void ToSupportedChar_OrdinaryCharacter_IsUnchanged()
        {
            Assert.AreEqual('A', MozaCharacterMapper.ToSupportedChar('A'));
        }

        [TestMethod]
        public void ToSupportedText_ContainsBallotBox_ReplacesOnlyThatCharacter()
        {
            Assert.AreEqual("A□B", MozaCharacterMapper.ToSupportedText("A☐B"));
        }

        [TestMethod]
        public void ToSupportedText_NoSubstitutionsNeeded_ReturnsSameText()
        {
            Assert.AreEqual("HELLO", MozaCharacterMapper.ToSupportedText("HELLO"));
        }

        [TestMethod]
        public void ToSupportedText_NullOrEmpty_ReturnsInputUnchanged()
        {
            Assert.IsNull(MozaCharacterMapper.ToSupportedText(null));
            Assert.AreEqual("", MozaCharacterMapper.ToSupportedText(""));
        }
    }
}
