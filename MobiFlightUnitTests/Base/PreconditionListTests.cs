using MobiFlight.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class PreconditionListTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            PreconditionList o = new PreconditionList();
            PreconditionList clone = o.Clone() as PreconditionList;

            Assert.IsNotNull(clone);
            Assert.AreEqual(o.Count, clone.Count);
            Assert.AreEqual(o.ExecuteOnFalse, clone.ExecuteOnFalse);
            Assert.AreEqual(o.FalseCaseValue, clone.FalseCaseValue);

            o.Add(new Precondition());
            o.ExecuteOnFalse = true;
            o.FalseCaseValue = "1";

            Assert.AreNotEqual(o.Count, clone.Count);
            Assert.AreNotEqual(o.ExecuteOnFalse, clone.ExecuteOnFalse);
            Assert.AreNotEqual(o.FalseCaseValue, clone.FalseCaseValue);

            clone = o.Clone() as PreconditionList;
            Assert.AreEqual(o.Count, clone.Count);
            Assert.AreEqual(o.ExecuteOnFalse, clone.ExecuteOnFalse);
            Assert.AreEqual(o.FalseCaseValue, clone.FalseCaseValue);
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            PreconditionList o = new PreconditionList();
            Assert.IsNotNull(o.GetEnumerator());
            Assert.IsTrue(o.GetEnumerator() is List<Precondition>.Enumerator);
        }

        [TestMethod()]
        public void AddTest()
        {
            PreconditionList o = new PreconditionList();
            Assert.IsTrue(o.Count == 0);
            o.Add(new Precondition());
            Assert.IsTrue(o.Count == 1);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            PreconditionList o = new PreconditionList();
            Precondition p = new Precondition();
            o.Add(p);
            Assert.IsTrue(o.Count == 1);
            o.Remove(p);
            Assert.IsTrue(o.Count == 0);
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            PreconditionList o = new PreconditionList();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            PreconditionList o = new PreconditionList();
            String s = System.IO.File.ReadAllText(@"assets\Base\PreconditionList\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("preconditions");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.Count, 0);

            o = new PreconditionList();
            s = System.IO.File.ReadAllText(@"assets\Base\PreconditionList\ReadXmlTest.2.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("preconditions");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.Count, 2);
            Assert.AreEqual(o.ExecuteOnFalse, false);
            Assert.AreEqual(o.FalseCaseValue, "");
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(sw, settings);

            PreconditionList o = new PreconditionList();
            o.Add(new Precondition()
            {
                Type = "config",
                Label = "TestPreCon",
                Active = true,
                Ref = "TestRef",
                Operand = "<",
                Value = "TestValue",
                Logic = "or"
            });
            o.ExecuteOnFalse = true;
            o.FalseCaseValue = "1";
            o.WriteXml(xmlWriter);

            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\Base\PreconditionList\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            PreconditionList o1 = new PreconditionList();
            PreconditionList o2 = new PreconditionList();

            Assert.IsTrue(o1.Equals(o2));
            o1.Add(new Precondition());
            o1.ExecuteOnFalse = true;
            o1.FalseCaseValue = "1";

            Assert.IsFalse(o1.Equals(o2));

            o2.Add(new Precondition());
            o2.ExecuteOnFalse = true;
            o2.FalseCaseValue = "1";

            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void PreconditionList_ShouldSerializeAndDeserializeCorrectly_WithNonEmptyPreconditions()
        {
            var originalList = new PreconditionList
            {
                new Precondition
                {
                    Type = "config",
                    Label = "TestPreCon1",
                    Active = true,
                    Ref = "TestRef1",
                    Operand = "<",
                    Value = "TestValue1",
                    Logic = "or"
                },
                new Precondition
                {
                    Type = "config",
                    Label = "TestPreCon2",
                    Active = false,
                    Ref = "TestRef2",
                    Operand = ">",
                    Value = "TestValue2",
                    Logic = "and"
                }
            };

            string json = JsonConvert.SerializeObject(originalList);
            PreconditionList deserializedList = JsonConvert.DeserializeObject<PreconditionList>(json);

            Assert.IsNotNull(deserializedList);
            Assert.AreEqual(originalList.Count, deserializedList.Count);

            var originalArray = originalList.ToArray();
            var deserializedArray = deserializedList.ToArray();

            for (int i = 0; i < originalList.Count; i++)
            {
                Assert.AreEqual(originalArray[i].Type, deserializedArray[i].Type);
                // labels are not serialized
                Assert.AreNotEqual(originalArray[i].Label, deserializedArray[i].Label);
                // labels are using specifc format
                var Ref = originalArray[i].Ref;
                var Operand = originalArray[i].Operand;
                var Value = originalArray[i].Value;
                var Logic = originalArray[i].Logic;
                var Label = $"Config: <Ref:{Ref}> {Operand} {Value} <Logic:{Logic}>";
                Assert.AreEqual(Label, deserializedArray[i].Label);
                Assert.AreEqual(originalArray[i].Active, deserializedArray[i].Active);
                Assert.AreEqual(originalArray[i].Ref, deserializedArray[i].Ref);
                Assert.AreEqual(originalArray[i].Operand, deserializedArray[i].Operand);
                Assert.AreEqual(originalArray[i].Value, deserializedArray[i].Value);
                Assert.AreEqual(originalArray[i].Logic, deserializedArray[i].Logic);
            }
        }

        [TestMethod()]
        public void PreconditionList_WithEmptyPrecondition_ShouldNotSerializeNull()
        {
            // Arrange
            var list = new PreconditionList();
            var emptyPrecondition = new Precondition(); // Type="none", all fields null
            list.Add(emptyPrecondition);

            // Act
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented, settings);

            // Assert
            Assert.IsFalse(json.Contains("null"), "Serialized JSON should not contain 'null' for empty preconditions");
            // Should serialize as empty array or with no preconditions
            Assert.IsTrue(json.Contains("[]") || !json.Contains("\"Preconditions\""),
                "Empty preconditions should result in empty array or no Preconditions property");
        }

        [TestMethod()]
        public void PreconditionList_WithMixedPreconditions_ShouldOnlySerializeNonEmpty()
        {
            // Arrange
            var list = new PreconditionList();

            // Add empty precondition
            var emptyPrecondition = new Precondition();
            list.Add(emptyPrecondition);

            // Add valid precondition
            var validPrecondition = new Precondition
            {
                Type = "config",
                Ref = "TestRef",
                Operand = "=",
                Value = "1",
                Active = true
            };
            list.Add(validPrecondition);

            // Add another empty one
            var anotherEmpty = new Precondition();
            list.Add(anotherEmpty);

            // Act
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented, settings);

            // Assert
            Assert.IsFalse(json.Contains("null"), "Serialized JSON should not contain 'null'");
            Assert.IsTrue(json.Contains("TestRef"), "Valid precondition should be serialized");

            // Count how many preconditions are in the output
            var deserializedList = JsonConvert.DeserializeObject<PreconditionList>(json);
            Assert.AreEqual(1, deserializedList.Count, "Only non-empty precondition should be in deserialized list");
        }

        [TestMethod()]
        public void PreconditionList_AllEmptyPreconditions_ShouldSerializeAsEmptyArray()
        {
            // Arrange
            var list = new PreconditionList();
            list.Add(new Precondition());
            list.Add(new Precondition());
            list.Add(new Precondition());

            // Act
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented, settings);

            // Assert
            Assert.IsFalse(json.Contains("null"), "Serialized JSON should not contain 'null'");
        }

        [TestMethod()]
        public void ConfigFile_WithEmptyPreconditions_ShouldNotContainNull()
        {
            // Arrange
            var configFile = new ConfigFile();
            var configItem = new OutputConfigItem
            {
                Name = "Test",
                Preconditions = new PreconditionList()
            };
            configItem.Preconditions.Add(new Precondition()); // Empty precondition
            configFile.ConfigItems.Add(configItem);

            // Act
            var json = configFile.ToJson();

            // Assert
            Assert.IsFalse(json.Contains("null"), "ConfigFile JSON should not contain 'null' for empty preconditions");
        }
    }
}