using System;

namespace MobiFlight.Base
{
    /// <summary>
    /// Represents a generic controller device for configuration persistence
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// Gets or sets the name of the controller
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the serial number associated with the controller
        /// </summary>
        public string Serial { get; set; }

        public Controller()
        {
            Name = "";
            Serial = "";
        }

        public Controller(string name, string serial)
        {
            Name = name ?? "";
            Serial = serial ?? "";
        }

        /// <summary>
        /// Creates a Controller from a ModuleSerial string in the format "Name/ Serial"
        /// </summary>
        public static Controller FromModuleSerial(string moduleSerial)
        {
            if (string.IsNullOrEmpty(moduleSerial))
                return new Controller();

            var name = SerialNumber.ExtractDeviceName(moduleSerial);
            var serial = SerialNumber.ExtractSerial(moduleSerial);
            return new Controller(name, serial);
        }

        /// <summary>
        /// Converts this Controller to a ModuleSerial string in the format "Name/ Serial"
        /// </summary>
        public string ToModuleSerial()
        {
            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Serial))
                return "";
            
            if (string.IsNullOrEmpty(Serial))
                return Name;

            return $"{Name}{SerialNumber.SerialSeparator}{Serial}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Controller other))
                return false;

            return Name == other.Name && Serial == other.Serial;
        }

        public override int GetHashCode()
        {
            // Simple hash combining for .NET Framework 4.8
            // Using pattern recommended for older .NET versions
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                hash = hash * 23 + (Serial?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public Controller Clone()
        {
            return new Controller(Name, Serial);
        }
    }
}
