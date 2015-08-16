using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;

namespace MobiFlight
{
    class MobiFlightStepper28BYJ : MobiFlightStepper
    {      
        public MobiFlightStepper28BYJ()
        {
            _name = "Stepper 28BYJ";
            StepperNumber = 0;
            InputRevolutionSteps = 1000;
            OutputRevolutionSteps = 2040;
        }
    }
}
