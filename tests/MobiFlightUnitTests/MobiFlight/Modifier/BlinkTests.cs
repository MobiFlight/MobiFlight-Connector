using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobiFlight.Modifier.Tests
{
    [TestClass()]
    public class BlinkTests
    {
        [TestMethod()]
        public void EqualsTest()
        {
            // Check for equality between two Blink objects
            var blink1 = new Blink();
            var blink2 = new Blink();
            Assert.AreEqual(blink1, blink2);

            // Check for inequality between two Blink objects
            blink1.BlinkValue = "1";
            Assert.AreNotEqual(blink1, blink2);

            blink2.BlinkValue = "1";
            Assert.AreEqual(blink1, blink2);

            blink1.OnOffSequence.Add(1);
            Assert.AreNotEqual(blink1, blink2);

            blink2.OnOffSequence.Add(1);
            Assert.AreEqual(blink1, blink2);
        }

        [TestMethod()]
        public void CloneTest()
        {
            // Check for equality between two Blink objects
            var blink1 = new Blink();
            blink1.BlinkValue = "1";
            blink1.OnOffSequence.Add(200);
            blink1.OnOffSequence.Add(500);
            var blink2 = blink1.Clone() as Blink;
            Assert.IsNotNull(blink2);
            Assert.AreEqual(blink1, blink2);
            Assert.AreNotSame(blink1, blink2);
        }

        [TestMethod()]
        public void Blink_JsonSerializationTest()
        {
            var blink = new Blink()
            {
                Active = true,
                BlinkValue = "1",
                OnOffSequence = new List<int>() { 200, 500 }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(blink);

            Assert.Contains("\"Active\":true", json);
            Assert.Contains("\"Type\":\"Blink\"", json);
            Assert.Contains("\"BlinkValue\":\"1\"", json);
            Assert.Contains("\"OnOffSequence\":[200,500]", json);

            var deserializedBlink = Newtonsoft.Json.JsonConvert.DeserializeObject<Blink>(json);
            Assert.AreEqual(blink, deserializedBlink);
        }

        [TestMethod()]
        public void Blink_JsonDeserializationTest()
        {
            var json = "{\"Active\":true,\"Type\":\"Blink\",\"BlinkValue\":\"1\",\"OnOffSequence\":[200,500]}";
            var blink = Newtonsoft.Json.JsonConvert.DeserializeObject<Blink>(json);
            Assert.IsNotNull(blink);
            Assert.IsTrue(blink.Active);
            Assert.AreEqual("1", blink.BlinkValue);
            CollectionAssert.AreEqual(new List<int>() { 200, 500 }, blink.OnOffSequence);
        }
        [TestMethod]
        public async Task Blink_Apply()
        {
            //Test if blink is (On)

            var blinkOn = new Blink
            {
                Active = true,
                BlinkValue = "0",
                OnOffSequence = new List<int> { 500, 500 }
            };
            var valueOn = new ConnectorValue
            {
                type = FSUIPCOffsetType.Integer,
                Float64 = 1
            };

            valueOn = blinkOn.Apply(valueOn, new List<ConfigRefValue>());
            Assert.AreEqual(1, valueOn.Float64);

            //Test if Blink is (Off)

            var blinkOff = new Blink
            {
                Active = true,
                BlinkValue = "1",
                OnOffSequence = new List<int> { 500, 500 }
            };
            var valueOff = new ConnectorValue
            {
                type = FSUIPCOffsetType.Integer,
                Float64 = 0
            };

            valueOff = blinkOff.Apply(valueOff, new List<ConfigRefValue>());
            Assert.AreEqual(0, valueOff.Float64);

            //Test string input

            var blinkString = new Blink
            {
                Active = true,
                BlinkValue = "1",
                OnOffSequence = new List<int> { -1, 500 }
            };
            var valueString = new ConnectorValue
            {
                type = FSUIPCOffsetType.String,
                String = "0"
            };

            valueString = blinkString.Apply(valueString, new List<ConfigRefValue>());
            Assert.AreEqual("1", valueString.String);

            // Test invalid BlinkValue (Double.TryParse returns false)

            var blinkInvalid = new Blink
            {
                Active = true,
                BlinkValue = "ABC",
                OnOffSequence = new List<int> { -1, 500 }
            };
            var valueInvalid = new ConnectorValue
            {
                type = FSUIPCOffsetType.Integer,
                Float64 = 0
            };

            valueInvalid = blinkInvalid.Apply(valueInvalid, new List<ConfigRefValue>());
            Assert.AreEqual(FSUIPCOffsetType.String, valueInvalid.type);
            Assert.AreEqual("ABC", valueInvalid.String);

            //Test if Active = false

            var blinkActive = new Blink
            {
                Active = false,
                BlinkValue = "1",
                OnOffSequence = new List<int> { 500, 500 }
            };
            var valueActive = new ConnectorValue
            {
                type = FSUIPCOffsetType.Integer,
                Float64 = 1
            };

            valueActive = blinkActive.Apply(valueActive, new List<ConfigRefValue>());
            Assert.AreEqual(1, valueActive.Float64);
            Assert.AreEqual(FSUIPCOffsetType.Integer, valueActive.type);
        }
        [TestMethod]
        public void Blink_ReturnsClone()
        {
            var blink = new Blink()
            {
                Active = true,
                BlinkValue = "1",
                OnOffSequence = new List<int>() { 200, 500 }
            };
            var value = new ConnectorValue()
            {
                type = FSUIPCOffsetType.Float,
                Float64 = 1
            };
            var result = blink.Apply(value, new List<ConfigRefValue>());
            Assert.AreNotSame(value, result);

        }
        [TestMethod]
        public void Blink_ApplyDoesNotChangeOriginalValue()
        {
            var blink = new Blink()
            {
                Active = true,
                BlinkValue = "2",
                OnOffSequence = new List<int>() { -1, 500 }
            };
            var value = new ConnectorValue()
            {
                type = FSUIPCOffsetType.Float,
                Float64 = 1
            };
            var originalBlink = value.Clone() as ConnectorValue;
            var result = blink.Apply(value, new List<ConfigRefValue>());
            Assert.AreNotEqual(originalBlink.Float64, result.Float64);
        }
    }
}