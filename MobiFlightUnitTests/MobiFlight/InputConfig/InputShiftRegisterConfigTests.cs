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
    public class InputShiftRegisterConfigTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            InputShiftRegisterConfig o = generateTestObject();
            InputShiftRegisterConfig c = (InputShiftRegisterConfig)o.Clone();

            Assert.AreNotSame(o, c, "Cloned object is the same");
            Assert.AreEqual((o.onPress as EventIdInputAction).EventId, (c.onPress as EventIdInputAction).EventId, "OnPress is not correct");
            Assert.AreEqual((o.onRelease as JeehellInputAction).EventId, (c.onRelease as JeehellInputAction).EventId, "OnRelase is not correct");
        }

        private InputShiftRegisterConfig generateTestObject()
        {
            InputShiftRegisterConfig o = new InputShiftRegisterConfig();
            o.ExtPin = 1;
            o.onPress = new EventIdInputAction() { EventId = 12345 };
            o.onRelease = new JeehellInputAction() { EventId = 127, Param = "123" };
            return o;
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            InputShiftRegisterConfig o = new InputShiftRegisterConfig();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            InputShiftRegisterConfig o = new InputShiftRegisterConfig();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputShiftRegisterConfig\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("inputShiftRegister");
            o.ReadXml(xmlReader);

            Assert.AreEqual(1, o.ExtPin, "ExtPin not the same");
            Assert.AreEqual(12345, (o.onPress as EventIdInputAction).EventId, "EventId not the same");
            Assert.AreEqual(127, (o.onRelease as JeehellInputAction).EventId, "EventId not the same");
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

            InputShiftRegisterConfig o = generateTestObject();
            xmlWriter.WriteStartElement("inputShiftRegister");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputShiftRegisterConfig\WriteXmlTest.1.xml");

            Assert.AreEqual(result, s, "The both strings are not equal");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            InputShiftRegisterConfig o1 = new InputShiftRegisterConfig();
            InputShiftRegisterConfig o2 = new InputShiftRegisterConfig();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}