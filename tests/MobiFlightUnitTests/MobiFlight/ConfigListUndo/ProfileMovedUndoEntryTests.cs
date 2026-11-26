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
        [DataRow(0, 1, 1, 0)]
        [DataRow(0, 1, 0, 1)]
        [DataRow(1, 0, 0, 1)]
        [DataRow(1, 0, 1, 0)]
        [DataRow(0, 1, 2, 2)]
        public void TryUndo_RestoresOriginalOrderAndActiveIndex(
            int originalIndex,
            int targetIndex,
            int activeIndex,
            int expectedActiveIndex)
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
                TestProject,
                movedFile,
                originalIndex,
                targetIndex,
                targetFile);

            TestProject.ConfigFiles.Move(originalIndex, targetIndex);

            Context.ActiveConfigIndex = activeIndex;

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

            Assert.AreEqual(expectedActiveIndex, Context.ActiveConfigIndex);

            AssertProjectPublished(entry);

            Publisher.Verify(
                publisher => publisher.Publish(
                    It.Is<ProfileMoveUndone>(message =>
                        message.FromIndex == targetIndex &&
                        message.ToIndex == originalIndex)),
                Times.Once());
        }

        [TestMethod]
        public void TryUndo_TargetProfileChanged_ReturnsFalse()
        {
            // Arrange
            var secondFile = new ConfigFile
            {
                Label = "Second Profile"
            };

            TestProject.ConfigFiles.Add(secondFile);

            var entry = new ProfileMovedUndoEntry(
                TestProject,
                TestFile,
                0,
                1,
                secondFile);

            TestProject.ConfigFiles.Move(0, 1);

            var replacementFile = new ConfigFile
            {
                Label = "Replacement Profile"
            };

            TestProject.ConfigFiles[0] = replacementFile;

            Context.ActiveConfigIndex = 1;

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(2, TestProject.ConfigFiles);

            Assert.AreSame(replacementFile, TestProject.ConfigFiles[0]);

            Assert.AreSame(TestFile, TestProject.ConfigFiles[1]);

            Assert.AreEqual(1, Context.ActiveConfigIndex);
        }

        [TestMethod]
        [DataRow(0, 1, false)]
        [DataRow(1, 0, false)]
        [DataRow(-1, 0, true)]
        [DataRow(3, 1, true)]
        [DataRow(0, -1, true)]
        [DataRow(0, 3, true)]
        [DataRow(0, 0, true)]
        [DataRow(0, 2, true)]
        public void SkipRecording_ValidatesMoveIndices(
            int originalIndex,
            int targetIndex,
            bool expected)
        {
            // Arrange
            TestProject.ConfigFiles.Add(new ConfigFile
            {
                Label = "Second Profile"
            });

            TestProject.ConfigFiles.Add(new ConfigFile
            {
                Label = "Third Profile"
            });

            var file =
                originalIndex >= 0 &&
                originalIndex < TestProject.ConfigFiles.Count
                    ? TestProject.ConfigFiles[originalIndex]
                    : TestFile;

            // Act
            bool result = ProfileMovedUndoEntry.SkipRecording(
                TestProject,
                file,
                originalIndex,
                targetIndex);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SkipRecording_ProfileMismatch_ReturnsTrue()
        {
            // Arrange
            var secondFile = new ConfigFile
            {
                Label = "Second Profile"
            };

            TestProject.ConfigFiles.Add(secondFile);

            // Act
            bool result = ProfileMovedUndoEntry.SkipRecording(
                TestProject,
                secondFile,
                0,
                1);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SkipRecording_MissingProjectOrFile_ReturnsTrue()
        {
            // Arrange
            TestProject.ConfigFiles.Add(new ConfigFile
            {
                Label = "Second Profile"
            });

            // Act & Assert
            Assert.IsTrue(
                ProfileMovedUndoEntry.SkipRecording(
                    null,
                    TestFile,
                    0,
                    1));

            Assert.IsTrue(
                ProfileMovedUndoEntry.SkipRecording(
                    TestProject,
                    null,
                    0,
                    1));
        }
    }
}