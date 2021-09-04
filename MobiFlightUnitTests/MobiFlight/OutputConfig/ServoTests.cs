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
    public class ServoTests
    {
        [TestMethod()]
        public void ServoTest()
        {
            Servo o = new Servo();
            Assert.IsNotNull(o);
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            Servo o = new Servo();

            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\Servo\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("display");
            o.ReadXml(xmlReader);

            Assert.AreEqual("Address", o.Address);
            Assert.AreEqual("160", o.Max);
            Assert.AreEqual("10", o.Min);
            Assert.AreEqual("50", o.MaxRotationPercent);
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

            Servo o = new Servo();
            o.Address = "Address";
            o.Max = "160";
            o.Min = "10";
            o.MaxRotationPercent = "50";

            xmlWriter.WriteStartElement("display");
            xmlWriter.WriteAttributeString("type", "Servo");
            xmlWriter.WriteAttributeString("serial", "DisplaySerial");
            xmlWriter.WriteAttributeString("trigger", "normal");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\Servo\WriteXmlTest.1.xml");

            Assert.AreEqual(result, s, "The both strings are not equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            Servo o = new Servo();
            o.Address = "Address";
            o.Max = "123";
            o.Min = "012";
            o.MaxRotationPercent = "50";

            Servo clone = o.Clone() as Servo;

            Assert.AreEqual(o.Address, clone.Address);
            Assert.AreEqual(o.Max, clone.Max);
            Assert.AreEqual(o.Min, clone.Min);
            Assert.AreEqual(o.MaxRotationPercent, clone.MaxRotationPercent);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Servo o1 = new Servo();
            Servo o2 = new Servo();

            Assert.IsTrue(o1.Equals(o2));

            o1.Address = "Address";
            o1.Max = "123";
            o1.Min = "012";
            o1.MaxRotationPercent = "50";

            Assert.IsFalse(o1.Equals(o2));

            o2.Address = "Address";
            o2.Max = "123";
            o2.Min = "012";
            o2.MaxRotationPercent = "50";

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}