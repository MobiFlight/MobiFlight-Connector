using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ConfigRefTests
    {
        [TestMethod()]
        public void GetSchemaTest()
        {
            ConfigRef o = new ConfigRef();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            ConfigRef o = new ConfigRef();
            String s = System.IO.File.ReadAllText(@"assets\Base\ConfigRef\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("configref");
            o.ReadXml(xmlReader);
            // the first one is empty so the values should not be initialized
            Assert.IsNull(o.Ref, "Ref is not null");
            Assert.IsNull(o.Placeholder, "Placeholder is not null");

            
            o.ReadXml(xmlReader);
            // the second one is set so the values should be initialized
            Assert.AreEqual(o.Ref, "d88beacf-a305-4964-a4be-caf6693e18eb", "Ref is not matching");
            Assert.AreEqual(o.Placeholder, "?", "Placeholder is not matching");

            o.ReadXml(xmlReader);
            // the second one is set so the values should be initialized
            Assert.AreEqual(o.Ref, "2ab3b1b5-fbd2-4b8c-9366-ad948fff8135", "Ref is not matching");
            Assert.AreEqual(o.Placeholder, "#", "Placeholder is not matching");

            o = new ConfigRef(); // we have to make sure to use a new one otherwise 
                                 // it would hold the information from last read
            o.ReadXml(xmlReader);
            // the last one is empty so the values should not be initialized
            Assert.IsNull(o.Ref, "Ref is not null");
            Assert.IsNull(o.Placeholder, "Placeholder is not null");
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

            ConfigRef o = new ConfigRef();
            o.Placeholder = "#";
            o.Ref = "Ref123";
            o.Active = true;

            xmlWriter.WriteStartElement("configrefs");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\Base\ConfigRef\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            ConfigRef o = new ConfigRef();
            o.Active = true;
            o.Ref = "Ref123";
            o.Placeholder = "#";

            ConfigRef c = o.Clone() as ConfigRef;

            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(o.Active, c.Active, "Active not the same");
            Assert.AreEqual(o.Ref, c.Ref, "Ref not the same");
            Assert.AreEqual(o.Placeholder, c.Placeholder, "Placeholder not the same");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            ConfigRef o1 = new ConfigRef();
            ConfigRef o2 = new ConfigRef();
            
            Assert.IsTrue(o1.Equals(o2));
            o1.Placeholder = "#";
            o1.Ref = "Ref123";
            o1.Active = true;
            Assert.IsFalse(o1.Equals(o2));

            o2.Placeholder = "#";
            o2.Ref = "Ref123";
            o2.Active = true;
            Assert.IsTrue(o1.Equals(o2));
        }
    }
}