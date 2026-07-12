using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MobiFlight.Modifier.Tests
{
    [TestClass()]
    public class PaddingTests
    {
        [TestMethod()]
        public void Constructor_ShouldSetExpectedDefaults()
        {
            var padding = new Padding();
            Assert.IsFalse(padding.Active, "Active should be false by default");
            Assert.AreEqual(' ', padding.Character, "Character should be ' ' by default");
            Assert.AreEqual(5, padding.Length, "Length should be 5 by default");
            Assert.AreEqual(Padding.PaddingDirection.Left, padding.Direction, "Direction should be Left by default");
        }

        [TestMethod()]
        public void EqualsTest_ShouldReturnExpectedResult()
        {
            // Check for equality between two Padding objects
            var padding1 = new Padding();
            var padding2 = new Padding();
            Assert.AreEqual(padding1, padding2);

            // Check for inequality between two Padding objects
            padding1.Active = true;
            Assert.AreNotEqual(padding1, padding2);
            padding2.Active = true;
            Assert.AreEqual(padding1, padding2);

            padding1.Length = 6;
            Assert.AreNotEqual(padding1, padding2);
            padding2.Length = 6;
            Assert.AreEqual(padding1, padding2);

            padding1.Character = '0';
            Assert.AreNotEqual(padding1, padding2);
            padding2.Character = '0';
            Assert.AreEqual(padding1, padding2);
        }

        [TestMethod()]
        public void GetHashCode_ShouldReturnExpectedResult()
        {
            var padding1 = new Padding();
            var padding2 = new Padding();
            Assert.AreEqual(padding1.GetHashCode(), padding2.GetHashCode());
        }

        [TestMethod()]
        public void Padding_Json_SerializationTest()
        {
            var padding = new Padding
            {
                Active = true,
                Character = '0',
                Length = 10,
                Direction = Padding.PaddingDirection.Right
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(padding);
            var deserializedPadding = Newtonsoft.Json.JsonConvert.DeserializeObject<Padding>(json);
            Assert.AreEqual(padding, deserializedPadding);
        }

        [TestMethod()]
        public void Padding_Json_DeserializationWorksCorrectly()
        {
            var json = "{\"Type\":\"Padding\",\"Active\":true,\"Character\":\"0\",\"Length\":10,\"Direction\":\"Right\"}";
            var deserializedPadding = Newtonsoft.Json.JsonConvert.DeserializeObject<Padding>(json);
            Assert.IsTrue(deserializedPadding.Active);
            Assert.AreEqual('0', deserializedPadding.Character);
            Assert.AreEqual(10, deserializedPadding.Length);
            Assert.AreEqual(Padding.PaddingDirection.Right, deserializedPadding.Direction);
        }

        [TestMethod()]
        public void Padding_Clone_CreatesExpectedClone()
        {
            var padding = new Padding
            {
                Active = true,
                Character = '0',
                Length = 10,
                Direction = Padding.PaddingDirection.Right
            };
            var clonedPadding = padding.Clone() as Padding;
            Assert.IsNotNull(clonedPadding);
            Assert.AreEqual(padding, clonedPadding);
            Assert.AreNotSame(padding, clonedPadding);
        }

        [TestMethod()]
        public void Padding_Apply_PaddingLeftCorrectly()
        {
            var padding = new Padding()
            {
                Active = true,
                Character = '0',
                Length = 6,
                Direction = Padding.PaddingDirection.Left
            };

            var value = new ConnectorValue() {  Float64 = 123 };

            var result = padding.Apply(value, new List<ConfigRefValue>());

            Assert.AreEqual("000123", result.String);
        }

        [TestMethod()]
        public void Padding_Apply_PaddingRightCorrectly()
        {
            var padding = new Padding()
            {
                Active = true,
                Character = '0',
                Length = 6,
                Direction = Padding.PaddingDirection.Right
            };

            var value = new ConnectorValue() { Float64 = 123 };

            var result = padding.Apply(value, new List<ConfigRefValue>());

            Assert.AreEqual("123000", result.String);
        }

        [TestMethod()]
        public void Padding_Apply_PaddingWillTruncateLeftAlignedCorrectly()
        {
            var padding = new Padding()
            {
                Active = true,
                Character = '0',
                Length = 6,
                Direction = Padding.PaddingDirection.Left
            };

            var value = new ConnectorValue() { Float64 = 1234567 };

            var result = padding.Apply(value, new List<ConfigRefValue>());

            Assert.AreEqual("234567", result.String);
        }

        [TestMethod()]
        public void Padding_Apply_PaddingWillTruncateRightAlignedCorrectly()
        {
            var padding = new Padding()
            {
                Active = true,
                Character = '0',
                Length = 6,
                Direction = Padding.PaddingDirection.Right
            };

            var value = new ConnectorValue() { Float64 = 1234567 };

            var result = padding.Apply(value, new List<ConfigRefValue>());

            Assert.AreEqual("123456", result.String);
        }
    }
}