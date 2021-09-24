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
        public String InputRev { get; set; }
        public String TestValue { get; set; }
        public String OutputRev { get; set; }
        public bool CompassMode { get; set; }

        public Stepper()
        {
            Address = "";
            InputRev = "1000";
            OutputRev = "2040";
            TestValue = "500";
            CompassMode = false;
        }

        public object Clone()
        {
            Stepper clone = new Stepper();
            clone.Address       = (String) Address.Clone();
            clone.InputRev      = (String) InputRev.Clone();
            clone.OutputRev     = (String) OutputRev.Clone();
            clone.TestValue     = (String) TestValue.Clone();
            clone.CompassMode   = CompassMode;
            return clone;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Stepper &&
                this.Address == (obj as Stepper).Address &&
                this.InputRev == (obj as Stepper).InputRev &&
                this.TestValue == (obj as Stepper).TestValue &&
                this.OutputRev == (obj as Stepper).OutputRev &&
                this.CompassMode == (obj as Stepper).CompassMode
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
                InputRev = reader["stepperInputRev"];
                TestValue = reader["stepperInputRev"];
            }
            if (reader["stepperOutputRev"] != null && reader["stepperOutputRev"] != "")
            {
                OutputRev = reader["stepperOutputRev"];
            }
            if (reader["stepperTestValue"] != null && reader["stepperTestValue"] != "")
            {
                TestValue = reader["stepperTestValue"];
            }

            if (reader["stepperCompassMode"] != null && reader["stepperCompassMode"] != "")
            {
                CompassMode = bool.Parse(reader["stepperCompassMode"]);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("stepperAddress", Address);
            writer.WriteAttributeString("stepperInputRev", InputRev);
            writer.WriteAttributeString("stepperOutputRev", OutputRev);
            writer.WriteAttributeString("stepperTestValue", TestValue);
            writer.WriteAttributeString("stepperCompassMode", CompassMode.ToString());
        }
    }
}
