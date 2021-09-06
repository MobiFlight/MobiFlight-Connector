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
    public class LuaMacroInputActionTests
    {
        LuaMacroInputAction generateTestObject()
        {
            LuaMacroInputAction o = new LuaMacroInputAction();
            o.MacroName = "Macro Name";
            o.MacroValue = "Macro Value";
            return o;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            LuaMacroInputAction o1 = new LuaMacroInputAction();
            LuaMacroInputAction o2 = new LuaMacroInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }
    }
}