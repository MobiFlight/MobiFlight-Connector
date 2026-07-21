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
        public void SetDisplay_SendsCorrectCommand_WithAllPinsSet()
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
            var firstByteValue = "�/\0";
            var secondByteValue = "�/\0";

            // Act
            module.Display(outputPins, value);
            WaitForQueueUpdate();

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write after brigthness change should always send command.");
        }

        [TestMethod()]
        public void SetDisplay_SendsCorrectCommand_WithSomePinsSet()
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
            var firstByteValue = "0/\0";
            var secondByteValue = "@/\0";

            // Act
            module.Display(outputPins, value);
            WaitForQueueUpdate();

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write after brigthness change should always send command.");
        }

        [TestMethod()]
        public void SetDisplay_SendsCorrectCommand_ByteOrderIsCorrect()
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
            var firstByteValue = "@/\0";

            // and the first last
            var secondByteValue = "A/\0";

            // Act
            module.Display(outputPins, value);
            WaitForQueueUpdate();

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write after brigthness change should always send command.");
        }

        private void WaitForQueueUpdate()
        {
            var task = Task.Run(() =>
            {
                Thread.Sleep(100);
            });
            task.Wait();
        }
    }
}