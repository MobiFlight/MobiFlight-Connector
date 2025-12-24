using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MobiFlight.Base.Migration.Tests
{
    [TestClass]
    public class ProjectMigrationTests
    {
        private string _testDirectory;

        [TestInitialize]
        public void Setup()
        {
            // Create a temporary test directory
            _testDirectory = Path.Combine(Path.GetTempPath(), "ProjectMigrationTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test directory
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        #region Project.ApplyMigrations Tests

        [TestMethod]
        public void ApplyMigrations_CurrentSchemaVersion_NoMigrationNeeded()
        {
            // Arrange
            var project = new Project();
            var currentSchemaVersionDocument = JObject.Parse($@"{{
                ""_version"": ""{project.SchemaVersion}"",
                ""Name"": ""Test Project"",
                ""ConfigFiles"": []
            }}");

            // Use reflection to access private method
            var applyMigrationsMethod = typeof(Project).GetMethod("ApplyMigrations",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = applyMigrationsMethod.Invoke(project, new object[] { currentSchemaVersionDocument, false }) as JObject;

            // Assert
            Assert.AreEqual(project.SchemaVersion.ToString(), result["_version"].ToString());
            Assert.AreEqual("Test Project", result["Name"].ToString());
        }

        [TestMethod]
        public void ApplyMigrations_NoVersionField_DefaultsToCurrentSchemaVersion()
        {
            // Arrange
            var project = new Project();
            var documentWithoutVersion = JObject.Parse(@"{
                ""Name"": ""Legacy Project"",
                ""ConfigFiles"": []
            }");

            var applyMigrationsMethod = typeof(Project).GetMethod("ApplyMigrations",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = applyMigrationsMethod.Invoke(project, new object[] { documentWithoutVersion, false }) as JObject;

            // Assert
            Assert.AreEqual(project.SchemaVersion.ToString(), result["_version"].ToString());
            Assert.AreEqual("Legacy Project", result["Name"].ToString());
        }

        #endregion

        #region Project File Integration Tests

        [TestMethod]
        public void OpenFile_LegacyJsonProject_MigratesAndLoads()
        {
            // Arrange
            var legacyProjectJson = @"{
                ""_version"": ""0.8"",
                ""Name"": ""Legacy Integration Project"",
                ""ConfigFiles"": [
                    {
                        ""Label"": ""Integration Config"",
                        ""EmbedContent"": true,
                        ""ConfigItems"": [
                            {
                                ""Name"": ""Integration Output"",
                                ""Type"": ""OutputConfigItem"",
                                ""GUID"": ""test-guid-123"",
                                ""Preconditions"": [
                                    {
                                        ""PreconditionType"": ""variable"",
                                        ""PreconditionRef"": ""altitude"",
                                        ""PreconditionOperand"": "">"",
                                        ""PreconditionValue"": ""10000"",
                                        ""PreconditionLogic"": ""or"",
                                        ""PreconditionActive"": true
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }";

            var testProjectFile = Path.Combine(_testDirectory, "legacy_project.mfproj");
            File.WriteAllText(testProjectFile, legacyProjectJson);

            // Act
            var project = new Project();
            project.FilePath = testProjectFile;
            project.OpenFile();

            // Assert
            Assert.AreEqual("Legacy Integration Project", project.Name);
            Assert.HasCount(1, project.ConfigFiles);

            var configFile = project.ConfigFiles[0];
            Assert.AreEqual("Integration Config", configFile.Label);
            Assert.HasCount(1, configFile.ConfigItems);

            var configItem = configFile.ConfigItems[0];
            Assert.AreEqual("Integration Output", configItem.Name);
            Assert.AreEqual("test-guid-123", configItem.GUID);

            // The deserialization should work because migration happened before JSON was processed
            // Note: The exact precondition validation would depend on your Precondition class implementation
            Assert.AreEqual(1, configItem.Preconditions.Count);
        }

        [TestMethod]
        public void SaveFile_AddsCurrentSchemaVersion()
        {
            // Arrange
            var project = new Project();
            project.Name = "Version Test Project";
            project.ConfigFiles.Add(new ConfigFile { Label = "Test Config", EmbedContent = true });

            var testProjectFile = Path.Combine(_testDirectory, "version_test.mfproj");
            project.FilePath = testProjectFile;

            // Act
            project.SaveFile();

            // Assert
            Assert.IsTrue(File.Exists(testProjectFile));

            var savedContent = File.ReadAllText(testProjectFile);
            var savedDocument = JObject.Parse(savedContent);

            Assert.AreEqual(project.SchemaVersion.ToString(), savedDocument["_version"].ToString());
            Assert.AreEqual("Version Test Project", savedDocument["Name"].ToString());
        }

        [TestMethod]
        public void OpenFile_ModernJsonProject_LoadsWithoutMigration()
        {
            // Arrange
            var modernProject = new Project();
            var modernProjectJson = JsonConvert.SerializeObject(new
            {
                _version = modernProject.SchemaVersion.ToString(),
                Name = "Modern Project",
                ConfigFiles = new[]
                {
                    new
                    {
                        Label = "Modern Config",
                        EmbedContent = true,
                        ConfigItems = new[]
                        {
                            new
                            {
                                Name = "Modern Output",
                                Type = "OutputConfigItem",
                                GUID = "modern-guid-456",
                                Preconditions = new[]
                                {
                                    new
                                    {
                                        type = "config",
                                        @ref = "modern_ref",
                                        operand = "=",
                                        value = "42",
                                        logic = "and",
                                        active = true
                                    }
                                }
                            }
                        }
                    }
                }
            }, Formatting.Indented);

            var testProjectFile = Path.Combine(_testDirectory, "modern_project.mfproj");
            File.WriteAllText(testProjectFile, modernProjectJson);

            // Act
            var project = new Project();
            project.FilePath = testProjectFile;
            project.OpenFile();

            // Assert
            Assert.AreEqual("Modern Project", project.Name);
            Assert.HasCount(1, project.ConfigFiles);

            var configFile = project.ConfigFiles[0];
            Assert.AreEqual("Modern Config", configFile.Label);
            Assert.HasCount(1, configFile.ConfigItems);
        }

        [TestMethod]
        public void OpenFile_ModernJsonProject_ShouldRemoveEmptyPreconditions()
        {
            // Arrange
            var modernProjectJson = JsonConvert.SerializeObject(new
            {
                _version = "0.8",
                Name = "Modern Project",
                ConfigFiles = new[]
                {
                    new
                    {
                        Label = "Modern Config",
                        EmbedContent = true,
                        ConfigItems = new[]
                        {
                            new
                            {
                                Name = "Modern Output",
                                Type = "OutputConfigItem",
                                GUID = "modern-guid-456",
                                Preconditions = new[]
                                {
                                    new
                                    {
                                        type = "none",
                                        serial = null as string,
                                        @ref = null as string,
                                        pin = null as string,
                                        operand = "=",
                                        value = null as string,
                                        logic = "and",
                                        active = true
                                    }
                                }
                            }
                        }
                    }
                }
            }, Formatting.Indented);

            var testProjectFile = Path.Combine(_testDirectory, "modern_project.mfproj");
            File.WriteAllText(testProjectFile, modernProjectJson);

            // Act
            var project = new Project();
            project.FilePath = testProjectFile;
            project.OpenFile();

            // Assert
            Assert.AreEqual("Modern Project", project.Name);
            Assert.HasCount(1, project.ConfigFiles);

            var configFile = project.ConfigFiles[0];
            Assert.AreEqual("Modern Config", configFile.Label);
            Assert.HasCount(1, configFile.ConfigItems);
            Assert.AreEqual(0, configFile.ConfigItems[0].Preconditions.Count);
        }

        #endregion

        #region OriginalVersion Tests

        [TestMethod]
        public void OpenFile_NoMigrationNeeded_OriginalVersionMatchesSchemaVersion()
        {
            // Arrange
            var project = new Project();
            var modernProjectJson = JsonConvert.SerializeObject(new
            {
                _version = project.SchemaVersion.ToString(),
                Name = "Modern Project",
                ConfigFiles = new object[0]
            }, Formatting.Indented);

            var testProjectFile = Path.Combine(_testDirectory, "modern_project.mfproj");
            File.WriteAllText(testProjectFile, modernProjectJson);

            // Act
            project.FilePath = testProjectFile;
            project.OpenFile();

            // Assert
            Assert.AreEqual(project.SchemaVersion, project.OriginalSchemaVersion);
        }

        [TestMethod]
        public void OpenFile_MigrationPerformed_OriginalVersionDiffersFromSchemaVersion()
        {
            // Arrange
            var originalVersion = new Version(0, 1);
            var legacyProjectJson = JsonConvert.SerializeObject(new
            {
                _version = originalVersion.ToString(),
                Name = "Legacy Project",
                ConfigFiles = new[]
                {
                    new
                    {
                        Label = "Legacy Config",
                        EmbedContent = true,
                        ConfigItems = new[]
                        {
                            new
                            {
                                Name = "Legacy Output",
                                Type = "OutputConfigItem",
                                GUID = "legacy-guid-123",
                                Preconditions = new[]
                                {
                                    new
                                    {
                                        PreconditionType = "config",
                                        PreconditionRef = "altitude",
                                        PreconditionOperand = ">",
                                        PreconditionValue = "10000"
                                    }
                                }
                            }
                        }
                    }
                }
            }, Formatting.Indented);

            var testProjectFile = Path.Combine(_testDirectory, "legacy_project.mfproj");
            File.WriteAllText(testProjectFile, legacyProjectJson);

            var project = new Project();

            // Act
            project.FilePath = testProjectFile;
            project.OpenFile();

            // Assert
            Assert.AreEqual(originalVersion, project.OriginalSchemaVersion);
            Assert.AreNotEqual(project.SchemaVersion, project.OriginalSchemaVersion);
        }

        [TestMethod]
        public void OpenFile_NoVersionInDocument_OriginalVersionDefaultsToV0_1()
        {
            // Arrange
            var projectWithoutVersionJson = JsonConvert.SerializeObject(new
            {
                Name = "Project Without Version",
                ConfigFiles = new object[0]
            }, Formatting.Indented);

            var testProjectFile = Path.Combine(_testDirectory, "no_version_project.mfproj");
            File.WriteAllText(testProjectFile, projectWithoutVersionJson);

            var project = new Project();

            // Act
            project.FilePath = testProjectFile;
            project.OpenFile();

            // Assert
            Assert.AreEqual(new Version(0, 1), project.OriginalSchemaVersion);
        }

        [TestMethod]
        public void OpenFile_InvalidVersionInDocument_OriginalVersionDefaultsToV0_1()
        {
            // Arrange
            var projectWithInvalidVersionJson = JsonConvert.SerializeObject(new
            {
                _version = "invalid.version.format",
                Name = "Project With Invalid Version",
                ConfigFiles = new object[0]
            }, Formatting.Indented);

            var testProjectFile = Path.Combine(_testDirectory, "invalid_version_project.mfproj");
            File.WriteAllText(testProjectFile, projectWithInvalidVersionJson);

            var project = new Project();

            // Act
            project.FilePath = testProjectFile;
            project.OpenFile();

            // Assert
            Assert.AreEqual(new Version(0, 1), project.OriginalSchemaVersion);
        }

        [TestMethod]
        public void OpenFile_FutureVersion_OriginalVersionPreserved()
        {
            // Arrange
            var futureVersion = new Version(2, 0);
            var futureProjectJson = JsonConvert.SerializeObject(new
            {
                _version = futureVersion.ToString(),
                Name = "Future Project",
                ConfigFiles = new object[0]
            }, Formatting.Indented);

            var testProjectFile = Path.Combine(_testDirectory, "future_project.mfproj");
            File.WriteAllText(testProjectFile, futureProjectJson);

            var project = new Project();

            // Act
            project.FilePath = testProjectFile;
            project.OpenFile();

            // Assert
            Assert.AreEqual(futureVersion, project.OriginalSchemaVersion);
        }

        [TestMethod]
        public void CreateNewProject_OriginalVersionIsNull()
        {
            // Arrange & Act
            var project = new Project();

            // Assert
            Assert.IsNull(project.OriginalSchemaVersion);
        }

        #endregion

        #region Error Handling Tests

        [TestMethod]
        public void ApplyMigrations_CorruptedDocument_HandlesGracefully()
        {
            // Arrange
            var project = new Project();
            var corruptedDocument = JObject.Parse(@"{
                ""_version"": ""0.8"",
                ""ConfigFiles"": [
                    {
                        ""ConfigItems"": [
                            {
                                ""Preconditions"": [
                                    {
                                        ""PreconditionType"": ""config""
                                        // Missing other required fields
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }");

            var applyMigrationsMethod = typeof(Project).GetMethod("ApplyMigrations",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act & Assert - Should not throw
            var result = applyMigrationsMethod.Invoke(project, new object[] { corruptedDocument, false }) as JObject;

            // Verify version was still updated
            Assert.AreEqual(project.SchemaVersion.ToString(), result["_version"].ToString());
        }

        [TestMethod]
        public void ApplyMigrations_InvalidVersionString_DefaultsToCurrentSchemaVersion()
        {
            // Arrange
            var project = new Project();
            var documentWithInvalidVersion = JObject.Parse(@"{
                ""_version"": ""invalid.version.string"",
                ""Name"": ""Invalid Version Project"",
                ""ConfigFiles"": []
            }");

            var applyMigrationsMethod = typeof(Project).GetMethod("ApplyMigrations",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = applyMigrationsMethod.Invoke(project, new object[] { documentWithInvalidVersion, false }) as JObject;

            // Assert
            Assert.AreEqual(project.SchemaVersion.ToString(), result["_version"].ToString());
            Assert.AreEqual("Invalid Version Project", result["Name"].ToString());
        }

        [TestMethod]
        public void ApplyMigrations_EmptyVersionString_DefaultsToCurrentSchemaVersion()
        {
            // Arrange
            var project = new Project();
            var documentWithEmptyVersion = JObject.Parse(@"{
                ""_version"": """",
                ""Name"": ""Empty Version Project"",
                ""ConfigFiles"": []
            }");

            var applyMigrationsMethod = typeof(Project).GetMethod("ApplyMigrations",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = applyMigrationsMethod.Invoke(project, new object[] { documentWithEmptyVersion, false }) as JObject;

            // Assert
            Assert.AreEqual(project.SchemaVersion.ToString(), result["_version"].ToString());
            Assert.AreEqual("Empty Version Project", result["Name"].ToString());
        }

        [TestMethod]
        public void ApplyMigrations_NullVersionField_DefaultsToCurrentSchemaVersion()
        {
            // Arrange
            var project = new Project();
            var documentWithNullVersion = JObject.Parse(@"{
                ""_version"": null,
                ""Name"": ""Null Version Project"",
                ""ConfigFiles"": []
            }");

            var applyMigrationsMethod = typeof(Project).GetMethod("ApplyMigrations",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = applyMigrationsMethod.Invoke(project, new object[] { documentWithNullVersion, false }) as JObject;

            // Assert
            Assert.AreEqual(project.SchemaVersion.ToString(), result["_version"].ToString());
            Assert.AreEqual("Null Version Project", result["Name"].ToString());
        }

        [TestMethod]
        public void GetDocumentSchemaVersion_ValidVersionString_ParsesCorrectly()
        {
            // Arrange
            var project = new Project();
            var document = JObject.Parse(@"{
                ""_version"": ""1.5"",
                ""Name"": ""Valid Version Project""
            }");

            // Use reflection to access private method
            var getVersionMethod = typeof(Project).GetMethod("GetDocumentSchemaVersion",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = getVersionMethod.Invoke(project, new object[] { document }) as Version;

            // Assert
            Assert.AreEqual(new Version(1, 5), result);
        }

        [TestMethod]
        public void GetDocumentSchemaVersion_ComplexVersionString_ParsesCorrectly()
        {
            // Arrange
            var project = new Project();
            var document = JObject.Parse(@"{
                ""_version"": ""2.1.3.4"",
                ""Name"": ""Complex Version Project""
            }");

            var getVersionMethod = typeof(Project).GetMethod("GetDocumentSchemaVersion",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = getVersionMethod.Invoke(project, new object[] { document }) as Version;

            // Assert
            Assert.AreEqual(new Version(2, 1, 3, 4), result);
        }

        [TestMethod]
        public void OpenFile_UnsupportedFileFormat_ThrowsInvalidDataException()
        {
            // Arrange
            var testProjectFile = Path.Combine(_testDirectory, "unsupported_project.txt");
            File.WriteAllText(testProjectFile, "This is not a valid project file format");

            var project = new Project();
            project.FilePath = testProjectFile;

            // Act & Assert
            Assert.ThrowsExactly<InvalidDataException>(() => project.OpenFile());
        }

        #endregion

        #region Future Migration Tests

        [TestMethod]
        public void ApplyMigrations_FutureVersion_NoDowngrade()
        {
            // Arrange - Simulate a project from a future version
            var project = new Project();
            var futureVersion = new Version(project.SchemaVersion.Major + 1, 0);
            var futureDocument = JObject.Parse($@"{{
                ""_version"": ""{futureVersion}"",
                ""Name"": ""Future Project"",
                ""ConfigFiles"": []
            }}");

            var applyMigrationsMethod = typeof(Project).GetMethod("ApplyMigrations",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = applyMigrationsMethod.Invoke(project, new object[] { futureDocument, false }) as JObject;

            // Assert - Should not downgrade version
            Assert.AreEqual(futureVersion.ToString(), result["_version"].ToString());
            Assert.AreEqual("Future Project", result["Name"].ToString());
        }

        #endregion
    }
}