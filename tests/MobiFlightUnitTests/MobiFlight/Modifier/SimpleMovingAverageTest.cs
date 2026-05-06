using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MobiFlight.Modifier.Tests
{
    [TestClass]
    public class SimpleMovingAverageTest
    {
        private static ConnectorValue CV(double value) => new ConnectorValue { Float64 = value };
        private static List<ConfigRefValue> NoRefs => new List<ConfigRefValue>();

        private SimpleMovingAverage Default() => new SimpleMovingAverage
        {
            Active = true,
            WindowSize = 4,
            Threshold = 16,
            JumpResetThreshold = 64
        };

        #region Active flag

        [TestMethod]
        public void Apply_WhenNotActive_ReturnsRawValue()
        {
            // Arrange
            var sma = Default();
            sma.Active = false;

            // Act
            var result = sma.Apply(CV(1234), NoRefs);

            // Assert
            Assert.AreEqual(1234, result.Float64);
        }

        #endregion

        #region Seeding

        [TestMethod]
        public void Apply_FirstCall_ReturnsInputValue()
        {
            // Arrange
            var sma = Default();

            // Act
            var result = sma.Apply(CV(1000), NoRefs);

            // Assert
            Assert.AreEqual(1000, result.Float64);
        }

        #endregion

        #region Noise suppression

        [TestMethod]
        public void Apply_SmallOscillation_DoesNotExceedThreshold()
        {
            // Arrange
            var sma = Default();
            sma.Apply(CV(1000), NoRefs);

            // Act & Assert
            for (int i = 0; i < 20; i++)
            {
                double raw = i % 2 == 0 ? 1008 : 992;
                var result = sma.Apply(CV(raw), NoRefs);
                Assert.AreEqual(1000, result.Float64, $"Unexpected emission on iteration {i}, raw={raw}");
            }
        }

        [TestMethod]
        public void Apply_StableInput_NeverChangesOutput()
        {
            // Arrange
            var sma = Default();
            sma.Apply(CV(5000), NoRefs);

            // Act & Assert
            for (int i = 0; i < 10; i++)
            {
                var result = sma.Apply(CV(5000), NoRefs);
                Assert.AreEqual(5000, result.Float64);
            }
        }

        #endregion

        #region Slow movement

        [TestMethod]
        public void Apply_SlowIncrement_EventuallyEmits()
        {
            // Arrange
            var sma = Default();
            sma.Apply(CV(1000), NoRefs);
            double lastEmitted = 1000;
            bool emitted = false;

            // Act
            for (int i = 1; i <= 20; i++)
            {
                var result = sma.Apply(CV(1000 + i * 4), NoRefs);
                if (result.Float64 != lastEmitted)
                {
                    emitted = true;
                    break;
                }
            }

            // Assert
            Assert.IsTrue(emitted, "Slow turn should eventually trigger an emission");
        }

        #endregion

        #region Jump reset

        [TestMethod]
        public void Apply_LargeJump_ReseetsAndEmitsImmediately()
        {
            // Arrange
            var sma = Default();
            sma.Apply(CV(1000), NoRefs);

            // Act
            var result = sma.Apply(CV(1128), NoRefs);

            // Assert
            Assert.AreEqual(1128, result.Float64, "Large jump should re-seed and emit new value immediately");
        }

        [TestMethod]
        public void Apply_SmallJumpBelowResetThreshold_DoesNotReseed()
        {
            // Arrange
            var sma = Default();
            sma.Apply(CV(1000), NoRefs);

            // Act
            var result = sma.Apply(CV(1032), NoRefs);

            // Assert
            Assert.AreEqual(1000, result.Float64, "Small jump should slide window, not re-seed");
        }

        #endregion

        #region Threshold behavior

        [TestMethod]
        public void Apply_NewValueWithinThreshold_DoesNotEmit()
        {
            // Arrange
            var sma = Default();
            sma.Apply(CV(1024), NoRefs);

            ConnectorValue result = null;
            // Act
            for (var i=0; i!=sma.WindowSize; ++i)
            {
                result = sma.Apply(CV(1024 + sma.Threshold-1), NoRefs);
            }
            // Assert
            Assert.AreEqual(1024, result.Float64, "Large jump should re-seed and emit new value immediately");
        }

        [TestMethod]
        public void Apply_NewValueWithinThreshold_Emits()
        {
            // Arrange
            var sma = Default();
            sma.Apply(CV(1024), NoRefs);

            ConnectorValue result = null;
            // Act
            for (var i = 0; i != sma.WindowSize; ++i)
            {
                result = sma.Apply(CV(1024 + sma.Threshold), NoRefs);
            }
            // Assert
            Assert.AreEqual(1024 + sma.Threshold, result.Float64, "Large jump should re-seed and emit new value immediately");
        }

        #endregion

        #region Clone

        [TestMethod]
        public void Clone_ProducesIndependentCopy()
        {
            // Arrange
            var sma = Default();
            sma.Apply(CV(1000), NoRefs);
            sma.Apply(CV(1020), NoRefs);

            // Act
            var clone = (SimpleMovingAverage)sma.Clone();
            sma.Apply(CV(2000), NoRefs);

            // Assert
            Assert.AreEqual(sma.WindowSize, clone.WindowSize);
            Assert.AreNotEqual(sma.SmoothedValue, clone.SmoothedValue, "Clone should be independent of original");
        }

        #endregion

        #region Equals

        [TestMethod]
        public void Equals_SameParameters_ReturnsTrue()
        {
            // Arrange
            var a = Default();
            var b = Default();

            // Act & Assert
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void Equals_DifferentWindowSize_ReturnsFalse()
        {
            // Arrange
            var a = Default();
            var b = Default();
            b.WindowSize = 16;

            // Act & Assert
            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void Equals_DifferentThreshold_ReturnsFalse()
        {
            // Arrange
            var a = Default();
            var b = Default();
            b.Threshold = 99;

            // Act & Assert
            Assert.IsFalse(a.Equals(b));
        }

        #endregion

        #region ToSummaryLabel

        [TestMethod]
        public void ToSummaryLabel_ContainsKeyParameters()
        {
            // Arrange
            var sma = Default();

            // Act
            var label = sma.ToSummaryLabel();

            // Assert
            StringAssert.Contains(label, "4");
            StringAssert.Contains(label, "16");
        }

        #endregion
    }
}