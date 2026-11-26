using MobiFlight.ConfigListUndo;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    [DoNotParallelize]
    public class ItemUpdatedUndoEntryTests : UndoEntryTestBase
    {
        [TestMethod]
        public void TryUndo_RestoresOriginalSnapshot()
        {
            // Arrange
            var item = CreateItem("Original", true);

            TestFile.ConfigItems.Add(item);

            var entry = new ItemUpdatedUndoEntry(
                TestProject,
                TestFile,
                0,
                item);

            item.Name = "Updated";
            item.Active = false;

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("ItemEdited", entry.HistoryAction);

            Assert.HasCount(1, TestFile.ConfigItems);

            var restored = TestFile.ConfigItems[0];

            Assert.AreEqual(item.GUID, restored.GUID);
            Assert.AreEqual("Original", restored.Name);
            Assert.IsTrue(restored.Active);

            Assert.AreNotSame(item, restored);

            AssertConfigItemsPublished(entry, TestFile, 0);
        }

        [TestMethod]
        public void HistoryAction_UsesChangeAnnotation()
        {
            // Arrange
            var item = CreateItem("Original");

            var entry = new ItemUpdatedUndoEntry(
                TestProject,
                TestFile,
                0,
                item,
                "rename");

            // Assert
            Assert.AreEqual("rename", entry.HistoryAction);
        }

        [TestMethod]
        public void TryUndo_MissingItem_ReturnsFalse()
        {
            // Arrange
            var item = CreateItem("Original");

            TestFile.ConfigItems.Add(item);

            var entry = new ItemUpdatedUndoEntry(
                TestProject,
                TestFile,
                0,
                item);

            TestFile.ConfigItems.Clear();

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(0, TestFile.ConfigItems);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ShouldMergeWithCreation_MatchingCreation_ReturnsTrue(bool isDuplicate)
        {
            // Arrange
            var item = CreateItem("Created");

            var creation = new ItemCreatedUndoEntry(
                TestProject,
                TestFile,
                0,
                item.GUID,
                isDuplicate);

            // Act
            bool result =
                ItemUpdatedUndoEntry.ShouldMergeWithCreation(
                    TestProject,
                    TestFile,
                    0,
                    item,
                    null,
                    creation);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldMergeWithCreation_WithAnnotation_ReturnsFalse()
        {
            // Arrange
            var item = CreateItem("Created");

            var creation = new ItemCreatedUndoEntry(
                TestProject,
                TestFile,
                0,
                item.GUID,
                false);

            // Act
            bool result =
                ItemUpdatedUndoEntry.ShouldMergeWithCreation(
                    TestProject,
                    TestFile,
                    0,
                    item,
                    "rename",
                    creation);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldMergeWithCreation_DifferentItem_ReturnsFalse()
        {
            // Arrange
            var created = CreateItem("Created");
            var edited = CreateItem("Edited");

            var creation = new ItemCreatedUndoEntry(
                TestProject,
                TestFile,
                0,
                created.GUID,
                false);

            // Act
            bool result =
                ItemUpdatedUndoEntry.ShouldMergeWithCreation(
                    TestProject,
                    TestFile,
                    0,
                    edited,
                    null,
                    creation);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldMergeWithCreation_NoPreviousEntry_ReturnsFalse()
        {
            // Arrange
            var item = CreateItem("Edited");

            // Act
            bool result =
                ItemUpdatedUndoEntry.ShouldMergeWithCreation(
                    TestProject,
                    TestFile,
                    0,
                    item,
                    null,
                    null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldMergeWithCreation_PreviousEntryIsNotCreation_ReturnsFalse()
        {
            // Arrange
            var item = CreateItem("Edited");

            var previousEntry = new ItemUpdatedUndoEntry(
                TestProject,
                TestFile,
                0,
                item);

            // Act
            bool result =
                ItemUpdatedUndoEntry.ShouldMergeWithCreation(
                    TestProject,
                    TestFile,
                    0,
                    item,
                    null,
                    previousEntry);

            // Assert
            Assert.IsFalse(result);
        }
    }
}