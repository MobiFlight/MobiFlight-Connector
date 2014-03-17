using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public interface IOutputModule
    {
        bool SetPin(string port, string pin, int value);
        bool SetDisplay(string name, int moduleNum, byte mask, string value);
        bool SetServo(string name, int value, int min, int max);
        bool SetStepper(string stepper, int value);
    }
}
