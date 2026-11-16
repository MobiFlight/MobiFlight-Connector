using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class DeviceReferenceTests
    {
        [TestMethod()]
        public void SplitNames_ReturnsEveryName()
        {
            CollectionAssert.AreEqual(new List<string> { "A01", "C05" }, DeviceReference.SplitNames("A01|C05"));
        }

        [TestMethod()]
        public void SplitNames_ReturnsSingleName_WhenThereIsNoSeparator()
        {
            CollectionAssert.AreEqual(new List<string> { "LED 1" }, DeviceReference.SplitNames("LED 1"));
        }

        [TestMethod()]
        public void SplitNames_TrimsNamesAndDropsEmptyEntries()
        {
            CollectionAssert.AreEqual(new List<string> { "A01", "C05" }, DeviceReference.SplitNames(" A01 || C05|"));
        }

        [TestMethod()]
        [DataRow(null)]
        [DataRow("")]
        public void SplitNames_ReturnsEmptyList_WhenThereAreNoNames(string names)
        {
            Assert.IsEmpty(DeviceReference.SplitNames(names));
        }
    }
}
