using MobiFlight.Base;
using MobiFlight.ConfigListUndo;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    [DoNotParallelize]
    public class ItemToggledUndoEntryTests : UndoEntryTestBase
    {
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TryUndo_RestoresOnlyActiveProperty(bool originalActive)
        {
            // Arrange
            var item = CreateItem("Original", originalActive);

            TestFile.ConfigItems.Add(item);

            var entry = new ItemToggledUndoEntry(
                TestFile,
                0,
                new List<IConfigItem> { item });

            item.Active = !originalActive;

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("Toggle", entry.HistoryAction);

            Assert.HasCount(1, TestFile.ConfigItems);

            var restored = TestFile.ConfigItems[0];

            Assert.AreEqual(originalActive, restored.Active);
            Assert.AreEqual("Original", restored.Name);
            Assert.AreEqual(item.GUID, restored.GUID);

            Assert.AreNotSame(item, restored);

            AssertConfigItemsPublished(entry, TestFile, 0);
        }

        [TestMethod]
        public void TryUndo_BulkToggle_RestoresIndividualActiveValues()
        {
            // Arrange
            var first = CreateItem("First", true);
            var second = CreateItem("Second", false);
            var third = CreateItem("Third", true);

            TestFile.ConfigItems.AddRange(new IConfigItem[] { first, second, third });

            var entry = new ItemToggledUndoEntry(
                TestFile,
                0,
                new List<IConfigItem> { first, second, third });

            first.Active = false;
            second.Active = false;
            third.Active = false;

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.HasCount(3, TestFile.ConfigItems);

            Assert.IsTrue(TestFile.ConfigItems[0].Active);
            Assert.IsFalse(TestFile.ConfigItems[1].Active);
            Assert.IsTrue(TestFile.ConfigItems[2].Active);

            Assert.AreEqual(first.GUID, TestFile.ConfigItems[0].GUID);
            Assert.AreEqual(second.GUID, TestFile.ConfigItems[1].GUID);
            Assert.AreEqual(third.GUID, TestFile.ConfigItems[2].GUID);
        }

        [TestMethod]
        public void TryUndo_MissingItem_ReturnsFalseWithoutPartialRestoration()
        {
            // Arrange
            var first = CreateItem("First", true);
            var second = CreateItem("Second", true);

            TestFile.ConfigItems.AddRange(new IConfigItem[] { first, second });

            var entry = new ItemToggledUndoEntry(
                TestFile,
                0,
                new List<IConfigItem> { first, second });

            first.Active = false;
            second.Active = false;

            TestFile.ConfigItems.RemoveAt(1);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(1, TestFile.ConfigItems);

            // No partial restoration is allowed.
            Assert.IsFalse(TestFile.ConfigItems[0].Active);

            Assert.AreSame(first, TestFile.ConfigItems[0]);
        }
    }
}