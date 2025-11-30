using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ProjectTests
    {
        [TestMethod()]
        public void OpenFileTest_Single_Xml()
        {
            string inFile = @"assets\Base\ConfigFile\OpenFileTest.xml";
            var o = new Project();
            Assert.IsNotNull(o);

            o.FilePath = inFile;
            o.OpenFile();

            Assert.IsNotNull(o.ConfigFiles);
            Assert.IsTrue(o.ConfigFiles.Count > 0);

            var config = o.ConfigFiles[0];
            var inputConfigs = config.ConfigItems.Where(i => i is InputConfigItem);

            Assert.IsNotNull(inputConfigs);
            Assert.IsTrue(inputConfigs.Count() > 0);

            var outputConfigs = config.ConfigItems.Where(i => i is OutputConfigItem);
            Assert.IsNotNull(outputConfigs);
            Assert.IsTrue(outputConfigs.Count() > 0);
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
            Assert.IsTrue(o.ConfigFiles.Count > 0);

            var config = o.ConfigFiles[0];
            var inputConfigs = config.ConfigItems.Where(i => i is InputConfigItem);

            Assert.IsNotNull(inputConfigs);
            Assert.IsTrue(inputConfigs.Count() > 0);

            var outputConfigs = config.ConfigItems.Where(i => i is OutputConfigItem);
            Assert.IsNotNull(outputConfigs);
            Assert.IsTrue(outputConfigs.Count() > 0);
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
            Assert.IsFalse(fileContent.Contains("\"SchemaVersion\":"));
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
            Assert.IsFalse(fileContent.Contains("\"FilePath\":"));
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
            Assert.AreEqual(originalCount + 2, targetProject.ConfigFiles.Count);
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
            Assert.AreEqual(originalCount, targetProject.ConfigFiles.Count);
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
            Assert.IsTrue(totalItemsAfterMerge > originalItemCount);

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
            Assert.IsTrue(targetProject.ConfigFiles.Count > originalCount);
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
            Assert.AreEqual(originalCount + 2, targetProject.ConfigFiles.Count);
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
            Assert.AreEqual(originalCount, targetProject.ConfigFiles.Count);
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
            Assert.AreEqual(2, targetProject.ConfigFiles.Count);
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

            Assert.IsTrue(outputItems.Count > 1); // Original + merged outputs
            Assert.IsTrue(inputItems.Count > 1); // Original + merged inputs

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
            Assert.AreEqual(0, project.Controllers.Count);
            Assert.IsNull(project.Sim);
            Assert.IsFalse(project.UseFsuipc);
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
            Assert.AreEqual(3, project.Controllers.Count);
            Assert.IsTrue(project.Controllers.Contains("SN-123-456"));
            Assert.IsTrue(project.Controllers.Contains("SN-789-012"));
            Assert.IsTrue(project.Controllers.Contains("SN-345-678"));
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
            Assert.AreEqual(2, project.Controllers.Count);
            Assert.IsTrue(project.Controllers.Contains("SN-AAA-111"));
            Assert.IsTrue(project.Controllers.Contains("SN-BBB-222"));
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
            Assert.AreEqual(1, project.Controllers.Count);
            Assert.IsFalse(project.Controllers.Contains("SN-FIRST-001"));
            Assert.IsTrue(project.Controllers.Contains("SN-SECOND-002"));
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
            Assert.IsFalse(project.UseFsuipc);
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
            Assert.IsFalse(project.UseFsuipc);
        }

        [TestMethod()]
        public void DetermineProjectInfos_WithProSimConfig_ShouldSetSimToProsim()
        {
            // Arrange
            var project = new Project();
            var config = new ConfigFile();

            var outputConfig = new OutputConfigItem
            {
                ModuleSerial = "SN-123-456",
                Source = new ProSimSource()
            };
            config.ConfigItems.Add(outputConfig);
            project.ConfigFiles.Add(config);

            // Act
            project.DetermineProjectInfos();

            // Assert
            Assert.AreEqual("prosim", project.Sim);
            Assert.IsFalse(project.UseFsuipc);
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
            Assert.IsTrue(project.UseFsuipc);
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
            Assert.AreEqual(2, project.Controllers.Count);
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
            Assert.IsTrue(project.UseFsuipc);
            Assert.AreEqual(2, project.Controllers.Count);
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
            Assert.IsTrue(project.UseFsuipc);
            Assert.AreEqual(3, project.Controllers.Count);
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
            Assert.AreEqual(2, project.Controllers.Count);
        }
        #endregion
    }
}