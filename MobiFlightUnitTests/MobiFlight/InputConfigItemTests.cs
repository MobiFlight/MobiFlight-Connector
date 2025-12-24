using MobiFlight;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.InputConfig;
using MobiFlight.OutputConfig;
using System;
using System.IO;
using System.Xml;
using MobiFlight.Base;
using System.Collections.Generic;
using System.Linq;

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
            Assert.AreEqual(0, o.Preconditions.Count, "Preconditions Count other than 0");
            Assert.AreEqual(InputConfigItem.TYPE_NOTSET, o.DeviceType, "Type other than NOTSET");
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

            Assert.AreEqual("TestSerial", o.ModuleSerial, "ModuleSerial not the same");
            Assert.AreEqual("TestName", o.DeviceName, "Name not the same");
            Assert.AreEqual(0, o.Preconditions.Count, "Preconditions Count not the same");
            Assert.AreEqual("Button", o.DeviceType, "Type not the same");
            Assert.IsNull(o.button.onPress, "button onpress not null");
            Assert.IsNotNull(o.button.onRelease, "button onRelease is null");
            Assert.IsNotNull(o.ConfigRefs, "ConfigRefs is null");
            Assert.HasCount(2, o.ConfigRefs);

            o = new InputConfigItem();
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputConfigItem\ReadXmlTest.2.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("settings");
            o.ReadXml(xmlReader);

            Assert.AreEqual("TestSerial", o.ModuleSerial, "ModuleSerial not the same");
            Assert.HasCount(0, o.Preconditions, "Preconditions Count not the same");
            Assert.AreEqual("TestName", o.DeviceName, "Name not the same");
            Assert.AreEqual("Button", o.DeviceType, "Type not the same");
            Assert.IsNull(o.button.onPress, "button onpress not null");
            Assert.IsNotNull(o.button.onRelease, "button onRelease is null");
            Assert.IsNull(o.encoder.onLeft, "encoder onLeft not null");
            Assert.IsNotNull(o.encoder.onLeftFast, "encoder onLeftFast is null");
            Assert.IsNull(o.encoder.onRight, "encoder onRight not null");
            Assert.IsNotNull(o.encoder.onRightFast, "encoder onRightFast is null");
            Assert.IsNotNull(o.ConfigRefs, "ConfigRefs is null");
            Assert.HasCount(0, o.ConfigRefs, "ConfigRefs.Count is not 2");

            o = new InputConfigItem();
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputConfigItem\ReadXmlTest.InputShiftRegister.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("settings");
            o.ReadXml(xmlReader);

            Assert.AreEqual("TestSerial", o.ModuleSerial, "ModuleSerial not the same");
            Assert.HasCount(0, o.Preconditions, "Preconditions Count not the same");
            Assert.AreEqual("TestName", o.DeviceName, "Name not the same");
            Assert.AreEqual("Button", o.DeviceType, "Type not the same");
            Assert.IsNull(o.inputShiftRegister.onPress, "button onpress not null");
            Assert.IsNotNull(o.inputShiftRegister.onRelease, "button onRelease is null");
            Assert.IsNotNull(o.inputShiftRegister.onRelease as JeehellInputAction, "OnRelease is not of type JeehellInputAction");

            o = new InputConfigItem();
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputConfigItem\ReadXmlTest.InputMultiplexer.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("settings");
            o.ReadXml(xmlReader);

            Assert.AreEqual("TestSerial", o.ModuleSerial, "ModuleSerial not the same");
            Assert.HasCount(0, o.Preconditions, "Preconditions Count not the same");
            Assert.AreEqual("TestName", o.DeviceName, "Name not the same");
            Assert.AreEqual("Button", o.DeviceType, "Type not the same");
            Assert.IsNull(o.inputMultiplexer.onPress, "button onpress not null");
            Assert.IsNotNull(o.inputMultiplexer.onRelease, "button onRelease is null");
            Assert.IsNotNull(o.inputMultiplexer.onRelease as JeehellInputAction, "OnRelease is not of type JeehellInputAction");

            o = new InputConfigItem();
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputConfigItem\ReadXmlTest.860.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("settings");
            o.ReadXml(xmlReader);

            Assert.AreEqual("737PEDESTAL1/ SN-769-a6a", o.ModuleSerial, "ModuleSerial not the same");
            Assert.AreEqual("Analog 67 A13", o.DeviceName, "Name not the same");
            Assert.HasCount(1, o.Preconditions, "Preconditions Count not the same");
            Assert.HasCount(1, o.ConfigRefs, "Config ref count is not correct");
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
            o.WriteXml(xmlWriter, false);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\InputConfigItem\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");

            // https://github.com/MobiFlight/MobiFlight-Connector/issues/797
            o = new InputConfigItem();
            o.DeviceType = InputConfigItem.TYPE_ANALOG;
            if (o.analog == null) o.analog = new InputConfig.AnalogInputConfig();
            o.analog.onChange = new MSFS2020CustomInputAction() { Command = "test", PresetId = Guid.NewGuid().ToString() };

            sw = new StringWriter();
            xmlWriter = System.Xml.XmlWriter.Create(sw, settings);
            xmlWriter.WriteStartElement("settings");
            o.WriteXml(xmlWriter, false);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            s = sw.ToString();

            StringReader sr = new StringReader(s);
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;

            XmlReader xmlReader = System.Xml.XmlReader.Create(sr, readerSettings);
            InputConfigItem o1 = new InputConfigItem();
            xmlReader.ReadToDescendant("settings");
            o1.ReadXml(xmlReader);

            Assert.IsNotNull(o1.analog, "Is null");
            Assert.AreEqual(o.analog.onChange is MSFS2020CustomInputAction, o1.analog.onChange is MSFS2020CustomInputAction, "Not of type MSFS2020CustomInputAction");
        }

        [TestMethod()]
        public void CloneTest()
        {
            InputConfigItem o = generateTestObject();
            InputConfigItem c = (InputConfigItem)o.Clone();

            Assert.IsNotNull(c.button, "Button is null");
            Assert.IsNull(c.encoder, "Encoder is not null");
            Assert.AreEqual(c.ModuleSerial, o.ModuleSerial, "Module Serial not the same");
            Assert.AreEqual(c.Name, o.Name, "Name not the same");
            Assert.HasCount(1, c.Preconditions, "Precondition Count is not 1");
        }

        private InputConfigItem generateTestObject()
        {
            InputConfigItem result = new InputConfigItem();
            result.Name = "Test Input Config Item";
            result.Active = false;
            result.GUID = "123-input";

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

            result.encoder = null;
            result.ModuleSerial = "TestSerial";
            result.DeviceName = "TestName";
            result.DeviceType = InputConfigItem.TYPE_BUTTON;
            result.Preconditions.Add(new Precondition() { Serial = "PreConTestSerial" });
            result.ConfigRefs.Add(new Base.ConfigRef() { Active = true, Placeholder = "@", Ref = "0b1c877f-baf3-4c69-99e6-6c31429fe3bd" });
            result.ConfigRefs.Add(new Base.ConfigRef() { Active = false, Placeholder = "%", Ref = "7d1370d3-56e9-497a-8abb-63ecc169defe" });

            return result;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            InputConfigItem o1 = new InputConfigItem();
            InputConfigItem o2 = new InputConfigItem();
            o1.GUID = o2.GUID;

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();
            Assert.IsTrue(o1.Equals(o2));

            var list1 = new List<IConfigItem>() { o1, o2 };
            var list2 = new List<IConfigItem>() { o1, o2 };

            Assert.IsTrue(list1.SequenceEqual(list2));
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
            Assert.HasCount(0, statistics);

            o.analog = new InputConfig.AnalogInputConfig();
            o.analog.onChange = new InputConfig.MSFS2020CustomInputAction();
            statistics = o.GetStatistics();
            Assert.HasCount(o.analog.GetStatistics().Count, statistics);
        }

        [TestMethod()]
        public void GetInputActionsByTypeTest()
        {
            InputConfigItem o = new InputConfigItem();
            o.analog = new InputConfig.AnalogInputConfig();
            o.analog.onChange = new VariableInputAction();

            var result = o.GetInputActionsByType(typeof(VariableInputAction));
            Assert.HasCount(1, result);

            o.encoder = new InputConfig.EncoderInputConfig();
            o.encoder.onLeft = new VariableInputAction();

            result = o.GetInputActionsByType(typeof(VariableInputAction));
            Assert.HasCount(2, result);

            o.button = new InputConfig.ButtonInputConfig();
            o.button.onPress = new VariableInputAction();

            result = o.GetInputActionsByType(typeof(VariableInputAction));
            Assert.HasCount(3, result);

            o.inputShiftRegister = new InputConfig.InputShiftRegisterConfig();
            o.inputShiftRegister.onPress = new VariableInputAction();

            result = o.GetInputActionsByType(typeof(VariableInputAction));
            Assert.HasCount(4, result);
        }
    }
}