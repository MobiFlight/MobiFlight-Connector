using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Modifier;
using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.Modifier.Tests
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

        [TestMethod()]
        public void ApplyTest()
        {
            var value = new ConnectorValue();
            var c = new Comparison();
            
            c.Active = true;
            c.Operand = "=";
            c.Value = "0";
            c.IfValue = "1";
            c.ElseValue = "2";

            value.type = FSUIPCOffsetType.Integer;
            value.Float64 = 0;

            Assert.AreEqual(1, c.Apply(value, new List<ConfigRefValue>()).Float64);
            Assert.AreNotEqual(2, c.Apply(value, new List<ConfigRefValue>()).Float64);
            Assert.AreEqual(FSUIPCOffsetType.Integer, c.Apply(value, new List<ConfigRefValue>()).type);

            value.type = FSUIPCOffsetType.Float;
            value.Float64 = 1;
            Assert.AreEqual(2, c.Apply(value, new List<ConfigRefValue>()).Float64);
            Assert.AreNotEqual(1, c.Apply(value, new List<ConfigRefValue>()).Float64);
            Assert.AreEqual(FSUIPCOffsetType.Float, c.Apply(value, new List<ConfigRefValue>()).type);


            c.Active = true;
            c.Operand = "=";
            c.Value = "0";
            c.IfValue = "0000";
            c.ElseValue = "1";

            value.type = FSUIPCOffsetType.Integer;
            value.Float64 = 0;

            Assert.AreEqual(FSUIPCOffsetType.Integer, c.Apply(value, new List<ConfigRefValue>()).type);
            Assert.AreEqual("0", c.Apply(value, new List<ConfigRefValue>()).Float64.ToString());

            c.Active = true;
            c.Operand = "=";
            c.Value = "hello";
            c.IfValue = "'world!'";
            c.ElseValue = "'works!'";

            value.type = FSUIPCOffsetType.String;
            value.Float64 = 0;
            value.String = "hello";

            Assert.AreEqual(FSUIPCOffsetType.String, c.Apply(value, new List<ConfigRefValue>()).type);
            Assert.AreEqual("'world!'", c.Apply(value, new List<ConfigRefValue>()).String);

            c.Active = true;
            c.Operand = "=";
            c.Value = "hello";
            c.IfValue = "'world!'";
            c.ElseValue = "'works!'";

            value.type = FSUIPCOffsetType.String;
            value.Float64 = 0;
            value.String = "it";

            Assert.AreEqual(FSUIPCOffsetType.String, c.Apply(value, new List<ConfigRefValue>()).type);
            Assert.AreEqual("'works!'", c.Apply(value, new List<ConfigRefValue>()).String);

        }
    }
}