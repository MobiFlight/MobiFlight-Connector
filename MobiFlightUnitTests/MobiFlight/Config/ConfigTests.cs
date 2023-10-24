using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Config.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Config.Tests
{
    [TestClass()]
    public class ConfigTests
    {
        [TestMethod()]
        public void invalidFormatDeserializeTest()
        {
            Assert.AreEqual(0, new Config().FromInternal("").Items.Count);
            Assert.AreEqual(0, new Config().FromInternal("invalid").Items.Count);
            Assert.AreEqual(0, new Config().FromInternal(":").Items.Count);
            Assert.AreEqual(0, new Config().FromInternal("1:").Items.Count);
            Assert.AreEqual(0, new Config().FromInternal("invalid:").Items.Count);
        }

        [TestMethod()]
        public void invalidDeviceDeserializeTest()
        {
            Assert.AreEqual(0, new Config().FromInternal("0.0.Device:").Items.Count);
            Assert.AreEqual(0, new Config().FromInternal("99.0.Device:").Items.Count);
        }

        [TestMethod()]
        public void buttonDeserializeTest()
        {
            Config o = new Config();

            Assert.AreEqual(1, o.FromInternal("1.0.Device:").Items.Count);

            Button expected = new Button();
            expected.Name = "Device";
            expected.Pin = "0";

            Assert.AreEqual(expected, o.Items.ElementAt(0));
        }

        [TestMethod()]
        public void singleDetentEncoderDeserializeTest()
        {
            Config o = new Config();

            Assert.AreEqual(1, o.FromInternal("2.0.1.Device:").Items.Count);

            Encoder expected = new Encoder();
            expected.Name = "Device";
            expected.PinLeft = "0";
            expected.PinRight = "1";

            Assert.AreEqual(expected, o.Items.ElementAt(0));
        }

        [TestMethod()]
        public void outputDeserializeTest()
        {
            Config o = new Config();

            Assert.AreEqual(1, o.FromInternal("3.0.Device:").Items.Count);

            Output expected = new Output();
            expected.Name = "Device";
            expected.Pin = "0";

            Assert.AreEqual(expected, o.Items.ElementAt(0));
        }

        [TestMethod()]
        public void ledModuleDeserializeTest()
        {
            Config o = new Config();

            Assert.AreEqual(1, o.FromInternal("16.0.0.1.2.3.4.Device:").Items.Count);

            LedModule expected = new LedModule();
            expected.Name = "Device";
            expected.DinPin = "0";
            expected.ClsPin = "1";
            expected.ClkPin = "2";
            expected.Brightness = 3;
            expected.NumModules = "4";

            Assert.AreEqual(expected, o.Items.ElementAt(0));
        }

        [TestMethod()]
        public void stepperDeprecatedDeserializeTest()
        {
            Config o = new Config();

            Assert.AreEqual(1, o.FromInternal("5.0.1.2.3.4.Device:").Items.Count);

            StepperDeprecatedV2 deprecated = new StepperDeprecatedV2();
            Stepper expected = new Stepper(deprecated);
            expected.Name = "Device";
            expected.Pin1 = "0";
            expected.Pin2 = "1";
            expected.Pin3 = "2";
            expected.Pin4 = "3";
            expected.BtnPin = "0";

            Assert.AreEqual(expected, o.Items.ElementAt(0));
        }

        [TestMethod()]
        public void servoDeserializeTest()
        {
            Config o = new Config();

            Assert.AreEqual(1, o.FromInternal("6.0.Device:").Items.Count);

            Servo expected = new Servo();
            expected.Name = "Device";
            expected.DataPin = "0";

            Assert.AreEqual(expected, o.Items.ElementAt(0));
        }

        [TestMethod()]
        public void lcdDisplayDeserializeTest()
        {
            Config o = new Config();

            Assert.AreEqual(1, o.FromInternal("7.0.1.2.Device:").Items.Count);

            LcdDisplay expected = new LcdDisplay();
            expected.Name = "Device";
            expected.Address = 0;
            expected.Cols = 1;
            expected.Lines = 2;

            Assert.AreEqual(expected, o.Items.ElementAt(0));
        }

        [TestMethod()]
        public void encoderDeserializeTest()
        {
            Config o = new Config();

            Assert.AreEqual(1, o.FromInternal("8.0.1.2.Device:").Items.Count);

            Encoder expected = new Encoder();
            expected.Name = "Device";
            expected.PinLeft = "0";
            expected.PinRight = "1";
            expected.EncoderType = "2";

            Assert.AreEqual(expected, o.Items.ElementAt(0));
        }

        [TestMethod()]
        public void stepperDeserializeTest()
        {
            Config o = new Config();

            Assert.AreEqual(1, o.FromInternal("9.0.1.2.3.4.Device:").Items.Count);

            Stepper expected = new Stepper();
            expected.Name = "Device";
            expected.Pin1 = "0";
            expected.Pin2 = "1";
            expected.Pin3 = "2";
            expected.Pin4 = "3";
            expected.BtnPin = "4";

            Assert.AreEqual(expected, o.Items.ElementAt(0));
        }

        [TestMethod()]
        public void combinedDeserializeTest()
        {
            Config o = new Config();

            Assert.AreEqual(9, o.FromInternal(
                "1.0.Device1:"
                + "2.0.1.Device2:"
                + "3.0.Device3:"
                + "4.0.1.2.3.4.Device4:"
                + "5.0.1.2.3.4.Device5:"
                + "6.0.Device6:"
                + "7.0.1.2.Device7:"
                + "8.0.1.2.Device8:"
                + "9.0.1.2.3.4.Device9:").Items.Count);

            Button expected1 = new Button();
            expected1.Name = "Device1";
            expected1.Pin = "0";

            Encoder expected2 = new Encoder();
            expected2.Name = "Device2";
            expected2.PinLeft = "0";
            expected2.PinRight = "1";

            Output expected3 = new Output();
            expected3.Name = "Device3";
            expected3.Pin = "0";

            LedModule expected4 = new LedModule();
            expected4.ModelType = LedModule.MODEL_TYPE_MAX72xx;
            expected4.Name = "Device4";
            expected4.DinPin = "0";
            expected4.ClsPin = "1";
            expected4.ClkPin = "2";
            expected4.Brightness = 3;
            expected4.NumModules = "4";

            Stepper expected5 = new Stepper();
            expected5.Name = "Device5";
            expected5.Pin1 = "0";
            expected5.Pin2 = "1";
            expected5.Pin3 = "2";
            expected5.Pin4 = "3";
            expected5.BtnPin = "0";

            Servo expected6 = new Servo();
            expected6.Name = "Device6";
            expected6.DataPin = "0";

            LcdDisplay expected7 = new LcdDisplay();
            expected7.Name = "Device7";
            expected7.Address = 0;
            expected7.Cols = 1;
            expected7.Lines = 2;

            Encoder expected8 = new Encoder();
            expected8.Name = "Device8";
            expected8.PinLeft = "0";
            expected8.PinRight = "1";
            expected8.EncoderType = "2";

            Stepper expected9 = new Stepper();
            expected9.Name = "Device9";
            expected9.Pin1 = "0";
            expected9.Pin2 = "1";
            expected9.Pin3 = "2";
            expected9.Pin4 = "3";
            expected9.BtnPin = "4";

            Assert.AreEqual(expected1, o.Items.ElementAt(0));
            Assert.AreEqual(expected2, o.Items.ElementAt(1));
            Assert.AreEqual(expected3, o.Items.ElementAt(2));
            Assert.AreEqual(expected4, o.Items.ElementAt(3));
            Assert.AreEqual(expected5, o.Items.ElementAt(4));
            Assert.AreEqual(expected6, o.Items.ElementAt(5));
            Assert.AreEqual(expected7, o.Items.ElementAt(6));
            Assert.AreEqual(expected8, o.Items.ElementAt(7));
            Assert.AreEqual(expected9, o.Items.ElementAt(8));
        }

        [TestMethod()]
        public void emptySerializeTest()
        {
            Config o = new Config();

            List<String> actual = o.ToInternal(100);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("", actual.ElementAt(0));
        }

        [TestMethod()]
        public void buttonSerializeTest()
        {
            Config o = new Config();

            Button device = new Button();
            device.Name = "Device";
            device.Pin = "0";
            o.Items.Add(device);

            List<String> actual = o.ToInternal(100);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("1.0.Device:", actual.ElementAt(0));
        }

        [TestMethod()]
        public void singleDetentEncoderSerializeTest()
        {
            Config o = new Config();

            Encoder device = new Encoder();
            device.Name = "Device";
            device.PinLeft = "0";
            device.PinRight = "1";
            o.Items.Add(device);

            List<String> actual = o.ToInternal(100);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("8.0.1.0.Device:", actual.ElementAt(0));
        }

        [TestMethod()]
        public void outputSerializeTest()
        {
            Config o = new Config();

            Output device = new Output();
            device.Name = "Device";
            device.Pin = "0";
            o.Items.Add(device);

            List<String> actual = o.ToInternal(100);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("3.0.Device:", actual.ElementAt(0));
        }

        [TestMethod()]
        public void ledModuleSerializeTest()
        {
            Config o = new Config();

            LedModule device = new LedModule();
            device.Name = "Device";
            device.DinPin = "0";
            device.ClsPin = "1";
            device.ClkPin = "2";
            device.Brightness = 3;
            device.NumModules = "4";
            o.Items.Add(device);

            List<String> actual = o.ToInternal(100);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("16.0.0.1.2.3.4.Device:", actual.ElementAt(0));
        }

        [TestMethod()]
        public void stepperDeprecatedSerializeTest()
        {
            Config o = new Config();

            Stepper device = new Stepper();
            device.Name = "Device";
            device.Pin1 = "0";
            device.Pin2 = "1";
            device.Pin3 = "2";
            device.Pin4 = "3";
            device.BtnPin = "0";
            o.Items.Add(device);

            List<String> actual = o.ToInternal(100);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("15.0.1.2.3.0.0.0.0.0.Device:", actual.ElementAt(0));
        }

        [TestMethod()]
        public void servoSerializeTest()
        {
            Config o = new Config();

            Servo device = new Servo();
            device.Name = "Device";
            device.DataPin = "0";
            o.Items.Add(device);

            List<String> actual = o.ToInternal(100);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("6.0.Device:", actual.ElementAt(0));
        }

        [TestMethod()]
        public void lcdDisplaySerializeTest()
        {
            Config o = new Config();

            LcdDisplay device = new LcdDisplay();
            device.Name = "Device";
            device.Address = 0;
            device.Cols = 1;
            device.Lines = 2;
            o.Items.Add(device);

            List<String> actual = o.ToInternal(100);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("7.0.1.2.Device:", actual.ElementAt(0));
        }

        [TestMethod()]
        public void encoderSerializeTest()
        {
            Config o = new Config();

            Encoder device = new Encoder();
            device.Name = "Device";
            device.PinLeft = "0";
            device.PinRight = "1";
            device.EncoderType = "2";
            o.Items.Add(device);

            List<String> actual = o.ToInternal(100);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("8.0.1.2.Device:", actual.ElementAt(0));
        }

        [TestMethod()]
        public void stepperSerializeTest()
        {
            Config o = new Config();

            Stepper device = new Stepper();
            device.Name = "Device";
            device.Pin1 = "0";
            device.Pin2 = "1";
            device.Pin3 = "2";
            device.Pin4 = "3";
            device.BtnPin = "4";
            o.Items.Add(device);

            List<String> actual = o.ToInternal(100);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("15.0.1.2.3.4.0.0.0.0.Device:", actual.ElementAt(0));
        }

        [TestMethod()]
        public void combinedSerializeTest()
        {
            Config o = new Config();

            Button device1 = new Button();
            device1.Name = "Device1";
            device1.Pin = "0";
            o.Items.Add(device1);

            Encoder device2 = new Encoder();
            device2.Name = "Device2";
            device2.PinLeft = "0";
            device2.PinRight = "1";
            o.Items.Add(device2);

            Output device3 = new Output();
            device3.Name = "Device3";
            device3.Pin = "0";
            o.Items.Add(device3);

            LedModule device4 = new LedModule();
            device4.Name = "Device4";
            device4.ModelType = LedModule.MODEL_TYPE_MAX72xx;
            device4.DinPin = "0";
            device4.ClsPin = "1";
            device4.ClkPin = "2";
            device4.Brightness = 3;
            device4.NumModules = "4";
            o.Items.Add(device4);

            Stepper device5 = new Stepper();
            device5.Name = "Device5";
            device5.Pin1 = "0";
            device5.Pin2 = "1";
            device5.Pin3 = "2";
            device5.Pin4 = "3";
            o.Items.Add(device5);

            Servo device6 = new Servo();
            device6.Name = "Device6";
            device6.DataPin = "0";
            o.Items.Add(device6);

            LcdDisplay device7 = new LcdDisplay();
            device7.Name = "Device7";
            device7.Address = 0;
            device7.Cols = 1;
            device7.Lines = 2;
            o.Items.Add(device7);

            Encoder device8 = new Encoder();
            device8.Name = "Device8";
            device8.PinLeft = "0";
            device8.PinRight = "1";
            device8.EncoderType = "2";
            o.Items.Add(device8);

            Stepper device9 = new Stepper();
            device9.Name = "Device9";
            device9.Pin1 = "0";
            device9.Pin2 = "1";
            device9.Pin3 = "2";
            device9.Pin4 = "3";
            device9.BtnPin = "4";
            o.Items.Add(device9);

            List<String> actual = o.ToInternal(1000);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(
                "1.0.Device1:"
                + "8.0.1.0.Device2:"
                + "3.0.Device3:"
                + $"{(int)DeviceType.LedModule}.0.0.1.2.3.4.Device4:"
                + "15.0.1.2.3.0.0.0.0.0.Device5:"
                + "6.0.Device6:"
                + "7.0.1.2.Device7:"
                + "8.0.1.2.Device8:"
                + "15.0.1.2.3.4.0.0.0.0.Device9:", actual.ElementAt(0));
        }

        [TestMethod()]
        public void combinedSmallSerializeTest()
        {
            Config o = new Config();

            Button device1 = new Button();
            device1.Name = "Device1";
            device1.Pin = "0";
            o.Items.Add(device1);

            Encoder device2 = new Encoder();
            device2.Name = "Device2";
            device2.PinLeft = "0";
            device2.PinRight = "1";
            o.Items.Add(device2);

            Output device3 = new Output();
            device3.Name = "Device3";
            device3.Pin = "0";
            o.Items.Add(device3);

            LedModule device4 = new LedModule();
            device4.Name = "Device4";
            device4.DinPin = "0";
            device4.ClsPin = "1";
            device4.ClkPin = "2";
            device4.Brightness = 3;
            device4.NumModules = "4";
            o.Items.Add(device4);

            Stepper device5 = new Stepper();
            device5.Name = "Device5";
            device5.Pin1 = "0";
            device5.Pin2 = "1";
            device5.Pin3 = "2";
            device5.Pin4 = "3";
            o.Items.Add(device5);

            Servo device6 = new Servo();
            device6.Name = "Device6";
            device6.DataPin = "0";
            o.Items.Add(device6);

            LcdDisplay device7 = new LcdDisplay();
            device7.Name = "Device7";
            device7.Address = 0;
            device7.Cols = 1;
            device7.Lines = 2;
            o.Items.Add(device7);

            Encoder device8 = new Encoder();
            device8.Name = "Device8";
            device8.PinLeft = "0";
            device8.PinRight = "1";
            device8.EncoderType = "2";
            o.Items.Add(device8);

            Stepper device9 = new Stepper();
            device9.Name = "Device9";
            device9.Pin1 = "0";
            device9.Pin2 = "1";
            device9.Pin3 = "2";
            device9.Pin4 = "3";
            device9.BtnPin = "4";
            o.Items.Add(device9);

            List<String> actual = o.ToInternal(1);
            Assert.AreEqual(9, actual.Count);
            Assert.AreEqual("1.0.Device1:", actual.ElementAt(0));
            Assert.AreEqual("8.0.1.0.Device2:", actual.ElementAt(1));
            Assert.AreEqual("3.0.Device3:", actual.ElementAt(2));
            Assert.AreEqual($"{(int)DeviceType.LedModule}.0.0.1.2.3.4.Device4:", actual.ElementAt(3));
            Assert.AreEqual("15.0.1.2.3.0.0.0.0.0.Device5:", actual.ElementAt(4));
            Assert.AreEqual("6.0.Device6:", actual.ElementAt(5));
            Assert.AreEqual("7.0.1.2.Device7:", actual.ElementAt(6));
            Assert.AreEqual("8.0.1.2.Device8:", actual.ElementAt(7));
            Assert.AreEqual("15.0.1.2.3.4.0.0.0.0.Device9:", actual.ElementAt(8));
        }
    }
}
