using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public interface IOutputModule
    {
        bool SetPin(string port, int pin, int value);
        bool SetDisplay(int module, int pos, string value);
        bool SetServo(int servo, int value);
        bool SetStepper(int stepper, int value);
    }
}
