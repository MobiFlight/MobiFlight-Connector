using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MobiFlight.Base.UpdateChecker;

namespace MobiFlight.Base.Tests
{
    [TestClass]
    public class AutoUpdateCheckerTests
    {
        [TestMethod]
        public void VersionCheck_StableVersion_ReturnsRelease()
        {
            var result = AutoUpdateChecker.VersionCheck(
                "##RESULT##|1|11.0.1.0",
                out var newVersion,
                out var channel);

            Assert.IsTrue(result);
            Assert.AreEqual("11.0.1.0", newVersion);
            Assert.AreEqual("RELEASE", channel);
        }

        [TestMethod]
        public void VersionCheck_BetaVersion_ReturnsBeta()
        {
            var result = AutoUpdateChecker.VersionCheck(
                "##RESULT##|1|11.0.1.2|BETA",
                out var newVersion,
                out var channel);

            Assert.IsTrue(result);
            Assert.AreEqual("11.0.1.2", newVersion);
            Assert.AreEqual("BETA", channel);
        }

        [TestMethod]
        public void VersionCheck_NoUpdate_ReturnsFalse()
        {
            var result = AutoUpdateChecker.VersionCheck(
                "##RESULT##|0",
                out var newVersion,
                out var channel);

            Assert.IsFalse(result);
            Assert.IsNull(newVersion);
            Assert.IsNull(channel);
        }

        [TestMethod]
        public void ReleaseVersion_StableVersion_RemovesZeroRevision()
        {
            var result = AutoUpdateChecker.ReleaseVersion("11.0.1.0");

            Assert.AreEqual("11.0.1", result);
        }

        [TestMethod]
        public void ReleaseVersion_BetaVersion_KeepsRevision()
        {
            var result = AutoUpdateChecker.ReleaseVersion("11.0.1.2");

            Assert.AreEqual("11.0.1.2", result);
        }

        [TestMethod]
        public void ReleaseVersion_InvalidVersion_ReturnsOriginalValue()
        {
            var result = AutoUpdateChecker.ReleaseVersion("invalid");

            Assert.AreEqual("invalid", result);
        }

        [TestMethod]
        public void IsBetaVersion_RevisionZero_ReturnsFalse()
        {
            var result = AutoUpdateChecker.IsBetaVersion(new Version(11, 0, 1, 0));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsBetaVersion_PositiveRevision_ReturnsTrue()
        {
            var result = AutoUpdateChecker.IsBetaVersion(new Version(11, 0, 1, 1));

            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(false, "RELEASE", "StableToStable")]
        [DataRow(false, "BETA", "StableToBeta")]
        [DataRow(true, "BETA", "BetaToBeta")]
        [DataRow(true, "RELEASE", "BetaToStable")]
        public void GetUpdatePath_ReturnsExpectedPath(
            bool currentIsBeta,
            string targetChannel,
            string expectedPath)
        {
            var result = AutoUpdateChecker.GetUpdatePath(currentIsBeta, targetChannel);

            Assert.AreEqual(expectedPath, result.ToString());
        }
    }
}