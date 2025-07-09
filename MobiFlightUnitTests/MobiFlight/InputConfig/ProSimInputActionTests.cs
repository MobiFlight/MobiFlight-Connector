using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.InputConfig;
using System;
using System.IO;
using System.Xml;

namespace MobiFlight.Tests.ProSim
{
    [TestClass()]
    public class ProSimInputActionTests
    {
        [TestMethod()]
        public void ProSimInputActionTest()
        {
            ProSimInputAction action = new ProSimInputAction();
            Assert.IsNotNull(action);
            Assert.AreEqual("", action.Path);
            Assert.AreEqual("$", action.Expression);
        }

        [TestMethod()]
        public void CloneTest()
        {
            ProSimInputAction original = new ProSimInputAction();
            original.Path = "test/dataref/path";
            original.Expression = "$ * 2";

            ProSimInputAction clone = (ProSimInputAction)original.Clone();
            
            Assert.AreNotSame(original, clone, "Cloned object should not be the same reference");
            Assert.AreEqual(original.Path, clone.Path, "Cloned Path should be the same");
            Assert.AreEqual(original.Expression, clone.Expression, "Cloned Expression should be the same");
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            ProSimInputAction action = new ProSimInputAction();
            
            // Test reading XML with both attributes
            string xml = "<ProSimInputAction path=\"test/dataref/path\" expression=\"$ * 2\" />";
            StringReader sr = new StringReader(xml);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            
            XmlReader xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("ProSimInputAction");
            action.ReadXml(xmlReader);
            
            Assert.AreEqual("test/dataref/path", action.Path, "Path should be read correctly from XML");
            Assert.AreEqual("$ * 2", action.Expression, "Expression should be read correctly from XML");
        }

        [TestMethod()]
        public void ReadXmlTest_EmptyAttributes()
        {
            ProSimInputAction action = new ProSimInputAction();
            
            // Test reading XML with empty attributes
            string xml = "<ProSimInputAction path=\"\" expression=\"\" />";
            StringReader sr = new StringReader(xml);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            
            XmlReader xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("ProSimInputAction");
            action.ReadXml(xmlReader);
            
            Assert.AreEqual("", action.Path, "Path should be empty when path attribute is empty");
            Assert.AreEqual("", action.Expression, "Expression should be empty when expression attribute is empty");
        }

        [TestMethod()]
        public void ReadXmlTest_MissingAttributes()
        {
            ProSimInputAction action = new ProSimInputAction();
            
            // Test reading XML with missing attributes
            string xml = "<ProSimInputAction />";
            StringReader sr = new StringReader(xml);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            
            XmlReader xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("ProSimInputAction");
            action.ReadXml(xmlReader);
            
            Assert.AreEqual("", action.Path, "Path should be empty string when path attribute is missing");
            Assert.AreEqual("$", action.Expression, "Expression should be default value when expression attribute is missing");
        }

        [TestMethod()]
        public void ReadXmlTest_MissingPathAttribute()
        {
            ProSimInputAction action = new ProSimInputAction();
            
            // Test reading XML with missing path attribute
            string xml = "<ProSimInputAction expression=\"$ * 2\" />";
            StringReader sr = new StringReader(xml);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            
            XmlReader xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("ProSimInputAction");
            action.ReadXml(xmlReader);
            
            Assert.AreEqual("", action.Path, "Path should be empty string when path attribute is missing");
            Assert.AreEqual("$ * 2", action.Expression, "Expression should be read correctly from XML");
        }

        [TestMethod()]
        public void ReadXmlTest_MissingExpressionAttribute()
        {
            ProSimInputAction action = new ProSimInputAction();
            
            // Test reading XML with missing expression attribute
            string xml = "<ProSimInputAction path=\"test/dataref/path\" />";
            StringReader sr = new StringReader(xml);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            
            XmlReader xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("ProSimInputAction");
            action.ReadXml(xmlReader);
            
            Assert.AreEqual("test/dataref/path", action.Path, "Path should be read correctly from XML");
            Assert.AreEqual("$", action.Expression, "Expression should be default value when expression attribute is missing");
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            ProSimInputAction action = new ProSimInputAction();
            action.Path = "test/dataref/path";
            action.Expression = "$ * 2";
            
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            
            XmlWriter xmlWriter = XmlWriter.Create(sw, settings);
            xmlWriter.WriteStartElement("ProSimInputAction");
            action.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            
            string result = sw.ToString();
            
            // Verify the XML contains the expected attributes
            Assert.IsTrue(result.Contains("path=\"test/dataref/path\""), "XML should contain path attribute");
            Assert.IsTrue(result.Contains("expression=\"$ * 2\""), "XML should contain expression attribute");
        }

        [TestMethod()]
        public void WriteXmlTest_EmptyValues()
        {
            ProSimInputAction action = new ProSimInputAction();
            action.Path = "";
            action.Expression = "";
            
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            
            XmlWriter xmlWriter = XmlWriter.Create(sw, settings);
            xmlWriter.WriteStartElement("ProSimInputAction");
            action.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            
            string result = sw.ToString();
            
            // Verify the XML contains the expected attributes
            Assert.IsTrue(result.Contains("path=\"\""), "XML should contain empty path attribute");
            Assert.IsTrue(result.Contains("expression=\"\""), "XML should contain empty expression attribute");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            ProSimInputAction action1 = new ProSimInputAction();
            ProSimInputAction action2 = new ProSimInputAction();
            
            // Test equality with same default values
            Assert.IsTrue(action1.Equals(action2), "Objects with same default values should be equal");
            
            // Test equality with same non-default values
            action1.Path = "test/path";
            action1.Expression = "test/expression";
            action2.Path = "test/path";
            action2.Expression = "test/expression";
            Assert.IsTrue(action1.Equals(action2), "Objects with same values should be equal");
            
            // Test inequality with different paths
            action2.Path = "different/path";
            Assert.IsFalse(action1.Equals(action2), "Objects with different paths should not be equal");
            
            // Test inequality with different expressions
            action2.Path = "test/path";
            action2.Expression = "different/expression";
            Assert.IsFalse(action1.Equals(action2), "Objects with different expressions should not be equal");
            
            // Test with null
            Assert.IsFalse(action1.Equals(null), "Object should not equal null");
            
            // Test with different type
            Assert.IsFalse(action1.Equals("string"), "Object should not equal different type");
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            ProSimInputAction action1 = new ProSimInputAction();
            ProSimInputAction action2 = new ProSimInputAction();
            
            // Test hash codes for equal objects
            Assert.AreEqual(action1.GetHashCode(), action2.GetHashCode(), "Equal objects should have same hash code");
            
            // Test hash codes for different objects
            action1.Path = "test/path";
            action1.Expression = "test/expression";
            Assert.AreNotEqual(action1.GetHashCode(), action2.GetHashCode(), "Different objects should have different hash codes");
        }

        [TestMethod()]
        public void RoundTripXmlTest()
        {
            ProSimInputAction original = new ProSimInputAction();
            original.Path = "test/dataref/path";
            original.Expression = "$ * 2 + @";
            
            // Write to XML
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            
            XmlWriter xmlWriter = XmlWriter.Create(sw, settings);
            xmlWriter.WriteStartElement("ProSimInputAction");
            original.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            
            string xml = sw.ToString();
            
            // Read from XML
            ProSimInputAction deserialized = new ProSimInputAction();
            StringReader sr = new StringReader(xml);
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            
            XmlReader xmlReader = XmlReader.Create(sr, readerSettings);
            xmlReader.ReadToDescendant("ProSimInputAction");
            deserialized.ReadXml(xmlReader);
            
            // Verify round trip
            Assert.AreEqual(original.Path, deserialized.Path, "Path should be preserved through XML round trip");
            Assert.AreEqual(original.Expression, deserialized.Expression, "Expression should be preserved through XML round trip");
        }

        [TestMethod()]
        public void RoundTripXmlTest_WithSpecialCharacters()
        {
            ProSimInputAction original = new ProSimInputAction();
            original.Path = "test/dataref/path/with/special/chars";
            original.Expression = "$ * 2 + @ + \"test\"";
            
            // Write to XML
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            
            XmlWriter xmlWriter = XmlWriter.Create(sw, settings);
            xmlWriter.WriteStartElement("ProSimInputAction");
            original.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            
            string xml = sw.ToString();
            
            // Read from XML
            ProSimInputAction deserialized = new ProSimInputAction();
            StringReader sr = new StringReader(xml);
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            
            XmlReader xmlReader = XmlReader.Create(sr, readerSettings);
            xmlReader.ReadToDescendant("ProSimInputAction");
            deserialized.ReadXml(xmlReader);
            
            // Verify round trip
            Assert.AreEqual(original.Path, deserialized.Path, "Path should be preserved through XML round trip");
            Assert.AreEqual(original.Expression, deserialized.Expression, "Expression should be preserved through XML round trip");
        }
    }
} 