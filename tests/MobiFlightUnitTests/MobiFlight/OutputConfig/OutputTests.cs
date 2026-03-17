using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml;

namespace MobiFlight.OutputConfig.Tests
{
    [TestClass()]
    public class OutputTests
    {
        [TestMethod()]
        public void PinTest()
        {
            Output o = new Output();
            Assert.IsNotNull(o);
            Assert.AreEqual(byte.MaxValue, o.Brightness);
            Assert.IsFalse(o.PwmMode);
        }

        [TestMethod()]
        public void CloneTest()
        {
            Output o = new Output();
            o.Pin = "DisplayPin";
            o.Brightness = 128;
            o.PwmMode = true;
            Output clone = o.Clone() as Output;

            Assert.AreEqual(o.Pin,clone.Pin);
            Assert.AreEqual(o.Brightness, clone.Brightness);
            Assert.AreEqual(o.PwmMode, clone.PwmMode);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Output o1 = new Output();
            Output o2 = new Output();            
            Assert.IsTrue(o1.Equals(o2));

            o1.Pin = "DisplayPin";
            o1.Brightness = 128;
            o1.PwmMode = true;
            Assert.IsFalse(o1.Equals(o2));

            o2.Pin = "DisplayPin";
            o2.Brightness = 128;
            o2.PwmMode = true;
            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            Output o = new Output();

            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\Pin\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("display");
            o.ReadXml(xmlReader);

            Assert.AreEqual("Pin", o.Pin);
            Assert.AreEqual(128, o.Brightness);
            Assert.IsTrue(o.PwmMode);
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

            Output o = new Output();
            o.Pin = "Pin";
            o.Brightness = 128;
            o.PwmMode = true;

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