using CommandMessenger;
using CommandMessenger.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class MobiFlightShiftRegisterTests
    {
        internal class TestableMobiFlightShiftRegister : MobiFlightShiftRegister
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
        public async Task Stop_WithMultipleShifters_CreatesCorrectMessage()
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

            var outputPins = "0|1|2|3|4|5|6|7|8|9|10|11|12|13|14|15";
            var valueOff = "0";

            // Act
            module.Stop();

            // We have to trigger the aggregation instead of using a timer
            module.TriggerAggregationInsteadOfTimer();
            await WaitForCommandMessengerQueueIsProcessed(100);

            // Assert
            var DataExpected = $"{commandId},{module.ModuleNumber},{outputPins},{valueOff};";

            mockTransport.Verify(t => t.Write(It.Is<byte[]>(b => System.Text.Encoding.ASCII.GetString(b) == DataExpected)), Times.Once, "The on write should occur once.");
        }

        [TestMethod()]
        public async Task Stop_WithNoShifters_DoesNotCreateMessage()
        {
            var mockTransport = new Mock<ITransport>();

            // Arrange
            var module = new TestableMobiFlightShiftRegister
            {
                ModuleNumber = 0,
                NumberOfShifters = 0,
                CmdMessenger = new CmdMessenger(mockTransport.Object),
            };

            module.CmdMessenger.Connect();

            // Act
            module.Stop();

            // We have to trigger the aggregation instead of using a timer
            module.TriggerAggregationInsteadOfTimer();
            await WaitForCommandMessengerQueueIsProcessed(100);

            // Assert
            mockTransport.Verify(t => t.Write(It.IsAny<byte[]>()), Times.Never, "No write should occur.");

        }
        private async Task WaitForCommandMessengerQueueIsProcessed(int timeout = 100)
        {
            await Task.Delay(timeout);
        }
    }
}