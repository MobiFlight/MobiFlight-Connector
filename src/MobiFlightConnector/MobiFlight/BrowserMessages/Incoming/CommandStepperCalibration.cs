using MobiFlight.Base;
using MobiFlight.OutputConfig;
using System.Runtime.Serialization;

namespace MobiFlight.BrowserMessages.Incoming
{
    public enum StepperCalibrationAction
    {
        [EnumMember(Value = "move")]
        move,
        [EnumMember(Value = "home")]
        home
    }
    public class CommandStepperCalibration
    {
        public Controller Controller;
        public Stepper Stepper;
        public StepperCalibrationAction Action = StepperCalibrationAction.move;
        public int? Steps = 0;
    }
}