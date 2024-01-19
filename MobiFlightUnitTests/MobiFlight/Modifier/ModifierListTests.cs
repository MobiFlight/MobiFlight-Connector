using MobiFlight.Modifier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
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

            modifierList.Items.Add(new Transformation() { Active = true });

            Assert.IsFalse(modifierList.Equals(modifierList2));

            modifierList2.Items.Add(new Transformation() { Active = true });

            Assert.IsTrue(modifierList.Equals(modifierList2));

            modifierList.Items.Add(new Transformation() { Active = true, Expression = "$+1" });
            Assert.IsFalse (modifierList.Equals(modifierList2));
            modifierList2.Items.Add(new Transformation() { Active = true, Expression = "$+1" });
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

        [TestMethod()]
        public void ContainsModifierTest()
        {
            var modifierList = new ModifierList();
            var modifier1 = new Transformation() { Active = true, Expression = "$+1" };
            var modifier2 = new Transformation() { Active = true, Expression = "$+2" };

            modifierList.Items.Add(modifier1);

            Assert.IsFalse(modifierList.ContainsModifier(modifier2));

            modifierList.Items.Add(modifier2);
            Assert.IsTrue(modifierList.ContainsModifier(modifier2));

            var modifier3 = new Comparison() { Active = true };
            Assert.IsFalse(modifierList.ContainsModifier(modifier3));
        }

        [TestMethod()]
        public void AddModifierOnceTest()
        {
            var modifierList = new ModifierList();
            var modifier1 = new Transformation() { Active = true, Expression = "$+1" };
            Assert.IsTrue(modifierList.Items.Count == 0);
            modifierList.AddModifierOnce(modifier1, true);
            Assert.IsTrue(modifierList.Items.Count == 1);
            modifierList.AddModifierOnce(modifier1, true);
            Assert.IsTrue(modifierList.Items.Count == 1);

            var modifier2 = new Transformation() { Active = true, Expression = "$+2" };
            modifierList.AddModifierOnce(modifier2, true);
            Assert.IsTrue(modifierList.Items.Count == 2);
            Assert.IsTrue(modifierList.Items.First() == modifier2);

            var modifier3 = new Transformation() { Active = true, Expression = "$+3" };
            modifierList.AddModifierOnce(modifier3, false);
            Assert.IsTrue(modifierList.Items.Last() == modifier3);
        }

        [TestMethod()]
        public void EqualsTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReadXmlTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void WriteXmlTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CloneTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ContainsModifierTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddModifierOnceTest1()
        {

        }
    }
}