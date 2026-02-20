using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class PreconditionTests
    {
        [TestMethod()]
        public void PreconditionTest()
        {
            Precondition o = new Precondition();
            Assert.IsNotNull(o, "Object is null");
            Assert.AreEqual("none", o.Type, "Type is not none");
            Assert.IsTrue(o.Active, "Active is not true");
            Assert.AreEqual("and", o.Logic, "Precondition logic is not and");
            Assert.AreEqual(Precondition.OPERAND_DEFAULT, o.Operand, "Precondition operand is not the OPERAND_DEFAULT");
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            Precondition o = new Precondition();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            Precondition o = new Precondition();
            String s = System.IO.File.ReadAllText(@"assets\Base\Precondition\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("precondition");
            o.ReadXml(xmlReader);

            Assert.IsTrue(o.Active, "Active not the same");
            Assert.AreEqual("TestLabel", o.Label, "Label not the same");
            Assert.AreEqual("or", o.Logic, "Logic not the same");
            Assert.AreEqual("<", o.Operand, "Operand not the same");
            Assert.IsNull(o.Pin, "Pin not the same");
            Assert.AreEqual("TestRef", o.Ref, "Ref not the same");
            Assert.IsNull(o.Serial, "Serial not the same");
            Assert.AreEqual("config", o.Type, "Type not the same");
            Assert.AreEqual("0", o.Value, "Value not the same");

            o = new Precondition();
            o.ReadXml(xmlReader);

            Assert.IsTrue(o.Active, "Active not the same");
            Assert.AreEqual("TestLabel", o.Label, "Label not the same");
            Assert.AreEqual("or", o.Logic, "Logic not the same");
            Assert.AreEqual("<", o.Operand, "Operand not the same");
            Assert.AreEqual("TestPin", o.Pin, "Pin not the same");
            Assert.IsNull(o.Ref, "Ref not the same");
            Assert.AreEqual("TestSerial", o.Serial, "Serial not the same");
            Assert.AreEqual("pin", o.Type, "Type not the same");
            Assert.AreEqual("0", o.Value, "Value not the same");
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

            Precondition o = _generateTestObject();
            xmlWriter.WriteStartElement("preconditions");
            o.WriteXml(xmlWriter);
            o.Type = "pin";
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\Base\Precondition\WriteXmlTest.1.xml");

            Assert.AreEqual(result, s, "The both strings are not equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            Precondition o = _generateTestObject();
            Precondition c = (Precondition)o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(c.Active, o.Active, "Active not the same");
            Assert.AreEqual(c.Label, o.Label, "Label not the same");
            Assert.AreEqual(c.Logic, o.Logic, "Logic not the same");
            Assert.AreEqual(c.Operand, o.Operand, "Operand not the same");
            Assert.AreEqual(c.Pin, o.Pin, "Pin not the same");
            Assert.AreEqual(c.Ref, o.Ref, "Ref not the same");
            Assert.AreEqual(c.Serial, o.Serial, "Serial not the same");
            Assert.AreEqual(c.Type, o.Type, "Type not the same");
            Assert.AreEqual(c.Value, o.Value, "Value not the same");
        }

        [TestMethod()]
        public void IsEmpty_ShouldReturnTrueForDefaultPrecondition()
        {
            Precondition o = new Precondition();
            Assert.IsTrue(o.IsEmpty(), "Default Precondition is not empty");
        }

        [TestMethod()]
        public void IsEmpty_ShouldReturnFalseForNonDefaultPrecondition()
        {
            Precondition o = _generateTestObject();
            Assert.IsFalse(o.IsEmpty(), "Non-default Precondition is empty");
        }

        [TestMethod()]
        public void IsEmpty_ShouldReturnTrueWhenTypeIsNoneAndOtherFieldsHaveValues()
        {
            var o = new Precondition();
            o.Type = "none";
            o.Ref = "SomeRef";

            Assert.IsTrue(o.IsEmpty(), "Precondition with Type 'none' is always empty");

            o.Ref = null;
            o.Serial = "SomeSerial";
            Assert.IsTrue(o.IsEmpty(), "Precondition with Type 'none' is always empty");

            o.Serial = null;
            o.Pin = "SomePin";
            Assert.IsTrue(o.IsEmpty(), "Precondition with Type 'none' is always empty");

            o.Pin = null;
            o.Value = "SomeValue";
            Assert.IsTrue(o.IsEmpty(), "Precondition with Type 'none' is always empty");
        }

        [TestMethod()]
        public void IsEmpty_ShouldReturnTrueWhenTypeIsSomeTypeAndAllOtherFieldsAreNull()
        {
            var o = new Precondition();
            o.Type = "config";
            
            Assert.IsTrue(o.IsEmpty(), "Precondition with Type 'none' is always empty");
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Precondition o = _generateTestObject();
            Assert.AreEqual("TestPreCon", o.ToString(), "String value is not correct");
        }

        private Precondition _generateTestObject()
        {
            Precondition o = new Precondition();
            o.Active = true;
            o.Label = "TestPreCon";
            o.Logic = Precondition.LOGIC_OR;
            o.Operand = "<";
            o.Pin = "TestPin";
            o.Ref = "TestRef";
            o.Serial = "TestSerial";
            o.Type = "config";
            o.Value = "TestValue";

            return o;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Precondition o1 = new Precondition();
            Precondition o2 = new Precondition();

            Assert.IsTrue(o1.Equals(o2));

            o1 = _generateTestObject();

            Assert.IsFalse(o1.Equals(o2));

            o2 = _generateTestObject();
            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod]
        public void Empty_Precondition_ShouldSerializeAsEmptyString()
        {
            // Arrange
            var p = new Precondition(); // defaults to Type == "none" and other fields null

            // Act
            var json = JsonConvert.SerializeObject(p);

            // Assert
            Assert.AreEqual("", json, "Empty Precondition must be serialized to empty when the converter is enabled.");
        }

        [TestMethod]
        public void NonEmpty_Precondition_ShouldNotSerializeAsEmptyString()
        {
            // Arrange
            var p = _generateTestObject();

            // Act
            var json = JsonConvert.SerializeObject(p);

            // Assert
            Assert.AreNotEqual("", json, "Non-empty Precondition must not be serialized to empty when the converter is enabled.");
            Assert.IsFalse(string.IsNullOrEmpty(json), "Serialized JSON should not be empty.");
        }
    }
}