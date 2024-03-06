using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.OutputConfig.Tests
{
    [TestClass()]
    public class LedModuleTests
    {
        [TestMethod()]
        public void LedModuleTest()
        {
            LedModule o = new LedModule();
            Assert.IsNotNull(o);
            Assert.AreEqual(1,o.Connector);
            Assert.AreEqual("0",o.Address);
            Assert.AreEqual(false,o.Padding);
            Assert.AreEqual(false,o.ReverseDigits);
            Assert.AreEqual(string.Empty, o.BrightnessReference);
            Assert.AreEqual("0",o.PaddingChar);
            Assert.AreEqual(8, o.ModuleSize);
            Assert.IsNotNull(o.Digits);
            Assert.IsNotNull(o.DecimalPoints);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            LedModule o1 = new LedModule();
            LedModule o2 = new LedModule();

            Assert.IsTrue(o1.Equals(o2));

            o1.Connector = 2;
            o1.Address = "1";
            o1.Padding = true;
            o1.ReverseDigits = true;
            o1.BrightnessReference = "DisplayLedBrightnessReference";
            o1.PaddingChar = " ";
            o1.ModuleSize = 4;
            o1.Digits = new List<string>() { "1", "2" }; ;
            o1.DecimalPoints = new List<string>() { "3", "4" }; ;

            Assert.IsFalse(o1.Equals(o2));

            o2.Connector = 2;
            o2.Address = "1";
            o2.Padding = true;
            o2.ReverseDigits = true;
            o2.BrightnessReference = "DisplayLedBrightnessReference";
            o2.PaddingChar = " ";
            o2.ModuleSize = 4;
            o2.Digits = new List<string>() { "1", "2" }; ;
            o2.DecimalPoints = new List<string>() { "3", "4" }; ;

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}