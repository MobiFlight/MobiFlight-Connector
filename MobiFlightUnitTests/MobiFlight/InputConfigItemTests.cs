using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class InputConfigItemTests
    {
        [TestMethod()]
        public void InputConfigItemTest()
        {
            Assert.Fail();
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
            Assert.Fail();
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CloneTest()
        {
            InputConfigItem o = generateTestObject();
            InputConfigItem c = (InputConfigItem) o.Clone();

            Assert.IsNotNull(c.button, "Button is null");
            Assert.IsNotNull(c.encoder, "Encoder is null");
            Assert.AreEqual(o.ModuleSerial, c.ModuleSerial, "Module Serial not the same");
            Assert.AreEqual(o.Name, c.Name, "Name not the same");
            Assert.AreEqual(c.Preconditions.Count, 1, "Precondition Count is not 1");
        }

        private InputConfigItem generateTestObject()
        {
            InputConfigItem result = new InputConfigItem();
            result.button = new InputConfig.ButtonInputConfig();
            result.encoder = new InputConfig.EncoderInputConfig();
            result.ModuleSerial = "TestSerial";
            result.Name = "TestName";
            result.Preconditions.Add(new Precondition() { PreconditionSerial = "PreConTestSerial" });

            return result;
        }
    }
}