using CommandMessenger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlightUnitTests.mock.CommandMessenger;
using Newtonsoft.Json.Linq;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class BinaryMobiFlightShiftRegisterTests
    {
        [TestMethod()]
        public void BinaryMobiFlightShiftRegister_HasCorrectDefaults()
        {
            // Arrange
            var module = new BinaryMobiFlightShiftRegister();
            Assert.AreEqual(0, module.NumberOfShifters, "Default number of shifters should be 0.");
            Assert.AreEqual(0, module.ModuleNumber, "Default module number should be 0.");
            Assert.AreEqual(50, module.AggregationWindowInMs, "Default aggregation window should be 50ms.");
        }

        [TestMethod()]
        public void ConvertStringToByteArray_ShouldReturnCorrectByteArray()
        {
            // Arrange
            string input = "0|1|2|3|4|5|6|7";
            int numberOfShifters = 2;
            // Act
            byte[] result = BinaryMobiFlightShiftRegister.ConvertStringToByteArray(input, numberOfShifters);
            // Assert
            Assert.HasCount(2, result);
            Assert.AreEqual(0b11111111, result[0]); // all set
            Assert.AreEqual(0b00000000, result[1]); // non set

            input = "8|9|10|11|12|13|14|15";
            // Act
            result = BinaryMobiFlightShiftRegister.ConvertStringToByteArray(input, numberOfShifters);
            // Assert
            Assert.HasCount(2, result);
            Assert.AreEqual(0b00000000, result[0]); // none set
            Assert.AreEqual(0b11111111, result[1]); // all set

            input = "0|8";
            // Act
            result = BinaryMobiFlightShiftRegister.ConvertStringToByteArray(input, numberOfShifters);
            // Assert
            Assert.HasCount(2, result);
            Assert.AreEqual(0b00000001, result[0]); // none set
            Assert.AreEqual(0b00000001, result[1]); // all set
        }

        [TestMethod()]
        public void ConvertStringToByteArray_UseLsbEncoding()
        {
            // Arrange
            string input = "0|1|14|15";
            int numberOfShifters = 2;
            // Act
            byte[] result = BinaryMobiFlightShiftRegister.ConvertStringToByteArray(input, numberOfShifters);
            // Assert
            Assert.HasCount(2, result);
            Assert.AreEqual(0b00000011, result[0]); // none set
            Assert.AreEqual(0b11000000, result[1]); // all set
        }

        [TestMethod()]
        public async Task SetDisplay_SendsCorrectCommand_WithAllPinsSet()
        {
            // Arrange
            var module = new BinaryMobiFlightShiftRegister() {
                ModuleNumber = 0,
                NumberOfShifters = 2
            };
            var mockTransport = new MockTransport();
            module.CmdMessenger = new CmdMessenger(mockTransport);
            module.CmdMessenger.Connect();
            var commandId = (byte)MobiFlightModule.Command.SetShiftRegisterPins;
            var outputPins = "0|1|2|3|4|5|6|7|8|9|10|11|12|13|14|15";
            var value = "1";
            mockTransport.Clear();
            var firstByteValue = "255";
            var secondByteValue = "255";

            // Act
            module.Display(outputPins, value);
            await WaitForQueueUpdate(200);

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write after brigthness change should always send command.");
        }

        [TestMethod()]
        public async Task SetDisplay_SendsCorrectCommand_WithSomePinsSet()
        {
            // Arrange
            var module = new BinaryMobiFlightShiftRegister()
            {
                ModuleNumber = 0,
                NumberOfShifters = 2
            };
            var mockTransport = new MockTransport();
            module.CmdMessenger = new CmdMessenger(mockTransport);
            module.CmdMessenger.Connect();
            var commandId = (byte)MobiFlightModule.Command.SetShiftRegisterPins;
            var outputPins = "4|5|14";
            var value = "1";
            mockTransport.Clear();
            var firstByteValue = "64";
            var secondByteValue = "48";

            // Act
            module.Display(outputPins, value);
            await WaitForQueueUpdate(200);

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write after brigthness change should always send command.");
        }

        [TestMethod()]
        public async Task SetDisplay_SendsCorrectCommand_ByteOrderIsCorrect()
        {
            // Arrange
            var module = new BinaryMobiFlightShiftRegister()
            {
                ModuleNumber = 0,
                NumberOfShifters = 2
            };
            var mockTransport = new MockTransport();
            module.CmdMessenger = new CmdMessenger(mockTransport);
            module.CmdMessenger.Connect();
            var commandId = (byte)MobiFlightModule.Command.SetShiftRegisterPins;
            var outputPins = "0|6|14";
            var value = "1";
            mockTransport.Clear();
            // the second shifter comes first
            var firstByteValue = "64";

            // and the first last
            var secondByteValue = "65";

            // Act
            module.Display(outputPins, value);
            await WaitForQueueUpdate(200);

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write after brigthness change should always send command.");
        }

        private async Task WaitForQueueUpdate(int timeout = 100)
        {
            await Task.Delay(timeout);
        }
    }
}