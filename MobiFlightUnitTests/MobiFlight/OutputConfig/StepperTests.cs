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
    public class StepperTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            Stepper o = new Stepper();
            Stepper clone = o.Clone() as Stepper;

            o.Address = "Address";
            o.CompassMode = true;
            o.InputRev = 1000;
            o.OutputRev = 2040;
            o.TestValue = 1024;

            clone = o.Clone() as Stepper;
            Assert.AreEqual(o.Address, clone.Address);
            Assert.AreEqual(o.CompassMode, clone.CompassMode);
            Assert.AreEqual(o.InputRev, clone.InputRev);
            Assert.AreEqual(o.OutputRev, clone.OutputRev);
            Assert.AreEqual(o.TestValue, clone.TestValue);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Stepper o1 = new Stepper();
            Stepper o2 = new Stepper();

            Assert.IsTrue(o1.Equals(o2));
            o1.Address = "Address";
            o1.CompassMode = true;
            o1.InputRev = 1000;
            o1.OutputRev = 2040;
            o1.TestValue = 1024;

            Assert.IsFalse(o1.Equals(o2));

            o2.Address = "Address";
            o2.CompassMode = true;
            o2.InputRev = 1000;
            o2.OutputRev = 2040;
            o2.TestValue = 1024;

            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            Stepper o = new Stepper();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            Stepper o = new Stepper();

            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\Stepper\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("display");
            o.ReadXml(xmlReader);

            Assert.AreEqual("Address", o.Address);
            Assert.AreEqual(true, o.CompassMode);
            Assert.AreEqual(1000, o.InputRev);
            Assert.AreEqual(2040, o.OutputRev);
            Assert.AreEqual(1024, o.TestValue);
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

            Stepper o = new Stepper();
            o.Address = "Address";
            o.CompassMode = true;
            o.InputRev = 1000;
            o.OutputRev = 2040;
            o.TestValue = 1024;

            xmlWriter.WriteStartElement("display");
            xmlWriter.WriteAttributeString("type", "Stepper");
            xmlWriter.WriteAttributeString("serial", "DisplaySerial");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\Stepper\WriteXmlTest.1.xml");

            Assert.AreEqual(result, s, "The both strings are not equal");
        }
    }
}