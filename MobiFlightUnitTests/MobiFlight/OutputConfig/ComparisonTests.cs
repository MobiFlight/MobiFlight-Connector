using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.OutputConfig.Tests
{
    [TestClass()]
    public class ComparisonTests
    {
        [TestMethod()]
        public void ComparisonTest()
        {
            Comparison o = new Comparison();
            Assert.IsNotNull(o, "Object is null");
            Assert.AreEqual(false, o.Active, "Active not true");
            Assert.AreEqual("", o.Value, "Value not empty");
            Assert.AreEqual("", o.Operand, "Operand not empty");
            Assert.AreEqual("", o.IfValue, "IfValue not empty");
            Assert.AreEqual("", o.ElseValue, "ElseValue not empty");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Comparison o1 = new Comparison();
            Comparison o2 = new Comparison();
            Assert.IsTrue(o1.Equals(o2));

            o1.Active = true;
            o1.Operand = "&&";
            o1.Value = "Value";
            o1.IfValue = "IfValue";
            o1.ElseValue = "ElseValue";
            Assert.IsFalse(o1.Equals(o2));

            o2.Active = true;
            o2.Operand = "&&";
            o2.Value = "Value";
            o2.IfValue = "IfValue";
            o2.ElseValue = "ElseValue";
            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            Comparison o = new Comparison();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            Comparison o = new Comparison();

            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\Comparison\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("comparison");
            o.ReadXml(xmlReader);

            Assert.AreEqual("Value", o.Value, "Value does not match.");
            Assert.AreEqual("Operand", o.Operand, "Operand does not match.");
            Assert.AreEqual("ifValue", o.IfValue, "ifValue does not match.");
            Assert.AreEqual("elseValue", o.ElseValue, "elseValue does not match.");
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

            Comparison o = new Comparison();
            o.Active = true;
            o.Operand = "&&";
            o.Value = "Value";
            o.IfValue = "IfValue";
            o.ElseValue = "ElseValue";

            o.WriteXml(xmlWriter);
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\Comparison\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }
    }
}