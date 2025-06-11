using System;
using System.Collections.Generic;

namespace MobiFlight
{
    public interface MobiFlightCacheInterface
    {
        // Task<IEnumerable<MobiFlightModule>> GetModulesAsync();
        IEnumerable<MobiFlightModule> GetModules();

        void SetMobiFlightVariable(MobiFlightVariable value);

        MobiFlightVariable GetMobiFlightVariable(String name);

        void Set(string serial, OutputConfig.CustomDevice deviceConfig, string value);
        void SetValue(string serial, string name, string value);
        void SetDisplay(string serial, string address, byte connector, List<string> digits, List<string> decimalPoints, string value, bool reverse);
        void SetDisplayBrightness(string serial, string address, byte connector, string value);
        void SetServo(string serial, string address, string value, int min, int max, byte maxRotationPercent);
        void SetStepper(string serial, string address, string value, int inputRevolutionSteps, int outputRevolutionSteps, bool CompassMode, Int16 speed = 0, Int16 acceleration = 0);
        void ResetStepper(string serial, string address);
        void SetLcdDisplay(string serial, OutputConfig.LcdDisplay LcdConfig, string value, List<ConfigRefValue> replacements);
        void SetShiftRegisterOutput(string serial, string shiftRegName, string outputPin, string value);
    }
}
