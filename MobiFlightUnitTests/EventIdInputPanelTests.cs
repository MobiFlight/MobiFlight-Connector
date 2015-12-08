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
            Int32 Param = 123456;

            EventIdInputAction inputAction = new EventIdInputAction();
            inputAction.EventId = EventId;
            inputAction.Param = Param;

            inputAction.WriteXml(xmlWriter);
            xmlWriter.Flush();
            string s = sw.ToString();
            Assert.AreEqual(
"<?xml version=\"1.0\" encoding=\"utf-16\"?>" + "\r\n" +
"<onPress type=\"EventIdInputAction\" >" + "\r\n" +
"  <value x=\"0\" y=\"0\" />" + "\r\n" +
"  <value x=\"0.5\" y=\"2\" />" + "\r\n" +
"  <value x=\"1\" y=\"1\" />" + "\r\n" +
"</onPress>",
                s,
                "Serialization did not work correctly"
                );
        }

        [TestMethod]
        public void InterpolationDeserializesCorrectly()
        {
            String s =
"<?xml version=\"1.0\" encoding=\"utf-16\"?>" + "\r\n" +
"<interpolation>" + "\r\n" +
"  <value x=\"0\" y=\"0\" />" + "\r\n" +
"  <value x=\"0.5\" y=\"2\" />" + "\r\n" +
"  <value x=\"1\" y=\"1\" />" + "\r\n" +
"</interpolation>";

            StringReader sr = new StringReader(s);
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr);
            EventIdInputAction i = new EventIdInputAction();
            i.ReadXml(xmlReader);
            Assert.AreEqual(123456, i.EventId, "Value of EventId is wrong");
            Assert.AreEqual(654321, i.Param, "Value of Param is wrong");
        }
    }
}
