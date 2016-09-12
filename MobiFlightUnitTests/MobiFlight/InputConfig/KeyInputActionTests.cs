using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.InputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass()]
    public class KeyInputActionTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            KeyInputAction o = new KeyInputAction();
            o.Alt = true;
            o.Control = true;
            o.Shift = true;
            o.Key = Keys.A;

            KeyInputAction i = (KeyInputAction) o.Clone();
            Assert.AreEqual(o.Shift, i.Shift, "SHIFT value differs");
            Assert.AreEqual(o.Alt, i.Alt, "ALT value differs");
            Assert.AreEqual(o.Control, i.Control, "CONTROL value differs");
            Assert.AreEqual(o.Key, i.Key, "Key value differs");
        }
    }
}