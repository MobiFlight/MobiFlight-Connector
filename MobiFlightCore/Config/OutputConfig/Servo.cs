using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.OutputConfig
{
    public class Servo
    {
        public string Address { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string MaxRotationPercent { get; set; }

        public Servo ()
        {

        }

        public void ReadXml(XmlReader reader)
        {
            // ignore empty values
            if (reader["servoAddress"] != null && reader["servoAddress"] != "")
            {
                Address = reader["servoAddress"];
            }
            if (reader["servoMin"] != null && reader["servoMin"] != "")
            {
                Min = reader["servoMin"];
            }
            if (reader["servoMax"] != null && reader["servoMax"] != "")
            {
                Max = reader["servoMax"];
            }

            if (reader["servoMaxRotationPercent"] != null && reader["servoMaxRotationPercent"] != "")
            {
                MaxRotationPercent = reader["servoMaxRotationPercent"];
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("servoAddress", Address);
            writer.WriteAttributeString("servoMin", Min);
            writer.WriteAttributeString("servoMax", Max);
            writer.WriteAttributeString("servoMaxRotationPercent", MaxRotationPercent);
        }

        public Servo Clone()
        {
            Servo clone = new Servo();
            clone.Address = this.Address;
            clone.Max = this.Max;
            clone.Min = this.Min;
            clone.MaxRotationPercent = this.MaxRotationPercent;
            return clone;
        }

        public override bool Equals(object obj)
        {
            return
                (obj != null) && (obj is Servo) &&
                this.Address == (obj as Servo).Address &&
                this.Min == (obj as Servo).Min &&
                this.Max == (obj as Servo).Max &&
                this.MaxRotationPercent == (obj as Servo).MaxRotationPercent;
        }
    }
}
