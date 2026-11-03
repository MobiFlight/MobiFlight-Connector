using MobiFlight.Firmware;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class DeviceCapabilityTests
    {
        #region Serialization

        [TestMethod()]
        public void Serialize_ShouldWriteNoProperties_WhenAllPropertiesAreNull()
        {
            // Arrange
            var capability = new DeviceCapability();

            // Act
            var json = JObject.Parse(JsonConvert.SerializeObject(capability));

            // Assert
            Assert.IsEmpty(json);
        }

        [TestMethod()]
        public void Serialize_ShouldWriteOnlyPropertiesThatAreSet()
        {
            // Arrange
            var capability = new DeviceCapability { Rows = 2, Columns = 16 };

            // Act
            var json = JObject.Parse(JsonConvert.SerializeObject(capability));

            // Assert
            Assert.HasCount(2, json);
            Assert.AreEqual(2, (int)json["Rows"]);
            Assert.AreEqual(16, (int)json["Columns"]);
        }

        [TestMethod()]
        public void Serialize_ShouldWriteFalseValues_WhenBooleanPropertiesAreFalse()
        {
            // Arrange
            var capability = new DeviceCapability { Pwm = false, AutoZero = false };

            // Act
            var json = JObject.Parse(JsonConvert.SerializeObject(capability));

            // Assert
            Assert.HasCount(2, json);
            Assert.IsFalse((bool)json["Pwm"]);
            Assert.IsFalse((bool)json["AutoZero"]);
        }

        [TestMethod()]
        public void Serialize_ShouldWriteProfileAndMessageTypes_WhenSet()
        {
            // Arrange
            var capability = new DeviceCapability
            {
                Profile = new StepperProfilePreset { id = 1 },
                MessageTypes = [new CustomDeviceMessageType(3, "Label", "Description")]
            };

            // Act
            var json = JObject.Parse(JsonConvert.SerializeObject(capability));

            // Assert
            Assert.AreEqual(1, (int)json["Profile"]["id"]);
            Assert.AreEqual(3, (int)json["MessageTypes"][0]["Id"]);
        }

        #endregion
    }
}