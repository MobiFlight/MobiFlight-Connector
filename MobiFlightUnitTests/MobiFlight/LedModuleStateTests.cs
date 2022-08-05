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
    public class LedModuleStateTests
    {
        [TestMethod()]
        public void SetRequiresUpdateTest()
        {
            LedModuleState state = new LedModuleState();
            Assert.IsTrue(state.SetRequiresUpdate("12345678", 0, 255));
            Assert.IsFalse(state.SetRequiresUpdate("12345678", 0, 255));
            Assert.IsTrue(state.SetRequiresUpdate("87654321", 0, 255));
            Assert.IsTrue(state.SetRequiresUpdate("11111111", 0, 255));
            Assert.IsTrue(state.SetRequiresUpdate("2", 0, 2));
            Assert.IsFalse(state.SetRequiresUpdate("2", 0, 2));
            Assert.IsTrue(state.SetRequiresUpdate("12121212", 0, 255));
            Assert.IsFalse(state.SetRequiresUpdate("2222", 0, 2^1+2^3+2^5+2^7));
            Assert.IsTrue(state.SetRequiresUpdate("3333", 0, 2 ^ 1 + 2 ^ 3 + 2 ^ 5 + 2 ^ 7));

            // check the decimal points
            Assert.IsTrue(state.SetRequiresUpdate("12121212", 1, 255));
            Assert.IsFalse(state.SetRequiresUpdate("12121212", 1, 255));
            Assert.IsTrue(state.SetRequiresUpdate("12121212", 2 ^ 1 + 2 ^ 3 + 2 ^ 5 + 2 ^ 7, 255));
            Assert.IsFalse (state.SetRequiresUpdate("12121212", 2 ^ 1 + 2 ^ 3 + 2 ^ 5 + 2 ^ 7, 255));
        }
    }
}