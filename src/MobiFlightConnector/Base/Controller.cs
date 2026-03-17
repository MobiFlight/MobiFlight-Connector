namespace MobiFlight.Base
{
    /// <summary>
    /// Represents a generic controller device
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// Gets or sets the name of the controller
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the serial number associated with the object.
        /// </summary>
        public string Serial { get; set; }


        public Controller() {}

        public Controller(Controller other)
        {
            if (other == null) return;

            Name = other.Name;
            Serial = other.Serial;
        }

        public object Clone()
        {
            return new Controller(this); 
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = obj as Controller;

            return Name.AreEqual(other.Name) && Serial.AreEqual(other.Serial);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Use null-safe hash code calculation
                hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                hash = hash * 23 + (Serial?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public override string ToString()
        {
            return $"{Name}:{Serial}";
        }
    }
}
