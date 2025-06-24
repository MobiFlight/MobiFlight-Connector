using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class MidiBoardDefinitionTests
    {
        [TestMethod]
        public void MapDeviceNameToLabel_ReturnsLabel_WhenDeviceNameExists()
        {
            // Arrange
            var inputDefinition = new MidiInputDefinition
            {
                Label = "Key %",
                LabelIds = new[] { "1", "2", "3", "4", "5", "6", "7", "8" },
                InputType = MidiBoardDeviceType.Button,
                MessageType = MidiMessageType.Note,
                MessageChannel = 1,
                MessageIds = new List<byte> { 10, 20, 30, 40, 50, 60, 70, 80 },
            };
            
            var midiBoard = new MidiBoardDefinition
            {
                Inputs = new List<MidiInputDefinition> { inputDefinition }
            };

            // Act
            var label = midiBoard.MapDeviceNameToLabel("Note 1_20");

            // Assert
            Assert.AreEqual("Key 2", label);
        }

        [TestMethod]
        public void MapDeviceNameToLabel_ReturnsDeviceName_WhenDeviceNameNotFound()
        {
            // Arrange
            var inputDefinition = new MidiInputDefinition
            {
                Label = "Key %",
                LabelIds = new[] { "1", "2", "3", "4", "5", "6", "7", "8" },
                InputType = MidiBoardDeviceType.Button,
                MessageType = MidiMessageType.Note,
                MessageChannel = 1,
                MessageIds = new List<byte> { 10, 20, 30, 40, 50, 60, 70, 80 },
            };

            var midiBoard = new MidiBoardDefinition
            {
                Inputs = new List<MidiInputDefinition> { inputDefinition }
            };

            // Act
            var label = midiBoard.MapDeviceNameToLabel("Note 1_21");

            // Assert
            Assert.AreEqual("Note 1_21", label);
        }

        [TestMethod]
        public void MapDeviceNameToLabel_ReturnsDeviceName_WhenDeviceNameNotFoundForChannel()
        {
            // Arrange
            var inputDefinition = new MidiInputDefinition
            {
                Label = "Key %",
                LabelIds = new[] { "1", "2", "3", "4", "5", "6", "7", "8" },
                InputType = MidiBoardDeviceType.Button,
                MessageType = MidiMessageType.Note,
                MessageChannel = 1,
                MessageIds = new List<byte> { 10, 20, 30, 40, 50, 60, 70, 80 },
            };

            var midiBoard = new MidiBoardDefinition
            {
                Inputs = new List<MidiInputDefinition> { inputDefinition }
            };

            // Act
            var label = midiBoard.MapDeviceNameToLabel("Note 2_20");

            // Assert
            Assert.AreEqual("Note 2_20", label);
        }
    }
}
