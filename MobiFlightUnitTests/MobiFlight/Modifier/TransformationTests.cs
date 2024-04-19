using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace MobiFlight.Modifier.Tests
{
    class TransformationForDeprecatedTest : Transformation
    {
    }

    [TestClass()]
    public class TransformationTests
    {
        [TestMethod()]
        public void ApplyTest()
        {
            TransformationForDeprecatedTest t = new TransformationForDeprecatedTest();
            List<ConfigRefValue> configRefs = new List<ConfigRefValue>();

            // this is needed for correct conversion
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            // Number type
            var ConnectValue = new ConnectorValue() { Float64 = 1 };
            
            t.Expression = "$*0.5";
            Assert.AreEqual(0.5, t.Apply(ConnectValue, configRefs).Float64);
            
            t.Expression = "$*2";
            Assert.AreEqual(2, t.Apply(ConnectValue, configRefs).Float64);

            t.Expression = "$*2.0";
            Assert.AreEqual(2, t.Apply(ConnectValue, configRefs).Float64);

            t.Expression = "$*2";
            ConnectValue.Float64 = 1.2;
            Assert.AreEqual(2.4, t.Apply(ConnectValue, configRefs).Float64);
            
            t.Expression = "Round(14.6,0)";
            Assert.AreEqual(15, t.Apply(ConnectValue, configRefs).Float64);

            // Test strings
            // https://github.com/MobiFlight/MobiFlight-Connector/issues/1348
            t.Expression = "'Hello'";
            ConnectValue.Float64 = 0;
            Assert.AreEqual(FSUIPCOffsetType.String, t.Apply(ConnectValue, configRefs).type);
            Assert.AreEqual("Hello", t.Apply(ConnectValue, configRefs).String);
            Assert.AreEqual(0, t.Apply(ConnectValue, configRefs).Float64);

            // test 0.000 format
            // https://github.com/MobiFlight/MobiFlight-Connector/issues/1628
            t.Expression = "if($=0,'0.000',1)";
            var step1Result = t.Apply(ConnectValue, configRefs);
            Assert.AreEqual("0.000", step1Result.String);
            t.Expression = "if('$'=1,2,'$')";
            Assert.AreEqual("0.000", t.Apply(step1Result, configRefs).String);
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
        }

        private Transformation generateTestObject()
        {
            Transformation t = new Transformation();
            t.Active = true;
            t.Expression = "$+1";
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