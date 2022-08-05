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
        public void DisplayRequiresUpdateTest()
        {
            LedModuleState state = new LedModuleState();
            Assert.IsTrue(state.DisplayRequiresUpdate("12345678", 0, 255));
            Assert.IsFalse(state.DisplayRequiresUpdate("12345678", 0, 255));
            Assert.IsTrue(state.DisplayRequiresUpdate("87654321", 0, 255));
            Assert.IsTrue(state.DisplayRequiresUpdate("11111111", 0, 255));
            Assert.IsTrue(state.DisplayRequiresUpdate("2", 0, 2));
            Assert.IsFalse(state.DisplayRequiresUpdate("2", 0, 2));
            Assert.IsTrue(state.DisplayRequiresUpdate("12121212", 0, 255));
            Assert.IsFalse(state.DisplayRequiresUpdate("2222", 0, 2 ^ 1 + 2 ^ 3 + 2 ^ 5 + 2 ^ 7));
            Assert.IsTrue(state.DisplayRequiresUpdate("3333", 0, 2 ^ 1 + 2 ^ 3 + 2 ^ 5 + 2 ^ 7));

            // check the decimal points
            Assert.IsTrue(state.DisplayRequiresUpdate("12121212", 1, 255));
            Assert.IsFalse(state.DisplayRequiresUpdate("12121212", 1, 255));
            Assert.IsTrue(state.DisplayRequiresUpdate("12121212", 2 ^ 1 + 2 ^ 3 + 2 ^ 5 + 2 ^ 7, 255));
            Assert.IsFalse(state.DisplayRequiresUpdate("12121212", 2 ^ 1 + 2 ^ 3 + 2 ^ 5 + 2 ^ 7, 255));
            Assert.IsTrue(state.DisplayRequiresUpdate("12121212", 2 ^ 1 + 2 ^ 3 + 2 ^ 5, 255));
        }

        [TestMethod()]
        public void SetBrightnessRequiresUpdateTest()
        {
            LedModuleState state = new LedModuleState();
            Assert.IsTrue(state.SetBrightnessRequiresUpdate("15"));
            Assert.IsFalse(state.SetBrightnessRequiresUpdate("15"));
            Assert.IsTrue(state.SetBrightnessRequiresUpdate("0"));
            Assert.IsFalse(state.SetBrightnessRequiresUpdate("0"));
        }
    }
}