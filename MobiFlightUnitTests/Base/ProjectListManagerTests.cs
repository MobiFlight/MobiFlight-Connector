using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ProjectListManagerTests
    {
        private StringCollection originalRecentFiles;

        [TestInitialize]
        public void Setup()
        {
            // Save original RecentFiles
            originalRecentFiles = Properties.Settings.Default.RecentFiles;

            // Initialize with clean state
            Properties.Settings.Default.RecentFiles = new StringCollection();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Restore original RecentFiles
            Properties.Settings.Default.RecentFiles = originalRecentFiles;
            Properties.Settings.Default.Save();
        }

        [TestMethod()]
        public void InitializeFromSettings_WithEmptyRecentFiles_ShouldCreateEmptyList()
        {
            // Arrange
            var manager = new ProjectListManager();

            // Act
            manager.InitializeFromSettings();
            var result = manager.GetProjectFiles();

            // Assert
            Assert.IsEmpty(result, "Project list should be empty");
        }

        [TestMethod()]
        public void InitializeFromSettings_WithExistingFiles_ShouldCopyToProjectList()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\project1.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\project2.mfproj");
            var manager = new ProjectListManager();

            // Act
            manager.InitializeFromSettings();
            var result = manager.GetProjectFiles();

            // Assert
            Assert.HasCount(2, result, "Should have 2 projects");
            Assert.AreEqual("C:\\project1.mfproj", result[0]);
            Assert.AreEqual("C:\\project2.mfproj", result[1]);
        }

        [TestMethod()]
        public void OpenProject_WithNewFile_ShouldAddToTopOfBothLists()
        {
            // Arrange
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.OpenProject("C:\\new.mfproj");

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(1, projectFiles, "Should have 1 project in stable list");
            Assert.AreEqual("C:\\new.mfproj", projectFiles[0]);

            Assert.HasCount(1, Properties.Settings.Default.RecentFiles, "Should have 1 in RecentFiles");
            Assert.AreEqual("C:\\new.mfproj", Properties.Settings.Default.RecentFiles[0]);
        }

        [TestMethod()]
        public void OpenProject_WithExistingFileInRecentFiles_ShouldReorderRecentFilesButNotProjectList()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\project1.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\project2.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\project3.mfproj");

            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act - Open project2 which is already in the list
            manager.OpenProject("C:\\project2.mfproj");

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(3, projectFiles, "Should still have 3 projects");
            Assert.AreEqual("C:\\project1.mfproj", projectFiles[0], "Project list order should not change");
            Assert.AreEqual("C:\\project2.mfproj", projectFiles[1]);
            Assert.AreEqual("C:\\project3.mfproj", projectFiles[2]);

            Assert.HasCount(3, Properties.Settings.Default.RecentFiles, "RecentFiles should have 3 items");
            Assert.AreEqual("C:\\project2.mfproj", Properties.Settings.Default.RecentFiles[0], "RecentFiles should reorder to top");
            Assert.AreEqual("C:\\project1.mfproj", Properties.Settings.Default.RecentFiles[1]);
            Assert.AreEqual("C:\\project3.mfproj", Properties.Settings.Default.RecentFiles[2]);
        }

        [TestMethod()]
        public void OpenProject_WithNullOrEmpty_ShouldNotAddToList()
        {
            // Arrange
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.OpenProject(null);
            manager.OpenProject("");
            manager.OpenProject("   ");

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.IsEmpty(projectFiles, "Should not add null or empty files");
            Assert.IsEmpty(Properties.Settings.Default.RecentFiles, "RecentFiles should be empty");
        }

        [TestMethod()]
        public void RemoveProject_WithExistingFile_ShouldRemoveFromBothLists()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\project1.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\project2.mfproj");

            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.RemoveProject("C:\\project1.mfproj");

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(1, projectFiles, "Should have 1 project left");
            Assert.AreEqual("C:\\project2.mfproj", projectFiles[0]);

            Assert.HasCount(1, Properties.Settings.Default.RecentFiles);
            Assert.AreEqual("C:\\project2.mfproj", Properties.Settings.Default.RecentFiles[0]);
        }

        [TestMethod()]
        public void RemoveProject_WithNonExistingFile_ShouldNotThrow()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\project1.mfproj");
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.RemoveProject("C:\\nonexistent.mfproj");

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(1, projectFiles, "Should still have 1 project");
        }

        [TestMethod()]
        public void RemoveProjectByIndex_WithValidIndex_ShouldRemoveFromBothLists()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\project1.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\project2.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\project3.mfproj");

            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.RemoveProjectByIndex(1); // Remove project2

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(2, projectFiles, "Should have 2 projects left");
            Assert.AreEqual("C:\\project1.mfproj", projectFiles[0]);
            Assert.AreEqual("C:\\project3.mfproj", projectFiles[1]);

            Assert.HasCount(2, Properties.Settings.Default.RecentFiles);
            Assert.DoesNotContain("C:\\project2.mfproj", Properties.Settings.Default.RecentFiles);
        }

        [TestMethod()]
        public void RemoveProjectByIndex_WithInvalidIndex_ShouldNotThrow()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\project1.mfproj");
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.RemoveProjectByIndex(-1);
            manager.RemoveProjectByIndex(10);

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(1, projectFiles, "Should still have 1 project");
        }

        [TestMethod()]
        public void RemoveMissingFiles_WithMultipleFiles_ShouldRemoveAllFromBothLists()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\project1.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\project2.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\project3.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\project4.mfproj");

            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            var missingFiles = new List<string>
            {
                "C:\\project2.mfproj",
                "C:\\project4.mfproj"
            };

            // Act
            manager.RemoveMissingFiles(missingFiles);

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(2, projectFiles, "Should have 2 projects left");
            Assert.AreEqual("C:\\project1.mfproj", projectFiles[0]);
            Assert.AreEqual("C:\\project3.mfproj", projectFiles[1]);

            Assert.HasCount(2, Properties.Settings.Default.RecentFiles);
            Assert.AreEqual("C:\\project1.mfproj", Properties.Settings.Default.RecentFiles[0]);
            Assert.AreEqual("C:\\project3.mfproj", Properties.Settings.Default.RecentFiles[1]);
        }

        [TestMethod()]
        public void RemoveMissingFiles_WithNull_ShouldNotThrow()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\project1.mfproj");
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.RemoveMissingFiles(null);

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(1, projectFiles, "Should still have 1 project");
        }

        [TestMethod()]
        public void GetProjectFiles_ShouldReturnCopyOfList()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\project1.mfproj");
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            var result1 = manager.GetProjectFiles();
            result1.Add("C:\\modified.mfproj"); // Modify the returned list
            var result2 = manager.GetProjectFiles();

            // Assert
            Assert.HasCount(1, result2, "Original list should not be modified");
            Assert.AreEqual("C:\\project1.mfproj", result2[0]);
        }

        [TestMethod()]
        public void OpenProject_MultipleNewProjects_ShouldMaintainInsertionOrder()
        {
            // Arrange
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.OpenProject("C:\\first.mfproj");
            manager.OpenProject("C:\\second.mfproj");
            manager.OpenProject("C:\\third.mfproj");

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(3, projectFiles);
            Assert.AreEqual("C:\\third.mfproj", projectFiles[0], "Most recent should be first");
            Assert.AreEqual("C:\\second.mfproj", projectFiles[1]);
            Assert.AreEqual("C:\\first.mfproj", projectFiles[2]);
        }

        [TestMethod()]
        public void OpenProject_SameFileTwice_ShouldOnlyAddOnceToProjectList()
        {
            // Arrange
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.OpenProject("C:\\project.mfproj");
            manager.OpenProject("C:\\project.mfproj");

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(1, projectFiles, "Should only have one entry");
            Assert.AreEqual("C:\\project.mfproj", projectFiles[0]);

            // RecentFiles should also have only one entry (moved to top)
            Assert.HasCount(1, Properties.Settings.Default.RecentFiles);
        }

        [TestMethod()]
        public void ComplexScenario_OpenThenReopen_ShouldMaintainProjectListStability()
        {
            // Arrange - Start with 3 projects
            Properties.Settings.Default.RecentFiles.Add("C:\\A.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\B.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\C.mfproj");

            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act - User clicks on B (middle item) from UI
            manager.OpenProject("C:\\B.mfproj");

            // Assert - Project list order unchanged
            var projectFiles = manager.GetProjectFiles();
            Assert.AreEqual("C:\\A.mfproj", projectFiles[0], "A stays in position");
            Assert.AreEqual("C:\\B.mfproj", projectFiles[1], "B stays in position");
            Assert.AreEqual("C:\\C.mfproj", projectFiles[2], "C stays in position");

            // RecentFiles reordered
            Assert.AreEqual("C:\\B.mfproj", Properties.Settings.Default.RecentFiles[0], "B moved to top in RecentFiles");
            Assert.AreEqual("C:\\A.mfproj", Properties.Settings.Default.RecentFiles[1]);
            Assert.AreEqual("C:\\C.mfproj", Properties.Settings.Default.RecentFiles[2]);
        }

        [TestMethod()]
        public void CheckForMissingFiles_WithNullList_ShouldReturnEmptyList()
        {
            // Act
            var result = ProjectListManager.CheckForMissingFiles(null);

            // Assert
            Assert.IsEmpty(result, "Should return empty list for null input");
        }

        [TestMethod()]
        public void CheckForMissingFiles_WithEmptyStrings_ShouldAddToMissingList()
        {
            // Arrange
            var files = new List<string> { "", "  ", null };

            // Act
            var result = ProjectListManager.CheckForMissingFiles(files);

            // Assert
            Assert.HasCount(3, result, "Should identify empty/whitespace/null as missing");
            Assert.Contains("", result, "Empty string should be missing");
            Assert.Contains("  ", result, "Whitespace should be missing");
            Assert.Contains(null as string, result, "Null should be missing");
        }

        [TestMethod()]
        public void CheckForMissingFiles_WithNonExistentFiles_ShouldAddToMissingList()
        {
            // Arrange
            var files = new List<string>
            {
                "C:\\doesnotexist.mfproj",
                "C:\\alsomissing.mfproj"
            };

            // Act
            var result = ProjectListManager.CheckForMissingFiles(files);

            // Assert
            Assert.HasCount(2, result, "Both non-existent files should be in missing list");
            CollectionAssert.AreEqual(files, result, "Should return all non-existent files");
        }

        [TestMethod()]
        public async Task CleanMissingFilesAsync_WithMissingFiles_ShouldRemoveThemFromBothLists()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\exists1.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\missing1.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\exists2.mfproj");
            Properties.Settings.Default.RecentFiles.Add("C:\\missing2.mfproj");

            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            await manager.CleanMissingFilesAsync().ConfigureAwait(false);

            // Assert - All non-existent files should be removed
            var projectFiles = manager.GetProjectFiles();
            Assert.IsEmpty(projectFiles, "All files should be removed since none exist");
            Assert.IsEmpty(Properties.Settings.Default.RecentFiles, "RecentFiles should be empty");
        }

        [TestMethod()]
        public async Task CleanMissingFilesAsync_WithNoMissingFiles_ShouldNotModifyLists()
        {
            // Arrange
            // Note: We can't easily create actual files in unit tests,
            // so this test verifies that empty list doesn't cause issues
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            await manager.CleanMissingFilesAsync().ConfigureAwait(false);

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.IsEmpty(projectFiles, "List should remain empty");
        }

        [TestMethod()]
        public void OpenProject_AfterInitialization_WithNewFile_ShouldAddToFront()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\existing.mfproj");
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.OpenProject("C:\\new.mfproj");

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(2, projectFiles);
            Assert.AreEqual("C:\\new.mfproj", projectFiles[0], "New file should be at front");
            Assert.AreEqual("C:\\existing.mfproj", projectFiles[1]);
        }

        [TestMethod()]
        public void RemoveMissingFiles_WithEmptyList_ShouldNotModifyAnything()
        {
            // Arrange
            Properties.Settings.Default.RecentFiles.Add("C:\\project1.mfproj");
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act
            manager.RemoveMissingFiles(new List<string>());

            // Assert
            var projectFiles = manager.GetProjectFiles();
            Assert.HasCount(1, projectFiles, "Should still have 1 project");
            Assert.HasCount(1, Properties.Settings.Default.RecentFiles);
        }

        [TestMethod()]
        public void MultipleOperations_ShouldKeepBothListsInSync()
        {
            // Arrange
            var manager = new ProjectListManager();
            manager.InitializeFromSettings();

            // Act - Complex sequence of operations
            manager.OpenProject("C:\\A.mfproj");
            manager.OpenProject("C:\\B.mfproj");
            manager.OpenProject("C:\\C.mfproj");
            manager.RemoveProject("C:\\B.mfproj");
            manager.OpenProject("C:\\D.mfproj");

            // Assert - Both lists should be in sync
            var projectFiles = manager.GetProjectFiles();
            var recentFiles = Properties.Settings.Default.RecentFiles.Cast<string>().ToList();

            Assert.HasCount(3, projectFiles);
            Assert.HasCount(3, recentFiles, "Both lists should have same count");

            // Verify all items exist in both lists
            foreach (var file in projectFiles)
            {
                Assert.Contains(file, recentFiles, $"{file} should exist in both lists");
            }
        }
    }
}