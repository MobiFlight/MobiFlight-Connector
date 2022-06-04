using System;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.InputConfig;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass]
    public class MSFS2020EventIdInputActionTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            MSFS2020EventIdInputAction o = generateTestObject();
            MSFS2020EventIdInputAction c = (MSFS2020EventIdInputAction)o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(o.EventId, c.EventId, "EventId not the same");
        }

        private MSFS2020EventIdInputAction generateTestObject()
        {
            MSFS2020EventIdInputAction o = new MSFS2020EventIdInputAction();
            o.EventId = "TestEvent";
            
            return o;
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            MSFS2020EventIdInputAction o = new MSFS2020EventIdInputAction();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\MSFS2020EventIdInputAction\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("onPress");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.EventId, "ReadTestEvent", "EventId not the same");
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

            MSFS2020EventIdInputAction o = generateTestObject();
            o.EventId = "WriteInputEvent";
            xmlWriter.WriteStartElement("onPress");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\MSFS2020EventIdInputAction\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void executeTest()
        {
            MSFS2020EventIdInputAction o = generateTestObject();
            o.EventId = "MyEventId";
            MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock mock = new MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock();
            MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock simConnectMock = new MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock();
            MobiFlightUnitTests.mock.xplane.XplaneCacheMock xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            CacheCollection cacheCollection = new CacheCollection()
            {
                fsuipcCache = mock,
                simConnectCache = simConnectMock,
                moduleCache = null,
                xplaneCache = xplaneCacheMock
            };

            o.execute(cacheCollection, null, null);
            Assert.AreEqual(simConnectMock.Writes.Count, 1, "The message count is not as expected");
            Assert.AreEqual(simConnectMock.Writes[0], "SetEventID>MyEventId", "The Write Value is wrong");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            MSFS2020EventIdInputAction o1 = new MSFS2020EventIdInputAction();
            MSFS2020EventIdInputAction o2 = new MSFS2020EventIdInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}
