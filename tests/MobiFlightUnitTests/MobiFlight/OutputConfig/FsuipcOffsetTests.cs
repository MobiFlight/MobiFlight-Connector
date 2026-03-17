using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.OutputConfig.Tests
{
    [TestClass()]
    public class FsuipcOffsetTests
    {
        [TestMethod()]
        public void FsuipcOffsetTest()
        {
            FsuipcOffset o = new FsuipcOffset();
            Assert.IsNotNull(o, "Object is null");
            Assert.AreEqual(FsuipcOffset.OffsetNull, o.Offset);
            Assert.AreEqual(0xFF, o.Mask);
            Assert.AreEqual(FSUIPCOffsetType.Integer, o.OffsetType);
            Assert.AreEqual(1, o.Size);
            Assert.IsFalse(o.BcdMode);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            FsuipcOffset o1 = new FsuipcOffset();
            FsuipcOffset o2 = new FsuipcOffset();

            Assert.IsTrue(o1.Equals(o2));

            o1.Offset = 0x1234;
            o1.OffsetType = FSUIPCOffsetType.Float;
            o1.Size = 1;
            o1.BcdMode = true;

            Assert.IsFalse(o1.Equals(o2));

            o2.Offset = 0x1234;
            o2.OffsetType = FSUIPCOffsetType.Float;
            o2.Size = 1;
            o2.BcdMode = true;

            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void CloneTest()
        {
            FsuipcOffset o = new FsuipcOffset();
            FsuipcOffset clone = o.Clone() as FsuipcOffset;


            Assert.AreEqual(clone.Offset, o.Offset, "Mask is not the same");
            Assert.AreEqual(clone.OffsetType, o.OffsetType, "OffsetType is not the same");
            Assert.AreEqual(clone.Size, o.Size, "Size is not the same");
            Assert.AreEqual(clone.BcdMode, o.BcdMode, "BcdMode is not the same");
        }
    }
}