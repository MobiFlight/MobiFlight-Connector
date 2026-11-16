using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Xml;

namespace MobiFlight.OutputConfig.Tests
{
    [TestClass()]
    public class LedModuleAddressTests
    {
        #region SplitAddress

        [TestMethod()]
        [DataRow("Display:2", "Display", 2)]
        [DataRow("Display:10", "Display", 10)]
        [DataRow("Left:Right:3", "Left:Right", 3)]
        public void SplitAddress_ShouldReturnNameAndModule_WhenAddressHasModule(string address, string expectedName, int expectedModule)
        {
            // Arrange
            var led = new LedModule { DisplayLedAddress = address };

            // Act
            var (name, module) = led.SplitAddress();

            // Assert
            Assert.AreEqual(expectedName, name);
            Assert.AreEqual(expectedModule, module);
        }

        [TestMethod()]
        [DataRow("Display", "Display")]
        [DataRow("Display:abc", "Display:abc")]
        [DataRow("", "")]
        [DataRow(null, "")]
        public void SplitAddress_ShouldReturnFirstModule_WhenAddressHasNoModule(string address, string expectedName)
        {
            // Arrange
            var led = new LedModule { DisplayLedAddress = address };

            // Act
            var (name, module) = led.SplitAddress();

            // Assert
            Assert.AreEqual(expectedName, name);
            Assert.AreEqual(1, module);
        }

        #endregion

        #region Xml

        [TestMethod()]
        public void XmlRead_ShouldMergeConnectorIntoAddress_WhenConnectorAttributeIsPresent()
        {
            // Arrange
            var led = new LedModule();
            using var reader = XmlReader.Create(new StringReader(@"<display ledAddress=""COM_NAV"" ledConnector=""3"" />"));
            reader.MoveToContent();

            // Act
            led.XmlRead(reader);

            // Assert
            Assert.AreEqual("COM_NAV:3", led.DisplayLedAddress);
        }

        [TestMethod()]
        public void XmlRead_ShouldKeepAddress_WhenConnectorAttributeIsMissing()
        {
            // Arrange
            var led = new LedModule();
            using var reader = XmlReader.Create(new StringReader(@"<display ledAddress=""COM_NAV:3"" />"));
            reader.MoveToContent();

            // Act
            led.XmlRead(reader);

            // Assert
            Assert.AreEqual("COM_NAV:3", led.DisplayLedAddress);
        }

        [TestMethod()]
        public void WriteXml_ShouldWriteNameAndModuleAsSeparateAttributes()
        {
            // Arrange
            var led = new LedModule { DisplayLedAddress = "COM_NAV:3" };
            var builder = new StringBuilder();

            // Act
            using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                writer.WriteStartElement("display");
                led.WriteXml(writer);
                writer.WriteEndElement();
            }

            // Assert
            var xml = builder.ToString();
            Assert.Contains("ledAddress=\"COM_NAV\"", xml);
            Assert.Contains("ledConnector=\"3\"", xml);
        }

        #endregion
    }
}