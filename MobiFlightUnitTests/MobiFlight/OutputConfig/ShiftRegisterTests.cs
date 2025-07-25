using MobiFlight.OutputConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Xml;
using System;

namespace MobiFlight.OutputConfig.Tests
{
    [TestClass()]
    public class ShiftRegisterTests
    {
        [TestMethod()]
        public void ShiftRegisterTest()
        {
            var o = new ShiftRegister();
            Assert.IsNotNull(o);
            Assert.AreEqual("", o.Address);
            Assert.AreEqual("", o.Pin);
            Assert.AreEqual(byte.MaxValue, o.Brightness);
            Assert.IsFalse(o.PWM);

            o.Pin = "Pin";
            Assert.AreEqual("Pin", o.Pin);

            o.Address = "Address";
            Assert.AreEqual("Address", o.Address);
            Assert.AreEqual(o.Address, o.Name);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            var o = new ShiftRegister();
            var other = new ShiftRegister();
            Assert.IsTrue(o.Equals(other));

            o.PWM = true;
            Assert.IsFalse(o.Equals(other));

            other.PWM = o.PWM;
            Assert.IsTrue(o.Equals(other));

            o.Address = "Address";
            Assert.IsFalse(o.Equals(other));
            other.Address = o.Address;
            Assert.IsTrue(o.Equals(other));

            o.Brightness = 1;
            Assert.IsFalse(o.Equals(other));
            other.Brightness = o.Brightness;
            Assert.IsTrue(o.Equals(other));

        }
        
        [TestMethod()]
        public void ReadXmlTest()
        {
            var o = new ShiftRegister();

            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\ShiftRegister\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("display");
            o.ReadXml(xmlReader);

            Assert.AreEqual("Output 4", o.Pin);
            Assert.AreEqual(128, o.Brightness);
            Assert.AreEqual(true, o.PWM);
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

            var o = new ShiftRegister();
            o.Address = "ShiftRegister";
            o.Pin = "Output 4";
            o.Brightness = 128;
            o.PWM = true;

            xmlWriter.WriteStartElement("display");
            xmlWriter.WriteAttributeString("type", MobiFlightShiftRegister.TYPE);
            xmlWriter.WriteAttributeString("serial", "DisplaySerial");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\ShiftRegister\WriteXmlTest.1.xml");

            Assert.AreEqual(result, s, "The both strings are not equal");
        }

        [TestMethod()]
        public void ToStringTest()
        {
            // The list of selected pins is stored as a string in this format: Output 1|Output 2|Output 10
            // That's too wordy to display in UI so strip out the "Output " and "|", then separate
            // the pin assignments with a comma. The resulting display string is: ShiftRegister:1,2,10.
            //
            // The UI forces at least one pin assignment so there is no case where the resulting string
            // would be ShiftRegister: (with no pins listed and a trailing colon).
            
            // return $"{Address}:{Pin.Replace("Output ", ",").Replace("|", "").TrimStart(',')}";

            var o = new ShiftRegister();
            o.Pin = "Output 1|Output 2|Output 3";
            o.Address = "Address";
            Assert.AreEqual("Address:1,2,3", o.ToString());

            o.Pin = "Output 1";
            o.Address = "Address";
            Assert.AreEqual("Address:1", o.ToString());
        }

        [TestMethod()]
        public void CloneTest()
        {
            var o = new ShiftRegister();
            o.Pin = "Pin";
            o.Address = "Address";
            o.Brightness = 128;
            o.PWM = true;

            var clone = o.Clone() as ShiftRegister;

            Assert.AreEqual(o.Pin, clone.Pin);
            Assert.AreEqual(o.Address, clone.Address);
            Assert.AreEqual(o.Brightness, clone.Brightness);
            Assert.AreEqual(o.PWM, clone.PWM);

            Assert.AreNotSame(o, clone);
        }
    }
}