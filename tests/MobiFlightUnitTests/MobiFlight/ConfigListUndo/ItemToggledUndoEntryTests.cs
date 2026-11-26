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
                TestProject,
                TestFile,
                0,
                new List<IConfigItem> { item });

            item.Active = !originalActive;
            item.Name = "Changed Name";

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("Toggle", entry.HistoryAction);

            Assert.AreEqual(originalActive, item.Active);

            // Other properties must not be restored.
            Assert.AreEqual("Changed Name", item.Name);

            // The original object must remain in the list.
            Assert.AreSame(item, TestFile.ConfigItems[0]);

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
                TestProject,
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

            Assert.IsTrue(first.Active);
            Assert.IsFalse(second.Active);
            Assert.IsTrue(third.Active);
        }

        [TestMethod]
        public void TryUndo_MissingItem_ReturnsFalseWithoutPartialRestoration()
        {
            // Arrange
            var first = CreateItem("First", true);
            var second = CreateItem("Second", true);

            TestFile.ConfigItems.AddRange(new IConfigItem[] { first, second });

            var entry = new ItemToggledUndoEntry(
                TestProject,
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
            Assert.IsFalse(first.Active);

            Assert.AreSame(first, TestFile.ConfigItems[0]);
        }
    }
}