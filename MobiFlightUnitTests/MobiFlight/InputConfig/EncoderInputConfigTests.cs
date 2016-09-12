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
    public class EncoderInputConfigTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            EncoderInputConfig o = generateTestObject();
            EncoderInputConfig c = (EncoderInputConfig) o.Clone();

            Assert.AreNotSame(o, c, "Objects are the same");
            Assert.AreEqual((o.onLeft as FsuipcOffsetInputAction).FSUIPCOffset, (c.onLeft as FsuipcOffsetInputAction).FSUIPCOffset, "onLeft are not cloned correctly");
            Assert.AreEqual((o.onLeftFast as FsuipcOffsetInputAction).FSUIPCOffset, (c.onLeftFast as FsuipcOffsetInputAction).FSUIPCOffset, "onLeftFast are not cloned correctly");
            Assert.AreEqual((o.onRight as FsuipcOffsetInputAction).FSUIPCOffset, (c.onRight as FsuipcOffsetInputAction).FSUIPCOffset, "onRight are not cloned correctly");
            Assert.AreEqual((o.onRightFast as FsuipcOffsetInputAction).FSUIPCOffset, (c.onRightFast as FsuipcOffsetInputAction).FSUIPCOffset, "onRightFast are not cloned correctly");
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            EncoderInputConfig o = generateTestObject();
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

        private EncoderInputConfig generateTestObject()
        {
            EncoderInputConfig result = new EncoderInputConfig();
            result.onLeft = new FsuipcOffsetInputAction() { FSUIPCOffset = 0x1234 };
            result.onRight = new FsuipcOffsetInputAction() { FSUIPCOffset = 0x2345 };
            result.onLeftFast = new FsuipcOffsetInputAction() { FSUIPCOffset = 0x3456 };
            result.onRightFast = new FsuipcOffsetInputAction() { FSUIPCOffset = 0x4567 };

            return result;
        }
    }
}