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
        public void ApplyMigrations_CurrentVersion_NoMigrationNeeded()
        {
            // Arrange
            var project = new Project();
            var currentVersionDocument = JObject.Parse($@"{{
                ""_version"": ""{project.Version}"",
                ""Name"": ""Test Project"",
                ""ConfigFiles"": []
            }}");

            // Use reflection to access private method
            var applyMigrationsMethod = typeof(Project).GetMethod("ApplyMigrations", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = applyMigrationsMethod.Invoke(project, new object[] { currentVersionDocument }) as JObject;

            // Assert
            Assert.AreEqual(project.Version.ToString(), result["_version"].ToString());
            Assert.AreEqual("Test Project", result["Name"].ToString());
        }

        [TestMethod]
        public void ApplyMigrations_NoVersionField_DefaultsToV1_0()
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
            var result = applyMigrationsMethod.Invoke(project, new object[] { documentWithoutVersion }) as JObject;

            // Assert
            Assert.AreEqual(project.Version.ToString(), result["_version"].ToString());
            Assert.AreEqual("Legacy Project", result["Name"].ToString());
        }

        [TestMethod]
        public void ApplyMigrations_LegacyPreconditions_MigratesCorrectly()
        {
            // Arrange
            var project = new Project();
            var legacyDocument = JObject.Parse(@"{
                ""_version"": ""1.0"",
                ""Name"": ""Legacy Project"",
                ""ConfigFiles"": [
                    {
                        ""Label"": ""Test Config"",
                        ""ConfigItems"": [
                            {
                                ""Name"": ""Test Output"",
                                ""Preconditions"": [
                                    {
                                        ""PreconditionType"": ""config"",
                                        ""PreconditionRef"": ""test_ref"",
                                        ""PreconditionOperand"": ""equals"",
                                        ""PreconditionValue"": ""1"",
                                        ""PreconditionLogic"": ""and"",
                                        ""PreconditionActive"": ""true""
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }");

            var applyMigrationsMethod = typeof(Project).GetMethod("ApplyMigrations", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = applyMigrationsMethod.Invoke(project, new object[] { legacyDocument }) as JObject;

            // Assert
            var precondition = result["ConfigFiles"][0]["ConfigItems"][0]["Preconditions"][0];
            
            // Verify migration occurred
            Assert.AreEqual("config", precondition["type"].ToString());
            Assert.AreEqual("test_ref", precondition["ref"].ToString());
            Assert.AreEqual("=", precondition["operand"].ToString()); // "equals" -> "="
            Assert.AreEqual("1", precondition["value"].ToString());
            Assert.AreEqual("and", precondition["logic"].ToString());
            Assert.AreEqual(true, precondition["active"].Value<bool>()); // "true" -> true
            
            // Verify old properties are removed
            Assert.IsNull(precondition["PreconditionType"]);
            Assert.IsNull(precondition["PreconditionRef"]);
            Assert.IsNull(precondition["PreconditionOperand"]);
            Assert.IsNull(precondition["PreconditionValue"]);
            Assert.IsNull(precondition["PreconditionLogic"]);
            Assert.IsNull(precondition["PreconditionActive"]);
            
            // Verify version was updated
            Assert.AreEqual(project.Version.ToString(), result["_version"].ToString());
        }

        #endregion

        #region Project File Integration Tests

        [TestMethod]
        public void OpenFile_LegacyJsonProject_MigratesAndLoads()
        {
            // Arrange
            var legacyProjectJson = @"{
                ""_version"": ""1.0"",
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
                                        ""PreconditionOperand"": ""greater"",
                                        ""PreconditionValue"": ""10000"",
                                        ""PreconditionLogic"": ""or"",
                                        ""PreconditionActive"": ""1""
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
            Assert.AreEqual(1, project.ConfigFiles.Count);
            
            var configFile = project.ConfigFiles[0];
            Assert.AreEqual("Integration Config", configFile.Label);
            Assert.AreEqual(1, configFile.ConfigItems.Count);
            
            var configItem = configFile.ConfigItems[0];
            Assert.AreEqual("Integration Output", configItem.Name);
            Assert.AreEqual("test-guid-123", configItem.GUID);
            
            // The deserialization should work because migration happened before JSON was processed
            // Note: The exact precondition validation would depend on your Precondition class implementation
            Assert.AreEqual(1, configItem.Preconditions.Count);
        }

        [TestMethod]
        public void SaveFile_AddsCurrentVersion()
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
            
            Assert.AreEqual(project.Version.ToString(), savedDocument["_version"].ToString());
            Assert.AreEqual("Version Test Project", savedDocument["Name"].ToString());
        }

        [TestMethod]
        public void OpenFile_ModernJsonProject_LoadsWithoutMigration()
        {
            // Arrange
            var modernProject = new Project();
            var modernProjectJson = JsonConvert.SerializeObject(new
            {
                _version = modernProject.Version.ToString(),
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
            Assert.AreEqual(1, project.ConfigFiles.Count);
            
            var configFile = project.ConfigFiles[0];
            Assert.AreEqual("Modern Config", configFile.Label);
            Assert.AreEqual(1, configFile.ConfigItems.Count);
        }

        #endregion

        #region Error Handling Tests

        [TestMethod]
        public void ApplyMigrations_CorruptedDocument_HandlesGracefully()
        {
            // Arrange
            var project = new Project();
            var corruptedDocument = JObject.Parse(@"{
                ""_version"": ""1.0"",
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
            var result = applyMigrationsMethod.Invoke(project, new object[] { corruptedDocument }) as JObject;
            
            // Verify version was still updated
            Assert.AreEqual(project.Version.ToString(), result["_version"].ToString());
        }

        [TestMethod]
        public void ApplyMigrations_InvalidVersionString_DefaultsToV1_0()
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
            var result = applyMigrationsMethod.Invoke(project, new object[] { documentWithInvalidVersion }) as JObject;

            // Assert
            Assert.AreEqual(project.Version.ToString(), result["_version"].ToString());
            Assert.AreEqual("Invalid Version Project", result["Name"].ToString());
        }

        [TestMethod]
        public void ApplyMigrations_EmptyVersionString_DefaultsToV1_0()
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
            var result = applyMigrationsMethod.Invoke(project, new object[] { documentWithEmptyVersion }) as JObject;

            // Assert
            Assert.AreEqual(project.Version.ToString(), result["_version"].ToString());
            Assert.AreEqual("Empty Version Project", result["Name"].ToString());
        }

        [TestMethod]
        public void ApplyMigrations_NullVersionField_DefaultsToV1_0()
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
            var result = applyMigrationsMethod.Invoke(project, new object[] { documentWithNullVersion }) as JObject;

            // Assert
            Assert.AreEqual(project.Version.ToString(), result["_version"].ToString());
            Assert.AreEqual("Null Version Project", result["Name"].ToString());
        }

        [TestMethod]
        public void GetDocumentVersion_ValidVersionString_ParsesCorrectly()
        {
            // Arrange
            var project = new Project();
            var document = JObject.Parse(@"{
                ""_version"": ""1.5"",
                ""Name"": ""Valid Version Project""
            }");

            // Use reflection to access private method
            var getVersionMethod = typeof(Project).GetMethod("GetDocumentVersion", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = getVersionMethod.Invoke(project, new object[] { document }) as Version;

            // Assert
            Assert.AreEqual(new Version(1, 5), result);
        }

        [TestMethod]
        public void GetDocumentVersion_ComplexVersionString_ParsesCorrectly()
        {
            // Arrange
            var project = new Project();
            var document = JObject.Parse(@"{
                ""_version"": ""2.1.3.4"",
                ""Name"": ""Complex Version Project""
            }");

            var getVersionMethod = typeof(Project).GetMethod("GetDocumentVersion", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = getVersionMethod.Invoke(project, new object[] { document }) as Version;

            // Assert
            Assert.AreEqual(new Version(2, 1, 3, 4), result);
        }

        #endregion

        #region Future Migration Tests

        [TestMethod]
        public void ApplyMigrations_FutureVersion_NoDowngrade()
        {
            // Arrange - Simulate a project from a future version
            var project = new Project();
            var futureVersion = new Version(project.Version.Major + 1, 0);
            var futureDocument = JObject.Parse($@"{{
                ""_version"": ""{futureVersion}"",
                ""Name"": ""Future Project"",
                ""ConfigFiles"": []
            }}");

            var applyMigrationsMethod = typeof(Project).GetMethod("ApplyMigrations", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = applyMigrationsMethod.Invoke(project, new object[] { futureDocument }) as JObject;

            // Assert - Should not downgrade version
            Assert.AreEqual(futureVersion.ToString(), result["_version"].ToString());
            Assert.AreEqual("Future Project", result["Name"].ToString());
        }

        #endregion
    }
}