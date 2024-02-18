﻿using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace MobiFlight.Frontend
{
    public class OutputConfigItemAdapter : IConfigItem
    {
        private OutputConfigItem item;
        private string rawValue;
        private string modifiedValue;
        public OutputConfigItemAdapter(OutputConfigItem item) { 
            this.item = item;
        }

        private string DetermineComponent()
        {
            var result = item.Pin.DisplayPin;
            if (item.DisplayType == MobiFlightLedModule.TYPE) return item.LedModule.DisplayLedAddress;
            if (item.DisplayType == MobiFlightLcdDisplay.TYPE) return item.LcdDisplay.Address;
            if (item.DisplayType == MobiFlightServo.TYPE) return item.Servo.Address;
            if (item.DisplayType == MobiFlightStepper.TYPE) return item.Stepper.Address;
            if (item.DisplayType == MobiFlightCustomDevice.TYPE) return item.CustomDevice.CustomName;
            return result;
        }

        public string GUID { get => item.GUID; set => item.GUID = value; }
        public bool Active { get => item.Active; set => item.Active = value; }
        public string Description { get => item.Description; set => item.Description = value; }
        public string Device { get => item.DisplaySerial; set => throw new System.NotImplementedException(); }
        public string Component { get => DetermineComponent(); set => throw new System.NotImplementedException(); }
        public string Type { get => item.DisplayType; set => throw new System.NotImplementedException(); }
        public string[] Tags { get => new List<string>().ToArray(); set => throw new System.NotImplementedException(); }
        public string[] Status { get => new List<string>().ToArray(); set => throw new System.NotImplementedException(); }
        public string RawValue { get => rawValue; set => rawValue = value; }
        public string ModifiedValue { get => modifiedValue; set => modifiedValue = value; }
    }
}