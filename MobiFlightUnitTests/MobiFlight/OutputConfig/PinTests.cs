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
    public class PinTests
    {
        [TestMethod()]
        public void PinTest()
        {
            Pin o = new Pin();
            Assert.IsNotNull(o);
            Assert.AreEqual(byte.MaxValue, o.DisplayPinBrightness);
            Assert.AreEqual(false, o.DisplayPinPWM);
        }

        [TestMethod()]
        public void CloneTest()
        {
            Pin o = new Pin();
            o.DisplayPin = "DisplayPin";
            o.DisplayPinBrightness = 128;
            o.DisplayPinPWM = true;
            Pin clone = o.Clone() as Pin;

            Assert.AreEqual(o.DisplayPin,clone.DisplayPin);
            Assert.AreEqual(o.DisplayPinBrightness, clone.DisplayPinBrightness);
            Assert.AreEqual(o.DisplayPinPWM, clone.DisplayPinPWM);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Pin o1 = new Pin();
            Pin o2 = new Pin();            
            Assert.IsTrue(o1.Equals(o2));

            o1.DisplayPin = "DisplayPin";
            o1.DisplayPinBrightness = 128;
            o1.DisplayPinPWM = true;
            Assert.IsFalse(o1.Equals(o2));

            o2.DisplayPin = "DisplayPin";
            o2.DisplayPinBrightness = 128;
            o2.DisplayPinPWM = true;
            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            Pin o = new Pin();

            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\Pin\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("display");
            o.ReadXml(xmlReader);

            Assert.AreEqual("Pin", o.DisplayPin);
            Assert.AreEqual(128, o.DisplayPinBrightness);
            Assert.AreEqual(true, o.DisplayPinPWM);
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

            Pin o = new Pin();
            o.DisplayPin = "Pin";
            o.DisplayPinBrightness = 128;
            o.DisplayPinPWM = true;

            xmlWriter.WriteStartElement("display");
                xmlWriter.WriteAttributeString("type", MobiFlightOutput.TYPE);
                xmlWriter.WriteAttributeString("serial", "DisplaySerial");
                xmlWriter.WriteAttributeString("trigger", "normal");
                o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\Pin\WriteXmlTest.1.xml");

            Assert.AreEqual(result, s, "The both strings are not equal");
        }
    }
}