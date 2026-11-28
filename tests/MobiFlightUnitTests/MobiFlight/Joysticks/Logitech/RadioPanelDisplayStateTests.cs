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

        #region EncodeNumber

        [TestMethod]
        public void EncodeNumber_SquawkWithLeadingZero_KeepsAllDigitsRightAligned()
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeNumber("0123");

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0x0F, 0, 1, 2, 3 }, encoded);
        }

        [TestMethod]
        public void EncodeNumber_Decimal_SetsDecimalPointOnLastIntegerDigit()
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeNumber("12.3");

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0x0F, 0x0F, 1, 0xD2, 3 }, encoded);
        }

        [TestMethod]
        public void EncodeNumber_AdfFrequencyWithDecimal_UsesAllFiveDigits()
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeNumber("1250.5");

            // Assert
            CollectionAssert.AreEqual(new byte[] { 1, 2, 5, 0xD0, 5 }, encoded);
        }

        [TestMethod]
        public void EncodeNumber_TooManyDecimals_AreTruncatedNotRounded()
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeNumber("123.456");

            // Assert
            CollectionAssert.AreEqual(new byte[] { 1, 2, 0xD3, 4, 5 }, encoded);
        }

        [TestMethod]
        [DataRow("0", new byte[] { 0x0F, 0x0F, 0x0F, 0x0F, 0 })]
        [DataRow(".5", new byte[] { 0x0F, 0x0F, 0x0F, 0xD0, 5 })]
        [DataRow(" 350 ", new byte[] { 0x0F, 0x0F, 3, 5, 0 })]
        [DataRow("12.", new byte[] { 0x0F, 0x0F, 0x0F, 1, 2 })]
        public void EncodeNumber_EdgeCases(string input, byte[] expected)
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeNumber(input);

            // Assert
            CollectionAssert.AreEqual(expected, encoded);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(".")]
        [DataRow("abc")]
        [DataRow("-5")]
        [DataRow("1.2.3")]
        [DataRow("1,5")]
        [DataRow("123456")]
        public void EncodeNumber_EmptyInvalidOrTooLong_BlanksDisplay(string input)
        {
            // Act
            var encoded = RadioPanelDisplayState.EncodeNumber(input);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0x0F, 0x0F, 0x0F, 0x0F, 0x0F }, encoded);
        }

        #endregion

        #region Addresses

        [TestMethod]
        // Modes and slots are passed as ints because the enums are internal.
        [DataRow("LCD COM1 ACTIVE", (int)RadioPanelMode.Com1, (int)RadioPanelDisplaySlot.Active)]
        [DataRow("LCD COM2 STBY", (int)RadioPanelMode.Com2, (int)RadioPanelDisplaySlot.Standby)]
        [DataRow("LCD NAV1 ACTIVE", (int)RadioPanelMode.Nav1, (int)RadioPanelDisplaySlot.Active)]
        [DataRow("LCD XPDR STBY", (int)RadioPanelMode.Xpdr, (int)RadioPanelDisplaySlot.Standby)]
        public void TryParseAddress_KnownAddress_ReturnsModeAndSlot(string address, int expectedMode, int expectedSlot)
        {
            // Act
            var found = RadioPanelDisplayState.TryParseAddress(address, out var mode, out var slot);

            // Assert
            Assert.IsTrue(found);
            Assert.AreEqual((RadioPanelMode)expectedMode, mode);
            Assert.AreEqual((RadioPanelDisplaySlot)expectedSlot, slot);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("LCD COM3 ACTIVE")]
        [DataRow("lcd com1 active")]
        public void TryParseAddress_UnknownAddress_ReturnsFalse(string address)
        {
            // Act & Assert
            Assert.IsFalse(RadioPanelDisplayState.TryParseAddress(address, out _, out _));
        }

        #endregion

        #region ToFeatureReport

        private static RadioPanelDisplayState CreateComState(RadioPanelMode upper, RadioPanelMode lower)
        {
            var state = new RadioPanelDisplayState();
            state.SetValue(RadioPanelMode.Com1, RadioPanelDisplaySlot.Active, "123450");
            state.SetValue(RadioPanelMode.Com1, RadioPanelDisplaySlot.Standby, "124850");
            state.SetValue(RadioPanelMode.Com2, RadioPanelDisplaySlot.Active, "118000");
            state.SetValue(RadioPanelMode.Com2, RadioPanelDisplaySlot.Standby, "121500");
            state.SetModes(upper, lower);
            return state;
        }

        [TestMethod]
        public void ToFeatureReport_SelectedModes_PlacesFrequenciesInDisplayOrder()
        {
            // Arrange
            var state = CreateComState(RadioPanelMode.Com1, RadioPanelMode.Com2);

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
        public void ToFeatureReport_RowWithUnknownMode_IsBlank()
        {
            // Arrange
            var state = CreateComState(RadioPanelMode.None, RadioPanelMode.Com2);

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
        public void ToFeatureReport_ModeWithoutValues_IsBlank()
        {
            // Arrange
            var state = CreateComState(RadioPanelMode.Adf, RadioPanelMode.Com1);

            // Act
            var report = state.ToFeatureReport();

            // Assert
            for (var i = 1; i <= 10; i++)
            {
                Assert.AreEqual(RadioPanelDisplayState.Blank, report[i], $"Upper row byte {i}");
            }
        }

        [TestMethod]
        public void ToFeatureReport_NavMode_UsesFrequencyFormat()
        {
            // Arrange
            var state = new RadioPanelDisplayState();
            state.SetValue(RadioPanelMode.Nav1, RadioPanelDisplaySlot.Active, "110500");
            state.SetValue(RadioPanelMode.Nav1, RadioPanelDisplaySlot.Standby, "113.9");
            state.SetModes(RadioPanelMode.Nav1, RadioPanelMode.None);

            // Act
            var report = state.ToFeatureReport();

            // Assert
            CollectionAssert.AreEqual(new byte[] { 1, 1, 0xD0, 5, 0 }, report[1..6], "Upper active");
            CollectionAssert.AreEqual(new byte[] { 1, 1, 0xD3, 9, 0 }, report[6..11], "Upper standby");
        }

        [TestMethod]
        public void ToFeatureReport_XpdrAndDmeModes_UseNumberFormat()
        {
            // Arrange
            var state = new RadioPanelDisplayState();
            state.SetValue(RadioPanelMode.Xpdr, RadioPanelDisplaySlot.Active, "0123");
            state.SetValue(RadioPanelMode.Dme, RadioPanelDisplaySlot.Active, "12.3");
            state.SetValue(RadioPanelMode.Dme, RadioPanelDisplaySlot.Standby, "140");
            state.SetModes(RadioPanelMode.Xpdr, RadioPanelMode.Dme);

            // Act
            var report = state.ToFeatureReport();

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0x0F, 0, 1, 2, 3 }, report[1..6], "Upper active (XPDR)");
            CollectionAssert.AreEqual(new byte[] { 0x0F, 0x0F, 0x0F, 0x0F, 0x0F }, report[6..11], "Upper standby (XPDR, no value)");
            CollectionAssert.AreEqual(new byte[] { 0x0F, 0x0F, 1, 0xD2, 3 }, report[11..16], "Lower active (DME)");
            CollectionAssert.AreEqual(new byte[] { 0x0F, 0x0F, 1, 4, 0 }, report[16..21], "Lower standby (DME)");
        }

        [TestMethod]
        public void ToFeatureReport_BothRowsOnSameMode_ShowSameValues()
        {
            // Arrange
            var state = CreateComState(RadioPanelMode.Com2, RadioPanelMode.Com2);

            // Act
            var report = state.ToFeatureReport();

            // Assert
            CollectionAssert.AreEqual(report[11..21], report[1..11]);
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

        #region SetValue and SetModes

        [TestMethod]
        public void SetValue_ReportsWhetherValueChanged()
        {
            // Arrange
            var state = new RadioPanelDisplayState();

            // Act & Assert
            Assert.IsTrue(state.SetValue(RadioPanelMode.Adf, RadioPanelDisplaySlot.Active, "350"), "New value");
            Assert.IsFalse(state.SetValue(RadioPanelMode.Adf, RadioPanelDisplaySlot.Active, "350"), "Same value");
            Assert.IsTrue(state.SetValue(RadioPanelMode.Adf, RadioPanelDisplaySlot.Active, "351"), "Changed value");
            Assert.IsFalse(state.SetValue(RadioPanelMode.None, RadioPanelDisplaySlot.Active, "1"), "No mode");
        }

        [TestMethod]
        public void ClearValues_BlanksDisplaysButKeepsModes()
        {
            // Arrange
            var state = CreateComState(RadioPanelMode.Com1, RadioPanelMode.Com2);

            // Act
            state.ClearValues();
            var report = state.ToFeatureReport();

            // Assert
            for (var i = 1; i <= 20; i++)
            {
                Assert.AreEqual(RadioPanelDisplayState.Blank, report[i], $"Byte {i}");
            }
            Assert.AreEqual(RadioPanelMode.Com1, state.UpperMode);
            Assert.AreEqual(RadioPanelMode.Com2, state.LowerMode);
            Assert.IsTrue(state.SetValue(RadioPanelMode.Com1, RadioPanelDisplaySlot.Active, "123450"), "Same value as before counts as new after clearing");
        }

        [TestMethod]
        public void SetModes_ReportsWhetherModesChanged()
        {
            // Arrange
            var state = new RadioPanelDisplayState();

            // Act & Assert
            Assert.IsTrue(state.SetModes(RadioPanelMode.Com1, RadioPanelMode.Nav1), "Initial modes");
            Assert.IsFalse(state.SetModes(RadioPanelMode.Com1, RadioPanelMode.Nav1), "Same modes");
            Assert.IsTrue(state.SetModes(RadioPanelMode.Com1, RadioPanelMode.Xpdr), "Lower changed");
        }

        #endregion

        #region Definition

        [TestMethod]
        public void Definition_EveryDisplayOutputIdIsAKnownAddress()
        {
            // Arrange
            var definition = RadioPanelDefinition.Load();
            var displays = definition.Outputs.Where(o => o.Type == "LcdDisplay").ToList();

            // Assert
            Assert.HasCount(RadioPanelModes.Count * 2, displays, "One active and one standby display per mode");
            foreach (var display in displays)
            {
                Assert.IsTrue(RadioPanelDisplayState.TryParseAddress(display.Id, out _, out _), $"Unknown display id '{display.Id}'");
                Assert.AreEqual<byte>(5, display.Cols, $"{display.Id} columns");
            }
        }

        #endregion
    }
}
