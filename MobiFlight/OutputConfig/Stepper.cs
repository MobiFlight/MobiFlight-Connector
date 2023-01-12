using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.OutputConfig
{
    public class Stepper : IXmlSerializable, ICloneable
    {
        public const string Type = "Stepper";
        public String Address { get; set; }
        public Int16 InputRev { get; set; }
        public Int16 TestValue { get; set; }
        public Int16 OutputRev { get; set; }
        public bool CompassMode { get; set; }
        public Int16 Acceleration { get; set; }
        public Int16 Speed { get; set; }

        public Stepper()
        {
            Address = "";
            InputRev = 0;
            OutputRev = 0;
            TestValue = 500;
            CompassMode = false;
            Acceleration = 0;
            Speed = 0;
        }

        public object Clone()
        {
            Stepper clone = new Stepper();
            clone.Address       = (String) Address.Clone();
            clone.InputRev      = InputRev;
            clone.OutputRev     = OutputRev;
            clone.TestValue     = TestValue;
            clone.CompassMode   = CompassMode;
            clone.Speed         = Speed;
            clone.Acceleration  = Acceleration;

            return clone;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Stepper &&
                this.Address == (obj as Stepper).Address &&
                this.InputRev == (obj as Stepper).InputRev &&
                this.TestValue == (obj as Stepper).TestValue &&
                this.OutputRev == (obj as Stepper).OutputRev &&
                this.CompassMode == (obj as Stepper).CompassMode &&
                this.Acceleration == (obj as Stepper).Acceleration &&
                this.Speed == (obj as Stepper).Speed
                ;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // ignore empty values
            if (reader["stepperAddress"] != null && reader["stepperAddress"] != "")
            {
                Address = reader["stepperAddress"];
            }

            if (reader["stepperInputRev"] != null && reader["stepperInputRev"] != "")
            {
                InputRev = Int16.Parse(reader["stepperInputRev"]);
                TestValue = Int16.Parse(reader["stepperInputRev"]);
            }

            if (reader["stepperOutputRev"] != null && reader["stepperOutputRev"] != "")
            {
                OutputRev = Int16.Parse(reader["stepperOutputRev"]);
            }

            if (reader["stepperTestValue"] != null && reader["stepperTestValue"] != "")
            {
                TestValue = Int16.Parse(reader["stepperTestValue"]);
            }

            if (reader["stepperCompassMode"] != null && reader["stepperCompassMode"] != "")
            {
                CompassMode = bool.Parse(reader["stepperCompassMode"]);
            }

            if (reader["stepperAcceleration"] != null && reader["stepperAcceleration"] != "")
            {
                Acceleration = Int16.Parse(reader["stepperAcceleration"]);
            }

            if (reader["stepperSpeed"] != null && reader["stepperSpeed"] != "")
            {
                Speed = Int16.Parse(reader["stepperSpeed"]);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("stepperAddress", Address);
            writer.WriteAttributeString("stepperInputRev", InputRev.ToString());
            writer.WriteAttributeString("stepperOutputRev", OutputRev.ToString());
            writer.WriteAttributeString("stepperTestValue", TestValue.ToString());
            writer.WriteAttributeString("stepperCompassMode", CompassMode.ToString());
            writer.WriteAttributeString("stepperAcceleration", Acceleration.ToString());
            writer.WriteAttributeString("stepperSpeed", Speed.ToString());
        }
    }
}
