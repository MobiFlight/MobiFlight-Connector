using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ProjectListManagerTests
    {
        private StringCollection originalRecentFiles;
        private Mock<ControllerBindingService> mockBindingService;
        private string _tempDirectory;

        [TestInitialize]
        public void Setup()
        {
            // Save original RecentFiles
            originalRecentFiles = Properties.Settings.Default.RecentFiles;

            mockBindingService = new Mock<ControllerBindingService>(null);
            mockBindingService.Setup(x => x.PerformAutoBinding(It.IsAny<Project>())).Returns(new List<ControllerBinding>());

            // Initialize with clean state
            Properties.Settings.Default.RecentFiles = new StringCollection();

            // Create a temporary test directory
            _tempDirectory = Path.Combine(Path.GetTempPath(), "ProjectMigrationTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Restore original RecentFiles
            Properties.Settings.Default.RecentFiles = originalRecentFiles;
            Properties.Settings.Default.Save();

            try
            {
                // Clean up test directory
                if (Directory.Exists(_tempDirectory))
                {
                    Directory.Delete(_tempDirectory, true);
                }
            }
            catch { }
        }

        /// <summary>
        /// Helper method to create a temporary test file
        /// </summary>
        private string CreateTestFile(string fileName)
        {
            var filePath = Path.Combine(_tempDirectory, fileName);
            var testProject = new Project() { FilePath = filePath };
            testProject.SaveFile();
            return filePath;
        }

        [TestMethod()]
        public async Task InitializeFromSettings_WithEmptyRecentFiles_ShouldCreateEmptyList()
        {
            // Arrange
            var manager = new ProjectListManager();

            // Act
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);
            var result = manager.GetProjects();

            // Assert
            Assert.IsEmpty(result, "Project list should be empty");
        }

        [TestMethod()]
        public async Task InitializeFromSettings_WithExistingFiles_ShouldCopyToProjectList()
        {
            // Arrange
            var project1 = CreateTestFile("project1.mfproj");
            var project2 = CreateTestFile("project2.mfproj");
            Properties.Settings.Default.RecentFiles.Add(project1);
            Properties.Settings.Default.RecentFiles.Add(project2);
            var manager = new ProjectListManager();

            // Act
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);
            var result = manager.GetProjects();

            // Assert
            Assert.HasCount(2, result, "Should have 2 projects");
            Assert.AreEqual(project1, result[0].FilePath);
            Assert.AreEqual(project2, result[1].FilePath);
        }

        [TestMethod()]
        public async Task OpenProject_WithNewFile_ShouldAddToTopOfBothLists()
        {
            // Arrange
            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);
            var newFile = CreateTestFile("new.mfproj");

            // Act
            manager.OpenProject(new ProjectInfo() { FilePath = newFile });

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(1, projectFiles, "Should have 1 project in stable list");
            Assert.AreEqual(newFile, projectFiles[0].FilePath);

            Assert.HasCount(1, Properties.Settings.Default.RecentFiles, "Should have 1 in RecentFiles");
            Assert.AreEqual(newFile, Properties.Settings.Default.RecentFiles[0]);
        }

        [TestMethod()]
        public async Task OpenProject_WithExistingFileInRecentFiles_ShouldReorderRecentFilesButNotProjectList()
        {
            // Arrange
            var project1 = CreateTestFile("project1.mfproj");
            var project2 = CreateTestFile("project2.mfproj");
            var project3 = CreateTestFile("project3.mfproj");

            Properties.Settings.Default.RecentFiles.Add(project1);
            Properties.Settings.Default.RecentFiles.Add(project2);
            Properties.Settings.Default.RecentFiles.Add(project3);

            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act - Open project2 which is already in the list
            manager.OpenProject(new ProjectInfo() { FilePath = project2 });

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(3, projectFiles, "Should still have 3 projects");
            Assert.AreEqual(project1, projectFiles[0].FilePath, "Project list order should not change");
            Assert.AreEqual(project2, projectFiles[1].FilePath);
            Assert.AreEqual(project3, projectFiles[2].FilePath);

            Assert.HasCount(3, Properties.Settings.Default.RecentFiles, "RecentFiles should have 3 items");
            Assert.AreEqual(project2, Properties.Settings.Default.RecentFiles[0], "RecentFiles should reorder to top");
            Assert.AreEqual(project1, Properties.Settings.Default.RecentFiles[1]);
            Assert.AreEqual(project3, Properties.Settings.Default.RecentFiles[2]);
        }

        [TestMethod()]
        public async Task OpenProject_WithNullOrEmpty_ShouldNotAddToList()
        {
            // Arrange
            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            manager.OpenProject(null);
            manager.OpenProject(new ProjectInfo() { FilePath = "" });
            manager.OpenProject(new ProjectInfo() { FilePath = "   " });

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.IsEmpty(projectFiles, "Should not add null or empty files");
            Assert.IsEmpty(Properties.Settings.Default.RecentFiles, "RecentFiles should be empty");
        }

        [TestMethod()]
        public async Task RemoveProject_WithExistingFile_ShouldRemoveFromBothLists()
        {
            // Arrange
            var project1 = CreateTestFile("project1.mfproj");
            var project2 = CreateTestFile("project2.mfproj");

            Properties.Settings.Default.RecentFiles.Add(project1);
            Properties.Settings.Default.RecentFiles.Add(project2);

            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            manager.RemoveProject(project1);

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(1, projectFiles, "Should have 1 project left");
            Assert.AreEqual(project2, projectFiles[0].FilePath);

            Assert.HasCount(1, Properties.Settings.Default.RecentFiles);
            Assert.AreEqual(project2, Properties.Settings.Default.RecentFiles[0]);
        }

        [TestMethod()]
        public async Task RemoveProject_WithNonExistingFile_ShouldNotThrow()
        {
            // Arrange
            var project1 = CreateTestFile("project1.mfproj");
            Properties.Settings.Default.RecentFiles.Add(project1);
            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            manager.RemoveProject("C:\\nonexistent.mfproj");

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(1, projectFiles, "Should still have 1 project");
        }

        [TestMethod()]
        public async Task RemoveProjectByIndex_WithValidIndex_ShouldRemoveFromBothLists()
        {
            // Arrange
            var project1 = CreateTestFile("project1.mfproj");
            var project2 = CreateTestFile("project2.mfproj");
            var project3 = CreateTestFile("project3.mfproj");

            Properties.Settings.Default.RecentFiles.Add(project1);
            Properties.Settings.Default.RecentFiles.Add(project2);
            Properties.Settings.Default.RecentFiles.Add(project3);

            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            manager.RemoveProjectByIndex(1); // Remove project2

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(2, projectFiles, "Should have 2 projects left");
            Assert.AreEqual(project1, projectFiles[0].FilePath);
            Assert.AreEqual(project3, projectFiles[1].FilePath);

            Assert.HasCount(2, Properties.Settings.Default.RecentFiles);
            Assert.DoesNotContain(project2, Properties.Settings.Default.RecentFiles);
        }

        [TestMethod()]
        public async Task RemoveProjectByIndex_WithInvalidIndex_ShouldNotThrow()
        {
            // Arrange
            var project1 = CreateTestFile("project1.mfproj");
            Properties.Settings.Default.RecentFiles.Add(project1);
            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            manager.RemoveProjectByIndex(-1);
            manager.RemoveProjectByIndex(10);

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(1, projectFiles, "Should still have 1 project");
        }

        [TestMethod()]
        public async Task RemoveMissingFiles_WithMultipleFiles_ShouldRemoveAllFromBothLists()
        {
            // Arrange
            var project1 = CreateTestFile("project1.mfproj");
            var project2 = CreateTestFile("project2.mfproj");
            var project3 = CreateTestFile("project3.mfproj");
            var project4 = CreateTestFile("project4.mfproj");

            Properties.Settings.Default.RecentFiles.Add(project1);
            Properties.Settings.Default.RecentFiles.Add(project2);
            Properties.Settings.Default.RecentFiles.Add(project3);
            Properties.Settings.Default.RecentFiles.Add(project4);

            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            var missingFiles = new List<string>
            {
                project2,
                project4
            };

            // Act
            manager.RemoveMissingFiles(missingFiles);

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(2, projectFiles, "Should have 2 projects left");
            Assert.AreEqual(project1, projectFiles[0].FilePath);
            Assert.AreEqual(project3, projectFiles[1].FilePath);

            Assert.HasCount(2, Properties.Settings.Default.RecentFiles);
            Assert.AreEqual(project1, Properties.Settings.Default.RecentFiles[0]);
            Assert.AreEqual(project3, Properties.Settings.Default.RecentFiles[1]);
        }

        [TestMethod()]
        public async Task RemoveMissingFiles_WithNull_ShouldNotThrow()
        {
            // Arrange
            var project1 = CreateTestFile("project1.mfproj");
            Properties.Settings.Default.RecentFiles.Add(project1);
            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            manager.RemoveMissingFiles(null);

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(1, projectFiles, "Should still have 1 project");
        }

        [TestMethod()]
        public async Task GetProjects_ShouldReturnCopyOfList()
        {
            // Arrange
            var project1 = CreateTestFile("project1.mfproj");
            Properties.Settings.Default.RecentFiles.Add(project1);
            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            var result1 = manager.GetProjects();
            result1.Add(new ProjectInfo() { FilePath = "C:\\modified.mfproj" }); // Modify the returned list
            var result2 = manager.GetProjects();

            // Assert
            Assert.HasCount(1, result2, "Original list should not be modified");
            Assert.AreEqual(project1, result2[0].FilePath);
        }

        [TestMethod()]
        public async Task OpenProject_MultipleNewProjects_ShouldMaintainInsertionOrder()
        {
            // Arrange
            var manager = new ProjectListManager();
            var testfiles = new List<string>() {
                CreateTestFile("first.mfproj"),
                CreateTestFile("second.mfproj"),
                CreateTestFile("third.mfproj"),
            };

            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            testfiles.ForEach(file =>
            {
                manager.OpenProject(new ProjectInfo() { FilePath = file });
            });

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(3, projectFiles);
            Assert.AreEqual(testfiles[2], projectFiles[0].FilePath, "Third (most recent) should be first");
            Assert.AreEqual(testfiles[1], projectFiles[1].FilePath);
            Assert.AreEqual(testfiles[0], projectFiles[2].FilePath);
        }

        [TestMethod()]
        public async Task OpenProject_SameFileTwice_ShouldOnlyAddOnceToProjectList()
        {
            // Arrange
            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);
            var projectFile = CreateTestFile("project.mfproj");

            // Act
            manager.OpenProject(new ProjectInfo() { FilePath = projectFile });
            manager.OpenProject(new ProjectInfo() { FilePath = projectFile });

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(1, projectFiles, "Should only have one entry");
            Assert.AreEqual(projectFile, projectFiles[0].FilePath);

            // RecentFiles should also have only one entry (moved to top)
            Assert.HasCount(1, Properties.Settings.Default.RecentFiles);
        }

        [TestMethod()]
        public async Task ComplexScenario_OpenThenReopen_ShouldMaintainProjectListStability()
        {
            // Arrange - Start with 3 projects
            var projectA = CreateTestFile("A.mfproj");
            var projectB = CreateTestFile("B.mfproj");
            var projectC = CreateTestFile("C.mfproj");

            Properties.Settings.Default.RecentFiles.Add(projectA);
            Properties.Settings.Default.RecentFiles.Add(projectB);
            Properties.Settings.Default.RecentFiles.Add(projectC);

            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act - User clicks on B (middle item) from UI
            manager.OpenProject(new ProjectInfo() { FilePath = projectB });

            // Assert - Project list order unchanged
            var projectFiles = manager.GetProjects();
            Assert.AreEqual(projectA, projectFiles[0].FilePath, "A stays in position");
            Assert.AreEqual(projectB, projectFiles[1].FilePath, "B stays in position");
            Assert.AreEqual(projectC, projectFiles[2].FilePath, "C stays in position");

            // RecentFiles reordered
            Assert.AreEqual(projectB, Properties.Settings.Default.RecentFiles[0], "B moved to top in RecentFiles");
            Assert.AreEqual(projectA, Properties.Settings.Default.RecentFiles[1]);
            Assert.AreEqual(projectC, Properties.Settings.Default.RecentFiles[2]);
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
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            await manager.CleanMissingFilesAsync().ConfigureAwait(false);

            // Assert - All non-existent files should be removed
            var projectFiles = manager.GetProjects();
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
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            await manager.CleanMissingFilesAsync().ConfigureAwait(false);

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.IsEmpty(projectFiles, "List should remain empty");
        }

        [TestMethod()]
        public async Task OpenProject_AfterInitialization_WithNewFile_ShouldAddToFront()
        {
            // Arrange
            var existingFile = CreateTestFile("existing.mfproj");
            Properties.Settings.Default.RecentFiles.Add(existingFile);
            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            var newFile = CreateTestFile("new.mfproj");

            // Act
            manager.OpenProject(new ProjectInfo() { FilePath = newFile });

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(2, projectFiles);
            Assert.AreEqual(newFile, projectFiles[0].FilePath, "New file should be at front");
            Assert.AreEqual(existingFile, projectFiles[1].FilePath);
        }

        [TestMethod()]
        public async Task RemoveMissingFiles_WithEmptyList_ShouldNotModifyAnything()
        {
            // Arrange
            var project1 = CreateTestFile("project1.mfproj");
            Properties.Settings.Default.RecentFiles.Add(project1);
            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            // Act
            manager.RemoveMissingFiles(new List<string>());

            // Assert
            var projectFiles = manager.GetProjects();
            Assert.HasCount(1, projectFiles, "Should still have 1 project");
            Assert.HasCount(1, Properties.Settings.Default.RecentFiles);
        }

        [TestMethod()]
        public async Task MultipleOperations_ShouldKeepBothListsInSync()
        {
            // Arrange
            var manager = new ProjectListManager();
            await manager.InitializeFromSettingsAsync(mockBindingService.Object);

            var projectA = CreateTestFile("A.mfproj");
            var projectB = CreateTestFile("B.mfproj");
            var projectC = CreateTestFile("C.mfproj");
            var projectD = CreateTestFile("D.mfproj");

            // Act - Complex sequence of operations
            manager.OpenProject(new ProjectInfo() { FilePath = projectA });
            manager.OpenProject(new ProjectInfo() { FilePath = projectB });
            manager.OpenProject(new ProjectInfo() { FilePath = projectC });
            manager.RemoveProject(projectB);
            manager.OpenProject(new ProjectInfo() { FilePath = projectD });

            // Assert - Both lists should be in sync
            var projectFiles = manager.GetProjects();
            var recentFiles = Properties.Settings.Default.RecentFiles.Cast<string>().ToList();

            Assert.HasCount(3, projectFiles);
            Assert.HasCount(3, recentFiles, "Both lists should have same count");

            // Verify all items exist in both lists
            foreach (var file in projectFiles)
            {
                Assert.Contains(file.FilePath, recentFiles, $"{file.FilePath} should exist in both lists");
            }
        }
    }
}