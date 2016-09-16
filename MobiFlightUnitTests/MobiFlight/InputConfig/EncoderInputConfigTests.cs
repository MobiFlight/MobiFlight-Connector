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
    public class EncoderInputConfigTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            EncoderInputConfig o = generateTestObject();
            EncoderInputConfig c = (EncoderInputConfig) o.Clone();

            Assert.AreNotSame(o, c, "Objects are the same");
            Assert.AreEqual((o.onLeft as FsuipcOffsetInputAction).FSUIPCOffset, (c.onLeft as FsuipcOffsetInputAction).FSUIPCOffset, "onLeft are not cloned correctly");
            Assert.AreEqual((o.onLeftFast as FsuipcOffsetInputAction).FSUIPCOffset, (c.onLeftFast as FsuipcOffsetInputAction).FSUIPCOffset, "onLeftFast are not cloned correctly");
            Assert.AreEqual((o.onRight as FsuipcOffsetInputAction).FSUIPCOffset, (c.onRight as FsuipcOffsetInputAction).FSUIPCOffset, "onRight are not cloned correctly");
            Assert.AreEqual((o.onRightFast as FsuipcOffsetInputAction).FSUIPCOffset, (c.onRightFast as FsuipcOffsetInputAction).FSUIPCOffset, "onRightFast are not cloned correctly");
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            EncoderInputConfig o = generateTestObject();
            Assert.IsNull(o.GetSchema());
        }
        
        [TestMethod()]
        public void ReadXmlTest()
        {
            EncoderInputConfig o = new EncoderInputConfig();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\EncoderInputConfig\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("encoder");
            o.ReadXml(xmlReader);

            Assert.AreEqual((o.onLeft as EventIdInputAction).EventId, 12345, "EventId not the same");
            Assert.AreEqual((o.onLeftFast as JeehellInputAction).EventId, 127, "EventId not the same");
            Assert.AreEqual((o.onRight as JeehellInputAction).EventId, 126, "EventId not the same");
            Assert.AreEqual((o.onRightFast as JeehellInputAction).EventId, 125, "EventId not the same");
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

            EncoderInputConfig o = generateTestObject();
            xmlWriter.WriteStartElement("encoder");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\EncoderInputConfig\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        private EncoderInputConfig generateTestObject()
        {
            EncoderInputConfig result = new EncoderInputConfig();
            result.onLeft = new FsuipcOffsetInputAction() { FSUIPCOffset = 0x1234 };
            result.onRight = new FsuipcOffsetInputAction() { FSUIPCOffset = 0x2345 };
            result.onLeftFast = new FsuipcOffsetInputAction() { FSUIPCOffset = 0x3456 };
            result.onRightFast = new FsuipcOffsetInputAction() { FSUIPCOffset = 0x4567 };

            return result;
        }
    }
}