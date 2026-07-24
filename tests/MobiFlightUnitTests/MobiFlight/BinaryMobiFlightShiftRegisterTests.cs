using CommandMessenger;
using CommandMessenger.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlightUnitTests.mock.CommandMessenger;
using Moq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class BinaryMobiFlightShiftRegisterTests
    {
        class TestableMobiFlightShiftRegister : BinaryMobiFlightShiftRegister
        {
            public void TriggerAggregationInsteadOfTimer()
            {
                // Stop the timer to prevent it from firing
                AggregationTimer.Stop();

                // Manually call the aggregation processing method
                // this is deterministic for testing in CI pipeline
                ProcessAggregatedPins(null, null);
            }
        }

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
            await WaitForCommandMessengerQueueIsProcessed(100);

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write should have all output pins set.");
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
            await WaitForCommandMessengerQueueIsProcessed(100);

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write should have some output pins set.");
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
            await WaitForCommandMessengerQueueIsProcessed(100);

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            Assert.AreEqual(DataExpected, mockTransport.DataWrite, "Write should have correct byte order.");
        }

        [TestMethod()]
        public async Task SetDisplay_Aggregation_WorksCorrectly_ForMultiplePins()
        {
            var writes = new List<string>();
            var mockTransport = new Mock<ITransport>();
            mockTransport
            .Setup(t => t.Write(It.IsAny<byte[]>()))
            .Callback<byte[]>(b => writes.Add(Encoding.ASCII.GetString(b)));

            // Arrange
            var module = new TestableMobiFlightShiftRegister
            {
                ModuleNumber = 0,
                NumberOfShifters = 2,
                // CmdMessenger's send queue combines consecutive non-ack commands into a single
                // Write() call as long as they fit within the send buffer (default 60 chars).
                // Whether the "on" and "off" commands get combined depends on background-thread
                // scheduling, which is racy between local runs and CI. Forcing a buffer of 1 char
                // makes CmdMessenger flush after every single command deterministically.
                CmdMessenger = new CmdMessenger(mockTransport.Object, 1)
            };

            module.CmdMessenger.Connect();

            var commandId = (byte)MobiFlightModule.Command.SetShiftRegisterPins;

            var outputPins = "0|6|14";
            var outputPinsOff = "1|7|15";

            var value = "1";
            var valueOff = "0";
            // the second shifter comes first
            var firstByteValue = "64";
            var firstByteValueOff = "128";

            // and the first last
            var secondByteValue = "65";
            var secondByteValueOff = "130";

            // Act
            module.Display(outputPins, value);
            module.Display(outputPinsOff, valueOff);
            // We have to trigger the aggregation instead of using a timer
            module.TriggerAggregationInsteadOfTimer();
            await WaitForCommandMessengerQueueIsProcessed(100);

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            var DataExpectedOff = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{valueOff},{firstByteValueOff},{secondByteValueOff};";

            Assert.HasCount(2, writes, "There should be two writes.");
            Assert.AreEqual(writes[0], DataExpected, "The on write should occur once.");
            Assert.AreEqual(writes[1], DataExpectedOff, "The off write should occur once.");
        }

        [TestMethod()]
        public async Task SetDisplay_Aggregation_WorksCorrectly_ForSamePins()
        {
            var mockTransport = new Mock<ITransport>();

            // Arrange
            var module = new TestableMobiFlightShiftRegister
            {
                ModuleNumber = 0,
                NumberOfShifters = 2,
                CmdMessenger = new CmdMessenger(mockTransport.Object),
            };

            module.CmdMessenger.Connect();

            var commandId = (byte)MobiFlightModule.Command.SetShiftRegisterPins;

            var outputPins = "0|6|14";
            var outputPinsOff = "0|6|14";

            var value = "1";
            var valueOff = "0";
            // the second shifter comes first
            var firstByteValue = "64";
            var firstByteValueOff = "64";

            // and the first last
            var secondByteValue = "65";
            var secondByteValueOff = "65";

            // Act
            module.Display(outputPins, value);
            module.Display(outputPinsOff, valueOff);

            // We have to trigger the aggregation instead of using a timer
            module.TriggerAggregationInsteadOfTimer();
            await WaitForCommandMessengerQueueIsProcessed(100);

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{value},{firstByteValue},{secondByteValue};";
            var DataExpectedOff = $"{commandId},{module.ModuleNumber},{module.NumberOfShifters},{valueOff},{firstByteValueOff},{secondByteValueOff};";

            mockTransport.Verify(t => t.Write(It.Is<byte[]>(b => System.Text.Encoding.ASCII.GetString(b) == DataExpected)), Times.Never, "The on write should never occur.");
            mockTransport.Verify(t => t.Write(It.Is<byte[]>(b => System.Text.Encoding.ASCII.GetString(b) == DataExpectedOff)), Times.Once, "The off write should occur once.");
        }

        private async Task WaitForCommandMessengerQueueIsProcessed(int timeout = 100)
        {
            await Task.Delay(timeout);
        }
    }
}