using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using MobiFlight;
using System.IO;
using System.Data;
using MobiFlight.InputConfig;

namespace MobiFlightUnitTests
{
    [TestClass]
    public class EventIdInputPanelTests
    {
        [TestMethod]
        public void EventIdInputActionSerializesCorrectly()
        {
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(sw, settings);

            Int32 EventId = 321430;
            String Param = "123456";

            EventIdInputAction inputAction = new EventIdInputAction();
            inputAction.EventId = EventId;
            inputAction.Param = Param;
            xmlWriter.WriteStartElement("onPress");
            inputAction.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();
            Assert.AreEqual(
"<?xml version=\"1.0\" encoding=\"utf-16\"?>" + "\r\n" +
"<onPress type=\"EventIdInputAction\" eventId=\"321430\" param=\"123456\" />",
                s,
                "Serialization did not work correctly"
                );
        }

        [TestMethod]
        public void EventIdInputActionDeserializesCorrectly()
        {
            String s =
"<?xml version=\"1.0\" encoding=\"utf-16\"?>" + "\r\n" +
"<onPress type=\"EventIdInputAction\" eventId=\"321430\" param=\"123456\" />";

            StringReader sr = new StringReader(s);
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr);
            EventIdInputAction i = new EventIdInputAction();
            xmlReader.ReadToDescendant("onPress");
            i.ReadXml(xmlReader);
            Assert.AreEqual(321430, i.EventId, "Value of EventId is wrong");
            Assert.AreEqual("123456", i.Param, "Value of Param is wrong");
        }
    }
}
