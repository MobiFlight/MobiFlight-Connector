using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;

namespace MobiFlight
{
    public class ConfigFile
    {
        public String FileName { get; set; }
        System.Xml.XmlDocument xmlConfig = new System.Xml.XmlDocument();

        public ConfigFile(String FileName)
        {
            this.FileName = FileName;
        }

        public void OpenFile()
        {
            if (FileName == null) throw new Exception("File yet not set");
            xmlConfig.Load(FileName);
        }

        public void SaveFile(DataSet outputConfig, DataSet inputConfig)
        {
            xmlConfig.RemoveAll();
            XmlDeclaration xmlDeclaration = xmlConfig.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            xmlConfig.InsertBefore(xmlDeclaration, xmlConfig.DocumentElement);
            
            XmlElement root = xmlConfig.CreateElement("MobiflightConnector");
            // Create a new element and add it to the document.
            StringWriter sw = new StringWriter();
            outputConfig.WriteXml(sw, XmlWriteMode.IgnoreSchema);
            string s = sw.ToString();
            XmlDocument tmpDoc = new XmlDocument();
            tmpDoc.LoadXml(s);

            XmlElement outputs = xmlConfig.CreateElement("outputs");
            outputs.InnerXml = tmpDoc.DocumentElement.SelectSingleNode("/outputs").InnerXml;

            sw = new StringWriter();
            inputConfig.WriteXml(sw, XmlWriteMode.IgnoreSchema);
            s = sw.ToString();
            tmpDoc = new XmlDocument();
            tmpDoc.LoadXml(s);

            XmlElement inputs = xmlConfig.CreateElement("inputs");
            inputs.InnerXml = tmpDoc.DocumentElement.SelectSingleNode("/inputs").InnerXml;

            root.AppendChild(outputs);
            root.AppendChild(inputs);
            xmlConfig.AppendChild(root);
            xmlConfig.Save(FileName);
        }

        private XmlReader getConfig(String xpath)
        {
            // first try the new way... if this fails try the old way
            System.Xml.XmlNode outputConfig = xmlConfig.DocumentElement.SelectSingleNode(xpath);
            if (outputConfig == null) throw new InvalidExpressionException();
            
            System.IO.StringReader reader = new System.IO.StringReader(outputConfig.OuterXml);
            System.Xml.XmlReader xReader = System.Xml.XmlReader.Create(reader);
            return xReader;
        }

        public XmlReader getInputConfig()
        {
            XmlReader result = null;
            if (xmlConfig.DocumentElement == null) OpenFile();

            try
            {
                result = getConfig("/MobiflightConnector/inputs");
            }
            catch (InvalidExpressionException e)
            {
                throw e;
            }
            return result;
        }

        public XmlReader getOutputConfig()
        {
            XmlReader result = null;
            if (xmlConfig.DocumentElement == null) OpenFile();
            bool fallback = false;
            try
            {
                result = getConfig("/MobiflightConnector/outputs");
            }
            catch (InvalidExpressionException e)
            {
                fallback = true;
            }

            if (fallback)
            {
                fallback = false;
                // fallback for old configs
                try
                {
                    result = getConfig("/MobiflightConnector");
                }
                catch (Exception ex)
                {
                    fallback = true;
                }
            }

            if (fallback)
            {
                fallback = false;
                // fallback for old configs
                try
                {
                    result = getConfig("/ArcazeUsbConnector");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error: Loading config");
                }
            }
          
            return result;
        }
    }
}
