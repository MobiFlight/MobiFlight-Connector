using CommandMessenger;
using System;
using System.Linq;

namespace MobiFlight
{
    public class BinaryMobiFlightShiftRegister : MobiFlightShiftRegister
    {
        public override void Display(String outputPins, String value)
        {
            if (!_initialized) Initialize();

            var command = new SendCommand((int)MobiFlightModule.Command.SetShiftRegisterPins);

            // Let's strip the static label
            String pinsOnly = outputPins.Replace(LABEL_PREFIX + " ", "");

            // clamp and reverse the string
            if (value.Length > 8) value = value.Substring(0, 8);

            var byteMask = ConvertStringToByteArray(pinsOnly, NumberOfShifters).Reverse();

            command.AddArgument(this.ModuleNumber);
            command.AddArgument(NumberOfShifters);
            command.AddArgument(value);
            byteMask.ToList().ForEach(b => command.AddBinArgument(b));

            Log.Instance.log($"Command: SetShiftRegisterPin <{(int)MobiFlightModule.Command.SetShiftRegisterPins},{this.ModuleNumber},{value},{NumberOfShifters},{string.Join(",", byteMask)} ;>.", LogSeverity.Debug);
            // Send command
            CmdMessenger.SendCommand(command);
        }

        // Convert string to byte array
        // value is a string of 1|2|6|7|8
        public static byte[] ConvertStringToByteArray(string value, int numberOfShifters)
        {
            var selectedPins = value.Split('|').Select(int.Parse).ToList();
            var byteArray = new byte[numberOfShifters];

            selectedPins.ForEach(pin =>
            {
                int shifterIndex = pin / 8;
                int bitIndex = pin % 8;
                if (shifterIndex < numberOfShifters)
                {
                    byteArray[shifterIndex] |= (byte)(1 << bitIndex);
                }
            });
            return byteArray;
        }
    }
}