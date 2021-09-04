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
            EncoderInputConfig c = (EncoderInputConfig)o.Clone();

            Assert.AreNotSame(o, c, "Objects are the same");
            Assert.AreEqual((o.onLeft as FsuipcOffsetInputAction).FSUIPC.Offset, (c.onLeft as FsuipcOffsetInputAction).FSUIPC.Offset, "onLeft are not cloned correctly");
            Assert.AreEqual((o.onLeftFast as KeyInputAction).Key, (c.onLeftFast as KeyInputAction).Key, "onLeftFast are not cloned correctly");
            Assert.AreEqual((o.onRight as EventIdInputAction).EventId, (c.onRight as EventIdInputAction).EventId, "onRight are not cloned correctly");
            Assert.AreEqual((o.onRightFast as VJoyInputAction).sendValue, (c.onRightFast as VJoyInputAction).sendValue, "onRightFast are not cloned correctly");
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

            Assert.AreEqual(result, s, "The both strings are not equal");
        }

        private EncoderInputConfig generateTestObject()
        {
            EncoderInputConfig result = new EncoderInputConfig();
            result.onLeft = new FsuipcOffsetInputAction() { FSUIPC = new OutputConfig.FsuipcOffset() { Offset = 0x1234 } };
            result.onLeftFast = new KeyInputAction() { Alt = true, Control = true, Key = System.Windows.Forms.Keys.A, Shift = true };
            result.onRight = new EventIdInputAction() { EventId = 123456, Param = "1" };
            result.onRightFast = new VJoyInputAction() { axisString = "Z", buttonComand = true, buttonNr = 1, sendValue = "sendValue", vJoyID = 1 };

            return result;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            EncoderInputConfig o1 = new EncoderInputConfig();
            EncoderInputConfig o2 = new EncoderInputConfig();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}