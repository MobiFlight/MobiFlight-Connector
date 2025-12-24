using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass()]
    public class JeehellInputActionTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            JeehellInputAction o = generateTestObject();
            JeehellInputAction c = (JeehellInputAction)o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(c.EventId, o.EventId, "EventId not the same");
            Assert.AreEqual(c.Param, o.Param, "Param not the same");
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            JeehellInputAction o = new JeehellInputAction();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\JeehellInputAction\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("onPress");
            o.ReadXml(xmlReader);

            Assert.AreEqual(13, o.EventId, "EventId not the same");
            Assert.AreEqual(Int16.MaxValue.ToString(), o.Param, "Param not the same");
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

            JeehellInputAction o = generateTestObject();
            xmlWriter.WriteStartElement("onPress");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\JeehellInputAction\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void executeTest()
        {
            JeehellInputAction o = generateTestObject();
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
            Assert.HasCount(3, mock.Writes, "The message count is not as expected");
            Assert.AreEqual(0x73CD, mock.Writes[0].Offset,"The Param Offset is wrong");
            Assert.AreEqual(Int16.MaxValue.ToString(), mock.Writes[0].Value, "The Param Value is wrong");
            Assert.AreEqual(0x73CC, mock.Writes[1].Offset, "The Base Offset is wrong");
            Assert.AreEqual("13", mock.Writes[1].Value, "The Base Value is wrong");
            Assert.AreEqual("Write", mock.Writes[2].Value, "The Write Value is wrong");
        }

        JeehellInputAction generateTestObject()
        {
            JeehellInputAction o = new JeehellInputAction();
            o.EventId = 13;
            o.Param = Int16.MaxValue.ToString();
            return o;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            JeehellInputAction o1 = new JeehellInputAction();
            JeehellInputAction o2 = new JeehellInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}