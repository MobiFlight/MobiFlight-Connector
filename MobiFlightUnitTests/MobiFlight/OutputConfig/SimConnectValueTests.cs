using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.OutputConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.OutputConfig.Tests
{
    [TestClass()]
    public class SimConnectValueTests
    {
        [TestMethod()]
        public void ReadXmlTest()
        {
            SimConnectValue o = new SimConnectValue();

            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\SimConnectValue\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("source");
            o.ReadXml(xmlReader);

            Assert.AreEqual(SimConnectVarType.CODE, o.VarType);
            Assert.AreEqual("(A:COM RECEIVE:1,Bool)", o.Value);
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(sw, settings);

            SimConnectValue o = new SimConnectValue();
            o.VarType = SimConnectVarType.CODE;
            o.Value = "(A:COM RECEIVE:1,Bool)";

            xmlWriter.WriteStartElement("source");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\OutputConfig\SimConnectValue\WriteXmlTest.1.xml");

            Assert.AreEqual(result, s, "The both strings are not equal");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            SimConnectValue o1 = new SimConnectValue();
            SimConnectValue o2 = new SimConnectValue();
            Assert.IsTrue(o1.Equals(o2));

            o1.VarType = SimConnectVarType.AVAR;
            o1.Value = "(A:COM RECEIVE:1,Bool)";

            Assert.IsFalse(o1.Equals(o2));

            o2.VarType = SimConnectVarType.AVAR;
            o2.Value = "(A:COM RECEIVE:1,Bool)";

            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void CloneTest()
        {
            SimConnectValue o = new SimConnectValue();
            o.VarType = SimConnectVarType.AVAR;
            o.Value = "(A:COM RECEIVE:1,Bool)";

            SimConnectValue clone = o.Clone() as SimConnectValue;

            Assert.IsNotNull(clone);
            Assert.AreEqual(o.VarType, clone.VarType);
            Assert.AreEqual(o.Value, clone.Value);
        }
    }
}