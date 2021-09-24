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
    public class VariableOutputTests
    {
        [TestMethod()]
        public void VariableOutputTest()
        {
            VariableOutput o = new VariableOutput();
            Assert.IsNotNull(o, "Object is null");

            Assert.IsNotNull(o.Variable, "Variable is not null");
            Assert.IsNotNull(o.Variable.Name, "Variable.Name is null");
        }

        [TestMethod()]
        public void CloneTest()
        {
            VariableOutput o = new VariableOutput();
            o.Variable.Name = "Test";
            VariableOutput c = o.Clone() as VariableOutput;
            Assert.IsNotNull(c, "Clone is null");
            Assert.AreEqual(o.Variable.Name, c.Variable.Name, "Address are not the same");
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            LcdDisplay o = new LcdDisplay();

            Assert.IsNull(o.GetSchema(), "Schema not null");
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            VariableOutput o = new VariableOutput();

            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\VariableOutput\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("display");
            o.ReadXml(xmlReader);

            Assert.AreEqual("ReadXMLTest", o.Variable.Name, "Variable.Name does not match.");
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

            VariableOutput o = new VariableOutput();
            o.Variable.Name = "WriteXMLTest";

            xmlWriter.WriteStartElement("display");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\VariableOutput\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            VariableOutput o1 = new VariableOutput();
            VariableOutput o2 = new VariableOutput();

            Assert.IsTrue(o1.Equals(o2));

            o1.Variable = new MobiFlightVariable() { Expression = "1+2" };
            Assert.IsFalse(o1.Equals(o2));

            o2.Variable = new MobiFlightVariable() { Expression = "1+2" };
            Assert.IsTrue(o1.Equals(o2));
        }
    }
}