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

        public override string ToString()
        {
            return $"{Name}:{Serial}";
        }
    }
}
