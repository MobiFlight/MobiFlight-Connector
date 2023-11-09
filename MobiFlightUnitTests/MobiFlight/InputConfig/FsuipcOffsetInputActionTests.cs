using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.InputConfig;
using MobiFlight.OutputConfig;
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
    public class FsuipcOffsetInputActionTests
    {
        [TestMethod()]
        public void FsuipcOffsetInputActionTest()
        {
            FsuipcOffsetInputAction o = new FsuipcOffsetInputAction();
            Assert.AreEqual(o.FSUIPC.Offset, FsuipcOffset.OffsetNull, "FSUIPCOffset is not FSUIPCOffsetNull");
            Assert.AreEqual(o.FSUIPC.Mask, 0xFF, "FSUIPCMask is not 0xFF");
            Assert.AreEqual(o.FSUIPC.OffsetType, FSUIPCOffsetType.Integer, "FSUIPCOffsetType not correct");
            Assert.AreEqual(o.FSUIPC.Size, 1, "FSUIPCSize not correct");
            Assert.AreEqual(o.FSUIPC.BcdMode, false, "Not correct");
            Assert.AreEqual(o.Value, "", "Value not correct");
            Assert.IsNotNull(o.Modifiers.Transformation, "Transform not initialized");
        }

        [TestMethod()]
        public void CloneTest()
        {
            FsuipcOffsetInputAction o = generateTestObject();
            FsuipcOffsetInputAction c = (FsuipcOffsetInputAction)o.Clone();

            Assert.AreNotSame(o, c, "Objects are the same");
            Assert.AreEqual(o.FSUIPC.BcdMode, c.FSUIPC.BcdMode, "FSUIPCBcdMode are not the same");
            Assert.AreEqual(o.FSUIPC.Mask, c.FSUIPC.Mask, "FSUIPCMask are not the same");
            Assert.AreEqual(o.FSUIPC.Offset, c.FSUIPC.Offset, "FSUIPCOffset are not the same");
            Assert.AreEqual(o.FSUIPC.OffsetType, c.FSUIPC.OffsetType, "FSUIPCOffsetType are not the same");
            Assert.AreEqual(o.FSUIPC.Size, c.FSUIPC.Size, "FSUIPCSize are not the same");
            Assert.AreEqual(o.Value, c.Value, "Value are not the same");
            Assert.AreEqual(o.Modifiers.Transformation.Expression, c.Modifiers.Transformation.Expression, "Value are not the same");
        }

        private FsuipcOffsetInputAction generateTestObject()
        {
            FsuipcOffsetInputAction o = new FsuipcOffsetInputAction();
            o.FSUIPC.BcdMode = true;
            o.FSUIPC.Mask = 0xFFFF;
            o.FSUIPC.Offset = 0x1234;
            o.FSUIPC.OffsetType = FSUIPCOffsetType.Float;
            o.FSUIPC.Size = 2;
            o.Value = "$+1";
            o.Modifiers.Transformation.Expression = "$*1";
            return o;
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

            FsuipcOffsetInputAction o = generateTestObject();
            xmlWriter.WriteStartElement("onPress");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\FsuipcOffsetInputAction\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            FsuipcOffsetInputAction o = new FsuipcOffsetInputAction();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\FsuipcOffsetInputAction\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("onPress");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.FSUIPC.BcdMode, true, "FSUIPCBcdMode are not the same");
            Assert.AreEqual(o.FSUIPC.Mask, 0xFFFFFFFF, "FSUIPCMask are not the same");
            Assert.AreEqual(o.FSUIPC.Offset, 0x1234, "FSUIPCOffset are not the same");
            Assert.AreEqual(o.FSUIPC.OffsetType, FSUIPCOffsetType.Float, "FSUIPCOffsetType are not the same");
            Assert.AreEqual(o.FSUIPC.Size, 4, "FSUIPCSize are not the same");
            Assert.AreEqual(o.Value, "$-1", "Value are not the same");
        }

        [TestMethod()]
        public void executeTest()
        {
            FsuipcOffsetInputAction o = generateTestObject();
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

            o.FSUIPC.OffsetType = FSUIPCOffsetType.Integer;
            o.FSUIPC.BcdMode = false;
            o.Value = "12";
            o.execute(cacheCollection, null, new List<ConfigRefValue>());
            Assert.AreEqual(mock.Writes.Count, 2, "The message count is not as expected"); // there is one write in the mock for setting the offset and one write for writing to the cache.
            Assert.AreEqual(mock.Writes[0].Offset, 0x1234, "The Offset is wrong");
            Assert.AreEqual(mock.Writes[0].Value, "12", "The Param Value is wrong");

            mock.Clear();
            // validate config references work
            o.Value = "1+#";
            List<ConfigRefValue> configrefs = new List<ConfigRefValue>();
            configrefs.Add(new ConfigRefValue() { ConfigRef = new Base.ConfigRef() { Active = true, Placeholder = "#" }, Value = "1" });
            o.execute(cacheCollection, null, configrefs);

            Assert.AreEqual(2, mock.Writes.Count, "The message count is not as expected");
            Assert.AreEqual("2", mock.Writes[0].Value, mock.Writes[0].Value, "The Write Value is wrong");

            // test https://github.com/Mobiflight/MobiFlight-Connector/issues/438
            mock.Clear();
            o.Value = "Round(@*359/1023,0)";
            configrefs = new List<ConfigRefValue>();
            o.execute(cacheCollection, new InputEventArgs() { Value = 359 }, configrefs);

            Assert.AreEqual(2, mock.Writes.Count, "The message count is not as expected");
            Assert.AreEqual(Math.Round((359 * 359 / 1023f), 0).ToString(), mock.Writes[0].Value, mock.Writes[0].Value, "The Write Value is wrong");

            // test https://github.com/Mobiflight/MobiFlight-Connector/issues/438
            mock.Clear();
            o.Value = "@*359/1023";
            configrefs = new List<ConfigRefValue>();
            o.execute(cacheCollection, new InputEventArgs() { Value = 359 }, configrefs);

            Assert.AreEqual(2, mock.Writes.Count, "The message count is not as expected");
            Assert.AreEqual(Math.Floor((359 * 359 / 1023f)).ToString(), mock.Writes[0].Value, mock.Writes[0].Value, "The Write Value is wrong");

            // test https://github.com/Mobiflight/MobiFlight-Connector/issues/340
            mock.Clear();
            o.Value = "Potatosalad1*2+$/126";
            configrefs = new List<ConfigRefValue>();

            try
            {
                o.execute(cacheCollection, new InputEventArgs() { Value = 359 }, configrefs);
                Assert.Fail();
            }
            catch (FormatException)
            {
                Assert.AreEqual(0, mock.Writes.Count, "The message count is not as expected");
            }
        }

        [TestMethod()]
        public void EqualsTest()
        {
            FsuipcOffsetInputAction o1 = new FsuipcOffsetInputAction();
            FsuipcOffsetInputAction o2 = new FsuipcOffsetInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}