using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace MobiFlight.Base.Tests
{
    [TestClass]
    public class UiCultureResolverTests
    {
        #region FromSetting

        [TestMethod]
        [DataRow("de-DE", "de")]
        [DataRow("es-ES", "es")]
        [DataRow("fi-FI", "fi")]
        [DataRow("ja-JP", "ja")]
        [DataRow("ko-KR", "ko")]
        [DataRow("pt-PT", "pt")]
        [DataRow("ru-RU", "ru")]
        [DataRow("en-US", "en")]
        public void FromSetting_ShouldReturnLanguageOnly_WhenRegionIsPresent(string setting, string expected)
        {
            // Arrange

            // Act
            var result = UiCultureResolver.FromSetting(setting);

            // Assert
            Assert.AreEqual(expected, result.Name);
        }

        [TestMethod]
        [DataRow("de")]
        [DataRow("en")]
        public void FromSetting_ShouldReturnSameLanguage_WhenNoRegion(string setting)
        {
            // Arrange

            // Act
            var result = UiCultureResolver.FromSetting(setting);

            // Assert
            Assert.AreEqual(setting, result.Name);
        }

        [TestMethod]
        public void FromSetting_ShouldThrow_WhenCultureIsInvalid()
        {
            // Arrange
            var setting = "!!";

            // Act & Assert
            Assert.ThrowsExactly<CultureNotFoundException>(() => UiCultureResolver.FromSetting(setting));
        }

        #endregion
    }
}