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
    public class XplaneInputActionTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            XplaneInputAction o = generateTestObject();
            XplaneInputAction c = (XplaneInputAction)o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(o.InputType, c.InputType, "InputType not the same");
            Assert.AreEqual(o.Path, c.Path, "Path not the same");
            Assert.AreEqual(o.Expression, c.Expression, "Expression not the same");
        }

        private XplaneInputAction generateTestObject()
        {
            XplaneInputAction o = new XplaneInputAction();
            o.InputType = XplaneInputAction.INPUT_TYPE_DATAREF;
            o.Path = "/my/test/path";
            o.Expression = "$+1";

            return o;
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            XplaneInputAction o = new XplaneInputAction();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\XplaneInputAction\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("onPress");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.InputType, "DataRef", "InputType not the same");
            Assert.AreEqual(o.Path,"/my/test/path1", "Path not the same");
            Assert.AreEqual(o.Expression, "$+2");
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

            XplaneInputAction o = generateTestObject();
            xmlWriter.WriteStartElement("onPress");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\XplaneInputAction\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void executeTest()
        {
            XplaneInputAction o = generateTestObject();
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
            Assert.AreEqual(1, xplaneCacheMock.Writes.Count, "The message count is not as expected");

            xplaneCacheMock.Clear();
            // validate config references work
            o.Expression = "1+#";
            List<ConfigRefValue> configrefs = new List<ConfigRefValue>();
            configrefs.Add(new ConfigRefValue() { ConfigRef = new Base.ConfigRef() { Active = true, Placeholder = "#" }, Value = "1" });
            o.execute(cacheCollection, null, configrefs);

            Assert.AreEqual(1, xplaneCacheMock.Writes.Count, "The message count is not as expected");
            Assert.AreEqual(2, xplaneCacheMock.Writes[0].Value, "The Write Value is wrong");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            XplaneInputAction o1 = new XplaneInputAction();
            XplaneInputAction o2 = new XplaneInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}