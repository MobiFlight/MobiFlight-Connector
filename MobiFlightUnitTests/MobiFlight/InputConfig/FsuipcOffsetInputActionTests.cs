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
    public class FsuipcOffsetInputActionTests
    {
        [TestMethod()]
        public void FsuipcOffsetInputActionTest()
        {
            FsuipcOffsetInputAction o = new FsuipcOffsetInputAction();
            Assert.AreEqual(o.FSUIPCOffset, FsuipcOffsetInputAction.FSUIPCOffsetNull, "FSUIPCOffset is not FSUIPCOffsetNull");
            Assert.AreEqual(o.FSUIPCMask,0xFF,"FSUIPCMask is not 0xFF");
            Assert.AreEqual(o.FSUIPCOffsetType,FSUIPCOffsetType.Integer,"FSUIPCOffsetType not correct");
            Assert.AreEqual(o.FSUIPCSize,1,"FSUIPCSize not correct");
            Assert.AreEqual(o.FSUIPCBcdMode,false,"Not correct");
            Assert.AreEqual(o.Value,"","Value not correct");
        }

        [TestMethod()]
        public void CloneTest()
        {
            FsuipcOffsetInputAction o = generateTestObject();
            FsuipcOffsetInputAction c = (FsuipcOffsetInputAction) o.Clone();

            Assert.AreNotSame(o, c, "Objects are the same");
            Assert.AreEqual(o.FSUIPCBcdMode, c.FSUIPCBcdMode, "FSUIPCBcdMode are not the same");
            Assert.AreEqual(o.FSUIPCMask, c.FSUIPCMask, "FSUIPCMask are not the same");
            Assert.AreEqual(o.FSUIPCOffset, c.FSUIPCOffset, "FSUIPCOffset are not the same");
            Assert.AreEqual(o.FSUIPCOffsetType, c.FSUIPCOffsetType, "FSUIPCOffsetType are not the same");
            Assert.AreEqual(o.FSUIPCSize, c.FSUIPCSize, "FSUIPCSize are not the same");
        }

        private FsuipcOffsetInputAction generateTestObject()
        {
            FsuipcOffsetInputAction o = new FsuipcOffsetInputAction();
            o.FSUIPCBcdMode = true;
            o.FSUIPCMask = 0xFFFF;
            o.FSUIPCOffset = 0x1234;
            o.FSUIPCOffsetType = FSUIPCOffsetType.Float;
            o.FSUIPCSize = 2;
            return o;
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void executeTest()
        {
            FsuipcOffsetInputAction o = generateTestObject();
            MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock mock = new MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock();
            o.FSUIPCOffsetType = FSUIPCOffsetType.Integer;
            o.FSUIPCBcdMode = false;
            o.Value = "12";
            o.execute(mock);
            Assert.AreEqual(mock.Writes.Count, 1, "The message count is not as expected");
            Assert.AreEqual(mock.Writes[0].Offset, 0x1234, "The Offset is wrong");
            Assert.AreEqual(mock.Writes[0].Value, "12", "The Param Value is wrong");
        }
    }
}