namespace MobiFlight.Joysticks.Logitech.Tests
{
    [TestClass]
    public class RadioPanelDisplayStateTests
    {
        #region EncodeFrequency

        [TestMethod]
        [DataRow("123450")]
        [DataRow("123.45")]
        [DataRow("123.450")]
        public void EncodeFrequency_KhzOrMhz_EncodesDigitsWithDecimalPoint(string input)
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeFrequency(input);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 1, 2, 0xD3, 4, 5 }, encoded);
        }

        [TestMethod]
        public void EncodeFrequency_ExtraDecimals_AreTruncatedNotRounded()
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeFrequency("118.999");

            // Assert
            CollectionAssert.AreEqual(new byte[] { 1, 1, 0xD8, 9, 9 }, encoded);
        }

        [TestMethod]
        [DataRow("18.025")]
        [DataRow("18025")]
        public void EncodeFrequency_BelowHundredMhz_ShowsAllDigitsWithoutLeadingOne(string input)
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeFrequency(input);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 1, 0xD8, 0, 2, 5 }, encoded);
        }

        [TestMethod]
        public void EncodeFrequency_BelowHundredMhzWithExtraDecimals_IsTruncated()
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeFrequency("18.9999");

            // Assert
            CollectionAssert.AreEqual(new byte[] { 1, 0xD8, 9, 9, 9 }, encoded);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow("abc")]
        [DataRow("0")]
        [DataRow("-18.025")]
        [DataRow("200.00")]
        public void EncodeFrequency_EmptyInvalidOrOutOfBand_BlanksDisplay(string input)
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeFrequency(input);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0x0F, 0x0F, 0x0F, 0x0F, 0x0F }, encoded);
        }

        #endregion

        #region ToFeatureReport

        [TestMethod]
        public void ToFeatureReport_SelectedBands_PlacesFrequenciesInDisplayOrder()
        {
            // Arrange
            var state = new RadioPanelDisplayState();
            state.SetValues("123450", "124850", "118000", "121500", RadioPanelBand.Com1, RadioPanelBand.Com2);

            // Act
            var report = state.ToFeatureReport();

            // Assert
            Assert.HasCount(RadioPanelDisplayState.FeatureReportLength, report);
            Assert.AreEqual<byte>(0, report[0], "Report ID");
            CollectionAssert.AreEqual(new byte[] { 1, 2, 0xD3, 4, 5 }, report[1..6], "Upper active");
            CollectionAssert.AreEqual(new byte[] { 1, 2, 0xD4, 8, 5 }, report[6..11], "Upper standby");
            CollectionAssert.AreEqual(new byte[] { 1, 1, 0xD8, 0, 0 }, report[11..16], "Lower active");
            CollectionAssert.AreEqual(new byte[] { 1, 2, 0xD1, 5, 0 }, report[16..21], "Lower standby");
            Assert.AreEqual<byte>(0, report[21], "Trailer 1");
            Assert.AreEqual<byte>(0, report[22], "Trailer 2");
        }

        [TestMethod]
        public void ToFeatureReport_RowWithoutComBand_IsBlank()
        {
            // Arrange
            var state = new RadioPanelDisplayState();
            state.SetValues("123450", "124850", "118000", "121500", RadioPanelBand.None, RadioPanelBand.Com2);

            // Act
            var report = state.ToFeatureReport();

            // Assert
            for (var i = 1; i <= 10; i++)
            {
                Assert.AreEqual(RadioPanelDisplayState.Blank, report[i], $"Upper row byte {i}");
            }
            Assert.AreEqual<byte>(0xD8, report[13], "Lower row still shows COM2");
        }

        [TestMethod]
        public void ToFeatureReport_NewState_IsAllBlank()
        {
            // Arrange
            var state = new RadioPanelDisplayState();

            // Act
            var report = state.ToFeatureReport();

            // Assert
            for (var i = 1; i <= 20; i++)
            {
                Assert.AreEqual(RadioPanelDisplayState.Blank, report[i], $"Byte {i}");
            }
        }

        #endregion
    }
}
