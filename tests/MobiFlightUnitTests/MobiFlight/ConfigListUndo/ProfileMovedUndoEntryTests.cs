using MobiFlight.Base;
using MobiFlight.BrowserMessages.Outgoing;
using MobiFlight.ConfigListUndo;
using Moq;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    [DoNotParallelize]
    public class ProfileMovedUndoEntryTests : UndoEntryTestBase
    {
        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(0, 2)]
        [DataRow(2, 0)]

        public void TryUndo_RestoresOriginalOrderAndActiveIndex(int originalIndex, int targetIndex)
        {
            // Arrange
            var secondFile = new ConfigFile
            {
                Label = "Second Profile"
            };

            var thirdFile = new ConfigFile
            {
                Label = "Third Profile"
            };

            TestProject.ConfigFiles.Add(secondFile);
            TestProject.ConfigFiles.Add(thirdFile);

            var movedFile = TestProject.ConfigFiles[originalIndex];

            var targetFile = TestProject.ConfigFiles[targetIndex];

            var entry = new ProfileMovedUndoEntry(
                movedFile,
                originalIndex,
                targetIndex,
                targetFile);

            TestProject.ConfigFiles.Move(originalIndex, targetIndex);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("ProfileMoved", entry.HistoryAction);

            Assert.HasCount(3, TestProject.ConfigFiles);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[0]);

            Assert.AreSame(secondFile, TestProject.ConfigFiles[1]);

            Assert.AreSame(thirdFile, TestProject.ConfigFiles[2]);

            AssertProjectPublished(entry);

            Publisher.Verify(
                publisher => publisher.Publish(
                    It.Is<ProfileMoveUndone>(message =>
                        message.FromIndex == targetIndex &&
                        message.ToIndex == originalIndex)),
                Times.Once());
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(0, 2)]
        [DataRow(2, 0)]
        public void TryUndo_TargetProfileChanged_ReturnsFalse(int originalIndex, int targetIndex)
        {
            // Arrange
            var secondFile = new ConfigFile
            {
                Label = "Second Profile"
            };

            var thirdFile = new ConfigFile{ Label = "Third Profile" };

            TestProject.ConfigFiles.Add(secondFile);
            TestProject.ConfigFiles.Add(thirdFile);

            var movedFile = TestProject.ConfigFiles[originalIndex];

            var targetFile = TestProject.ConfigFiles[targetIndex];

            var entry = new ProfileMovedUndoEntry(
                movedFile,
                originalIndex,
                targetIndex,
                targetFile);

            TestProject.ConfigFiles.Move(originalIndex, targetIndex);

            int shiftedTargetIndex =
                originalIndex < targetIndex
                    ? targetIndex - 1
                    : targetIndex + 1;

            var replacementFile = new ConfigFile
            {
                Label = "Replacement Profile"
            };

            TestProject.ConfigFiles[shiftedTargetIndex] = replacementFile;

            Context.ActiveConfigIndex = targetIndex;

            var profilesBeforeUndo = TestProject.ConfigFiles.ToArray();

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(3, TestProject.ConfigFiles);

            for (int i = 0; i < profilesBeforeUndo.Length; i++)
            {
                Assert.AreSame(profilesBeforeUndo[i], TestProject.ConfigFiles[i]);
            }

            Assert.AreEqual(targetIndex, Context.ActiveConfigIndex);
        }
    }
}
