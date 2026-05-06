using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.Joysticks.Bodnar;
using SharpDX.DirectInput;
using System.Collections.Generic;
using DeviceType = MobiFlight.DeviceType;

namespace MobiFlightUnitTests.MobiFlight.Joysticks.Bodnar
{
    internal class TestableBodnarBoard : BodnarBoard
    {
        public List<InputEventArgs> CapturedEvents { get; } = new List<InputEventArgs>();
        public override string Serial { get => "JS-BODNAR-TEST"; }
        public TestableBodnarBoard() : base(32, null, null)
        {
            OnButtonPressed += (s, e) => CapturedEvents.Add(e);

            var axisName = "Axis X";
            var axisLabel = "Axis X";
            Axes.Add(new JoystickDevice() { Name = axisName, Label = axisLabel, Type = DeviceType.AnalogInput, JoystickDeviceType = JoystickDeviceType.Axis });
            EnumerateDevices();
        }

        public new JoystickState State { get => base.State; set => base.State = value; }

        public new void UpdateAxis(JoystickState state) => base.UpdateAxis(state);
    }

    [TestClass]
    public class BodnarBoardTests
    {
        private TestableBodnarBoard CreateBoard() => new TestableBodnarBoard();

        #region First report

        [TestMethod]
        public void UpdateAxis_FirstCall_TriggersEvent()
        {
            // Arrange
            var board = CreateBoard();

            // Act
            board.UpdateAxis(new JoystickState { X = 1024 });

            // Assert
            Assert.HasCount(1, board.CapturedEvents);
        }

        #endregion

        #region Noise suppression

        [TestMethod]
        public void UpdateAxis_SameQuantizedValue_DoesNotTriggerSecondEvent()
        {
            // Arrange
            var board = CreateBoard();
            var state = new JoystickState { X = 1024 };
            board.UpdateAxis(state);
            board.CapturedEvents.Clear();
            board.State = state;

            // Act
            board.UpdateAxis(new JoystickState { X = 1025 });

            // Assert
            Assert.HasCount(0, board.CapturedEvents);
        }

        #endregion

        #region Real movement

        [TestMethod]
        public void UpdateAxis_ValueCrossesQuantizationBoundary_TriggersEvent()
        {
            // Arrange
            var board = CreateBoard();
            var state = new JoystickState { X = 1024 };
            board.UpdateAxis(state);
            board.CapturedEvents.Clear();
            board.State = state;

            // Act
            // 8 times 
            for(var i=0; i < 8; i++)
            {
                board.UpdateAxis(new JoystickState { X = 1024 + 32 });
            }

            // Assert
            Assert.HasCount(1, board.CapturedEvents);
            Assert.AreEqual(1056, board.CapturedEvents[0].Value);
        }

        [TestMethod]
        public void UpdateAxis_EventContainsCorrectMetadata()
        {
            // Arrange
            var board = CreateBoard();
            var state = new JoystickState { X = 1024 };
            
            // Act
            board.UpdateAxis(state);

            // Assert
            Assert.AreEqual(DeviceType.AnalogInput, board.CapturedEvents[0].Type);
            Assert.AreEqual("Axis X", board.CapturedEvents[0].DeviceId);
        }

        #endregion
    }
}