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
    public class MSFS2020CustomInputActionTests
    {
        MSFS2020CustomInputAction generateTestObject()
        {
            MSFS2020CustomInputAction o = new MSFS2020CustomInputAction();
            o.Command = "(>K:TOGGLE_LIGHTS)";
            return o;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            MSFS2020CustomInputAction o1 = new MSFS2020CustomInputAction();
            MSFS2020CustomInputAction o2 = new MSFS2020CustomInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}