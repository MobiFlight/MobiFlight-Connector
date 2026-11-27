using MobiFlight.Base;
using MobiFlight.ConfigListUndo;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    [DoNotParallelize]
    public class ProfileDeletedUndoEntryTests : UndoEntryTestBase
    {
        [TestMethod]
        public void TryUndo_RestoresDeletedProfileAtOriginalIndex()
        {
            // Arrange
            var deletedFile = new ConfigFile
            {
                Label = "Deleted Profile"
            };

            var lastFile = new ConfigFile
            {
                Label = "Last Profile"
            };

            var item = CreateItem("Original Item", false);

            deletedFile.ConfigItems.Add(item);

            TestProject.ConfigFiles.Add(deletedFile);
            TestProject.ConfigFiles.Add(lastFile);

            var entry = new ProfileDeletedUndoEntry(deletedFile, 1);

            TestProject.ConfigFiles.RemoveAt(1);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("ProfileDeleted", entry.HistoryAction);

            Assert.HasCount(3, TestProject.ConfigFiles);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);

            Assert.AreSame(deletedFile, TestProject.ConfigFiles[1]);

            Assert.AreSame(lastFile, TestProject.ConfigFiles[2]);

            Assert.HasCount(1, deletedFile.ConfigItems);
            Assert.AreSame(item, deletedFile.ConfigItems[0]);

            Assert.AreEqual("Original Item", item.Name);
            Assert.IsFalse(item.Active);

            AssertProjectPublished(entry);
        }

        [TestMethod]
        public void TryUndo_RestoresDeletedLastProfile()
        {
            // Arrange
            var deletedFile = new ConfigFile
            {
                Label = "Last Profile"
            };

            TestProject.ConfigFiles.Add(deletedFile);

            var entry = new ProfileDeletedUndoEntry(deletedFile, 1);

            TestProject.ConfigFiles.RemoveAt(1);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);

            Assert.HasCount(2, TestProject.ConfigFiles);

            Assert.AreSame(deletedFile, TestProject.ConfigFiles[1]);
        }

        [TestMethod]
        public void TryUndo_ProfileAlreadyExists_ReturnsFalse()
        {
            // Arrange
            var deletedFile = new ConfigFile
            {
                Label = "Deleted Profile"
            };

            TestProject.ConfigFiles.Add(deletedFile);

            var entry = new ProfileDeletedUndoEntry(deletedFile, 1);

            // Simulate the profile already being restored.

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(2, TestProject.ConfigFiles);

            Assert.AreSame(deletedFile, TestProject.ConfigFiles[1]);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(2)]
        public void TryUndo_InvalidOriginalIndex_ReturnsFalse(
            int originalIndex)
        {
            // Arrange
            var deletedFile = new ConfigFile
            {
                Label = "Deleted Profile"
            };

            var entry = new ProfileDeletedUndoEntry(
                deletedFile,
                originalIndex);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(1, TestProject.ConfigFiles);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);
        }
    }
}