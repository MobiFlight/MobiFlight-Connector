using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.ProSim;
using Newtonsoft.Json;
using System;

namespace MobiFlight.Tests.ProSim
{
    [TestClass()]
    public class ProSimSourceTests
    {
        [TestMethod()]
        public void ProSimSourceTest()
        {
            ProSimSource source = new ProSimSource();
            Assert.IsNotNull(source);
            Assert.AreEqual("PROSIM", source.SourceType);
            Assert.IsNotNull(source.ProSimDataRef);
        }

        [TestMethod()]
        public void CloneTest()
        {
            ProSimSource original = new ProSimSource();
            original.ProSimDataRef.Path = "test/dataref/path";

            ProSimSource clone = (ProSimSource)original.Clone();
            
            Assert.AreNotSame(original, clone, "Cloned object should not be the same reference");
            Assert.AreEqual(original.ProSimDataRef.Path, clone.ProSimDataRef.Path, "Cloned ProSimDataRef Path should be the same");
        }

        [TestMethod()]
        public void JsonSerializationTest()
        {
            var original = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path"
                }
            };

            string json = JsonConvert.SerializeObject(original);
            var deserialized = JsonConvert.DeserializeObject<ProSimSource>(json);

            Assert.AreEqual(original.SourceType, deserialized.SourceType, "SourceType should be preserved through JSON serialization");
            Assert.AreEqual(original.ProSimDataRef.Path, deserialized.ProSimDataRef.Path, "ProSimDataRef Path should be preserved through JSON serialization");
        }

        [TestMethod()]
        public void JsonSerializationTest_EmptyPath()
        {
            var original = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = ""
                }
            };

            string json = JsonConvert.SerializeObject(original);
            var deserialized = JsonConvert.DeserializeObject<ProSimSource>(json);

            Assert.AreEqual(original.SourceType, deserialized.SourceType, "SourceType should be preserved through JSON serialization");
            Assert.AreEqual(original.ProSimDataRef.Path, deserialized.ProSimDataRef.Path, "Empty ProSimDataRef Path should be preserved through JSON serialization");
        }

        [TestMethod()]
        public void JsonSerializationTest_DefaultProSimDataRef()
        {
            var original = new ProSimSource();

            string json = JsonConvert.SerializeObject(original);
            var deserialized = JsonConvert.DeserializeObject<ProSimSource>(json);

            Assert.AreEqual(original.SourceType, deserialized.SourceType, "SourceType should be preserved through JSON serialization");
            Assert.IsNotNull(deserialized.ProSimDataRef, "ProSimDataRef should not be null");
            Assert.AreEqual("", deserialized.ProSimDataRef.Path, "Default ProSimDataRef Path should be empty string");
        }

        [TestMethod()]
        public void JsonSerializationTest_WithCustomSettings()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            var original = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path"
                }
            };

            string json = JsonConvert.SerializeObject(original, serializerSettings);
            var deserialized = JsonConvert.DeserializeObject<ProSimSource>(json, serializerSettings);

            Assert.AreEqual(original.SourceType, deserialized.SourceType, "SourceType should be preserved through JSON serialization with custom settings");
            Assert.AreEqual(original.ProSimDataRef.Path, deserialized.ProSimDataRef.Path, "ProSimDataRef Path should be preserved through JSON serialization with custom settings");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            ProSimSource source1 = new ProSimSource();
            ProSimSource source2 = new ProSimSource();
            
            // Test equality with same default values
            Assert.IsTrue(source1.Equals(source2), "Objects with same default values should be equal");
            
            // Test equality with same non-default values
            source1.ProSimDataRef.Path = "test/path";
            source2.ProSimDataRef.Path = "test/path";
            Assert.IsTrue(source1.Equals(source2), "Objects with same ProSimDataRef paths should be equal");
            
            // Test inequality with different paths
            source2.ProSimDataRef.Path = "different/path";
            Assert.IsFalse(source1.Equals(source2), "Objects with different ProSimDataRef paths should not be equal");
            
            // Test with null
            Assert.IsFalse(source1.Equals(null), "Object should not equal null");
            
            // Test with different type
            Assert.IsFalse(source1.Equals("string"), "Object should not equal different type");
        }

        [TestMethod()]
        public void EqualsTest_WithDifferentSourceTypes()
        {
            ProSimSource proSimSource = new ProSimSource();
            FsuipcSource fsuipcSource = new FsuipcSource();
            
            // Test that different source types are not equal
            Assert.IsFalse(proSimSource.Equals(fsuipcSource), "Different source types should not be equal");
            Assert.IsFalse(fsuipcSource.Equals(proSimSource), "Different source types should not be equal");
        }

        [TestMethod()]
        public void RoundTripJsonTest()
        {
            var original = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path/with/special/chars"
                }
            };

            string json = JsonConvert.SerializeObject(original);
            var deserialized = JsonConvert.DeserializeObject<ProSimSource>(json);

            // Verify round trip
            Assert.AreEqual(original.SourceType, deserialized.SourceType, "SourceType should be preserved through JSON round trip");
            Assert.AreEqual(original.ProSimDataRef.Path, deserialized.ProSimDataRef.Path, "ProSimDataRef Path should be preserved through JSON round trip");
        }

        [TestMethod()]
        public void JsonSerializationTest_AsSource()
        {
            Source original = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path"
                }
            };

            string json = JsonConvert.SerializeObject(original);
            var deserialized = JsonConvert.DeserializeObject<Source>(json);

            Assert.IsInstanceOfType(deserialized, typeof(ProSimSource), "Deserialized object should be of type ProSimSource");
            Assert.AreEqual(original.SourceType, deserialized.SourceType, "SourceType should be preserved through JSON serialization");
            Assert.AreEqual(((ProSimSource)original).ProSimDataRef.Path, ((ProSimSource)deserialized).ProSimDataRef.Path, "ProSimDataRef Path should be preserved through JSON serialization");
        }

        [TestMethod()]
        public void JsonSerializationTest_ComplexPath()
        {
            var original = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "sim/cockpit2/radios/indicators/hsi_obs_deg_mag_pilot"
                }
            };

            string json = JsonConvert.SerializeObject(original);
            var deserialized = JsonConvert.DeserializeObject<ProSimSource>(json);

            Assert.AreEqual(original.SourceType, deserialized.SourceType, "SourceType should be preserved through JSON serialization");
            Assert.AreEqual(original.ProSimDataRef.Path, deserialized.ProSimDataRef.Path, "Complex ProSimDataRef Path should be preserved through JSON serialization");
        }

        [TestMethod()]
        public void JsonSerializationTest_WithNullProSimDataRef()
        {
            var original = new ProSimSource
            {
                ProSimDataRef = null
            };

            string json = JsonConvert.SerializeObject(original);
            var deserialized = JsonConvert.DeserializeObject<ProSimSource>(json);

            Assert.AreEqual(original.SourceType, deserialized.SourceType, "SourceType should be preserved through JSON serialization");
            Assert.IsNull(deserialized.ProSimDataRef, "Null ProSimDataRef should be preserved through JSON serialization");
        }

        [TestMethod()]
        public void JsonSerializationTest_WithEmptyObject()
        {
            var original = new ProSimSource();

            string json = JsonConvert.SerializeObject(original);
            var deserialized = JsonConvert.DeserializeObject<ProSimSource>(json);

            Assert.AreEqual(original.SourceType, deserialized.SourceType, "SourceType should be preserved through JSON serialization");
            Assert.IsNotNull(deserialized.ProSimDataRef, "ProSimDataRef should not be null for default object");
            Assert.AreEqual("", deserialized.ProSimDataRef.Path, "Default ProSimDataRef Path should be empty string");
        }
    }
} 