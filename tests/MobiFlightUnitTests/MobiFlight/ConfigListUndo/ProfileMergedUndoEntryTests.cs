using MobiFlight.Base;
using MobiFlight.ConfigListUndo;
using MobiFlight.Controllers;
using SharpDX;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    [DoNotParallelize]
    public class ProfileMergedUndoEntryTests : UndoEntryTestBase
    {
        [TestMethod]
        public void TryUndo_RestoresOriginalProjectState()
        {
            // Arrange
            var originalItem = CreateItem("Original Item");

            TestFile.ConfigItems.Add(originalItem);

            var secondFile = new ConfigFile
            {
                Label = "Second Profile"
            };

            var secondItem = CreateItem("Second Item", false);

            secondFile.ConfigItems.Add(secondItem);
            TestProject.ConfigFiles.Add(secondFile);

            var originalBinding = new ControllerBinding();

            TestProject.ControllerBindings =
                new List<ControllerBinding>
                {
                    originalBinding
                };

            var mergedFile = new ConfigFile
            {
                Label = "Merged Profile"
            };

            mergedFile.ConfigItems.Add(CreateItem("Merged Item"));

            var entry = new ProfileMergedUndoEntry(
                TestProject,
                new List<ConfigFile> { mergedFile });

            // Simulate merge-related modifications.
            TestProject.ConfigFiles.Add(mergedFile);

            originalItem.Name = "Modified During Merge";
            originalItem.Active = false;

            TestFile.ConfigItems.Add(CreateItem("Unexpected Item"));

            secondFile.ConfigItems.Clear();

            TestProject.ControllerBindings.Clear();

            TestProject.ControllerBindings.Add(new ControllerBinding());

            Context.ActiveConfigIndex = 2;

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("ProfileMerged", entry.HistoryAction);

            Assert.HasCount(2, TestProject.ConfigFiles);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);

            Assert.AreSame(secondFile, TestProject.ConfigFiles[1]);

            // Verify original profile contents.
            Assert.HasCount(1, TestFile.ConfigItems);

            var restoredItem = TestFile.ConfigItems[0];

            Assert.AreEqual(originalItem.GUID, restoredItem.GUID);

            Assert.AreEqual("Original Item", restoredItem.Name);

            Assert.IsTrue(restoredItem.Active);

            Assert.AreNotSame(originalItem, restoredItem);

            // Verify second original profile.
            Assert.HasCount(1, secondFile.ConfigItems);

            var restoredSecondItem =secondFile.ConfigItems[0];

            Assert.AreEqual(secondItem.GUID, restoredSecondItem.GUID);

            Assert.AreEqual("Second Item", restoredSecondItem.Name);

            Assert.IsFalse(restoredSecondItem.Active);

            // Verify original controller bindings.
            Assert.HasCount(1, TestProject.ControllerBindings);

            Assert.AreSame(originalBinding, TestProject.ControllerBindings[0]);

            // Active index must remain valid.
            Assert.AreEqual(1, Context.ActiveConfigIndex);

            AssertProjectPublished(entry);
        }

        [TestMethod]
        public void TryUndo_MergedProfileMissing_ReturnsFalse()
        {
            // Arrange
            var originalItem = CreateItem("Original Item");

            TestFile.ConfigItems.Add(originalItem);

            var mergedFile = new ConfigFile
            {
                Label = "Merged Profile"
            };

            var entry = new ProfileMergedUndoEntry(
                TestProject,
                new List<ConfigFile> { mergedFile });

            TestProject.ConfigFiles.Add(mergedFile);

            originalItem.Name = "Modified Item";

            // Simulate an unexpected modification.
            TestProject.ConfigFiles.RemoveAt(1);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(1, TestProject.ConfigFiles);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);

            Assert.HasCount(1, TestFile.ConfigItems);

            Assert.AreSame(originalItem, TestFile.ConfigItems[0]);

            // Failed Undo must not restore the snapshot.
            Assert.AreEqual("Modified Item", originalItem.Name);
        }

        [TestMethod]
        public void TryUndo_MergedProfileOrderChanged_ReturnsFalse()
        {
            // Arrange
            var firstMergedFile = new ConfigFile
            {
                Label = "First Merged Profile"
            };

            var secondMergedFile = new ConfigFile
            {
                Label = "Second Merged Profile"
            };

            var entry = new ProfileMergedUndoEntry(
                TestProject,
                new List<ConfigFile> { firstMergedFile, secondMergedFile });

            TestProject.ConfigFiles.Add(firstMergedFile);
            TestProject.ConfigFiles.Add(secondMergedFile);

            // Unexpected change to the recorded order.
            TestProject.ConfigFiles.Move(1, 2);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(3, TestProject.ConfigFiles);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);

            Assert.AreSame(secondMergedFile, TestProject.ConfigFiles[1]);

            Assert.AreSame(firstMergedFile, TestProject.ConfigFiles[2]);
        }

        [TestMethod]
        public void RollbackPendingMerge_RestoresIncompleteMerge()
        {
            // Arrange
            var originalItem = CreateItem("Original Item");

            TestFile.ConfigItems.Add(originalItem);

            var originalBinding = new ControllerBinding();

            TestProject.ControllerBindings =
                new List<ControllerBinding>
                {
                    originalBinding
                };

            var firstMergedFile = new ConfigFile
            {
                Label = "First Merged Profile"
            };

            var secondMergedFile = new ConfigFile
            {
                Label = "Second Merged Profile"
            };

            var entry = new ProfileMergedUndoEntry(
                TestProject,
                new List<ConfigFile> { firstMergedFile, secondMergedFile });

            // Simulate a partially completed merge.
            TestProject.ConfigFiles.Add(firstMergedFile);

            originalItem.Name = "Modified During Merge";
            originalItem.Active = false;

            TestProject.ControllerBindings.Add(new ControllerBinding());

            // Normal Undo must reject an incomplete merge.
            Assert.IsFalse(
                entry.TryUndo(Context, out string reason),
                "Normal Undo must reject an incomplete merge.");

            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(2, TestProject.ConfigFiles);

            Assert.AreEqual("Modified During Merge", TestFile.ConfigItems[0].Name);

            // Act
            bool result = entry.RollbackPendingMerge(Context);

            // Assert
            Assert.IsTrue(result);

            Assert.HasCount(1, TestProject.ConfigFiles);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);

            Assert.HasCount(1, TestFile.ConfigItems);

            Assert.AreEqual("Original Item", TestFile.ConfigItems[0].Name);

            Assert.AreEqual(originalItem.GUID, TestFile.ConfigItems[0].GUID);

            Assert.IsTrue(TestFile.ConfigItems[0].Active);

            Assert.HasCount(1, TestProject.ControllerBindings);

            Assert.AreSame(originalBinding, TestProject.ControllerBindings[0]);

            AssertProjectPublished(entry);
        }

        [TestMethod]
        public void RollbackPendingMerge_InvalidContext_ReturnsFalse()
        {
            // Arrange
            var originalItem = CreateItem("Original Item");

            TestFile.ConfigItems.Add(originalItem);

            var mergedFile = new ConfigFile
            {
                Label = "Merged Profile"
            };

            var entry = new ProfileMergedUndoEntry(TestProject, new List<ConfigFile> { mergedFile });

            bool result = entry.RollbackPendingMerge(null);

            // Act & Assert
            Assert.IsFalse(result);

            Assert.HasCount(1, TestProject.ConfigFiles);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);

            Assert.HasCount(1, TestFile.ConfigItems);

            Assert.AreSame(originalItem, TestFile.ConfigItems[0]);
        }
    }
}