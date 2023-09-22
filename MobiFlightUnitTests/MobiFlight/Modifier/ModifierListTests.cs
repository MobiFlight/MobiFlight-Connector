using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Modifier;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.Modifier.Tests
{
    [TestClass()]
    public class ModifierListTests
    {
        [TestMethod()]
        public void EqualsTest()
        {
            var modifierList = new ModifierList();
            var modifierList2 = new ModifierList();

            Assert.IsTrue(modifierList.Equals(modifierList2));
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            var o = new ModifierList();
            string s = System.IO.File.ReadAllText(@"assets\MobiFlight\Modifier\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("modifiers");
            o.ReadXml(xmlReader);
            Assert.AreEqual(0, o.Items.Count(), "There should be 0 items in the list.");

            o = new ModifierList();
            s = System.IO.File.ReadAllText(@"assets\MobiFlight\Modifier\ReadXmlTest.2.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("modifiers");
            o.ReadXml(xmlReader);
            Assert.AreEqual(1, o.Items.Count(), "There should be 1 items in the list.");
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            StringWriter sw = new StringWriter();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(sw, settings);

            var o = new ModifierList();
            xmlWriter.WriteStartElement("settings");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            var o1 = new ModifierList();
            StringReader sr = new StringReader(s);
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            var xmlReader = System.Xml.XmlReader.Create(sr, readerSettings);
            xmlReader.ReadToDescendant("modifiers");
            o1.ReadXml(xmlReader);
            Assert.AreEqual(o, o1, "They should be equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            var modifierList = new ModifierList();
            var modifierList2 = modifierList.Clone() as ModifierList;
            Assert.AreEqual(modifierList, modifierList2);
        }
    }
}