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
            EventIdInputAction c = (EventIdInputAction)o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(o.EventId, c.EventId, "EventId not the same");
            Assert.AreEqual(o.Param, c.Param, "Param not the same");
        }

        private EventIdInputAction generateTestObject()
        {
            EventIdInputAction o = new EventIdInputAction();
            o.EventId = 12345;
            o.Param = "54321";
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
            Assert.AreEqual(o.Param, (Int32.MaxValue - 1).ToString(), "Param not the same");
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
            MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock simConnectMock = new MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock();
            MobiFlightUnitTests.mock.xplane.XplaneCacheMock xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            CacheCollection cacheCollection = new CacheCollection()
            {
                fsuipcCache = mock,
                simConnectCache = simConnectMock,
                moduleCache = null,
                xplaneCache = xplaneCacheMock
            };

            o.execute(cacheCollection, null, new List<ConfigRefValue>());
            Assert.AreEqual(1, mock.Writes.Count, "The message count is not as expected");
            Assert.AreEqual("SetEventID>" + o.EventId + ">" + o.Param, mock.Writes[0].Value, "The Write Value is wrong");

            mock.Clear();
            // validate config references work
            o.Param = "1+#";
            List<ConfigRefValue> configrefs = new List<ConfigRefValue>();
            configrefs.Add(new ConfigRefValue() { ConfigRef = new Base.ConfigRef() { Active = true, Placeholder = "#" }, Value = "1" });
            o.execute(cacheCollection, null, configrefs);

            Assert.AreEqual(1, mock.Writes.Count, "The message count is not as expected");
            Assert.AreEqual("SetEventID>" + o.EventId + ">" + 2, mock.Writes[0].Value, "The Write Value is wrong");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            EventIdInputAction o1 = new EventIdInputAction();
            EventIdInputAction o2 = new EventIdInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}