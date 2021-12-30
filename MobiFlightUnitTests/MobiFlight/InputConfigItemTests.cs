﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.OutputConfig;
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
            Assert.IsNotNull(o.ConfigRefs, "ConfigRefs is null");
            Assert.AreEqual(o.ConfigRefs.Count, 2, "ConfigRefs.Count is not 2");

            o = new InputConfigItem();
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputConfigItem\ReadXmlTest.2.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("settings");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.ModuleSerial, "TestSerial", "ModuleSerial not the same");
            Assert.AreEqual(o.Name, "TestName", "Name not the same");
            Assert.AreEqual(o.Preconditions.Count, 0, "Preconditions Count not the same");
            Assert.AreEqual(o.Type, "Button", "Type not the same");
            Assert.IsNull(o.button.onPress, "button onpress not null");
            Assert.IsNotNull(o.button.onRelease, "button onRelease is null");
            Assert.IsNull(o.encoder.onLeft, "encoder onLeft not null");
            Assert.IsNotNull(o.encoder.onLeftFast, "encoder onLeftFast is null");
            Assert.IsNull(o.encoder.onRight, "encoder onRight not null");
            Assert.IsNotNull(o.encoder.onRightFast, "encoder onRightFast is null");
            Assert.IsNotNull(o.ConfigRefs, "ConfigRefs is null");
            Assert.AreEqual(o.ConfigRefs.Count, 0, "ConfigRefs.Count is not 2");
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
            InputConfigItem c = (InputConfigItem)o.Clone();

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
            result.button.onRelease = new InputConfig.FsuipcOffsetInputAction()
            {
                FSUIPC = new FsuipcOffset()
                {
                    BcdMode = true,
                    Mask = 0xFFFF,
                    Offset = 0x1234,
                    Size = 2
                },
                Value = "1"
            };
            result.Type = InputConfigItem.TYPE_BUTTON;

            result.encoder = null;
            result.ModuleSerial = "TestSerial";
            result.Name = "TestName";
            result.Preconditions.Add(new Precondition() { PreconditionSerial = "PreConTestSerial" });
            result.ConfigRefs.Add(new Base.ConfigRef() { Active = true, Placeholder = "@", Ref = "0b1c877f-baf3-4c69-99e6-6c31429fe3bd" });
            result.ConfigRefs.Add(new Base.ConfigRef() { Active = false, Placeholder = "%", Ref = "7d1370d3-56e9-497a-8abb-63ecc169defe" });

            return result;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            InputConfigItem o1 = new InputConfigItem();
            InputConfigItem o2 = new InputConfigItem();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void GetStatisticsTest()
        {
            // https://github.com/MobiFlight/MobiFlight-Connector/issues/623
            InputConfigItem o = new InputConfigItem();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputConfigItem\ReadXmlTest.623.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("settings");
            o.ReadXml(xmlReader);

            var statistics = o.GetStatistics();
            Assert.IsNotNull(statistics, "Statistics should be always an empty Dictionary<String, int>");
            Assert.AreEqual(0, statistics.Count);

            o.analog = new InputConfig.AnalogInputConfig();
            o.analog.onChange = new InputConfig.MSFS2020CustomInputAction();
            statistics = o.GetStatistics();
            Assert.AreEqual(o.analog.GetStatistics().Count, statistics.Count);
        }
    }
}