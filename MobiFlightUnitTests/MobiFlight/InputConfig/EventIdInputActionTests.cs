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
    public class EventIdInputActionTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            EventIdInputAction o = generateTestObject();
            EventIdInputAction c = (EventIdInputAction) o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(o.EventId, c.EventId, "EventId not the same");
            Assert.AreEqual(o.Param, c.Param, "Param not the same");
        }

        private EventIdInputAction generateTestObject()
        {
            EventIdInputAction o = new EventIdInputAction();
            o.EventId = 12345;
            o.Param = 54321;
            return o;
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            EventIdInputAction o = new EventIdInputAction();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\EventIdInputAction\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("onPress");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.EventId, Int32.MaxValue, "EventId not the same");
            Assert.AreEqual(o.Param, Int32.MaxValue-1, "Param not the same");
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

            EventIdInputAction o = generateTestObject();
            xmlWriter.WriteStartElement("onPress");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\EventIdInputAction\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void executeTest()
        {
            EventIdInputAction o = generateTestObject();
            MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock mock = new MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock();
            o.execute(mock);
            Assert.AreEqual(mock.Writes.Count, 3, "The message count is not as expected");
            Assert.AreEqual(mock.Writes[0].Offset, 0x3114, "The Param Offset is wrong");
            Assert.AreEqual(mock.Writes[0].Value, "54321", "The Param Value is wrong");
            Assert.AreEqual(mock.Writes[1].Offset, 0x3110, "The Base Offset is wrong");
            Assert.AreEqual(mock.Writes[1].Value, "12345", "The Base Value is wrong");
            Assert.AreEqual(mock.Writes[2].Value, "ForceUpdate", "The ForceUpdate Value is wrong");
        }
    }
}