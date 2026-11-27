using MobiFlight.Base;
using MobiFlight.ConfigListUndo;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    [DoNotParallelize]
    public class ItemCreatedUndoEntryTests : UndoEntryTestBase
    {
        [TestMethod]
        [DataRow(false, "Add")]
        [DataRow(true, "Duplicate")]
        public void TryUndo_RemovesOnlyCreatedItem(bool isDuplicate, string expectedAction)
        {
            // Arrange
            var original = CreateItem("Original");
            var created = CreateItem("Created");

            TestFile.ConfigItems.Add(original);
            TestFile.ConfigItems.Add(created);

            var entry = new ItemCreatedUndoEntry(
                TestProject,
                TestFile,
                0,
                created.GUID,
                isDuplicate);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual(expectedAction, entry.HistoryAction);

            Assert.HasCount(1, TestFile.ConfigItems);

            Assert.AreSame(original, TestFile.ConfigItems[0]);

            AssertConfigItemsPublished(entry, TestFile, 0);
        }

        [TestMethod]
        public void TryUndo_MissingItem_ReturnsFalse()
        {
            // Arrange
            var original = CreateItem("Original");
            var created = CreateItem("Created");

            TestFile.ConfigItems.Add(original);

            var entry = new ItemCreatedUndoEntry(
                TestProject,
                TestFile,
                0,
                created.GUID,
                false);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(1, TestFile.ConfigItems);

            Assert.AreSame(original, TestFile.ConfigItems[0]);
        }

        [TestMethod]
        public void TryUndo_DifferentProject_ReturnsFalse()
        {
            // Arrange
            var item = CreateItem("Created");

            TestFile.ConfigItems.Add(item);

            var differentProject = new Project();

            var entry = new ItemCreatedUndoEntry(
                differentProject,
                TestFile,
                0,
                item.GUID,
                false);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);

            Assert.AreEqual("Project mismatch.", reason);

            Assert.HasCount(1, TestFile.ConfigItems);

            Assert.AreSame(item, TestFile.ConfigItems[0]);
        }

        [TestMethod]
        public void MatchesCreation_ValidatesRecordedIdentity()
        {
            // Arrange
            var item = CreateItem("Created");

            var entry = new ItemCreatedUndoEntry(
                TestProject,
                TestFile,
                0,
                item.GUID,
                false);

            // Assert
            Assert.IsTrue(entry.IsCreatedItem(TestProject, TestFile, 0, item.GUID));

            Assert.IsFalse(entry.IsCreatedItem(new Project(), TestFile, 0, item.GUID));

            Assert.IsFalse(entry.IsCreatedItem(TestProject, new ConfigFile(), 0, item.GUID));

            Assert.IsFalse(entry.IsCreatedItem(TestProject, TestFile, 1, item.GUID));

            Assert.IsFalse(entry.IsCreatedItem(TestProject, TestFile, 0, "different-guid"));
        }
    }
}