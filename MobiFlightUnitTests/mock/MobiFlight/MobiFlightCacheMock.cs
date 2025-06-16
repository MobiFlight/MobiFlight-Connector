using MobiFlight;
using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobiFlightUnitTests.mock.MobiFlight
{

    class MobiFlightCacheMock : MobiFlightCacheInterface
    {
        Dictionary<string, MobiFlightVariable> variables = new Dictionary<string, MobiFlightVariable>();

        public async Task<IEnumerable<MobiFlightModule>> GetModulesAsync()
        {
            return await Task.Run(() => {
                List<MobiFlightModule> list = new List<MobiFlightModule>()
                {
                    new MobiFlightModule("COM1", new Board())
                };

                    return list;
                }
            );
            
        }

        public IEnumerable<MobiFlightModule> GetModules()
        {
            return new List<MobiFlightModule>() {
                new MobiFlightModule("COM1", new Board())
            };
        }

        public void SetMobiFlightVariable(MobiFlightVariable value)
        {
            variables[value.Name] = value;
        }

        public MobiFlightVariable GetMobiFlightVariable(String name)
        {
            if (!variables.Keys.Contains(name))
            {
                variables[name] = new MobiFlightVariable();
            }

            return variables[name];
        }

        public void Set(string serial, CustomDevice deviceConfig, string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string serial, string name, string value)
        {
            throw new NotImplementedException();
        }

        public void SetDisplay(string serial, string address, byte connector, List<string> digits, List<string> decimalPoints, string value, bool reverse)
        {
            throw new NotImplementedException();
        }

        public void SetDisplayBrightness(string serial, string address, byte connector, string value)
        {
            throw new NotImplementedException();
        }

        public void SetServo(string serial, string address, string value, int min, int max, byte maxRotationPercent)
        {
            throw new NotImplementedException();
        }

        public void SetStepper(string serial, string address, string value, int inputRevolutionSteps, int outputRevolutionSteps, bool CompassMode, short speed = 0, short acceleration = 0)
        {
            throw new NotImplementedException();
        }

        public void ResetStepper(string serial, string address)
        {
            throw new NotImplementedException();
        }

        public void SetLcdDisplay(string serial, LcdDisplay LcdConfig, string value, List<ConfigRefValue> replacements)
        {
            throw new NotImplementedException();
        }

        public void SetShiftRegisterOutput(string serial, string shiftRegName, string outputPin, string value)
        {
            throw new NotImplementedException();
        }
    }
}
