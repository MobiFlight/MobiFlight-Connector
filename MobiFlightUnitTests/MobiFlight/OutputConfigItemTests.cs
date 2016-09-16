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
    public class OutputConfigItemTests
    {
        [TestMethod()]
        public void OutputConfigItemTest()
        {
            OutputConfigItem o = new OutputConfigItem();
            Assert.IsNotNull(o);
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            OutputConfigItem oci = new OutputConfigItem();
            Assert.IsNull(oci.GetSchema(), "The schema return value should be null");
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            OutputConfigItem oci = new OutputConfigItem();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\OutputConfigItem\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci.ReadXml(xmlReader);
            Assert.AreEqual (oci.FSUIPCOffsetType, FSUIPCOffsetType.Integer);
            Assert.AreEqual(oci.FSUIPCOffset, 0x034E);
            Assert.AreEqual(oci.FSUIPCSize, 2);
            Assert.AreEqual(oci.Transform.Expression, "$");
            Assert.AreEqual(oci.FSUIPCMask, 0xFFFF);
            Assert.AreEqual(oci.FSUIPCBcdMode, true);
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

            OutputConfigItem o = _generateConfigItem();
            xmlWriter.WriteStartElement("settings");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\OutputConfigItem\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            OutputConfigItem o = _generateConfigItem();
            
            OutputConfigItem c = (OutputConfigItem)o.Clone();
            Assert.AreEqual(o.FSUIPCOffset, c.FSUIPCOffset, "clone: FSUIPCOffset not the same");
            Assert.AreEqual(o.FSUIPCMask, c.FSUIPCMask, " clone: FSUIPCMask not the same");
            Assert.AreEqual(o.Transform.Expression, c.Transform.Expression, "clone: FSUIPCOffsetType not the same");
            Assert.AreEqual(o.FSUIPCOffsetType, c.FSUIPCOffsetType, "clone: FSUIPCOffsetType not the same");
            Assert.AreEqual(o.FSUIPCSize, c.FSUIPCSize, "clone: FSUIPCSize not the same");
            Assert.AreEqual(o.FSUIPCBcdMode, c.FSUIPCBcdMode, "clone: FSUIPCBcdMode not the same");
            Assert.AreEqual(o.ComparisonActive, c.ComparisonActive, "clone: ComparisonActive not the same");
            Assert.AreEqual(o.ComparisonOperand, c.ComparisonOperand, "clone: ComparisonOperand not the same");
            Assert.AreEqual(o.ComparisonValue, c.ComparisonValue, "clone: ComparisonValue not the same");
            Assert.AreEqual(o.ComparisonIfValue, c.ComparisonIfValue, "clone: ComparisonIfValue not the same");
            Assert.AreEqual(o.ComparisonElseValue, c.ComparisonElseValue, "clone: ComparisonElseValue not the same");

            Assert.AreEqual(o.DisplayPin, c.DisplayPin, "clone: DisplayPin not the same");
            Assert.AreEqual(o.DisplayType, c.DisplayType, "clone: DisplayType not the same");
            Assert.AreEqual(o.DisplaySerial, c.DisplaySerial, "clone: DisplaySerial not the same");
            Assert.AreEqual(o.DisplayPinBrightness, c.DisplayPinBrightness, "clone: DisplayPinBrightness not the same");
            Assert.AreEqual(o.DisplayLedConnector, c.DisplayLedConnector, "clone: DisplayLedConnector not the same");
            Assert.AreEqual(o.DisplayLedAddress, c.DisplayLedAddress, "clone: DisplayLedAddress not the same");
            Assert.AreEqual(o.DisplayLedPadding, c.DisplayLedPadding, "clone: DisplayLedPadding not the same");
            Assert.AreEqual(o.DisplayLedPaddingChar, c.DisplayLedPaddingChar, "clone: DisplayLedPaddingChar not the same");
            Assert.AreEqual(o.DisplayLedModuleSize, c.DisplayLedModuleSize, "clone: DisplayLedModuleSize not the same");
            
            Assert.AreEqual(o.DisplayLedDigits[0], c.DisplayLedDigits[0], "clone: DisplayLedDigits not the same");
            Assert.AreEqual(o.DisplayLedDecimalPoints[0], c.DisplayLedDecimalPoints[0], "clone: DisplayLedDecimalPoints not the same");
            Assert.AreEqual(o.ServoAddress, c.ServoAddress, "clone: ServoAddress not the same");
            Assert.AreEqual(o.ServoMax, c.ServoMax, "clone: ServoMax not the same");
            Assert.AreEqual(o.ServoMin, c.ServoMin, "clone: ServoMin not the same");
            Assert.AreEqual(o.ServoMaxRotationPercent, c.ServoMaxRotationPercent, "clone: ServoMaxRotationPercent not the same");

            Assert.AreEqual(o.StepperAddress, c.StepperAddress, "clone: StepperAddress not the same");
            Assert.AreEqual(o.StepperInputRev, c.StepperInputRev, "clone: StepperInputRev not the same");
            Assert.AreEqual(o.StepperOutputRev, c.StepperOutputRev, "clone: StepperOutputRev not the same");
            Assert.AreEqual(o.StepperTestValue, c.StepperTestValue, "clone: StepperTestValue not the same");

            Assert.AreEqual(o.BcdPins[0], c.BcdPins[0], "clone: BcdPins not the same");

            //o. = new Interpolation();
            Assert.AreEqual(o.Interpolation.Count, c.Interpolation.Count, "clone: Interpolation not the same");
            Assert.AreEqual(o.Preconditions.Count, c.Preconditions.Count, "clone: Preconditions not the same");
        }

        private OutputConfigItem _generateConfigItem()
        {
            OutputConfigItem o = new OutputConfigItem();
            
            o.FSUIPCOffset = 0x1234;
            o.FSUIPCMask = 0xFFFF;
            o.Transform = new Transformation();
            o.Transform.Expression = "$+123";
            o.Transform.SubStrEnd = 11;
            o.Transform.SubStrStart = 9;

            o.FSUIPCOffsetType = FSUIPCOffsetType.Float;
            o.FSUIPCSize = 2;
            o.FSUIPCBcdMode = true;
            o.ComparisonActive = true;
            o.ComparisonOperand = ">";
            o.ComparisonValue = "1";
            o.ComparisonIfValue = "2";
            o.ComparisonElseValue = "3";

            o.DisplayType = MobiFlight.DeviceType.Stepper.ToString("F");
            o.DisplaySerial = "Ser123";
            o.DisplayPin = "A01";
            o.DisplayPinBrightness = byte.MinValue;
            o.DisplayLedConnector = 2;
            o.DisplayLedAddress = "1";
            o.DisplayLedPadding = true;
            o.DisplayLedPaddingChar = "1";
            o.DisplayLedModuleSize = 7;
            o.DisplayLedDigits = new List<string>() { "1", "2" };
            o.DisplayLedDecimalPoints = new List<string>() { "3", "4" };

            o.BcdPins = new List<string>() { "Moop" };
            o.Interpolation = new Interpolation();
            o.Interpolation.Add(123, 456);

            o.Preconditions = new List<Precondition>();
            o.Preconditions.Add(new Precondition() { PreconditionLabel = "Test", PreconditionType = "config", PreconditionRef = "Ref123", PreconditionOperand = "op123", PreconditionValue = "val123", PreconditionLogic = "AND"  });

            o.ServoAddress = "A2";
            o.ServoMax = "11";
            o.ServoMin = "111";
            o.ServoMaxRotationPercent = "176";

            o.StepperAddress = "S22";
            o.StepperInputRev = "1123";
            o.StepperOutputRev = "3212";
            o.StepperTestValue = "212";

            return o;
        }
    }
}