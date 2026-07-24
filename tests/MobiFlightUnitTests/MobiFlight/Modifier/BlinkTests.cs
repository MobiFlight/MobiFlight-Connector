using MobiFlight.Modifier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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