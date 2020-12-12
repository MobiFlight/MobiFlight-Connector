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
    public class PmdgEventIdInputActionTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            PmdgEventIdInputAction o = generateTestObject();
            PmdgEventIdInputAction c = (PmdgEventIdInputAction) o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(o.EventId, c.EventId, "EventId not the same");
            Assert.AreEqual(o.Param, c.Param, "Param not the same");
            Assert.AreEqual(o.AircraftType, c.AircraftType, "Param not the same");
        }

        private PmdgEventIdInputAction generateTestObject()
        {
            PmdgEventIdInputAction o = new PmdgEventIdInputAction();
            o.EventId = Int32.MaxValue;
            o.Param = UInt32.MaxValue-1;
            o.AircraftType = PmdgEventIdInputAction.PmdgAircraftType.B747;

            return o;
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            PmdgEventIdInputAction o = new PmdgEventIdInputAction();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\PmdgEventIdInputAction\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("onPress");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.EventId, Int32.MaxValue, "EventId not the same");
            Assert.AreEqual(o.Param, UInt32.MaxValue-1, "Param not the same");
            Assert.AreEqual(o.AircraftType, PmdgEventIdInputAction.PmdgAircraftType.B777);
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

            PmdgEventIdInputAction o = generateTestObject();
            xmlWriter.WriteStartElement("onPress");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\PmdgEventIdInputAction\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void executeTest()
        {
            PmdgEventIdInputAction o = generateTestObject();
            MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock mock = new MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock();
            MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock simConnectMock = new MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock();

            o.execute(mock, simConnectMock, null);
            Assert.AreEqual(mock.Writes.Count, 1, "The message count is not as expected");
            Assert.AreEqual(mock.Writes[0].Value, "SetEventID", "The Write Value is wrong");
        }
    }
}