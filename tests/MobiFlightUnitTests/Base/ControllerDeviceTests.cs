using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ControllerDeviceTests
    {
        #region Serialization

        [TestMethod()]
        public void Serialize_ShouldOmitCapabilities_WhenCapabilitiesAreNull()
        {
            // Arrange
            var device = new ControllerDevice { Name = "Button1", Label = "Button1", Type = DeviceType.Button };

            // Act
            var json = JObject.Parse(JsonConvert.SerializeObject(device));

            // Assert
            Assert.IsNull(json.Property("Capabilities"));
        }

        [TestMethod()]
        public void Serialize_ShouldIncludeCapabilities_WhenCapabilitiesAreSet()
        {
            // Arrange
            var device = new ControllerDevice
            {
                Name = "Led1",
                Label = "Led1",
                Type = DeviceType.Output,
                Capabilities = new DeviceCapability { Pwm = true }
            };

            // Act
            var json = JObject.Parse(JsonConvert.SerializeObject(device));

            // Assert
            Assert.IsTrue((bool)json["Capabilities"]["Pwm"]);
        }

        [TestMethod()]
        public void Serialize_ShouldIncludeCapabilities_WhenDeviceIsHeldAsDeviceReference()
        {
            // Arrange
            var controller = new Controller { Name = "Board", Serial = "SN-1" };
            controller.Devices.Add(new ControllerDevice
            {
                Name = "Lcd1",
                Label = "Lcd1",
                Type = DeviceType.LcdDisplay,
                Capabilities = new DeviceCapability { Rows = 2, Columns = 16 }
            });

            // Act
            var json = JObject.Parse(JsonConvert.SerializeObject(controller));

            // Assert
            Assert.AreEqual(2, (int)json["Devices"][0]["Capabilities"]["Rows"]);
        }

        #endregion
    }
}