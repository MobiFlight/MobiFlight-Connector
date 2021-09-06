using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.InputConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass()]
    public class AnalogInputConfigTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            AnalogInputConfig o = generateTestObject();
            AnalogInputConfig c = (AnalogInputConfig)o.Clone();

            Assert.AreNotSame(o, c, "Cloned object is the same");
            Assert.AreEqual((o.onChange as VariableInputAction).Variable.Name, (c.onChange as VariableInputAction).Variable.Name, "onChange is not correct");
        }

        private AnalogInputConfig generateTestObject()
        {
            AnalogInputConfig o = new AnalogInputConfig();
            o.onChange = new VariableInputAction();
            (o.onChange as VariableInputAction).Variable.Name = "AnalogInputConfigTest";
            return o;
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            ButtonInputConfig o = new ButtonInputConfig();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            AnalogInputConfig o = new AnalogInputConfig();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\AnalogInputConfig\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("analog");
            o.ReadXml(xmlReader);

            Assert.AreEqual("AnalogInputConfigReadXML", (o.onChange as VariableInputAction).Variable.Name, "Variable.Name are not Equal");

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

            AnalogInputConfig o = generateTestObject();
            xmlWriter.WriteStartElement("analog");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\AnalogInputConfig\WriteXmlTest.1.xml");

            Assert.AreEqual(result, s, "The both strings are not equal");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            AnalogInputConfig o1 = new AnalogInputConfig();
            AnalogInputConfig o2 = new AnalogInputConfig();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}