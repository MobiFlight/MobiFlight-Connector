using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.InputConfig;
using MobiFlight.ProSim;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml;

namespace MobiFlight.Tests.ProSim
{
    [TestClass()]
    public class ProSimSerializationIntegrationTests
    {
        [TestMethod()]
        public void ProSimInputActionWithProSimSourceIntegrationTest()
        {
            // Create a ProSimInputAction with a ProSimSource
            var inputAction = new ProSimInputAction
            {
                Path = "test/dataref/path",
                Expression = "$ * 2 + @"
            };

            var source = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path"
                }
            };

            // Test XML serialization of ProSimInputAction
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            
            XmlWriter xmlWriter = XmlWriter.Create(sw, settings);
            xmlWriter.WriteStartElement("ProSimInputAction");
            inputAction.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            
            string xml = sw.ToString();
            
            // Test JSON serialization of ProSimSource
            string json = JsonConvert.SerializeObject(source);
            
            // Verify both serializations work correctly
            Assert.IsTrue(xml.Contains("path=\"test/dataref/path\""), "XML should contain path attribute");
            Assert.IsTrue(xml.Contains("expression=\"$ * 2 + @\""), "XML should contain expression attribute");
            Assert.IsTrue(json.Contains("\"Type\":\"ProSimSource\""), "JSON should contain Type");
            Assert.IsTrue(json.Contains("\"Path\":\"test/dataref/path\""), "JSON should contain Path");
        }

        [TestMethod()]
        public void ProSimInputActionXmlRoundTripWithProSimSourceJsonRoundTripTest()
        {
            // Create original objects
            var originalInputAction = new ProSimInputAction
            {
                Path = "test/dataref/path",
                Expression = "$ * 2 + @"
            };

            var originalSource = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path"
                }
            };

            // Serialize ProSimInputAction to XML
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            
            XmlWriter xmlWriter = XmlWriter.Create(sw, settings);
            xmlWriter.WriteStartElement("ProSimInputAction");
            originalInputAction.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            
            string xml = sw.ToString();
            
            // Serialize ProSimSource to JSON
            string json = JsonConvert.SerializeObject(originalSource);
            
            // Deserialize ProSimInputAction from XML
            ProSimInputAction deserializedInputAction = new ProSimInputAction();
            StringReader sr = new StringReader(xml);
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            
            XmlReader xmlReader = XmlReader.Create(sr, readerSettings);
            xmlReader.ReadToDescendant("ProSimInputAction");
            deserializedInputAction.ReadXml(xmlReader);
            
            // Deserialize ProSimSource from JSON
            var deserializedSource = JsonConvert.DeserializeObject<ProSimSource>(json);
            
            // Verify round trips work correctly
            Assert.AreEqual(originalInputAction.Path, deserializedInputAction.Path, "ProSimInputAction Path should be preserved through XML round trip");
            Assert.AreEqual(originalInputAction.Expression, deserializedInputAction.Expression, "ProSimInputAction Expression should be preserved through XML round trip");
            Assert.AreEqual(originalSource.SourceType, deserializedSource.SourceType, "ProSimSource SourceType should be preserved through JSON round trip");
            Assert.AreEqual(originalSource.ProSimDataRef.Path, deserializedSource.ProSimDataRef.Path, "ProSimSource ProSimDataRef Path should be preserved through JSON round trip");
        }

        [TestMethod()]
        public void ProSimInputActionCloneWithProSimSourceCloneTest()
        {
            // Create original objects
            var originalInputAction = new ProSimInputAction
            {
                Path = "test/dataref/path",
                Expression = "$ * 2 + @"
            };

            var originalSource = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path"
                }
            };

            // Clone both objects
            var clonedInputAction = (ProSimInputAction)originalInputAction.Clone();
            var clonedSource = (ProSimSource)originalSource.Clone();
            
            // Verify clones are not the same references
            Assert.AreNotSame(originalInputAction, clonedInputAction, "Cloned ProSimInputAction should not be the same reference");
            Assert.AreNotSame(originalSource, clonedSource, "Cloned ProSimSource should not be the same reference");
            
            // Verify clone values are the same
            Assert.AreEqual(originalInputAction.Path, clonedInputAction.Path, "Cloned ProSimInputAction Path should be the same");
            Assert.AreEqual(originalInputAction.Expression, clonedInputAction.Expression, "Cloned ProSimInputAction Expression should be the same");
            Assert.AreEqual(originalSource.SourceType, clonedSource.SourceType, "Cloned ProSimSource SourceType should be the same");
            Assert.AreEqual(originalSource.ProSimDataRef.Path, clonedSource.ProSimDataRef.Path, "Cloned ProSimSource ProSimDataRef Path should be the same");
        }

        [TestMethod()]
        public void ProSimInputActionEqualsWithProSimSourceEqualsTest()
        {
            // Create identical objects
            var inputAction1 = new ProSimInputAction
            {
                Path = "test/dataref/path",
                Expression = "$ * 2 + @"
            };

            var inputAction2 = new ProSimInputAction
            {
                Path = "test/dataref/path",
                Expression = "$ * 2 + @"
            };

            var source1 = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path"
                }
            };

            var source2 = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path"
                }
            };

            // Test equality
            Assert.IsTrue(inputAction1.Equals(inputAction2), "Identical ProSimInputAction objects should be equal");
            Assert.IsTrue(source1.Equals(source2), "Identical ProSimSource objects should be equal");
            
            // Test inequality with different values
            inputAction2.Path = "different/path";
            source2.ProSimDataRef.Path = "different/path";
            
            Assert.IsFalse(inputAction1.Equals(inputAction2), "ProSimInputAction objects with different paths should not be equal");
            Assert.IsFalse(source1.Equals(source2), "ProSimSource objects with different paths should not be equal");
        }

        [TestMethod()]
        public void ProSimInputActionXmlSerializationWithSpecialCharactersTest()
        {
            var inputAction = new ProSimInputAction
            {
                Path = "test/dataref/path/with/special/chars/&<>\"'",
                Expression = "$ * 2 + @ + \"test\" & < > \" '"
            };

            // Serialize to XML
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            
            XmlWriter xmlWriter = XmlWriter.Create(sw, settings);
            xmlWriter.WriteStartElement("ProSimInputAction");
            inputAction.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            
            string xml = sw.ToString();
            
            // Deserialize from XML
            ProSimInputAction deserialized = new ProSimInputAction();
            StringReader sr = new StringReader(xml);
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            
            XmlReader xmlReader = XmlReader.Create(sr, readerSettings);
            xmlReader.ReadToDescendant("ProSimInputAction");
            deserialized.ReadXml(xmlReader);
            
            // Verify special characters are handled correctly
            Assert.AreEqual(inputAction.Path, deserialized.Path, "Path with special characters should be preserved");
            Assert.AreEqual(inputAction.Expression, deserialized.Expression, "Expression with special characters should be preserved");
        }

        [TestMethod()]
        public void ProSimSourceJsonSerializationWithSpecialCharactersTest()
        {
            var source = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path/with/special/chars/&<>\"'"
                }
            };

            // Serialize to JSON
            string json = JsonConvert.SerializeObject(source);
            
            // Deserialize from JSON
            var deserialized = JsonConvert.DeserializeObject<ProSimSource>(json);
            
            // Verify special characters are handled correctly
            Assert.AreEqual(source.SourceType, deserialized.SourceType, "SourceType should be preserved");
            Assert.AreEqual(source.ProSimDataRef.Path, deserialized.ProSimDataRef.Path, "Path with special characters should be preserved");
        }

        [TestMethod()]
        public void ProSimInputActionAndProSimSourceMixedSerializationTest()
        {
            // Create objects with different data types
            var inputAction1 = new ProSimInputAction
            {
                Path = "test/dataref/path1",
                Expression = "$ * 2"
            };

            var inputAction2 = new ProSimInputAction
            {
                Path = "test/dataref/path2",
                Expression = "@ + 10"
            };

            var source1 = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path1"
                }
            };

            var source2 = new ProSimSource
            {
                ProSimDataRef = new ProSimDataRef
                {
                    Path = "test/dataref/path2"
                }
            };

            // Serialize all objects
            StringWriter sw1 = new StringWriter();
            XmlWriter xmlWriter1 = XmlWriter.Create(sw1, new XmlWriterSettings { Indent = true });
            xmlWriter1.WriteStartElement("ProSimInputAction");
            inputAction1.WriteXml(xmlWriter1);
            xmlWriter1.WriteEndElement();
            xmlWriter1.Flush();
            string xml1 = sw1.ToString();

            StringWriter sw2 = new StringWriter();
            XmlWriter xmlWriter2 = XmlWriter.Create(sw2, new XmlWriterSettings { Indent = true });
            xmlWriter2.WriteStartElement("ProSimInputAction");
            inputAction2.WriteXml(xmlWriter2);
            xmlWriter2.WriteEndElement();
            xmlWriter2.Flush();
            string xml2 = sw2.ToString();

            string json1 = JsonConvert.SerializeObject(source1);
            string json2 = JsonConvert.SerializeObject(source2);

            // Verify all serializations are different
            Assert.AreNotEqual(xml1, xml2, "Different ProSimInputAction objects should produce different XML");
            Assert.AreNotEqual(json1, json2, "Different ProSimSource objects should produce different JSON");

            // Verify each serialization contains the correct data
            Assert.IsTrue(xml1.Contains("path=\"test/dataref/path1\""), "First XML should contain first path");
            Assert.IsTrue(xml2.Contains("path=\"test/dataref/path2\""), "Second XML should contain second path");
            Assert.IsTrue(json1.Contains("\"Path\":\"test/dataref/path1\""), "First JSON should contain first path");
            Assert.IsTrue(json2.Contains("\"Path\":\"test/dataref/path2\""), "Second JSON should contain second path");
        }
    }
} 