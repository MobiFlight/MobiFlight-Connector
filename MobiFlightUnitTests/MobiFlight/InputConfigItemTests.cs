using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class InputConfigItemTests
    {
        [TestMethod()]
        public void InputConfigItemTest()
        {
            InputConfigItem o = new InputConfigItem();
            Assert.IsInstanceOfType(o, typeof(InputConfigItem), "Not of type InputConfigItem");
            Assert.AreEqual(o.Preconditions.Count, 0, "Preconditions Count other than 0");
            Assert.AreEqual(o.Type, InputConfigItem.TYPE_NOTSET, "Type other than NOTSET");
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            InputConfigItem o = generateTestObject();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            InputConfigItem o = new InputConfigItem();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputConfigItem\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("settings");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.ModuleSerial, "TestSerial", "ModuleSerial not the same");
            Assert.AreEqual(o.Name, "TestName", "Name not the same");
            Assert.AreEqual(o.Preconditions.Count, 0, "Preconditions Count not the same");
            Assert.AreEqual(o.Type, "Button", "Type not the same");
            Assert.IsNull(o.button.onPress, "button onpress not null");
            Assert.IsNotNull(o.button.onRelease, "button onRelease is null");
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

            InputConfigItem o = generateTestObject();
            xmlWriter.WriteStartElement("settings");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputConfigItem\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            InputConfigItem o = generateTestObject();
            InputConfigItem c = (InputConfigItem) o.Clone();

            Assert.IsNotNull(c.button, "Button is null");
            Assert.IsNull(c.encoder, "Encoder is not null");
            Assert.AreEqual(o.ModuleSerial, c.ModuleSerial, "Module Serial not the same");
            Assert.AreEqual(o.Name, c.Name, "Name not the same");
            Assert.AreEqual(c.Preconditions.Count, 1, "Precondition Count is not 1");
        }

        private InputConfigItem generateTestObject()
        {
            InputConfigItem result = new InputConfigItem();
            result.button = new InputConfig.ButtonInputConfig();
            result.button.onRelease = new InputConfig.FsuipcOffsetInputAction() {
                FSUIPCBcdMode = true,
                FSUIPCMask = 0xFFFF,
                FSUIPCOffset = 0x1234,
                FSUIPCSize = 2,
                Value = "1"
            };
            result.Type = InputConfigItem.TYPE_BUTTON;

            result.encoder = null;
            result.ModuleSerial = "TestSerial";
            result.Name = "TestName";
            result.Preconditions.Add(new Precondition() { PreconditionSerial = "PreConTestSerial" });

            return result;
        }
    }
}