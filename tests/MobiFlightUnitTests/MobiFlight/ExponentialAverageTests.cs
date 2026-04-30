using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MobiFlight.Modifier.Tests
{
    [TestClass()]
    public class ExponentialAverageTests
    {
        private static ConnectorValue CreateConnectorValue(double value)
        {
            return new ConnectorValue
            {
                type = FSUIPCOffsetType.Float,
                Float64 = value
            };
        }

        [TestMethod()]
        public void ExponentialAverage_DefaultValuesTest()
        {
            var modifier = new ExponentialAverage();
            Assert.IsFalse(modifier.Active);
            Assert.AreEqual(0.25, modifier.Alpha);
            Assert.AreEqual(3, modifier.Threshold);
            Assert.AreEqual(0, modifier.FilteredValue);
        }

        [TestMethod()]
        public void ExponentialAverage_CloneTest()
        {
            var modifier = new ExponentialAverage
            {
                Active = true,
                Alpha = 0.5,
                Threshold = 5
            };

            var clone = modifier.Clone() as ExponentialAverage;

            Assert.IsNotNull(clone);
            Assert.AreEqual(modifier.Active, clone.Active);
            Assert.AreEqual(modifier.Alpha, clone.Alpha);
            Assert.AreEqual(modifier.Threshold, clone.Threshold);
            Assert.AreEqual(modifier.FilteredValue, clone.FilteredValue);
        }

        [TestMethod()]
        public void ExponentialAverage_EqualsTest()
        {
            var o1 = new ExponentialAverage();
            var o2 = new ExponentialAverage();

            Assert.IsTrue(o1.Equals(o2));

            o1.Active = true;
            Assert.IsFalse(o1.Equals(o2));

            o2.Active = true;
            Assert.IsTrue(o1.Equals(o2));

            o1.Alpha = 0.5;
            Assert.IsFalse(o1.Equals(o2));

            o2.Alpha = 0.5;
            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void ExponentialAverage_Equals_NullAndWrongTypeTest()
        {
            var modifier = new ExponentialAverage();
            Assert.IsFalse(modifier.Equals(null));
            Assert.IsFalse(modifier.Equals("not an ExponentialAverage"));
        }

        [TestMethod()]
        public void ExponentialAverage_ApplyTest_InactivePassesThrough()
        {
            var modifier = new ExponentialAverage { Active = false };
            var input = CreateConnectorValue(42.0);

            var result = modifier.Apply(input, new List<ConfigRefValue>());

            Assert.AreEqual(42.0, result.Float64);
        }

        [TestMethod()]
        public void ExponentialAverage_ApplyTest_FirstValueInitializesFilter()
        {
            var modifier = new ExponentialAverage { Active = true, Alpha = 0.25, Threshold = 3 };
            var input = CreateConnectorValue(100.0);

            var result = modifier.Apply(input, new List<ConfigRefValue>());

            // On first call, FilteredValue and result should equal the input
            Assert.AreEqual(100.0, result.Float64);
            Assert.AreEqual(100.0, modifier.FilteredValue);
        }

        [TestMethod()]
        public void ExponentialAverage_ApplyTest_SmallChangeIsSuppressedByThreshold()
        {
            var modifier = new ExponentialAverage { Active = true, Alpha = 0.25, Threshold = 3 };

            // Initialize
            modifier.Apply(CreateConnectorValue(100.0), new List<ConfigRefValue>());

            // A change of 1 should not exceed the threshold of 3
            var result = modifier.Apply(CreateConnectorValue(101.0), new List<ConfigRefValue>());

            // FilteredValue should not change because delta <= Threshold
            Assert.AreEqual(100.0, result.Float64);
        }

        [TestMethod()]
        public void ExponentialAverage_ApplyTest_LargeChangeExceedsThreshold()
        {
            var modifier = new ExponentialAverage { Active = true, Alpha = 1.0, Threshold = 3 };

            // Initialize at 100
            modifier.Apply(CreateConnectorValue(100.0), new List<ConfigRefValue>());

            // With Alpha = 1.0, PreviousValue = currentValue exactly.
            // A jump of 10 should exceed the threshold of 3.
            var result = modifier.Apply(CreateConnectorValue(110.0), new List<ConfigRefValue>());

            // FilteredValue should update because delta > Threshold
            Assert.AreEqual(110.0, result.Float64);
            Assert.AreEqual(110.0, modifier.FilteredValue);
        }

        [TestMethod()]
        public void ExponentialAverage_ApplyTest_FilterConvergesOverMultipleCalls()
        {
            var modifier = new ExponentialAverage { Active = true, Alpha = 1.0, Threshold = 0 };

            modifier.Apply(CreateConnectorValue(0.0), new List<ConfigRefValue>());

            // With Alpha = 1.0 and Threshold = 0, every change should update FilteredValue immediately
            var result = modifier.Apply(CreateConnectorValue(50.0), new List<ConfigRefValue>());
            Assert.AreEqual(50.0, result.Float64);

            result = modifier.Apply(CreateConnectorValue(75.0), new List<ConfigRefValue>());
            Assert.AreEqual(75.0, result.Float64);
        }

        [TestMethod()]
        public void ExponentialAverage_ToSummaryLabelTest()
        {
            var modifier = new ExponentialAverage { Alpha = 0.25 };
            var label = modifier.ToSummaryLabel();
            Assert.AreEqual("Exponential Average with Alpha = 0.25", label);
        }

        [TestMethod()]
        public void ExponentialAverage_CopyConstructorTest()
        {
            var original = new ExponentialAverage
            {
                Active = true,
                Alpha = 0.75,
                Threshold = 5
            };
            // Apply once to set internal PreviousValue
            original.Apply(CreateConnectorValue(42.0), new List<ConfigRefValue>());

            var copy = new ExponentialAverage(original);

            Assert.AreEqual(original.Active, copy.Active);
            Assert.AreEqual(original.Alpha, copy.Alpha);
            Assert.AreEqual(original.FilteredValue, copy.FilteredValue);
        }
    }
}