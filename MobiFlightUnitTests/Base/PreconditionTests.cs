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
            Assert.AreEqual(o.PreconditionType, "none", "Type is not none");
            Assert.AreEqual(o.PreconditionActive, false, "Active is not false");
            Assert.AreEqual(o.PreconditionLogic, "and", "Precondition logic is not and");
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

            Assert.AreEqual(o.PreconditionActive, true, "Active not the same");
            Assert.AreEqual(o.PreconditionLabel, "TestLabel", "Label not the same");
            Assert.AreEqual(o.PreconditionLogic, "or", "Logic not the same");
            Assert.AreEqual(o.PreconditionOperand, "<", "Operand not the same");
            Assert.AreEqual(o.PreconditionPin, null, "Pin not the same");
            Assert.AreEqual(o.PreconditionRef, "TestRef", "Ref not the same");
            Assert.AreEqual(o.PreconditionSerial, null, "Serial not the same");
            Assert.AreEqual(o.PreconditionType, "config", "Type not the same");
            Assert.AreEqual(o.PreconditionValue, "0", "Value not the same");

            o = new Precondition();
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.PreconditionActive, true, "Active not the same");
            Assert.AreEqual(o.PreconditionLabel, "TestLabel", "Label not the same");
            Assert.AreEqual(o.PreconditionLogic, "or", "Logic not the same");
            Assert.AreEqual(o.PreconditionOperand, "<", "Operand not the same");
            Assert.AreEqual(o.PreconditionPin, "TestPin", "Pin not the same");
            Assert.AreEqual(o.PreconditionRef, null, "Ref not the same");
            Assert.AreEqual(o.PreconditionSerial, "TestSerial", "Serial not the same");
            Assert.AreEqual(o.PreconditionType, "pin", "Type not the same");
            Assert.AreEqual(o.PreconditionValue, "0", "Value not the same");
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
            o.PreconditionType = "pin";
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
            Assert.AreEqual(o.PreconditionActive, c.PreconditionActive, "Active not the same");
            Assert.AreEqual(o.PreconditionLabel, c.PreconditionLabel, "Label not the same");
            Assert.AreEqual(o.PreconditionLogic, c.PreconditionLogic, "Logic not the same");
            Assert.AreEqual(o.PreconditionOperand, c.PreconditionOperand, "Operand not the same");
            Assert.AreEqual(o.PreconditionPin, c.PreconditionPin, "Pin not the same");
            Assert.AreEqual(o.PreconditionRef, c.PreconditionRef, "Ref not the same");
            Assert.AreEqual(o.PreconditionSerial, c.PreconditionSerial, "Serial not the same");
            Assert.AreEqual(o.PreconditionType, c.PreconditionType, "Type not the same");
            Assert.AreEqual(o.PreconditionValue, c.PreconditionValue, "Value not the same");
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
            o.PreconditionActive = true;
            o.PreconditionLabel = "TestPreCon";
            o.PreconditionLogic = Precondition.LOGIC_OR;
            o.PreconditionOperand = "<";
            o.PreconditionPin = "TestPin";
            o.PreconditionRef = "TestRef";
            o.PreconditionSerial = "TestSerial";
            o.PreconditionType = "config";
            o.PreconditionValue = "TestValue";

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