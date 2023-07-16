using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ExtensionsTests
    {
        [TestMethod()]
        public void ReverseTest()
        {
            byte value = 1;
            Assert.AreEqual(128, value.Reverse(), $"The value was not reversed properly.");

            value = 128;
            Assert.AreEqual(1, value.Reverse(), $"The value was not reversed properly.");

            value = 0b11110000;
            Assert.AreEqual(0b00001111, value.Reverse(), $"The value was not reversed properly.");

            value = 0b10110111;
            Assert.AreEqual(0b11101101, value.Reverse(), $"The value was not reversed properly.");
        }
    }
}