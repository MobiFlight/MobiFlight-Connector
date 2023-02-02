using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.Base;
using MobiFlight.Modifier;
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
            Assert.AreEqual(oci.LedModule.DisplayLedBrightnessReference, "CF057791-E133-4638-A99E-FEF9B187C4DB");

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

            // read buttoninputaction
            // read problem with interpolation OUTSIDE of display node
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.ButtonInputAction.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci.ReadXml(xmlReader);
            Assert.IsNotNull(oci.ButtonInputConfig, "ButtonInputConfig null");
            Assert.IsNotNull(oci.ButtonInputConfig.onPress, "ButtonInputConfig.onPress null");
            Assert.IsNotNull(oci.ButtonInputConfig.onRelease, "ButtonInputConfig.onRelease null");
            Assert.IsNotNull(oci.ButtonInputConfig.onPress as MobiFlight.InputConfig.MSFS2020CustomInputAction, "Not of type MobiFlight.InputConfig.MSFS2020CustomInputAction");
            Assert.AreEqual("Test", (oci.ButtonInputConfig.onPress as MobiFlight.InputConfig.MSFS2020CustomInputAction).Command, "Not correct Command.");
            // read analoginputaction

            // read buttoninputaction
            // read problem with configrefs are not loaded correctly
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.ButtonInputActionConfigRef.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci = new OutputConfigItem();
            oci.ReadXml(xmlReader);
            Assert.IsNotNull(oci.ButtonInputConfig, "ButtonInputConfig null");
            Assert.IsNotNull(oci.ButtonInputConfig.onPress, "ButtonInputConfig.onPress null");
            Assert.IsNull(oci.ButtonInputConfig.onRelease, "ButtonInputConfig.onRelease is not null");
            Assert.IsNotNull(oci.ButtonInputConfig.onPress as MobiFlight.InputConfig.MSFS2020CustomInputAction, "Not of type MobiFlight.InputConfig.MSFS2020CustomInputAction");
            Assert.AreEqual(1, oci.ConfigRefs.Count, "Count is not 1");

            // read buttoninputaction
            // read problem with configrefs are not loaded correctly
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.AnalogInputActionProblem.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci = new OutputConfigItem();
            oci.ReadXml(xmlReader);
            Assert.IsNotNull(oci.AnalogInputConfig, "AnalogInputConfig null");
            Assert.IsNotNull(oci.AnalogInputConfig.onChange, "AnalogInputConfig.onPress null");
            Assert.IsNotNull(oci.AnalogInputConfig.onChange as MobiFlight.InputConfig.MSFS2020CustomInputAction, "Not of type MobiFlight.InputConfig.MSFS2020CustomInputAction");
            Assert.AreEqual(0, oci.ConfigRefs.Count, "ConfigRefs Count is not 1");

            // read buttoninputaction
            // read problem with configrefs are not loaded correctly
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.AnalogInputActionWithConfigRef.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci = new OutputConfigItem();
            oci.ReadXml(xmlReader);
            Assert.IsNotNull(oci.AnalogInputConfig, "AnalogInputConfig null");
            Assert.IsNotNull(oci.AnalogInputConfig.onChange, "AnalogInputConfig.onPress null");
            Assert.IsNotNull(oci.AnalogInputConfig.onChange as MobiFlight.InputConfig.MSFS2020CustomInputAction, "Not of type MobiFlight.InputConfig.MSFS2020CustomInputAction");
            Assert.AreEqual(1, oci.ConfigRefs.Count, "ConfigRefs Count is not 1");

            // problem with reading config after xplane native integration
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.Interpolation.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci = new OutputConfigItem();
            oci.ReadXml(xmlReader);
            Assert.AreEqual("Display Module", oci.DisplayType, "Display Type not Display Module");
            Assert.AreEqual(true, oci.Interpolation.Active, "AnalogInputConfig.onPress null");
            Assert.AreEqual(5, oci.Interpolation.Count, "Interpolation Count is not 5");
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
            Assert.AreEqual(o.Comparison.Active, c.Comparison.Active, "clone: ComparisonActive not the same");
            Assert.AreEqual(o.Comparison.Operand, c.Comparison.Operand, "clone: ComparisonOperand not the same");
            Assert.AreEqual(o.Comparison.Value, c.Comparison.Value, "clone: ComparisonValue not the same");
            Assert.AreEqual(o.Comparison.IfValue, c.Comparison.IfValue, "clone: ComparisonIfValue not the same");
            Assert.AreEqual(o.Comparison.ElseValue, c.Comparison.ElseValue, "clone: ComparisonElseValue not the same");

            Assert.AreEqual(o.Pin.DisplayPin, c.Pin.DisplayPin, "clone: DisplayPin not the same");
            Assert.AreEqual(o.DisplayType, c.DisplayType, "clone: DisplayType not the same");
            Assert.AreEqual(o.DisplaySerial, c.DisplaySerial, "clone: DisplaySerial not the same");
            Assert.AreEqual(o.Pin.DisplayPinBrightness, c.Pin.DisplayPinBrightness, "clone: DisplayPinBrightness not the same");
            Assert.AreEqual(o.Pin.DisplayPinPWM, c.Pin.DisplayPinPWM, "clone: DisplayPinPWM not the same");

            Assert.AreEqual(o.LedModule.DisplayLedConnector, c.LedModule.DisplayLedConnector, "clone: DisplayLedConnector not the same");
            Assert.AreEqual(o.LedModule.DisplayLedAddress, c.LedModule.DisplayLedAddress, "clone: DisplayLedAddress not the same");
            Assert.AreEqual(o.LedModule.DisplayLedPadding, c.LedModule.DisplayLedPadding, "clone: DisplayLedPadding not the same");
            Assert.AreEqual(o.LedModule.DisplayLedReverseDigits, c.LedModule.DisplayLedReverseDigits, "clone: DisplayLedReverseDigits not the same");
            Assert.AreEqual(o.LedModule.DisplayLedPaddingChar, c.LedModule.DisplayLedPaddingChar, "clone: DisplayLedPaddingChar not the same");
            Assert.AreEqual(o.LedModule.DisplayLedModuleSize, c.LedModule.DisplayLedModuleSize, "clone: DisplayLedModuleSize not the same");
            Assert.AreEqual(o.LedModule.DisplayLedBrightnessReference, c.LedModule.DisplayLedBrightnessReference, "clone: DisplayLedBrighntessReference is not the same");

            Assert.AreEqual(o.LedModule.DisplayLedDigits[0], c.LedModule.DisplayLedDigits[0], "clone: DisplayLedDigits not the same");
            Assert.AreEqual(o.LedModule.DisplayLedDecimalPoints[0], c.LedModule.DisplayLedDecimalPoints[0], "clone: DisplayLedDecimalPoints not the same");
            Assert.AreEqual(o.Servo.Address, c.Servo.Address, "clone: ServoAddress not the same");
            Assert.AreEqual(o.Servo.Max, c.Servo.Max, "clone: ServoMax not the same");
            Assert.AreEqual(o.Servo.Min, c.Servo.Min, "clone: ServoMin not the same");
            Assert.AreEqual(o.Servo.MaxRotationPercent, c.Servo.MaxRotationPercent, "clone: ServoMaxRotationPercent not the same");

            Assert.AreEqual(o.Stepper.Address, c.Stepper.Address, "clone: StepperAddress not the same");
            Assert.AreEqual(o.Stepper.InputRev, c.Stepper.InputRev, "clone: StepperInputRev not the same");
            Assert.AreEqual(o.Stepper.OutputRev, c.Stepper.OutputRev, "clone: StepperOutputRev not the same");
            Assert.AreEqual(o.Stepper.TestValue, c.Stepper.TestValue, "clone: StepperTestValue not the same");

            Assert.AreEqual(o.BcdPins[0], c.BcdPins[0], "clone: BcdPins not the same");

            // Shift Register
            Assert.AreEqual(o.ShiftRegister.Address, c.ShiftRegister.Address, "clone: ShiftRegister.Address not the same");
            Assert.AreEqual(o.ShiftRegister.Pin, c.ShiftRegister.Pin, "clone: ShiftRegister.Address not the same");

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
            o.Comparison.Active = true;
            o.Comparison.Operand = ">";
            o.Comparison.Value = "1";
            o.Comparison.IfValue = "2";
            o.Comparison.ElseValue = "3";

            o.DisplayType = MobiFlight.DeviceType.Stepper.ToString("F");
            o.DisplaySerial = "Ser123";
            o.Pin.DisplayPin = "A01";
            o.Pin.DisplayPinBrightness = byte.MinValue;
            o.Pin.DisplayPinPWM = true;
            o.LedModule.DisplayLedConnector = 2;
            o.LedModule.DisplayLedAddress = "1";
            o.LedModule.DisplayLedPadding = true;
            o.LedModule.DisplayLedReverseDigits = true;
            o.LedModule.DisplayLedPaddingChar = "1";
            o.LedModule.DisplayLedModuleSize = 7;
            o.LedModule.DisplayLedDigits = new List<string>() { "1", "2" };
            o.LedModule.DisplayLedDecimalPoints = new List<string>() { "3", "4" };
            o.LedModule.DisplayLedBrightnessReference = "CF057791-E133-4638-A99E-FEF9B187C4DB"; // testing with true as default is false
            o.BcdPins = new List<string>() { "Moop" };
            o.Interpolation = new Interpolation();
            o.Interpolation.Active = true;
            o.Interpolation.Add(123, 456);

            o.Preconditions = new PreconditionList();
            o.Preconditions.Add(new Precondition() { PreconditionLabel = "Test", PreconditionType = "config", PreconditionRef = "Ref123", PreconditionOperand = "op123", PreconditionValue = "val123", PreconditionLogic = "AND" });

            o.Servo.Address = "A2";
            o.Servo.Max = "11";
            o.Servo.Min = "111";
            o.Servo.MaxRotationPercent = "176";

            o.Stepper.Address = "S22";
            o.Stepper.InputRev = 1123;
            o.Stepper.OutputRev = 3212;
            o.Stepper.TestValue = 212;
            o.Stepper.CompassMode = true;

            o.ShiftRegister = new OutputConfig.ShiftRegister()
            {
                Address = "ShiftRegister",
                Pin = "99"
            };

            o.ButtonInputConfig = new InputConfig.ButtonInputConfig();
            o.AnalogInputConfig = new InputConfig.AnalogInputConfig();

            o.ConfigRefs.Add(new ConfigRef() { Active = true, Placeholder = "#", Ref = "123" });
            o.ConfigRefs.Add(new ConfigRef() { Active = false, Placeholder = "$", Ref = "321" });

            return o;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            OutputConfigItem o1 = new OutputConfigItem();
            OutputConfigItem o2 = new OutputConfigItem();

            Assert.IsTrue(o1.Equals(o2));

            o1 = _generateConfigItem();
            Assert.IsFalse(o1.Equals(o2));

            o2 = _generateConfigItem();

            Assert.IsTrue(o1.Equals(o2));

            o2.ShiftRegister.Address = "ShiftRegister1";
            o2.ShiftRegister.Pin = "69";

            Assert.IsFalse(o1.Equals(o2));

            // reset o2
            // https://github.com/MobiFlight/MobiFlight-Connector/issues/697
            o2 = _generateConfigItem();
            o2.Servo.MaxRotationPercent = "90";
            Assert.IsFalse(o1.Equals(o2));

            o2 = _generateConfigItem();
            o2.DisplayType = "nonsense";
            Assert.IsFalse(o1.Equals(o2));
        }
    }
}