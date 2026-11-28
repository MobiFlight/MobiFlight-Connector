using MobiFlight.Base;
using MobiFlight.ConfigListUndo;

namespace MobiFlight.Tests.ConfigListUndo
{
    [TestClass]
    [DoNotParallelize]
    public class ItemResortedUndoEntryTests : UndoEntryTestBase
    {
        [TestMethod]
        public void TryUndo_SameProfile_RestoresOriginalOrder()
        {
            // Arrange
            var first = CreateItem("First");
            var second = CreateItem("Second");
            var third = CreateItem("Third");

            TestFile.ConfigItems.AddRange(new IConfigItem[] { first, second, third });

            var entry = new ItemResortedUndoEntry(
                TestFile,
                0,
                TestFile,
                0,
                TestFile.ConfigItems,
                null);

            TestFile.ConfigItems.RemoveAt(0);
            TestFile.ConfigItems.Add(first);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);
            Assert.IsNull(reason);

            Assert.AreEqual("Resort", entry.HistoryAction);

            Assert.HasCount(3, TestFile.ConfigItems);

            Assert.AreSame(first, TestFile.ConfigItems[0]);
            Assert.AreSame(second, TestFile.ConfigItems[1]);
            Assert.AreSame(third, TestFile.ConfigItems[2]);

            AssertConfigItemsPublished(entry, TestFile, 0);
        }

        [TestMethod]
        public void TryUndo_CrossProfileMove_RestoresBothProfiles()
        {
            // Arrange
            var first = CreateItem("First");
            var moved = CreateItem("Moved");
            var targetItem = CreateItem("Target");

            var targetFile = new ConfigFile
            {
                Label = "Target Profile"
            };

            TestProject.ConfigFiles.Add(targetFile);

            TestFile.ConfigItems.AddRange(new IConfigItem[] { first, moved });

            targetFile.ConfigItems.Add(targetItem);

            var entry = new ItemResortedUndoEntry(
                TestFile,
                0,
                targetFile,
                1,
                TestFile.ConfigItems,
                targetFile.ConfigItems);

            TestFile.ConfigItems.RemoveAt(1);
            targetFile.ConfigItems.Add(moved);

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsTrue(result, reason);

            Assert.HasCount(2, TestFile.ConfigItems);
            Assert.HasCount(1, targetFile.ConfigItems);

            Assert.AreSame(first, TestFile.ConfigItems[0]);
            Assert.AreSame(moved, TestFile.ConfigItems[1]);

            Assert.AreSame(targetItem, targetFile.ConfigItems[0]);
        }

        [TestMethod]
        public void TryUndo_ChangedGuidSet_ReturnsFalseWithoutModifyingItems()
        {
            // Arrange
            var first = CreateItem("First");
            var second = CreateItem("Second");

            TestFile.ConfigItems.AddRange(new IConfigItem[] { first, second });

            var entry = new ItemResortedUndoEntry(
                TestFile,
                0,
                TestFile,
                0,
                TestFile.ConfigItems,
                null);

            TestFile.ConfigItems.Reverse();

            var unexpected = CreateItem("Unexpected");

            TestFile.ConfigItems.Add(unexpected);

            var currentOrder = TestFile.ConfigItems
                .Select(item => item.GUID)
                .ToArray();

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            CollectionAssert.AreEqual(
                currentOrder,
                TestFile.ConfigItems
                    .Select(item => item.GUID)
                    .ToArray());
        }

        [TestMethod]
        public void TryUndo_ChangedTargetProfile_ReturnsFalse()
        {
            // Arrange
            var item = CreateItem("Moved");

            TestFile.ConfigItems.Add(item);

            var targetFile = new ConfigFile
            {
                Label = "Target"
            };

            TestProject.ConfigFiles.Add(targetFile);

            var entry = new ItemResortedUndoEntry(
                TestFile,
                0,
                targetFile,
                1,
                TestFile.ConfigItems,
                targetFile.ConfigItems);

            TestFile.ConfigItems.Clear();
            targetFile.ConfigItems.Add(item);

            var replacementFile = new ConfigFile
            {
                Label = "Replacement"
            };

            TestProject.ConfigFiles[1] = replacementFile;

            // Act
            bool result = entry.TryUndo(Context, out string reason);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reason));

            Assert.HasCount(0, TestFile.ConfigItems);
            Assert.HasCount(0, replacementFile.ConfigItems);

            Assert.HasCount(1, targetFile.ConfigItems);
            Assert.AreSame(item, targetFile.ConfigItems[0]);
        }
    }
}