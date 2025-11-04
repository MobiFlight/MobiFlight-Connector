using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Assert.AreEqual(o.Type, "none", "Type is not none");
            Assert.AreEqual(o.Active, true, "Active is not true");
            Assert.AreEqual(o.Logic, "and", "Precondition logic is not and");
            Assert.AreEqual(o.Operand, Precondition.OPERAND_DEFAULT, "Precondition operand is not the OPERAND_DEFAULT");
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

            Assert.AreEqual(o.Active, true, "Active not the same");
            Assert.AreEqual(o.Label, "TestLabel", "Label not the same");
            Assert.AreEqual(o.Logic, "or", "Logic not the same");
            Assert.AreEqual(o.Operand, "<", "Operand not the same");
            Assert.AreEqual(o.Pin, null, "Pin not the same");
            Assert.AreEqual(o.Ref, "TestRef", "Ref not the same");
            Assert.AreEqual(o.Serial, null, "Serial not the same");
            Assert.AreEqual(o.Type, "config", "Type not the same");
            Assert.AreEqual(o.Value, "0", "Value not the same");

            o = new Precondition();
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.Active, true, "Active not the same");
            Assert.AreEqual(o.Label, "TestLabel", "Label not the same");
            Assert.AreEqual(o.Logic, "or", "Logic not the same");
            Assert.AreEqual(o.Operand, "<", "Operand not the same");
            Assert.AreEqual(o.Pin, "TestPin", "Pin not the same");
            Assert.AreEqual(o.Ref, null, "Ref not the same");
            Assert.AreEqual(o.Serial, "TestSerial", "Serial not the same");
            Assert.AreEqual(o.Type, "pin", "Type not the same");
            Assert.AreEqual(o.Value, "0", "Value not the same");
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

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            Precondition o = _generateTestObject();
            Precondition c = (Precondition)o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(o.Active, c.Active, "Active not the same");
            Assert.AreEqual(o.Label, c.Label, "Label not the same");
            Assert.AreEqual(o.Logic, c.Logic, "Logic not the same");
            Assert.AreEqual(o.Operand, c.Operand, "Operand not the same");
            Assert.AreEqual(o.Pin, c.Pin, "Pin not the same");
            Assert.AreEqual(o.Ref, c.Ref, "Ref not the same");
            Assert.AreEqual(o.Serial, c.Serial, "Serial not the same");
            Assert.AreEqual(o.Type, c.Type, "Type not the same");
            Assert.AreEqual(o.Value, c.Value, "Value not the same");
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Precondition o = _generateTestObject();
            Assert.AreEqual(o.ToString(), "TestPreCon", "String value is not correct");
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
    }
}