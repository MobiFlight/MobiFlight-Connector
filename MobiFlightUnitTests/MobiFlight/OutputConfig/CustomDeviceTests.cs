using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.OutputConfig.Tests
{
    [TestClass()]
    public class CustomDeviceTests
    {
        [TestMethod()]
        public void CustomDeviceTest()
        {
            var o = new CustomDevice();
            Assert.IsNotNull(o);
            Assert.AreEqual(null, o.CustomName);
            Assert.AreEqual(null, o.CustomType);
            Assert.AreEqual(null, o.Value);
            Assert.AreEqual(null, o.MessageType);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            var o1 = new CustomDevice();
            var o2 = new CustomDevice();
            Assert.IsTrue(o1.Equals(o2));

            o1.CustomName = "DisplayPin";
            o1.CustomType = "CustomType";
            o1.Value = "Value";
            o1.MessageType = "MessageType";
            Assert.IsFalse(o1.Equals(o2));

            o2.CustomName = "DisplayPin";
            o2.CustomType = "CustomType";
            o2.Value = "Value";
            o2.MessageType = "MessageType";
            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void CloneTest()
        {
            var o1 = new CustomDevice();
            o1.CustomName = "DisplayPin";
            o1.CustomType = "CustomType";
            o1.Value = "Value";
            o1.MessageType = "MessageType";
            var clone = (CustomDevice)o1.Clone();

            Assert.AreEqual(o1.CustomName, clone.CustomName);
            Assert.AreEqual(o1.CustomType, clone.CustomType);
            Assert.AreEqual(o1.Value, clone.Value);
            Assert.AreEqual(o1.MessageType, clone.MessageType);
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            var o = new CustomDevice();
            Assert.IsNull(o.GetSchema(), "Schema not null");
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            var o = new CustomDevice();

            string s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\CustomDevice\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            // <display type="CustomDevice" serial="Serial" trigger="normal" customType="TM1637" customName="TM1637 Display" messageType="3" value="$+2" />

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("display");
            o.ReadXml(xmlReader);

            Assert.AreEqual("TM1637 Display", o.CustomName);
            Assert.AreEqual("TM1637", o.CustomType);
            Assert.AreEqual("$+2", o.Value);
            Assert.AreEqual("3", o.MessageType);
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

            var o = new CustomDevice();
            o.CustomName = "TM1637 Display";
            o.CustomType = "TM1637";
            o.Value = "$+2";
            o.MessageType = "3";

            xmlWriter.WriteStartElement("display");
            xmlWriter.WriteAttributeString("type", CustomDevice.Type);
            xmlWriter.WriteAttributeString("serial", "Serial");
            xmlWriter.WriteAttributeString("trigger", "normal");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\CustomDevice\WriteXmlTest.1.xml");

            Assert.AreEqual(result, s, "The both strings are not equal");
        }
    }
}