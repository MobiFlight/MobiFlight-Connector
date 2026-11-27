using MobiFlight.Base;
using MobiFlight.ConfigListUndo;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    [DoNotParallelize]
    public class ProfileAddedUndoEntryTests : UndoEntryTestBase
    {
        [TestMethod]
        public void TryUndo_RemovesAddedProfile()
        {
            // Arrange
            var addedFile = new ConfigFile
            {
                Label = "Added Profile"
            };

            TestProject.ConfigFiles.Add(addedFile);
            Context.ActiveConfigIndex = 1;

            var entry = new ProfileAddedUndoEntry(TestProject, addedFile);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("ProfileAdded", entry.HistoryAction);

            Assert.HasCount(1, TestProject.ConfigFiles);
            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);

            Assert.AreEqual(0, Context.ActiveConfigIndex);

            AssertProjectPublished(entry);
        }

        [TestMethod]
        public void TryUndo_RemovesAddedProfileContainingItems()
        {
            // Arrange
            var originalItem = CreateItem("Original Item");
            TestFile.ConfigItems.Add(originalItem);

            var addedFile = new ConfigFile { Label = "Added Profile" };

            TestProject.ConfigFiles.Add(addedFile);

            var entry = new ProfileAddedUndoEntry(TestProject, addedFile);

            var item = CreateItem("New Item");
            addedFile.ConfigItems.Add(item);

            Context.ActiveConfigIndex = 1;

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("ProfileAdded", entry.HistoryAction);

            Assert.HasCount(1, TestProject.ConfigFiles);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);

            Assert.IsFalse(TestProject.ConfigFiles.Contains(addedFile));

            Assert.HasCount(1, TestFile.ConfigItems);
            Assert.AreSame(originalItem, TestFile.ConfigItems[0]);

            Assert.AreEqual(0, Context.ActiveConfigIndex);

            AssertProjectPublished(entry);
        }

        [TestMethod]
        public void TryUndo_MissingProfile_ReturnsFalse()
        {
            // Arrange
            var addedFile = new ConfigFile
            {
                Label = "Added Profile"
            };

            TestProject.ConfigFiles.Add(addedFile);

            var entry = new ProfileAddedUndoEntry(TestProject, addedFile);

            TestProject.ConfigFiles.RemoveAt(1);

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