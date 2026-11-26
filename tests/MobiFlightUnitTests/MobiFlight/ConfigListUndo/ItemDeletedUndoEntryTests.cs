using MobiFlight.Base;
using MobiFlight.ConfigListUndo;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    [DoNotParallelize]
    public class ItemDeletedUndoEntryTests : UndoEntryTestBase
    {
        [TestMethod]
        public void TryUndo_RestoresDeletedItemAtOriginalIndex()
        {
            // Arrange
            var first = CreateItem("First");
            var deleted = CreateItem("Deleted", false);
            var last = CreateItem("Last");

            TestFile.ConfigItems.AddRange(new IConfigItem[] { first, deleted, last });

            var entry = new ItemDeletedUndoEntry(
                TestProject,
                TestFile,
                0,
                TestFile.ConfigItems,
                new List<IConfigItem> { deleted });

            TestFile.ConfigItems.RemoveAt(1);

            // Change the old object after recording.
            // Undo must use the recorded snapshot.
            deleted.Name = "Modified";

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("Delete", entry.HistoryAction);

            Assert.HasCount(3, TestFile.ConfigItems);

            var restored = TestFile.ConfigItems[1];

            Assert.AreEqual(deleted.GUID, restored.GUID);
            Assert.AreEqual("Deleted", restored.Name);
            Assert.IsFalse(restored.Active);

            Assert.AreNotSame(deleted, restored);

            Assert.AreSame(first, TestFile.ConfigItems[0]);
            Assert.AreSame(last, TestFile.ConfigItems[2]);

            AssertConfigItemsPublished(entry, TestFile, 0);
        }

        [TestMethod]
        public void TryUndo_BulkDelete_RestoresOriginalOrder()
        {
            // Arrange
            var first = CreateItem("First");
            var second = CreateItem("Second");
            var third = CreateItem("Third");
            var fourth = CreateItem("Fourth");

            TestFile.ConfigItems.AddRange(
                new IConfigItem[]{first, second, third, fourth});

            var expectedOrder = TestFile.ConfigItems
                .Select(item => item.GUID)
                .ToArray();

            var entry = new ItemDeletedUndoEntry(
                TestProject,
                TestFile,
                0,
                TestFile.ConfigItems,
                new List<IConfigItem> { fourth, second });

            TestFile.ConfigItems.RemoveAt(3);
            TestFile.ConfigItems.RemoveAt(1);

            // Act
            bool result = entry.TryUndo(
                Context,
                out string reason);

            // Assert
            Assert.IsTrue(result, reason);

            CollectionAssert.AreEqual(
                expectedOrder,
                TestFile.ConfigItems
                    .Select(item => item.GUID)
                    .ToArray());
        }

        [TestMethod]
        public void TryUndo_BulkDeleteWithConflict_ReturnsFalseWithoutPartialRestoration()
        {
            // Arrange
            var first = CreateItem("First");
            var second = CreateItem("Second");

            TestFile.ConfigItems.AddRange(new IConfigItem[] { first, second });

            var entry = new ItemDeletedUndoEntry(
                TestProject,
                TestFile,
                0,
                TestFile.ConfigItems,
                new List<IConfigItem> { first, second });

            TestFile.ConfigItems.Clear();

            var conflicting = CreateItem("Conflict");
            conflicting.GUID = second.GUID;

            TestFile.ConfigItems.Add(conflicting);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(1, TestFile.ConfigItems);

            Assert.AreSame(conflicting, TestFile.ConfigItems[0]);

            Assert.IsFalse(TestFile.ConfigItems.Any(item => item.GUID == first.GUID));
        }
    }
}