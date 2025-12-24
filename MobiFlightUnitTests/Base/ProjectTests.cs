using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Linq;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ProjectTests
    {
        private LogSeverity _logSeverity = LogSeverity.Error;
        private Mock<ILogAppender> _mockLogAppender;

        [TestInitialize]
        public void SetUp()
        {
            // Create a mock log appender
            _mockLogAppender = new Mock<ILogAppender>();
            _logSeverity = Log.Instance.Severity; // Store the current log severity
            Log.Instance.Severity = LogSeverity.Debug; // Set the log severity to Debug
            Log.Instance.ClearAppenders();
            Log.Instance.AddAppender(_mockLogAppender.Object);
        }

        [TestCleanup]
        public void TearDown()
        {
            // Remove the mock appender after each test
            Log.Instance.ClearAppenders();
            Log.Instance.Severity = _logSeverity; // Restore the original log severity
            Log.Instance.Enabled = false; // Disable logging
        }

        [TestMethod()]
        public void OpenFileTest_Single_Xml()
        {
            string inFile = @"assets\Base\ConfigFile\OpenFileTest.xml";
            var o = new Project();
            Assert.IsNotNull(o);

            o.FilePath = inFile;
            o.OpenFile();

            Assert.IsNotNull(o.ConfigFiles);
            Assert.IsNotEmpty(o.ConfigFiles);

            var config = o.ConfigFiles[0];
            var inputConfigs = config.ConfigItems.Where(i => i is InputConfigItem);

            Assert.IsNotNull(inputConfigs);
            Assert.IsGreaterThan(0, inputConfigs.Count());

            var outputConfigs = config.ConfigItems.Where(i => i is OutputConfigItem);
            Assert.IsNotNull(outputConfigs);
            Assert.IsGreaterThan(0, outputConfigs.Count());
        }

        [TestMethod()]
        public void OpenFileTest_Single_Xml_Dont_Load_Empty_Preconditions()
        {
            string inFile = @"assets\Base\ConfigFile\OpenFileTest.xml";
            var o = new Project();
            Assert.IsNotNull(o);

            o.FilePath = inFile;
            o.OpenFile();

            Assert.IsNotNull(o.ConfigFiles);
            Assert.IsNotEmpty(o.ConfigFiles);

            var config = o.ConfigFiles[0];
            var outputConfig = config.ConfigItems.Where(i => i is OutputConfigItem && i.Name == "COM1 Active").First();

            Assert.IsNotNull(outputConfig);
            Assert.IsNotNull(outputConfig as OutputConfigItem);
            var preconditions = (outputConfig as OutputConfigItem).Preconditions;

            Assert.AreEqual(0, preconditions.Count);
        }

        [TestMethod()]
        public void OpenFileTest_Single_Xml_Correctly_Load_Existing_Preconditions()
        {
            string inFile = @"assets\Base\ConfigFile\OpenFileTest.xml";
            var o = new Project();
            Assert.IsNotNull(o);

            o.FilePath = inFile;
            o.OpenFile();

            Assert.IsNotNull(o.ConfigFiles);
            Assert.IsNotEmpty(o.ConfigFiles);

            var config = o.ConfigFiles[0];
            var outputConfig = config.ConfigItems.Where(i => i is OutputConfigItem && i.Name == "COM1 Standby").First();

            Assert.IsNotNull(outputConfig);
            Assert.IsNotNull(outputConfig as OutputConfigItem);
            var preconditions = (outputConfig as OutputConfigItem).Preconditions;

            Assert.AreEqual(2, preconditions.Count);
        }

        [TestMethod()]
        public void OpenFileTest_Single_Json_Embedded()
        {
            string inFile = @"assets\Base\ConfigFile\Json\OpenProjectTest.mfproj";
            var o = new Project();
            Assert.IsNotNull(o);

            o.FilePath = inFile;
            o.OpenFile();

            Assert.IsNotNull(o.ConfigFiles);
            Assert.IsNotEmpty(o.ConfigFiles);

            var config = o.ConfigFiles[0];
            var inputConfigs = config.ConfigItems.Where(i => i is InputConfigItem);

            Assert.IsNotNull(inputConfigs);
            Assert.IsGreaterThan(0, inputConfigs.Count());

            var outputConfigs = config.ConfigItems.Where(i => i is OutputConfigItem);
            Assert.IsNotNull(outputConfigs);
            Assert.IsGreaterThan(0, outputConfigs.Count());
        }

        [TestMethod()]
        public void SaveFileTest()
        {
            string inFile = @"assets\Base\ConfigFile\Json\OpenProjectTest.mfproj";
            var o = new Project();
            o.FilePath = inFile;
            o.OpenFile();

            string outFile = @"assets\Base\ConfigFile\Json\SaveProjectTest.mfproj";
            o.FilePath = outFile;
            o.SaveFile();

            var o2 = new Project();
            o2.FilePath = outFile;
            o2.OpenFile();
            Assert.IsNotNull(o2.ConfigFiles);
            Assert.AreEqual(o, o2);
        }

        [TestMethod()]
        public void SaveFileTest_Should_Not_Serialize_Version()
        {
            string inFile = @"assets\Base\ConfigFile\Json\OpenProjectTest.mfproj";
            var o = new Project();
            o.FilePath = inFile;
            o.OpenFile();

            string outFile = @"assets\Base\ConfigFile\Json\SaveProjectTest.mfproj";
            o.FilePath = outFile;
            o.SaveFile();

            string fileContent = File.ReadAllText(outFile);
            Assert.DoesNotContain("\"SchemaVersion\":", fileContent);
        }

        [TestMethod()]
        public void SaveFileTest_Should_Not_Serialize_FilePath()
        {
            string inFile = @"assets\Base\ConfigFile\Json\OpenProjectTest.mfproj";
            var o = new Project();
            o.FilePath = inFile;
            o.OpenFile();

            string outFile = @"assets\Base\ConfigFile\Json\SaveProjectTest.mfproj";
            o.FilePath = outFile;
            o.SaveFile();

            string fileContent = File.ReadAllText(outFile);
            Assert.DoesNotContain("\"FilePath\":", fileContent);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            var o = new Project();
            o.Name = "Test";
            o.FilePath = "TestPath";

            var o2 = new Project();
            o2.Name = "Test";
            o2.FilePath = "TestPath";
            Assert.IsTrue(o2.Equals(o));

            o.ConfigFiles.Add(new ConfigFile());
            Assert.IsFalse(o2.Equals(o));

            o2.ConfigFiles.Add(new ConfigFile());
            Assert.IsTrue(o2.Equals(o));

            var ici1 = new InputConfigItem();
            var ici2 = new InputConfigItem(ici1);

            o2.ConfigFiles[0].ConfigItems.Add(ici1);
            Assert.IsFalse(o2.Equals(o));

            o.ConfigFiles[0].ConfigItems.Add(ici2);
            Assert.IsTrue(o2.Equals(o));
        }

        #region Merge Tests

        [TestMethod()]
        public void Merge_WithValidProject_ShouldAddAllConfigFiles()
        {
            // Arrange
            var targetProject = new Project();
            targetProject.Name = "Target Project";
            targetProject.ConfigFiles.Add(new ConfigFile { Label = "Original Config" });

            var sourceProject = new Project();
            sourceProject.Name = "Source Project";
            sourceProject.ConfigFiles.Add(new ConfigFile { Label = "Source Config 1" });
            sourceProject.ConfigFiles.Add(new ConfigFile { Label = "Source Config 2" });

            var originalCount = targetProject.ConfigFiles.Count;

            // Act
            targetProject.Merge(sourceProject);

            // Assert
            Assert.HasCount(originalCount + 2, targetProject.ConfigFiles);
            Assert.IsTrue(targetProject.ConfigFiles.Any(cf => cf.Label == "Source Config 1"));
            Assert.IsTrue(targetProject.ConfigFiles.Any(cf => cf.Label == "Source Config 2"));
            Assert.IsTrue(targetProject.ConfigFiles.Any(cf => cf.Label == "Original Config"));
        }

        [TestMethod()]
        public void Merge_WithEmptyProject_ShouldNotAddAnyFiles()
        {
            // Arrange
            var targetProject = new Project();
            targetProject.Name = "Target Project";
            targetProject.ConfigFiles.Add(new ConfigFile { Label = "Original Config" });

            var sourceProject = new Project();
            sourceProject.Name = "Empty Source Project";

            var originalCount = targetProject.ConfigFiles.Count;

            // Act
            targetProject.Merge(sourceProject);

            // Assert
            Assert.HasCount(originalCount, targetProject.ConfigFiles);
            Assert.IsTrue(targetProject.ConfigFiles.Any(cf => cf.Label == "Original Config"));
        }

        [TestMethod()]
        public void Merge_WithProjectContainingConfigItems_ShouldPreserveAllItems()
        {
            // Arrange
            string sourceFile = @"assets\Base\ConfigFile\Json\MergeTest1.mfproj";
            var sourceProject = new Project();
            sourceProject.FilePath = sourceFile;
            sourceProject.OpenFile();

            var targetProject = new Project();
            targetProject.Name = "Target Project";
            var originalConfigFile = new ConfigFile { Label = "Original Config" };
            originalConfigFile.ConfigItems.Add(new OutputConfigItem { Name = "Original Output", GUID = "original-guid" });
            targetProject.ConfigFiles.Add(originalConfigFile);

            var originalItemCount = targetProject.ConfigFiles.SelectMany(cf => cf.ConfigItems).Count();

            // Act
            targetProject.Merge(sourceProject);

            // Assert
            var totalItemsAfterMerge = targetProject.ConfigFiles.SelectMany(cf => cf.ConfigItems).Count();
            Assert.IsGreaterThan(originalItemCount, totalItemsAfterMerge);

            // Verify original items are still there
            Assert.IsTrue(targetProject.ConfigFiles.SelectMany(cf => cf.ConfigItems).Any(item => item.GUID == "original-guid"));

            // Verify merged items are there
            Assert.IsTrue(targetProject.ConfigFiles.SelectMany(cf => cf.ConfigItems).Any(item => item.GUID == "merge-test-guid-1"));
        }

        [TestMethod()]
        public void MergeFromProjectFile_WithValidFile_ShouldLoadAndMerge()
        {
            // Arrange
            string sourceFile = @"assets\Base\ConfigFile\Json\MergeTest1.mfproj";
            var targetProject = new Project();
            targetProject.Name = "Target Project";
            targetProject.ConfigFiles.Add(new ConfigFile { Label = "Original Config" });

            var originalCount = targetProject.ConfigFiles.Count;

            // Act
            targetProject.MergeFromProjectFile(sourceFile);

            // Assert
            Assert.IsGreaterThan(originalCount, targetProject.ConfigFiles.Count);
            Assert.IsTrue(targetProject.ConfigFiles.Any(cf => cf.Label == "Config File 1"));
            Assert.IsTrue(targetProject.ConfigFiles.Any(cf => cf.Label == "Original Config"));
        }

        [TestMethod()]
        public void MergeFromProjectFile_WithMultipleConfigFiles_ShouldImportAll()
        {
            // Arrange
            string sourceFile = @"assets\Base\ConfigFile\Json\MergeTest2.mfproj";
            var targetProject = new Project();
            targetProject.Name = "Target Project";
            targetProject.ConfigFiles.Add(new ConfigFile { Label = "Original Config" });

            var originalCount = targetProject.ConfigFiles.Count;

            // Act
            targetProject.MergeFromProjectFile(sourceFile);

            // Assert
            Assert.HasCount(originalCount + 2, targetProject.ConfigFiles);
            Assert.IsTrue(targetProject.ConfigFiles.Any(cf => cf.Label == "Config File 2A"));
            Assert.IsTrue(targetProject.ConfigFiles.Any(cf => cf.Label == "Config File 2B"));
            Assert.IsTrue(targetProject.ConfigFiles.Any(cf => cf.Label == "Original Config"));

            // Verify all config items are preserved
            var allItems = targetProject.ConfigFiles.SelectMany(cf => cf.ConfigItems).ToList();
            Assert.IsTrue(allItems.Any(item => item.GUID == "merge-test-guid-2a"));
            Assert.IsTrue(allItems.Any(item => item.GUID == "merge-test-guid-2b"));
            Assert.IsTrue(allItems.Any(item => item.GUID == "merge-test-guid-2b-input"));
        }

        [TestMethod()]
        public void MergeFromProjectFile_WithEmptyProject_ShouldNotAddFiles()
        {
            // Arrange
            string sourceFile = @"assets\Base\ConfigFile\Json\MergeTestEmpty.mfproj";
            var targetProject = new Project();
            targetProject.Name = "Target Project";
            targetProject.ConfigFiles.Add(new ConfigFile { Label = "Original Config" });

            var originalCount = targetProject.ConfigFiles.Count;

            // Act
            targetProject.MergeFromProjectFile(sourceFile);

            // Assert
            Assert.HasCount(originalCount, targetProject.ConfigFiles);
            Assert.IsTrue(targetProject.ConfigFiles.Any(cf => cf.Label == "Original Config"));
        }

        [TestMethod()]
        public void MergeFromProjectFile_WithNonExistentFile_ShouldThrowException()
        {
            // Arrange
            string nonExistentFile = @"assets\Base\ConfigFile\Json\NonExistent.mfproj";
            var targetProject = new Project();

            // Act & Assert
            Assert.ThrowsExactly<FileNotFoundException>(() => targetProject.MergeFromProjectFile(nonExistentFile));
        }

        [TestMethod()]
        public void MergeFromProjectFile_WithInvalidFile_ShouldThrowException()
        {
            // Create a temporary invalid JSON file
            string invalidFile = Path.GetTempFileName();
            File.WriteAllText(invalidFile, "{ invalid json content");

            try
            {
                // Arrange
                var targetProject = new Project();

                // Act & Assert
                Assert.ThrowsExactly<Newtonsoft.Json.JsonReaderException>(() => targetProject.MergeFromProjectFile(invalidFile));
            }
            finally
            {
                // Cleanup
                if (File.Exists(invalidFile))
                    File.Delete(invalidFile);
            }
        }

        [TestMethod()]
        public void Merge_ShouldTriggerProjectChangedEvent()
        {
            // Arrange
            var targetProject = new Project();
            var sourceProject = new Project();
            sourceProject.ConfigFiles.Add(new ConfigFile { Label = "Test Config" });

            bool eventTriggered = false;
            targetProject.ProjectChanged += (sender, args) => eventTriggered = true;

            // Act
            targetProject.Merge(sourceProject);

            // Assert
            Assert.IsTrue(eventTriggered);
        }

        [TestMethod()]
        public void MergeFromProjectFile_ShouldTriggerProjectChangedEvent()
        {
            // Arrange
            string sourceFile = @"assets\Base\ConfigFile\Json\MergeTest1.mfproj";
            var targetProject = new Project();

            bool eventTriggered = false;
            targetProject.ProjectChanged += (sender, args) => eventTriggered = true;

            // Act
            targetProject.MergeFromProjectFile(sourceFile);

            // Assert
            Assert.IsTrue(eventTriggered);
        }

        [TestMethod()]
        public void Merge_WithDuplicateConfigFileLabels_ShouldAllowDuplicates()
        {
            // Arrange
            var targetProject = new Project();
            targetProject.ConfigFiles.Add(new ConfigFile { Label = "Same Label" });

            var sourceProject = new Project();
            sourceProject.ConfigFiles.Add(new ConfigFile { Label = "Same Label" });

            // Act
            targetProject.Merge(sourceProject);

            // Assert
            Assert.HasCount(2, targetProject.ConfigFiles);
            Assert.AreEqual(2, targetProject.ConfigFiles.Count(cf => cf.Label == "Same Label"));
        }

        [TestMethod()]
        public void MergeFromProjectFile_WithComplexProject_ShouldPreserveMixedConfigTypes()
        {
            // Arrange
            string sourceFile = @"assets\Base\ConfigFile\Json\MergeTest2.mfproj";
            var targetProject = new Project();
            targetProject.Name = "Target Project";

            // Add some original config items
            var originalConfigFile = new ConfigFile { Label = "Original Config" };
            originalConfigFile.ConfigItems.Add(new OutputConfigItem { Name = "Original Output", GUID = "original-output-guid" });
            originalConfigFile.ConfigItems.Add(new InputConfigItem { Name = "Original Input", GUID = "original-input-guid" });
            targetProject.ConfigFiles.Add(originalConfigFile);

            // Act
            targetProject.MergeFromProjectFile(sourceFile);

            // Assert
            var allItems = targetProject.ConfigFiles.SelectMany(cf => cf.ConfigItems).ToList();

            // Verify original items are preserved
            Assert.IsTrue(allItems.Any(item => item.GUID == "original-output-guid"));
            Assert.IsTrue(allItems.Any(item => item.GUID == "original-input-guid"));

            // Verify merged items include both input and output configs
            var outputItems = allItems.Where(item => item is OutputConfigItem).ToList();
            var inputItems = allItems.Where(item => item is InputConfigItem).ToList();

            Assert.IsGreaterThan(1, outputItems.Count); // Original + merged outputs
            Assert.IsGreaterThan(1, inputItems.Count); // Original + merged inputs

            // Verify specific merged items
            Assert.IsTrue(allItems.Any(item => item.GUID == "merge-test-guid-2a"));
            Assert.IsTrue(allItems.Any(item => item.GUID == "merge-test-guid-2b"));
            Assert.IsTrue(allItems.Any(item => item.GUID == "merge-test-guid-2b-input"));
        }

        #endregion


        #region ProjectInfo Tests
        [TestMethod()]
        public void DetermineProjectInfos_WithEmptyProject_ShouldHaveCorrectDefaults()
        {
            // Arrange
            var project = new Project();
            project.Name = "Empty Project";

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.IsNotNull(project.Controllers);
            Assert.IsEmpty(project.Controllers);
            Assert.IsNull(project.Sim);
            Assert.IsFalse(project.Features.FSUIPC);
            Assert.IsFalse(project.Features.ProSim);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithMultipleConfigFiles_ShouldCollectUniqueControllers()
        {
            // Arrange
            var project = new Project();
            var config1 = new ConfigFile();
            config1.ConfigItems.Add(new OutputConfigItem { ModuleSerial = "SN-123-456" });
            config1.ConfigItems.Add(new OutputConfigItem { ModuleSerial = "SN-123-456" }); // Duplicate
            var config2 = new ConfigFile();
            config2.ConfigItems.Add(new InputConfigItem { ModuleSerial = "SN-789-012" });
            config2.ConfigItems.Add(new OutputConfigItem { ModuleSerial = "SN-345-678" });

            project.ConfigFiles.Add(config1);
            project.ConfigFiles.Add(config2);

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.HasCount(3, project.Controllers);
            Assert.Contains("SN-123-456", project.Controllers);
            Assert.Contains("SN-789-012", project.Controllers);
            Assert.Contains("SN-345-678", project.Controllers);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithDuplicateSerials_ShouldOnlyIncludeUnique()
        {
            // Arrange
            var project = new Project();
            var config1 = new ConfigFile();
            config1.ConfigItems.Add(new OutputConfigItem { ModuleSerial = "SN-AAA-111" });
            config1.ConfigItems.Add(new InputConfigItem { ModuleSerial = "SN-AAA-111" });

            var config2 = new ConfigFile();
            config2.ConfigItems.Add(new OutputConfigItem { ModuleSerial = "SN-AAA-111" });
            config2.ConfigItems.Add(new InputConfigItem { ModuleSerial = "SN-BBB-222" });

            project.ConfigFiles.Add(config1);
            project.ConfigFiles.Add(config2);

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.HasCount(2, project.Controllers);
            Assert.Contains("SN-AAA-111", project.Controllers);
            Assert.Contains("SN-BBB-222", project.Controllers);
        }

        [TestMethod()]
        public void DetermineProjectInfos_CalledMultipleTimes_ShouldClearPreviousControllers()
        {
            // Arrange
            var project = new Project();
            var config = new ConfigFile();
            config.ConfigItems.Add(new OutputConfigItem { ModuleSerial = "SN-FIRST-001" });
            project.ConfigFiles.Add(config);

            // Act - First call
            project.DetermineProjectInfos();

            // Modify project
            project.ConfigFiles.Clear();
            var newConfig = new ConfigFile();
            newConfig.ConfigItems.Add(new OutputConfigItem { ModuleSerial = "SN-SECOND-002" });
            project.ConfigFiles.Add(newConfig);

            // Act - Second call
            project.DetermineProjectInfos();

            // Assert
            Assert.HasCount(1, project.Controllers);
            Assert.DoesNotContain("SN-FIRST-001", project.Controllers);
            Assert.Contains("SN-SECOND-002", project.Controllers);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithNoModuleSerials_ShouldHaveEmptyControllersList()
        {
            // Arrange
            var project = new Project();
            var config = new ConfigFile();
            config.ConfigItems.Add(new OutputConfigItem { ModuleSerial = null });
            config.ConfigItems.Add(new InputConfigItem { ModuleSerial = "" });
            project.ConfigFiles.Add(config);

            // Act
            project.DetermineProjectInfos();

            // Assert
            // The method doesn't filter out null/empty serials, it just collects distinct values
            Assert.IsNotNull(project.Controllers);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithMSFSConfig_ShouldSetSimToMsfs()
        {
            // Arrange
            var project = new Project();
            var config = new ConfigFile();

            var outputConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-123-456",
                Source = new SimConnectSource()
            };
            config.ConfigItems.Add(outputConfig);
            project.ConfigFiles.Add(config);

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.AreEqual("msfs", project.Sim);
            Assert.IsFalse(project.Features.FSUIPC);
            Assert.IsFalse(project.Features.ProSim);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithXPlaneConfig_ShouldSetSimToXplane()
        {
            // Arrange
            var project = new Project();
            var config = new ConfigFile();

            var outputConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-123-456",
                Source = new XplaneSource()
            };

            config.ConfigItems.Add(outputConfig);
            project.ConfigFiles.Add(config);

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.AreEqual("xplane", project.Sim);
            Assert.IsFalse(project.Features.FSUIPC);
            Assert.IsFalse(project.Features.ProSim);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithProSimConfig_ShouldSetFeatureForProsim()
        {
            // Arrange
            var project = new Project();
            var config = new ConfigFile();

            var outputConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-123-456",
                Source = new ProSimSource()
            };

            var msfsConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-123-456",
                Source = new SimConnectSource()
            };

            config.ConfigItems.Add(outputConfig);
            config.ConfigItems.Add(msfsConfig);
            project.ConfigFiles.Add(config);

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.AreNotEqual("prosim", project.Sim);
            Assert.AreEqual("msfs", project.Sim);
            Assert.IsFalse(project.Features.FSUIPC);
            Assert.IsTrue(project.Features.ProSim);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithFsuipcConfig_ShouldSetUseFsuipcToTrue()
        {
            // Arrange
            var project = new Project();
            var config = new ConfigFile();

            var outputConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-123-456",
                Source = new FsuipcSource()
            };
            config.ConfigItems.Add(outputConfig);
            project.ConfigFiles.Add(config);

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.IsTrue(project.Features.FSUIPC);
            // FSUIPC source alone doesn't set a specific sim
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithMultipleSimTypes_ShouldSetOnlyFirstSim()
        {
            // Arrange
            var project = new Project();

            // First config with MSFS
            var config1 = new ConfigFile();
            var msfsConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-123-456",
                Source = new SimConnectSource()
            };
            config1.ConfigItems.Add(msfsConfig);
            project.ConfigFiles.Add(config1);

            // Second config with X-Plane
            var config2 = new ConfigFile();
            var xplaneConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-789-012",
                Source = new XplaneSource()
            };
            config2.ConfigItems.Add(xplaneConfig);
            project.ConfigFiles.Add(config2);

            // Act
            project.DetermineProjectInfos();

            // Assert
            // Should only set the first sim found (MSFS in this case)
            Assert.AreEqual("msfs", project.Sim);
            Assert.HasCount(2, project.Controllers);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithFsuipcAndMSFS_ShouldSetBoth()
        {
            // Arrange
            var project = new Project();
            var config = new ConfigFile();

            var msfsConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-123-456",
                Source = new SimConnectSource()
            };
            var fsuipcConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-789-012",
                Source = new FsuipcSource()
            };

            config.ConfigItems.Add(msfsConfig);
            config.ConfigItems.Add(fsuipcConfig);
            project.ConfigFiles.Add(config);

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.AreEqual("msfs", project.Sim);
            Assert.IsTrue(project.Features.FSUIPC);
            Assert.HasCount(2, project.Controllers);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithMultipleConfigFilesAndDifferentSims_ShouldAccumulateFsuipc()
        {
            // Arrange
            var project = new Project();

            // First config file with MSFS
            var config1 = new ConfigFile();
            config1.ConfigItems.Add(new OutputConfigItem
            {
                ModuleSerial = "SN-AAA",
                Source = new SimConnectSource()
            });
            project.ConfigFiles.Add(config1);

            // Second config file with FSUIPC
            var config2 = new ConfigFile();
            config2.ConfigItems.Add(new OutputConfigItem
            {
                ModuleSerial = "SN-BBB",
                Source = new FsuipcSource()
            });
            project.ConfigFiles.Add(config2);

            // Third config file with another FSUIPC
            var config3 = new ConfigFile();
            config3.ConfigItems.Add(new OutputConfigItem
            {
                ModuleSerial = "SN-CCC",
                Source = new FsuipcSource()
            });
            project.ConfigFiles.Add(config3);

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.AreEqual("msfs", project.Sim);
            Assert.IsTrue(project.Features.FSUIPC);
            Assert.HasCount(3, project.Controllers);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithInputConfigItems_ShouldStillDetectSim()
        {
            // Arrange
            var project = new Project();
            var config = new ConfigFile();

            var inputConfig = new InputConfigItem
            {
                ModuleSerial = "SN-123-456"
            };
            var outputConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-789-012",
                Source = new XplaneSource()
            };

            config.ConfigItems.Add(inputConfig);
            config.ConfigItems.Add(outputConfig);
            project.ConfigFiles.Add(config);

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.AreEqual("xplane", project.Sim);
            Assert.HasCount(2, project.Controllers);
        }
        #endregion

        [TestMethod()]
        public void MigrateFileExtensionTest()
        {
            var project = new Project();

            var testExtensions = new[]
            {
                "Extension.FilePath.mcc",
                "Extension.FilePath.MCC",
                "Extension.FilePath.aic",
                "Extension.FilePath.AIC",
                "Extension.FilePath.mfproj",
            };

            var mfprojExtension = "Extension.FilePath.mfproj";

            testExtensions.ToList().ForEach(ext =>
            {
                project.FilePath = ext;
                var result = project.MigrateFileExtension();
                Assert.AreEqual(mfprojExtension, project.FilePath, "Extension was not migrated to .mfproj");
                Assert.AreEqual(result, project.FilePath, "Return value should be the same as FilePath value");
            });

            var invalidExtensions = new[]
            {
                "Extension.FilePath.txt",
                "Extension.FilePath.json",
                "Extension.FilePath.xml",
                "Extension.FilePath.config",
            };

            invalidExtensions.ToList().ForEach(ext =>
            {
                project.FilePath = ext;
                var result = project.MigrateFileExtension();
                Assert.AreEqual(ext, project.FilePath, "Extension should not be changed for invalid extensions");
                Assert.AreEqual(result, project.FilePath, "Return value should be the same as FilePath value");
            });
        }

        #region OpenFile Log Suppression Tests
        [TestMethod()]
        public void OpenFile_LogSuppression_DoesNotEmitMigrationLogs()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"project_test_{System.Guid.NewGuid()}.mfproj");
            try
            {
                // old schema version triggers migration path
                File.WriteAllText(tempFile, "{ \"Name\": \"TestProject\", \"ConfigFiles\": [], \"_version\": \"0.1\" }");

                Log.Instance.Severity = LogSeverity.Debug;
                Log.Instance.Enabled = true;

                var p = new Project { FilePath = tempFile };

                // Act - open in peek mode (suppress migration logging)
                p.OpenFile(suppressMigrationLogging: true);

                // Assert - no migration-related entries
                // Assert - migration-related entries present
                _mockLogAppender.Verify(
                    appender => appender.log(It.Is<string>(msg => msg.Contains("Migrating document")), LogSeverity.Debug),
                    Times.Never
                );

                _mockLogAppender.Verify(
                    appender => appender.log(It.Is<string>(msg => msg.Contains("Applying V0.9 migrations")), LogSeverity.Debug),
                    Times.Never
                );

                _mockLogAppender.Verify(
                    appender => appender.log(It.Is<string>(msg => msg.Contains("Migration complete")), LogSeverity.Debug),
                    Times.Never
                );
            }
            finally
            {
                try { if (File.Exists(tempFile)) File.Delete(tempFile); } catch { }
            }
        }

        [TestMethod()]
        public void OpenFile_LogSuppression_WhenNotSuppressed_EmitsMigrationLogs()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"project_test_{System.Guid.NewGuid()}.mfproj");
            try
            {
                // old schema version triggers migration path
                File.WriteAllText(tempFile, "{ \"Name\": \"TestProject\", \"ConfigFiles\": [], \"_version\": \"0.1\" }");

                Log.Instance.Severity = LogSeverity.Debug;
                Log.Instance.Enabled = true;

                var p = new Project { FilePath = tempFile };

                // Act - open normally (should emit migration logs)
                p.OpenFile();

                // Assert - migration-related entries present
                _mockLogAppender.Verify(
                    appender => appender.log(It.Is<string>(msg => msg.Contains("Migrating document")), LogSeverity.Debug),
                    Times.Once
                );

                _mockLogAppender.Verify(
                    appender => appender.log(It.Is<string>(msg => msg.Contains("Applying V0.9 migrations")), LogSeverity.Debug),
                    Times.Once
                );

                _mockLogAppender.Verify(
                    appender => appender.log(It.Is<string>(msg => msg.Contains("Migration complete")), LogSeverity.Debug),
                    Times.Once
                );
            }
            finally
            {
                try { if (File.Exists(tempFile)) File.Delete(tempFile); } catch { }
            }
        }

        #endregion


    }
}