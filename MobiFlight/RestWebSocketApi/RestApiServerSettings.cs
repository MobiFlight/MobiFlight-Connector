using MobiFlight.Config;
using MobiFlight.Properties;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.RestWebSocketApi
{
    public class RestApiServerSettings : IXmlSerializable
    {


        private bool _hasChanged = false;
        public bool HasChanged { get { return _hasChanged; } }


        public static RestApiServerSettings Load()
        {
            string config = Settings.Default.RestWebsocketServerConfig;
            if (config == "")
            {
                RestApiServerSettings r = new RestApiServerSettings();
                r.restPort = "9998";
                r.websocketPort = "9999";
                r.websocketServerInterface = "127.0.0.1";
                r.restServerInterface = "127.0.0.1";
                return r;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(RestApiServerSettings));
            StringReader w = new StringReader(config);
            return (RestApiServerSettings)serializer.Deserialize(w);
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RestApiServerSettings));
            StringBuilder builder = new StringBuilder();
            StringWriter w = new StringWriter(builder);
            serializer.Serialize(w, this);
            string s = w.ToString();
            Settings.Default.RestWebsocketServerConfig = s;
        }


        string _restPort;
        public string restPort
        {
            get { return _restPort; }
            set
            {
                if (_restPort == value) return;
                _restPort = value;
                _hasChanged = true;
            }
        }

        string _restServerInterface;
        public string restServerInterface
        {
            get { return _restServerInterface; }
            set
            {
                if (_restServerInterface == value) return;
                _restServerInterface = value;
                _hasChanged = true;
            }
        }

        string _websocketPort;
        public string websocketPort
        {
            get { return _websocketPort; }
            set
            {
                if (_websocketPort == value) return;
                _websocketPort = value;
                _hasChanged = true;
            }
        }

        string _websocketServerInterface;
        public string websocketServerInterface
        {
            get { return _websocketServerInterface; }
            set
            {
                if (_websocketServerInterface == value) return;
                _websocketServerInterface = value;
                _hasChanged = true;
            }
        }


        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            websocketServerInterface = reader["websocketServerInterface"];
            websocketPort = reader["websocketPort"];
            restServerInterface = reader["restServerInterface"];
            restPort = reader["restPort"];
            _hasChanged = false;
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("websocketServerInterface", websocketServerInterface);
            writer.WriteAttributeString("restServerInterface", restServerInterface);
            writer.WriteAttributeString("websocketPort", websocketPort);
            writer.WriteAttributeString("restPort", restPort);
        }
    }
}
