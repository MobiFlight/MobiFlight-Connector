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
    public class ButtonInputConfigTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            ButtonInputConfig o = generateTestObject();
            ButtonInputConfig c = (ButtonInputConfig) o.Clone();

            Assert.AreNotSame(o, c, "Cloned object is the same");
            Assert.AreEqual((o.onPress as EventIdInputAction).EventId, (c.onPress as EventIdInputAction).EventId, "OnPress is not correct");
            Assert.AreEqual((o.onRelease as EventIdInputAction).EventId, (c.onRelease as EventIdInputAction).EventId, "OnRelase is not correct");
        }

        private ButtonInputConfig generateTestObject()
        {
            ButtonInputConfig o = new ButtonInputConfig();
            o.onPress = new EventIdInputAction() { EventId = 12345 };
            o.onRelease = new EventIdInputAction() { EventId = 54321 };
            return o;
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            ButtonInputConfig o = new ButtonInputConfig();
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
    }
}