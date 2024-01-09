using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace MobiFlight.Modifier.Tests
{
    [TestClass()]
    public class SubstringTests
    {
        private Substring generateTestObject()
        {
            var o = new Substring();
            o.Active = true;
            o.Start = 1;
            o.End = 4;
            return o;
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            var o = new Substring();
            var s = File.ReadAllText(@"assets\MobiFlight\Modifier\Substring\ReadXmlTest.1.xml");
            var sr = new StringReader(s);
            var settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            var xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("substring");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.Active, true, "Active value differs");
            Assert.AreEqual(o.Start, 3, "Expression value differs");
            Assert.AreEqual(o.End, 10, "Expression value differs");
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            var sw = new StringWriter();
            var settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            var xmlWriter = XmlWriter.Create(sw, settings);

            var o = generateTestObject();
            o.Active = false;
            xmlWriter.WriteStartElement("settings");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            var s = sw.ToString();

            var result = File.ReadAllText(@"assets\MobiFlight\Modifier\Substring\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void CloneTest()
        {
            var o = generateTestObject();

            var c = (Substring)o.Clone();
            Assert.AreEqual(o.Active, c.Active, "Active value differs");
            Assert.AreEqual(o.Start, c.Start, "Start value differs");
            Assert.AreEqual(o.End, c.End, "End value differs");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            var o1 = new Substring();
            var o2 = new Substring();
            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();
            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void ApplyTest()
        {
            var o = generateTestObject();
            o.Active = true;
            o.Start = 0;
            o.End = 3;

            var value = new ConnectorValue() { type = FSUIPCOffsetType.String, String = "012345" };
            var configRefs = new System.Collections.Generic.List<ConfigRefValue>();
            Assert.AreEqual("0123", o.Apply(value, configRefs).String);

            o.Active = false;
            Assert.AreEqual(value.String, o.Apply(value, configRefs).String);

            o.Active = true;
            o.Start = 6;
            o.End = 7;
            Assert.AreEqual("", o.Apply(value, configRefs).String);

            // this doesn't make sense
            // it should be prevented by the UI
            // but we will return empty string in this case
            o.Active = true;
            o.Start = 2;
            o.End = 5;
            Assert.AreEqual("2345", o.Apply(value, configRefs).String);

            // this doesn't make sense
            // it should be prevented by the UI
            // but we will return empty string in this case
            o.Active = true;
            o.Start = 5;
            o.End = 2;
            Assert.AreEqual("", o.Apply(value, configRefs).String);
        }

        [TestMethod()]
        public void ToSummaryLabelTest()
        {
            var o = generateTestObject();
            Assert.AreEqual($"Substring: from {o.Start} to {o.End}", o.ToSummaryLabel());
        }
    }
}