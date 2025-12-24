using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.Modifier;
using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.IO;
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
            Assert.IsTrue(oci.Source is FsuipcSource);
            Assert.AreEqual(FSUIPCOffsetType.Integer, (oci.Source as FsuipcSource).FSUIPC.OffsetType);
            Assert.AreEqual(0x034E, (oci.Source as FsuipcSource).FSUIPC.Offset);
            Assert.AreEqual(2, (oci.Source as FsuipcSource).FSUIPC.Size);
            Assert.AreEqual(0xFFFF, (oci.Source as FsuipcSource).FSUIPC.Mask);
            Assert.IsTrue((oci.Source as FsuipcSource).FSUIPC.BcdMode);
            Assert.AreEqual("$+123", oci.Modifiers.Transformation.Expression);

            // read backward compatible
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.2.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci.ReadXml(xmlReader);
            Assert.IsTrue(oci.Modifiers.Transformation.Active);
            Assert.AreEqual("$*123", oci.Modifiers.Transformation.Expression);
            Assert.IsTrue(oci.Device is LedModule);
            Assert.AreEqual("CF057791-E133-4638-A99E-FEF9B187C4DB", (oci.Device as LedModule).DisplayLedBrightnessReference);

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
            Assert.IsTrue(oci.Modifiers.Interpolation.Active, "Interpolation is supposed to be active");
            Assert.AreEqual(5, oci.Modifiers.Interpolation.Count);

            // read problem with interpolation OUTSIDE of display node
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.5.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci.ReadXml(xmlReader);
            Assert.IsTrue(oci.Modifiers.Interpolation.Active, "Interpolation is supposed to be active");
            Assert.AreEqual(5, oci.Modifiers.Interpolation.Count);

            // read buttoninputaction
            // read problem with interpolation OUTSIDE of display node
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\ReadXmlTest.ButtonInputAction.xml");
            sr = new StringReader(s);
            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            oci.ReadXml(xmlReader);
            Assert.IsNotNull(oci.ButtonInputConfig, "ButtonInputConfig null");
            Assert.IsNotNull(oci.ButtonInputConfig.onPress, "ButtonInputConfig.onPress null");
            Assert.IsNotNull(oci.ButtonInputConfig.onRelease, "ButtonInputConfig.onRelease null");
            Assert.IsNotNull(oci.ButtonInputConfig.onPress as InputConfig.MSFS2020CustomInputAction, "Not of type MobiFlight.InputConfig.MSFS2020CustomInputAction");
            Assert.AreEqual("Test", (oci.ButtonInputConfig.onPress as InputConfig.MSFS2020CustomInputAction).Command, "Not correct Command.");
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
            Assert.IsNotNull(oci.ButtonInputConfig.onPress as InputConfig.MSFS2020CustomInputAction, "Not of type MobiFlight.InputConfig.MSFS2020CustomInputAction");
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
            Assert.AreEqual("Display Module", oci.DeviceType, "Display Type not Display Module");
            Assert.IsTrue(oci.Modifiers.Interpolation.Active, "AnalogInputConfig.onPress null");
            Assert.AreEqual(5, oci.Modifiers.Interpolation.Count, "Interpolation Count is not 5");
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
            o.WriteXml(xmlWriter, false);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\WriteXmlTest.1.xml");
            Assert.AreEqual(s, result, "The both strings are not equal");

            // Do the same for SimConnect
            o.Source = new SimConnectSource()
            {
                SimConnectValue = new SimConnectValue()
                {
                    UUID = "1234",
                    Value = "Test",
                    VarType = SimConnectVarType.CODE
                }
            };

            sw = new StringWriter();
            xmlWriter = System.Xml.XmlWriter.Create(sw, settings);
            xmlWriter.WriteStartElement("settings");
            o.WriteXml(xmlWriter, false);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            s = sw.ToString();

            result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\OutputConfigItem\WriteXmlTest.2.xml");
            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            OutputConfigItem o = _generateConfigItem();

            OutputConfigItem c = (OutputConfigItem)o.Clone();
            Assert.AreEqual(c.Name, o.Name, "clone: Name not the same");
            Assert.AreEqual(c.Active, o.Active, "clone: Active not the same");
            Assert.AreEqual(c.GUID, o.GUID, "clone: GUID not the same");
            Assert.AreEqual(c.Type, o.Type, "clone: Type not the same");

            Assert.AreEqual((o.Source as FsuipcSource).FSUIPC.Offset, (c.Source as FsuipcSource).FSUIPC.Offset, "clone: FSUIPCOffset not the same");
            Assert.AreEqual((o.Source as FsuipcSource).FSUIPC.Mask, (c.Source as FsuipcSource).FSUIPC.Mask, " clone: FSUIPCMask not the same");
            Assert.AreEqual((o.Source as FsuipcSource).FSUIPC.OffsetType, (c.Source as FsuipcSource).FSUIPC.OffsetType, "clone: FSUIPCOffsetType not the same");
            Assert.AreEqual((o.Source as FsuipcSource).FSUIPC.Size, (c.Source as FsuipcSource).FSUIPC.Size, "clone: FSUIPCSize not the same");
            Assert.AreEqual((o.Source as FsuipcSource).FSUIPC.BcdMode, (c.Source as FsuipcSource).FSUIPC.BcdMode, "clone: FSUIPCBcdMode not the same");
            Assert.AreEqual(o.Modifiers.Transformation.Expression, c.Modifiers.Transformation.Expression, "clone: FSUIPCOffsetType not the same");
            Assert.AreEqual(o.Modifiers.Comparison.Active, c.Modifiers.Comparison.Active, "clone: ComparisonActive not the same");
            Assert.AreEqual(o.Modifiers.Comparison.Operand, c.Modifiers.Comparison.Operand, "clone: ComparisonOperand not the same");
            Assert.AreEqual(o.Modifiers.Comparison.Value, c.Modifiers.Comparison.Value, "clone: ComparisonValue not the same");
            Assert.AreEqual(o.Modifiers.Comparison.IfValue, c.Modifiers.Comparison.IfValue, "clone: ComparisonIfValue not the same");
            Assert.AreEqual(o.Modifiers.Comparison.ElseValue, c.Modifiers.Comparison.ElseValue, "clone: ComparisonElseValue not the same");
            Assert.AreEqual(o.Modifiers.Interpolation.Count, c.Modifiers.Interpolation.Count, "clone: Interpolation count not right");

            Assert.AreEqual(c.DeviceType, o.DeviceType, "clone: DisplayType not the same");
            Assert.AreEqual(c.ModuleSerial, o.ModuleSerial, "clone: DisplaySerial not the same");

            if (o.DeviceType == MobiFlight.DeviceType.Output.ToString("F"))
            {
                Assert.AreEqual((o.Device as Output).Pin, (c.Device as Output).Pin, "clone: DisplayPin not the same");
                Assert.AreEqual((o.Device as Output).Brightness, (c.Device as Output).Brightness, "clone: DisplayPinBrightness not the same");
                Assert.AreEqual((o.Device as Output).PwmMode, (c.Device as Output).PwmMode, "clone: DisplayPinPWM not the same");
            }

            if (o.Device is LedModule)
            {
                Assert.AreEqual((o.Device as LedModule).DisplayLedConnector, (c.Device as LedModule).DisplayLedConnector, "clone: DisplayLedConnector not the same");
                Assert.AreEqual((o.Device as LedModule).DisplayLedAddress, (c.Device as LedModule).DisplayLedAddress, "clone: DisplayLedAddress not the same");
                Assert.AreEqual((o.Device as LedModule).DisplayLedPadding, (c.Device as LedModule).DisplayLedPadding, "clone: DisplayLedPadding not the same");
                Assert.AreEqual((o.Device as LedModule).DisplayLedReverseDigits, (c.Device as LedModule).DisplayLedReverseDigits, "clone: DisplayLedReverseDigits not the same");
                Assert.AreEqual((o.Device as LedModule).DisplayLedPaddingChar, (c.Device as LedModule).DisplayLedPaddingChar, "clone: DisplayLedPaddingChar not the same");
                Assert.AreEqual((o.Device as LedModule).DisplayLedModuleSize, (c.Device as LedModule).DisplayLedModuleSize, "clone: DisplayLedModuleSize not the same");
                Assert.AreEqual((o.Device as LedModule).DisplayLedDigits[0], (c.Device as LedModule).DisplayLedDigits[0], "clone: DisplayLedDigits not the same");
                Assert.AreEqual((o.Device as LedModule).DisplayLedDecimalPoints[0], (c.Device as LedModule).DisplayLedDecimalPoints[0], "clone: DisplayLedDecimalPoints not the same");
                Assert.AreEqual((o.Device as LedModule).DisplayLedBrightnessReference, (c.Device as LedModule).DisplayLedBrightnessReference, "clone: DisplayLedBrighntessReference is not the same");
            }

            if (o.Device is Servo)
            {
                var oServo = o.Device as Servo;
                var cServo = c.Device as Servo;

                Assert.AreEqual(cServo.Address, oServo.Address, "clone: ServoAddress not the same");
                Assert.AreEqual(cServo.Max, oServo.Max, "clone: ServoMax not the same");
                Assert.AreEqual(cServo.Min, oServo.Min, "clone: ServoMin not the same");
                Assert.AreEqual(cServo.MaxRotationPercent, oServo.MaxRotationPercent, "clone: ServoMaxRotationPercent not the same");
            }

            if (o.Device is Stepper) {
                var oStepper = o.Device as Stepper;
                var cStepper = c.Device as Stepper;

                Assert.AreEqual(cStepper.Address, oStepper.Address, "clone: StepperAddress not the same");
                Assert.AreEqual(cStepper.InputRev, oStepper.InputRev, "clone: StepperInputRev not the same");
                Assert.AreEqual(cStepper.OutputRev, oStepper.OutputRev, "clone: StepperOutputRev not the same");
                Assert.AreEqual(cStepper.TestValue, oStepper.TestValue, "clone: StepperTestValue not the same");
            }

            if (o.Device is ShiftRegister)
            {
                var oShiftRegister = o.Device as ShiftRegister;
                var cShiftRegister = c.Device as ShiftRegister;
                // Shift Register
                Assert.AreEqual(cShiftRegister.Address, oShiftRegister.Address, "clone: ShiftRegister.Address not the same");
                Assert.AreEqual(cShiftRegister.Pin, oShiftRegister.Pin, "clone: ShiftRegister.Address not the same");
            }

            if (o.Device is CustomDevice)
            {
                var oCustomDevice = o.Device as CustomDevice;
                var cCustomDevice = c.Device as CustomDevice;
                // Custom Device
                Assert.AreEqual(cCustomDevice.CustomType, oCustomDevice.CustomType, "clone: CustomDevice.CustomType not the same");
                Assert.AreEqual(cCustomDevice.CustomName, oCustomDevice.CustomName, "clone: CustomDevice.CustomName not the same");
                Assert.AreEqual(cCustomDevice.MessageType, oCustomDevice.MessageType, "clone: CustomDevice.MessageType not the same");
                Assert.AreEqual(cCustomDevice.Value, oCustomDevice.Value, "clone: CustomDevice.Value not the same");
            }

            //o. = new Interpolation();
            Assert.AreEqual(o.Modifiers.Interpolation.Active, c.Modifiers.Interpolation.Active, "clone: Interpolation.Active is not the same.");
            Assert.AreEqual(o.Modifiers.Interpolation.Count, c.Modifiers.Interpolation.Count, "clone: Interpolation.Count not the same");


            Assert.AreEqual(o.Preconditions.Count, c.Preconditions.Count, "clone: Preconditions.Count not the same");

            // Config References
            Assert.AreEqual(o.ConfigRefs.Count, c.ConfigRefs.Count, "clone: ConfigRefs.Count not the same");

        }

        private OutputConfigItem _generateConfigItem(string deviceType = "Stepper")
        {
            OutputConfigItem o = new OutputConfigItem();
            o.Name = "Test";
            o.Active = false;
            o.GUID = "123";

            o.Source = new FsuipcSource()
            {
                FSUIPC = new FsuipcOffset()
                {
                    Offset = 0x1234,
                    Mask = 0xFFFF,
                    OffsetType = FSUIPCOffsetType.Integer,
                    Size = 2,
                    BcdMode = true
                }
            };
            
            o.Modifiers.Transformation.Active = true;
            o.Modifiers.Transformation.Expression = "$+123";
            o.Modifiers.Comparison.Active = true;
            o.Modifiers.Comparison.Operand = ">";
            o.Modifiers.Comparison.Value = "1";
            o.Modifiers.Comparison.IfValue = "2";
            o.Modifiers.Comparison.ElseValue = "3";

            o.DeviceType = MobiFlight.DeviceType.Stepper.ToString("F");
            o.ModuleSerial = "Ser123";

            switch (deviceType)
            {
                case "Display":
                    o.DeviceType = MobiFlight.DeviceType.Output.ToString("F");
                    o.Device = new Output()
                    {
                        Pin = "A01",
                        Brightness = byte.MinValue,
                        PwmMode = true
                    };
                    break;

                case "LedModule":
                    o.DeviceType = MobiFlight.DeviceType.LedModule.ToString("F");
                    o.Device = new LedModule()
                    {
                        DisplayLedConnector = 2,
                        DisplayLedAddress = "1",
                        DisplayLedPadding = true,
                        DisplayLedReverseDigits = true,
                        DisplayLedPaddingChar = "1",
                        DisplayLedModuleSize = 7,
                        DisplayLedDigits = new List<string>() { "1", "2" },
                        DisplayLedDecimalPoints = new List<string>() { "3", "4" },
                        DisplayLedBrightnessReference = "CF057791-E133-4638-A99E-FEF9B187C4DB"
                    };
                    break;

                case "Servo":
                    o.DeviceType = MobiFlight.DeviceType.Servo.ToString("F");
                    o.Device = new Servo()
                    {
                        Address = "A2",
                        Max = "11",
                        Min = "111",
                        MaxRotationPercent = "176"
                    };
                    break;

                case "ShiftRegister":
                    o.DeviceType = MobiFlight.DeviceType.ShiftRegister.ToString("F");
                    o.Device = new ShiftRegister()
                    {
                        Address = "ShiftRegister",
                        Pin = "99"
                    };
                    break;

                case "Stepper":
                    o.DeviceType = MobiFlight.DeviceType.Stepper.ToString("F");
                    o.Device = new Stepper()
                    {
                        Address = "S22",
                        InputRev = 1123,
                        OutputRev = 3212,
                        TestValue = 212,
                        CompassMode = true
                    };
                    break;

                case "CustomDevice":
                    o.DeviceType = MobiFlight.DeviceType.CustomDevice.ToString("F");
                    o.Device = new CustomDevice()
                    {
                        CustomType = "TestCustomType",
                        CustomName = "Test Custom Name",
                        MessageType = 1,
                        Value = "Test Value"
                    };
                    break;

                default:
                    throw new ArgumentException("Invalid device type");
            }

            var i = new Interpolation() { Active = true };
            i.Add(123, 456);
            o.Modifiers.Items.Add(i);

            o.Preconditions = new PreconditionList();
            o.Preconditions.Add(new Precondition() { Label = "Test", Type = "config", Ref = "Ref123", Operand = "op123", Value = "val123", Logic = "AND" });

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
            // test with different GUID
            Assert.IsFalse(o1.Equals(o2));

            o1.GUID = o2.GUID;
            Assert.IsTrue(o1.Equals(o2));

            // test with same GUID
            o1 = _generateConfigItem();
            o2 = _generateConfigItem();
            Assert.IsTrue(o1.Equals(o2));

            // test with different GUID
            o2.GUID = "foobar";
            Assert.IsFalse(o1.Equals(o2));

            o2 = _generateConfigItem();
            // test with same GUID again.
            Assert.IsTrue(o1.Equals(o2));

            // Check for the ShiftRegister
            o1 = _generateConfigItem("ShiftRegister");
            o2 = _generateConfigItem("ShiftRegister");
            Assert.IsTrue(o1.Equals(o2));

            (o2.Device as ShiftRegister).Address = "ShiftRegister1";
            (o2.Device as ShiftRegister).Pin = "69";
            Assert.IsFalse(o1.Equals(o2));

            // reset o2
            // https://github.com/MobiFlight/MobiFlight-Connector/issues/697
            o1 = _generateConfigItem("Servo");
            o2 = _generateConfigItem("Servo");
            Assert.IsTrue(o1.Equals(o2));

            (o2.Device as Servo).MaxRotationPercent = "90";
            Assert.IsFalse(o1.Equals(o2));

            o2 = _generateConfigItem("Servo");
            Assert.IsTrue(o1.Equals(o2));
            o2.DeviceType = "nonsense";
            Assert.IsFalse(o1.Equals(o2));


            o1 = _generateConfigItem("CustomDevice");
            o2 = _generateConfigItem("CustomDevice");
            Assert.IsTrue(o1.Equals(o2));
            (o2.Device as CustomDevice).Value = "Test value will fail";
            Assert.IsFalse(o1.Equals(o2));

            o2 = _generateConfigItem("CustomDevice");
            Assert.IsTrue(o1.Equals(o2));
            o1.Status[ConfigItemStatusType.Source] = "Source not available";
            Assert.IsFalse(o1.Equals(o2));
            o2.Status[ConfigItemStatusType.Source] = "Source not available";
            Assert.IsTrue(o1.Equals(o2));
        }
    }
}