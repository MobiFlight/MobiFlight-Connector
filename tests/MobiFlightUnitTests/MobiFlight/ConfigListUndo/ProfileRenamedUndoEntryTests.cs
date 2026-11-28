using MobiFlight.Base;
using MobiFlight.ConfigListUndo;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    [DoNotParallelize]
    public class ProfileRenamedUndoEntryTests : UndoEntryTestBase
    {
        [TestMethod]
        public void TryUndo_RestoresOriginalLabel()
        {
            // Arrange
            TestFile.Label = "Original Profile";

            var item = CreateItem("Original Item");
            TestFile.ConfigItems.Add(item);

            var entry = new ProfileRenamedUndoEntry(TestFile, 0);

            TestFile.Label = "Renamed Profile";
            item.Name = "Changed Item";

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("ProfileRename", entry.HistoryAction);

            Assert.AreEqual("Original Profile", TestFile.Label);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);

            Assert.HasCount(1, TestFile.ConfigItems);
            Assert.AreSame(item, TestFile.ConfigItems[0]);

            // Only the profile label should be restored.
            Assert.AreEqual("Changed Item", item.Name);

            AssertProjectPublished(entry);
        }

        [TestMethod]
        public void TryUndo_ProfileRemoved_ReturnsFalse()
        {
            // Arrange
            TestFile.Label = "Original Profile";

            var entry = new ProfileRenamedUndoEntry(TestFile, 0);

            TestFile.Label = "Renamed Profile";

            TestProject.ConfigFiles.RemoveAt(0);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(0, TestProject.ConfigFiles);

            Assert.AreEqual("Renamed Profile", TestFile.Label);
        }

        [TestMethod]
        public void TryUndo_ProfileIndexChanged_ReturnsFalse()
        {
            // Arrange
            TestFile.Label = "Original Profile";

            var otherFile = new ConfigFile
            {
                Label = "Other Profile"
            };

            TestProject.ConfigFiles.Add(otherFile);

            var entry = new ProfileRenamedUndoEntry(
                TestFile,
                0);

            TestFile.Label = "Renamed Profile";

            TestProject.ConfigFiles.Move(0, 1);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.AreSame(otherFile, TestProject.ConfigFiles[0]);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[1]);

            Assert.AreEqual("Renamed Profile", TestFile.Label);

            Assert.AreEqual("Other Profile", otherFile.Label);
        }
    }
}