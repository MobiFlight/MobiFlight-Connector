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
    public class VariableInputActionTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            VariableInputAction o = generateTestObject();
            VariableInputAction c = (VariableInputAction)o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(o.Variable.TYPE, c.Variable.TYPE, "EventId not the same");
            Assert.AreEqual(o.Variable.Name, c.Variable.Name, "EventId not the same");
            Assert.AreEqual(o.Variable.Expression, c.Variable.Expression, "EventId not the same");
        }

        private VariableInputAction generateTestObject()
        {
            VariableInputAction o = new VariableInputAction();
            o.Variable.Name = "VariableInputActionTests";
            o.Variable.TYPE = "string";
            o.Variable.Text = "VariableInputActionTests";
            o.Variable.Expression = "$+1";
            return o;
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            VariableInputAction o = new VariableInputAction();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\VariableInputAction\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("onPress");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.Variable.TYPE, "string", "Variable.TYPE are not the same");
            Assert.AreEqual(o.Variable.Name, "VariableInputActionReadXMLTests", "Param not the same");
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

            VariableInputAction o = generateTestObject();
            o.Variable.Name = "VariableInputActionWriteXMLTests";
            xmlWriter.WriteStartElement("onPress");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\VariableInputAction\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void executeTest()
        {
            VariableInputAction o = generateTestObject();
            MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock mock = new MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock();
            MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock simConnectMock = new MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock();
            MobiFlightUnitTests.mock.MobiFlight.MobiFlightCacheMock mobiflightCacheMock = new MobiFlightUnitTests.mock.MobiFlight.MobiFlightCacheMock();
            MobiFlightUnitTests.mock.xplane.XplaneCacheMock xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            CacheCollection cacheCollection = new CacheCollection()
            {
                fsuipcCache = mock,
                simConnectCache = simConnectMock,
                moduleCache = mobiflightCacheMock,
                xplaneCache = xplaneCacheMock
            };

            o.Variable.TYPE = "number";
            o.Variable.Expression = "$+1";

            o.execute(cacheCollection, null, new List<ConfigRefValue>());
            Assert.AreEqual(mobiflightCacheMock.GetMobiFlightVariable("VariableInputActionTests").Number, 1, "The number is not correct.");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            VariableInputAction o1 = new VariableInputAction();
            VariableInputAction o2 = new VariableInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}