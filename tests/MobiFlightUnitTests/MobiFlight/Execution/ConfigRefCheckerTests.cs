using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.InputConfig;
using System.Collections.Generic;

namespace MobiFlight.Execution.Tests
{
    [TestClass]
    public class ConfigRefCheckerTests
    {
        #region UpdateStatus

        [TestMethod]
        public void UpdateStatus_ShouldSetMissing_WhenActiveReferenceDoesNotExist()
        {
            // Arrange
            var cfg = new OutputConfigItem();
            cfg.ConfigRefs.Add(new ConfigRef { Active = true, Ref = "deleted-guid", Placeholder = "#" });

            // Act
            ConfigRefChecker.UpdateStatus(cfg, new List<IConfigItem> { cfg });

            // Assert
            Assert.AreEqual("Missing", cfg.Status[ConfigItemStatusType.ConfigRef]);
        }

        [TestMethod]
        public void UpdateStatus_ShouldSetMissing_WhenInputConfigItemHasMissingReference()
        {
            // Arrange
            var cfg = new InputConfigItem();
            cfg.ConfigRefs.Add(new ConfigRef { Active = true, Ref = "deleted-guid", Placeholder = "#" });

            // Act
            ConfigRefChecker.UpdateStatus(cfg, new List<IConfigItem> { cfg });

            // Assert
            Assert.AreEqual("Missing", cfg.Status[ConfigItemStatusType.ConfigRef]);
        }

        [TestMethod]
        public void UpdateStatus_ShouldSetMissing_WhenOnlyOneOfTheReferencesDoesNotExist()
        {
            // Arrange
            var referenced = new OutputConfigItem();
            var cfg = new OutputConfigItem();
            cfg.ConfigRefs.Add(new ConfigRef { Active = true, Ref = referenced.GUID, Placeholder = "#" });
            cfg.ConfigRefs.Add(new ConfigRef { Active = true, Ref = "deleted-guid", Placeholder = "?" });

            // Act
            ConfigRefChecker.UpdateStatus(cfg, new List<IConfigItem> { cfg, referenced });

            // Assert
            Assert.AreEqual("Missing", cfg.Status[ConfigItemStatusType.ConfigRef]);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void UpdateStatus_ShouldNotSetStatus_WhenReferencedConfigItemExists(bool referencedConfigItemIsActive)
        {
            // Arrange
            var referenced = new OutputConfigItem { Active = referencedConfigItemIsActive };
            var cfg = new OutputConfigItem();
            cfg.ConfigRefs.Add(new ConfigRef { Active = true, Ref = referenced.GUID, Placeholder = "#" });

            // Act
            ConfigRefChecker.UpdateStatus(cfg, new List<IConfigItem> { cfg, referenced });

            // Assert
            Assert.IsFalse(cfg.Status.ContainsKey(ConfigItemStatusType.ConfigRef));
        }

        [TestMethod]
        public void UpdateStatus_ShouldNotSetStatus_WhenMissingReferenceIsNotActive()
        {
            // Arrange
            var cfg = new OutputConfigItem();
            cfg.ConfigRefs.Add(new ConfigRef { Active = false, Ref = "deleted-guid", Placeholder = "#" });

            // Act
            ConfigRefChecker.UpdateStatus(cfg, new List<IConfigItem> { cfg });

            // Assert
            Assert.IsFalse(cfg.Status.ContainsKey(ConfigItemStatusType.ConfigRef));
        }

        [TestMethod]
        public void UpdateStatus_ShouldRemoveStatus_WhenReferenceIsAvailableAgain()
        {
            // Arrange
            var referenced = new OutputConfigItem();
            var cfg = new OutputConfigItem();
            cfg.ConfigRefs.Add(new ConfigRef { Active = true, Ref = referenced.GUID, Placeholder = "#" });
            ConfigRefChecker.UpdateStatus(cfg, new List<IConfigItem> { cfg });

            // Act
            ConfigRefChecker.UpdateStatus(cfg, new List<IConfigItem> { cfg, referenced });

            // Assert
            Assert.IsFalse(cfg.Status.ContainsKey(ConfigItemStatusType.ConfigRef));
        }

        #endregion
    }
}
