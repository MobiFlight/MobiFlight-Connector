using MobiFlight.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MobiFlight
{
    [XmlRoot("MobiFlightConnector")]
    public class DeprecatedConfigFile : IConfigFile
    {
        [XmlElement("outputs")]
        [JsonIgnore]
        public List<OutputConfigItem> OutputConfigItems { get; set; } = new List<OutputConfigItem>();

        [XmlElement("inputs")]
        [JsonIgnore]
        public List<InputConfigItem> InputConfigItems { get; set; } = new List<InputConfigItem>();

        [XmlIgnore]
        public String FileName { get; set; }

        // create read only property to get the output config items
        [XmlIgnore]
        public List<IConfigItem> ConfigItems
        {
            get { return GetConfigItems(); }
            set { throw new NotImplementedException(); }
        }

        System.Xml.XmlDocument xmlConfig = new System.Xml.XmlDocument();

        public DeprecatedConfigFile() { }

        public DeprecatedConfigFile(String FileName)
        {
            this.FileName = FileName;
        }
        public void OpenFile()
        {
            if (FileName == null) throw new Exception("File yet not set");

            xmlConfig.Load(FileName);
            OutputConfigItems = GetOutputConfigItems();
            InputConfigItems = GetInputConfigItems();
        }
        public List<IConfigItem> GetConfigItems()
        {
            var result = new List<IConfigItem>();
            OutputConfigItems.ForEach(item => result.Add(item));
            InputConfigItems.ForEach(item => result.Add(item));

            return result;
        }

        protected List<OutputConfigItem> GetOutputConfigItems()
        {
            List<OutputConfigItem> result = new List<OutputConfigItem>();

            XmlNodeList outputs = xmlConfig.DocumentElement.SelectNodes("outputs/config");
            foreach (XmlNode item in outputs)
            {
                OutputConfigItem config = new OutputConfigItem();
                config.GUID = item.Attributes["guid"].Value;
                config.Active = item.SelectSingleNode("active").InnerText == "true";
                config.Name = item.SelectSingleNode("description").InnerText;
                try
                {
                    if (item.SelectSingleNode("settings") == null)
                    {
                        throw new Exception("Expected settings not available for XML node.");
                    }

                    System.IO.StringReader reader = new System.IO.StringReader(item.SelectSingleNode("settings").OuterXml);
                    System.Xml.XmlReader xReader = System.Xml.XmlReader.Create(reader);
                    config.ReadXml(xReader);
                }
                catch (Exception e)
                {
                    Log.Instance.log($"Error on reading XML config item {config.Name} / ({config.GUID}): {e.Message}", LogSeverity.Error);
                }
                finally
                {
                    result.Add(config);
                }
            }

            return result;
        }

        protected List<InputConfigItem> GetInputConfigItems()
        {
            List<InputConfigItem> result = new List<InputConfigItem>();

            XmlNodeList inputs = xmlConfig.DocumentElement.SelectNodes("inputs/config");
            foreach (XmlNode item in inputs)
            {
                InputConfigItem config = new InputConfigItem();
                config.GUID = item.Attributes["guid"].Value;
                config.Active = item.SelectSingleNode("active").InnerText == "true";
                config.Name = item.SelectSingleNode("description").InnerText;

                try
                {
                    if (item.SelectSingleNode("settings") == null)
                    {
                        throw new Exception("Expected settings not available for XML node.");
                    }
                    System.IO.StringReader reader = new System.IO.StringReader(item.SelectSingleNode("settings").OuterXml);
                    System.Xml.XmlReader xReader = System.Xml.XmlReader.Create(reader);
                    xReader.Read();
                    config.ReadXml(xReader);
                }
                catch (Exception e)
                {
                    Log.Instance.log($"Error on reading XML config item {config.Name} / ({config.GUID}): {e.Message}", LogSeverity.Error);
                }
                finally
                {
                    result.Add(config);
                }
            }

            return result;
        }

        public void SaveFile()
        {
            SaveFile(OutputConfigItems, InputConfigItems);
        }

        private void SaveFile(List<OutputConfigItem> outputConfigItems, List<InputConfigItem> inputConfigItems)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigFileWrapperXML));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            //namespaces.Add("xsd", "https://www.w3.org/2001/XMLSchema");

            var XmlConfig = new ConfigFileWrapperXML();
            XmlConfig.outputConfigs = new List<OutputConfigFileXmlElement>();
            outputConfigItems.ForEach(item => XmlConfig.outputConfigs.Add(new OutputConfigFileXmlElement() { guid = item.GUID, active = item.Active, description = item.Name, settings = item }));
            XmlConfig.inputConfigs = new List<InputConfigFileXmlElement>();
            inputConfigItems.ForEach(item => XmlConfig.inputConfigs.Add(new InputConfigFileXmlElement() { guid = item.GUID, active = item.Active, description = item.Name, settings = item }));

            using (StreamWriter writer = new StreamWriter(FileName))
            {
                serializer.Serialize(writer, XmlConfig, namespaces);
            }
        }

        public bool ReferenceOnly => false;

        public bool EmbedContent => false;

        // <summary>
        // due to the new settings-node there must be some routine to load 
        // data from legacy config files
        // </summary>
        //private void _applyBackwardCompatibilityLoading()
        //{
        //    foreach (DataRow row in outputConfigPanel.ConfigDataTable.Rows)
        //    {
        //        if (row["settings"].GetType() == typeof(System.DBNull))
        //        {
        //            OutputConfigItem cfgItem = new OutputConfigItem();

        //            if (row["fsuipcOffset"].GetType() != typeof(System.DBNull))
        //                cfgItem.FSUIPC.Offset = Int32.Parse(row["fsuipcOffset"].ToString().Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);

        //            if (row["fsuipcSize"].GetType() != typeof(System.DBNull))
        //                cfgItem.FSUIPC.Size = Byte.Parse(row["fsuipcSize"].ToString());

        //            if (row["mask"].GetType() != typeof(System.DBNull))
        //                cfgItem.FSUIPC.Mask = (row["mask"].ToString() != "") ? Int32.Parse(row["mask"].ToString()) : Int32.MaxValue;

        //             comparison
        //            if (row["comparison"].GetType() != typeof(System.DBNull))
        //            {
        //                cfgItem.Modifiers.Comparison.Active = true;
        //                cfgItem.Modifiers.Comparison.Operand = row["comparison"].ToString();
        //            }

        //            if (row["comparisonValue"].GetType() != typeof(System.DBNull))
        //            {
        //                cfgItem.Modifiers.Comparison.Value = row["comparisonValue"].ToString();
        //            }

        //            if (row["converter"].GetType() != typeof(System.DBNull))
        //            {
        //                if (row["converter"].ToString() == "Boolean")
        //                {
        //                    cfgItem.Modifiers.Comparison.IfValue = "1";
        //                    cfgItem.Modifiers.Comparison.ElseValue = "0";
        //                }
        //            }

        //            if (row["trigger"].GetType() != typeof(System.DBNull))
        //            {
        //                cfgItem.DisplayTrigger = row["trigger"].ToString();
        //            }

        //            if (row["usbArcazePin"].GetType() != typeof(System.DBNull))
        //            {
        //                cfgItem.DisplayType = MobiFlightOutput.TYPE;
        //                cfgItem.Pin.DisplayPin = row["usbArcazePin"].ToString();
        //            }

        //            if (row["arcazeSerial"].GetType() != typeof(System.DBNull))
        //            {
        //                cfgItem.DisplaySerial = row["arcazeSerial"].ToString();
        //            }

        //            row["settings"] = cfgItem;
        //        }
        //    }
        //}
    }
}