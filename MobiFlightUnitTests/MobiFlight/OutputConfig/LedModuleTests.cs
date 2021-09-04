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
            Assert.AreEqual(1,o.DisplayLedConnector);
            Assert.AreEqual("0",o.DisplayLedAddress);
            Assert.AreEqual(false,o.DisplayLedPadding);
            Assert.AreEqual(false,o.DisplayLedReverseDigits);
            Assert.AreEqual(string.Empty, o.DisplayLedBrightnessReference);
            Assert.AreEqual("0",o.DisplayLedPaddingChar);
            Assert.AreEqual(8, o.DisplayLedModuleSize);
            Assert.IsNotNull(o.DisplayLedDigits);
            Assert.IsNotNull(o.DisplayLedDecimalPoints);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            LedModule o1 = new LedModule();
            LedModule o2 = new LedModule();

            Assert.IsTrue(o1.Equals(o2));

            o1.DisplayLedConnector = 2;
            o1.DisplayLedAddress = "1";
            o1.DisplayLedPadding = true;
            o1.DisplayLedReverseDigits = true;
            o1.DisplayLedBrightnessReference = "DisplayLedBrightnessReference";
            o1.DisplayLedPaddingChar = " ";
            o1.DisplayLedModuleSize = 4;
            o1.DisplayLedDigits = new List<string>() { "1", "2" }; ;
            o1.DisplayLedDecimalPoints = new List<string>() { "3", "4" }; ;

            Assert.IsFalse(o1.Equals(o2));

            o2.DisplayLedConnector = 2;
            o2.DisplayLedAddress = "1";
            o2.DisplayLedPadding = true;
            o2.DisplayLedReverseDigits = true;
            o2.DisplayLedBrightnessReference = "DisplayLedBrightnessReference";
            o2.DisplayLedPaddingChar = " ";
            o2.DisplayLedModuleSize = 4;
            o2.DisplayLedDigits = new List<string>() { "1", "2" }; ;
            o2.DisplayLedDecimalPoints = new List<string>() { "3", "4" }; ;

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}