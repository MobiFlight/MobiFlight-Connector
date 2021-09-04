using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.InputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass()]
    public class VJoyInputActionTests
    {
        [TestMethod()]
        public void EqualsTest()
        {
            VJoyInputAction o1 = new VJoyInputAction();
            VJoyInputAction o2 = new VJoyInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }

        private VJoyInputAction generateTestObject()
        {
            VJoyInputAction o = new VJoyInputAction();
            o.axisString = "Y";
            o.buttonComand = true;
            o.buttonNr = 12;

            return o;
        }
    }
}