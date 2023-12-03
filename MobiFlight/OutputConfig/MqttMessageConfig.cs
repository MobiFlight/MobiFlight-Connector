using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.OutputConfig
{
    public class MqttMessageConfig
    {
        public const string TYPE = "MQTT Message";

        public string Topic { get; set; }
        public string ValuePrefix { get; set; }

        public MqttMessageConfig()
        {
            Topic = "";
        }

        public override bool Equals(object obj)
        {
            return (obj != null) &&
                (obj is MqttMessageConfig) &&
                this.Topic == (obj as MqttMessageConfig).Topic &&
                this.ValuePrefix == (obj as MqttMessageConfig).ValuePrefix;
        }

        public void ReadXml(XmlReader reader)
        {
            if (!String.IsNullOrEmpty(reader["topic"]))
            {
                Topic = reader["topic"];
                ValuePrefix = reader["valuePrefix"];
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("topic", Topic);
            writer.WriteAttributeString("valuePrefix", ValuePrefix);
        }

        public override string ToString()
        {
            return Topic;
        }

        public object Clone()
        {
            return new MqttMessageConfig()
            {
                Topic = Topic,
                ValuePrefix = ValuePrefix
            };
        }
    }
}