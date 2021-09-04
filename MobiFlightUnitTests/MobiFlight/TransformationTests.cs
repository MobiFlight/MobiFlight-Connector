using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class TransformationTests
    {
        [TestMethod()]
        public void ApplyTest()
        {
            Transformation t = new Transformation();
            List<ConfigRefValue> configRefs = new List<ConfigRefValue>();

            t.Expression = "$*0.5";
            Assert.AreEqual(1, t.Apply(1, configRefs));
            t.Active = true;
            Assert.AreEqual(0.5, t.Apply(1, configRefs));
            t.Expression = "$*2";
            Assert.AreEqual(2.0, t.Apply(1, configRefs));
            t.Expression = "$*2";
            Assert.AreEqual(2.4, t.Apply(1.2, configRefs));
            t.Expression = "$*2.0";
            Assert.AreEqual(2.0, t.Apply(1, configRefs));
            t.Expression = "Round(14.6,0)";
            Assert.AreEqual(15, t.Apply(1, configRefs));

            // test the substring stuff
            t.SubStrStart = 1;
            t.SubStrEnd = 5;
            t.Active = false;
            string test = "UnitTest";
            Assert.AreEqual(test, t.Apply(test));
            t.Active = true;
            Assert.AreEqual("nitT", t.Apply(test));

            // if SubStrEnd > length 
            t.SubStrEnd = 10;
            Assert.AreEqual("nitTest", t.Apply(test));
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            Transformation o = generateTestObject();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            Transformation o = new Transformation();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\Transformation\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("transformation");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.Active, true, "Active value differs");
            Assert.AreEqual(o.Expression, "$+2", "Expression value differs");
            Assert.AreEqual(o.SubStrStart, 3, "SubStrStartvalue differs");
            Assert.AreEqual(o.SubStrEnd, 10, "SubStrEnd value differs");
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

            Transformation o = generateTestObject();
            xmlWriter.WriteStartElement("settings");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\Transformation\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            Transformation o = generateTestObject();

            Transformation c = (Transformation)o.Clone();
            Assert.AreEqual(o.Active, c.Active, "Active value differs");
            Assert.AreEqual(o.Expression, c.Expression, "Expression value differs");
            Assert.AreEqual(o.SubStrStart, c.SubStrStart, "SubStrStartvalue differs");
            Assert.AreEqual(o.SubStrEnd, c.SubStrEnd, "SubStrEnd value differs");
        }

        private Transformation generateTestObject()
        {
            Transformation t = new Transformation();
            t.Active = true;
            t.Expression = "$+1";
            t.SubStrStart = 1;
            t.SubStrEnd = 5;
            return t;
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Transformation o1 = new Transformation();
            Transformation o2 = new Transformation();
            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();
            Assert.IsTrue(o1.Equals(o2));
        }
    }
}