using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MobiFlight.Modifier.Tests
{
    [TestClass]
    public class QuantizeTest
    {
        private static ConnectorValue CV(double value) => new ConnectorValue { Float64 = value };
        private static List<ConfigRefValue> NoRefs => new List<ConfigRefValue>();

        private Quantize Default() => new Quantize
        {
            Active = true,
            StepSize = 16
        };

        #region Active flag

        [TestMethod]
        public void Apply_WhenNotActive_ReturnsRawValue()
        {
            // Arrange
            var q = Default();
            q.Active = false;

            // Act
            var result = q.Apply(CV(1234), NoRefs);

            // apply multiple values being inactive
            result = q.Apply(CV(0), NoRefs);
            result = q.Apply(CV(512), NoRefs);

            // Assert
            Assert.AreEqual(512, result.Float64);
        }

        #endregion

        #region Quantization

        [TestMethod]
        public void Apply_FirstCall_ReturnsQuantizedValue()
        {
            // Arrange
            var q = Default();

            // Act
            var result = q.Apply(CV(1024), NoRefs);

            // Assert
            Assert.AreEqual(1024, result.Float64);
        }

        [TestMethod]
        public void Apply_ValueInLowerHalfOfBucket_RoundsDown()
        {
            // Arrange
            var q = Default();

            // Act
            var result = q.Apply(CV(7), NoRefs);

            // Assert
            Assert.AreEqual(0, result.Float64);
        }

        [TestMethod]
        public void Apply_ValueInUpperHalfOfBucket_RoundsUp()
        {
            // Arrange
            var q = Default();

            // Act
            var result = q.Apply(CV(9), NoRefs);

            // Assert
            Assert.AreEqual(16, result.Float64);
        }

        [TestMethod]
        public void Apply_ValueExactlyOnBoundary_QuantizesCorrectly()
        {
            // Arrange
            var q = Default();

            // Act
            var result = q.Apply(CV(8), NoRefs);

            // Assert — Math.Round uses banker's rounding, 8/16=0.5 rounds to 0
            Assert.AreEqual(0, result.Float64);
        }

        #endregion

        #region Noise suppression

        [TestMethod]
        public void Apply_OscillationWithinBucket_DoesNotChangeOutput()
        {
            // Arrange
            var q = Default();
            q.Apply(CV(1024), NoRefs);

            // Act & Assert — values 1016-1031 all quantize to 1024
            for (int raw = 1016; raw < 1032; raw++)
            {
                var result = q.Apply(CV(raw), NoRefs);
                Assert.AreEqual(1024, result.Float64, $"Unexpected change at raw={raw}");
            }
        }

        [TestMethod]
        public void Apply_SameQuantizedValue_DoesNotChangeOutput()
        {
            // Arrange
            var q = Default();
            q.Apply(CV(1024), NoRefs);

            // Act
            var result = q.Apply(CV(1024), NoRefs);

            // Assert
            Assert.AreEqual(1024, result.Float64);
        }

        #endregion

        #region Step changes

        [TestMethod]
        public void Apply_ValueCrossesBucketBoundary_EmitsNewValue()
        {
            // Arrange
            var q = Default();
            q.Apply(CV(1024), NoRefs);

            // Act
            var result = q.Apply(CV(1009), NoRefs);

            // Assert
            Assert.AreEqual(1008, result.Float64);
        }

        [TestMethod]
        public void Apply_LargeJump_EmitsQuantizedNewValue()
        {
            // Arrange
            var q = Default();
            q.Apply(CV(1024), NoRefs);

            // Act
            var result = q.Apply(CV(4096), NoRefs);

            // Assert
            Assert.AreEqual(4096, result.Float64);
        }

        #endregion

        #region Clone

        [TestMethod]
        public void Clone_ProducesIndependentCopy()
        {
            // Arrange
            var q = Default();
            q.Apply(CV(1024), NoRefs);

            // Act
            var clone = (Quantize)q.Clone();
            q.Apply(CV(2000), NoRefs);

            // Assert
            Assert.AreEqual(1024, clone.Apply(CV(1024), NoRefs).Float64,
                "Clone should be independent of original");
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
        public void Equals_DifferentStepSize_ReturnsFalse()
        {
            // Arrange
            var a = Default();
            var b = Default();
            b.StepSize = 32;

            // Act & Assert
            Assert.IsFalse(a.Equals(b));
        }

        #endregion

        #region ToSummaryLabel

        [TestMethod]
        public void ToSummaryLabel_ContainsStepSize()
        {
            // Arrange
            var q = Default();

            // Act
            var label = q.ToSummaryLabel();

            // Assert
            StringAssert.Contains(label, "16");
        }

        #endregion
    }
}