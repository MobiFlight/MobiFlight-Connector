using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass()]
    public class KeyInputActionTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            KeyInputAction o = generateTestObject();

            KeyInputAction i = (KeyInputAction)o.Clone();
            Assert.AreEqual(i.Shift, o.Shift, "SHIFT value differs");
            Assert.AreEqual(i.Alt, o.Alt, "ALT value differs");
            Assert.AreEqual(i.Control, o.Control, "CONTROL value differs");
            Assert.AreEqual(i.Key, o.Key, "Key value differs");
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

            KeyInputAction o = generateTestObject();
            xmlWriter.WriteStartElement("onPress");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\KeyInputAction\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            KeyInputAction o = new KeyInputAction();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\KeyInputAction\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("onPress");
            o.ReadXml(xmlReader);

            Assert.IsTrue(o.Alt, "Alt modifier not the same");
            Assert.IsTrue(o.Shift, "Shift modifier not the same");
            Assert.IsTrue(o.Control, "Ctrl modifier not the same");
            Assert.AreEqual(Keys.A, o.Key, "Key not the same");
        }

        [TestMethod()]
        public void executeTest()
        {
            KeyInputAction o = generateTestObject();
            MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock mock = new MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock();
            MobiFlightUnitTests.mock.MobiFlight.MobiFlightCacheMock mockMfCache = new MobiFlightUnitTests.mock.MobiFlight.MobiFlightCacheMock();
            MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock simConnectMock = new MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock();
            MobiFlightUnitTests.mock.xplane.XplaneCacheMock xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            CacheCollection cacheCollection = new CacheCollection()
            {
                fsuipcCache = mock,
                simConnectCache = simConnectMock,
                moduleCache = mockMfCache,
                xplaneCache = xplaneCacheMock
            };

            MobiFlightUnitTests.mock.Base.KeyboardInputMock keybMock = new MobiFlightUnitTests.mock.Base.KeyboardInputMock();
            o.Keyboard = keybMock;
            o.execute(cacheCollection, null, null);
            Assert.HasCount(1, keybMock.Writes, "The message count is not as expected");
            Assert.AreEqual("Ctrl+Shift+Alt+A", keybMock.Writes[0], "The sent string is wrong");
        }

        private KeyInputAction generateTestObject()
        {
            KeyInputAction o = new KeyInputAction();
            o.Alt = true;
            o.Control = true;
            o.Shift = true;
            o.Key = Keys.A;
            return o;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            KeyInputAction o1 = new KeyInputAction();
            KeyInputAction o2 = new KeyInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}