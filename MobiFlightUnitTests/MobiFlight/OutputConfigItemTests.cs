﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.Base;
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
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci.ReadXml(xmlReader);
            Assert.AreEqual(oci.FSUIPC.OffsetType, FSUIPCOffsetType.Integer);
            Assert.AreEqual(oci.FSUIPC.Offset, 0x034E);
            Assert.AreEqual(oci.FSUIPC.Size, 2);
            Assert.AreEqual(oci.FSUIPC.Mask, 0xFFFF);
            Assert.AreEqual(oci.FSUIPC.BcdMode, true);
            Assert.AreEqual(oci.Transform.Expression, "$+123");

            // read backward compatible
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.2.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci.ReadXml(xmlReader);
            Assert.AreEqual(oci.Transform.Active, true);
            Assert.AreEqual(oci.Transform.Expression, "$*123");
            Assert.AreEqual(oci.DisplayLedBrightnessReference, "CF057791-E133-4638-A99E-FEF9B187C4DB");

            // read problem with missing configrefs
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.3.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci.ReadXml(xmlReader);
            Assert.AreEqual(6, oci.Preconditions.Count);
            Assert.AreEqual(4, oci.ConfigRefs.Count);

            // read problem with interpolation inside of display node
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.4.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci.ReadXml(xmlReader);
            Assert.AreEqual(true, oci.Interpolation.Active, "Interpolation is supposed to be active");
            Assert.AreEqual(5, oci.Interpolation.Count);

            // read problem with interpolation OUTSIDE of display node
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.5.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci.ReadXml(xmlReader);
            Assert.AreEqual(true, oci.Interpolation.Active, "Interpolation is supposed to be active");
            Assert.AreEqual(5, oci.Interpolation.Count);
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

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            OutputConfigItem o = _generateConfigItem();
            
            OutputConfigItem c = (OutputConfigItem)o.Clone();
            Assert.AreEqual(o.FSUIPC.Offset, c.FSUIPC.Offset, "clone: FSUIPCOffset not the same");
            Assert.AreEqual(o.FSUIPC.Mask, c.FSUIPC.Mask, " clone: FSUIPCMask not the same");
            Assert.AreEqual(o.Transform.Expression, c.Transform.Expression, "clone: FSUIPCOffsetType not the same");
            Assert.AreEqual(o.FSUIPC.OffsetType, c.FSUIPC.OffsetType, "clone: FSUIPCOffsetType not the same");
            Assert.AreEqual(o.FSUIPC.Size, c.FSUIPC.Size, "clone: FSUIPCSize not the same");
            Assert.AreEqual(o.FSUIPC.BcdMode, c.FSUIPC.BcdMode, "clone: FSUIPCBcdMode not the same");
            Assert.AreEqual(o.ComparisonActive, c.ComparisonActive, "clone: ComparisonActive not the same");
            Assert.AreEqual(o.ComparisonOperand, c.ComparisonOperand, "clone: ComparisonOperand not the same");
            Assert.AreEqual(o.ComparisonValue, c.ComparisonValue, "clone: ComparisonValue not the same");
            Assert.AreEqual(o.ComparisonIfValue, c.ComparisonIfValue, "clone: ComparisonIfValue not the same");
            Assert.AreEqual(o.ComparisonElseValue, c.ComparisonElseValue, "clone: ComparisonElseValue not the same");

            Assert.AreEqual(o.DisplayPin, c.DisplayPin, "clone: DisplayPin not the same");
            Assert.AreEqual(o.DisplayType, c.DisplayType, "clone: DisplayType not the same");
            Assert.AreEqual(o.DisplaySerial, c.DisplaySerial, "clone: DisplaySerial not the same");
            Assert.AreEqual(o.DisplayPinBrightness, c.DisplayPinBrightness, "clone: DisplayPinBrightness not the same");
            Assert.AreEqual(o.DisplayPinPWM, c.DisplayPinPWM, "clone: DisplayPinPWM not the same");

            Assert.AreEqual(o.DisplayLedConnector, c.DisplayLedConnector, "clone: DisplayLedConnector not the same");
            Assert.AreEqual(o.DisplayLedAddress, c.DisplayLedAddress, "clone: DisplayLedAddress not the same");
            Assert.AreEqual(o.DisplayLedPadding, c.DisplayLedPadding, "clone: DisplayLedPadding not the same");
            Assert.AreEqual(o.DisplayLedReverseDigits, c.DisplayLedReverseDigits, "clone: DisplayLedReverseDigits not the same");
            Assert.AreEqual(o.DisplayLedPaddingChar, c.DisplayLedPaddingChar, "clone: DisplayLedPaddingChar not the same");
            Assert.AreEqual(o.DisplayLedModuleSize, c.DisplayLedModuleSize, "clone: DisplayLedModuleSize not the same");
            Assert.AreEqual(o.DisplayLedBrightnessReference, c.DisplayLedBrightnessReference, "clone: DisplayLedBrighntessReference is not the same");
            
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
            Assert.AreEqual(o.Interpolation.Active, c.Interpolation.Active, "clone: Interpolation.Active is not the same.");
            Assert.AreEqual(o.Interpolation.Count, c.Interpolation.Count, "clone: Interpolation.Count not the same");
            Assert.AreEqual(o.Preconditions.Count, c.Preconditions.Count, "clone: Preconditions.Count not the same");

            // Config References
            Assert.AreEqual(o.ConfigRefs.Count, c.ConfigRefs.Count, "clone: ConfigRefs.Count not the same");

        }

        private OutputConfigItem _generateConfigItem()
        {
            OutputConfigItem o = new OutputConfigItem();
            
            o.FSUIPC.Offset = 0x1234;
            o.FSUIPC.Mask = 0xFFFF;
            o.Transform = new Transformation();
            o.Transform.Active = true;
            o.Transform.Expression = "$+123";
            o.Transform.SubStrEnd = 11;
            o.Transform.SubStrStart = 9;

            o.FSUIPC.OffsetType = FSUIPCOffsetType.Float;
            o.FSUIPC.Size = 2;
            o.FSUIPC.BcdMode = true;
            o.ComparisonActive = true;
            o.ComparisonOperand = ">";
            o.ComparisonValue = "1";
            o.ComparisonIfValue = "2";
            o.ComparisonElseValue = "3";

            o.DisplayType = MobiFlight.DeviceType.Stepper.ToString("F");
            o.DisplaySerial = "Ser123";
            o.DisplayPin = "A01";
            o.DisplayPinBrightness = byte.MinValue;
            o.DisplayPinPWM = true;
            o.DisplayLedConnector = 2;
            o.DisplayLedAddress = "1";
            o.DisplayLedPadding = true;
            o.DisplayLedReverseDigits = true;
            o.DisplayLedPaddingChar = "1";
            o.DisplayLedModuleSize = 7;
            o.DisplayLedDigits = new List<string>() { "1", "2" };
            o.DisplayLedDecimalPoints = new List<string>() { "3", "4" };
            o.DisplayLedBrightnessReference = "CF057791-E133-4638-A99E-FEF9B187C4DB"; // testing with true as default is false
            o.BcdPins = new List<string>() { "Moop" };
            o.Interpolation = new Interpolation();
            o.Interpolation.Active = true;
            o.Interpolation.Add(123, 456);

            o.Preconditions = new PreconditionList();
            o.Preconditions.Add(new Precondition() { PreconditionLabel = "Test", PreconditionType = "config", PreconditionRef = "Ref123", PreconditionOperand = "op123", PreconditionValue = "val123", PreconditionLogic = "AND"  });

            o.ServoAddress = "A2";
            o.ServoMax = "11";
            o.ServoMin = "111";
            o.ServoMaxRotationPercent = "176";

            o.StepperAddress = "S22";
            o.StepperInputRev = "1123";
            o.StepperOutputRev = "3212";
            o.StepperTestValue = "212";
            o.StepperCompassMode = true;

            o.ConfigRefs.Add(new ConfigRef() { Active = true, Placeholder = "#", Ref = "123" });
            o.ConfigRefs.Add(new ConfigRef() { Active = false, Placeholder = "$", Ref = "321" });

            return o;
        }
    }
}