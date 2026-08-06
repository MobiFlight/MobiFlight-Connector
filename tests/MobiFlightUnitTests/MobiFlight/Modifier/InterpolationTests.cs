using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MobiFlight.Modifier.Tests
{
    [TestClass]
    public class InterpolationTests
    {

        [TestMethod]
        public void CloneTest()
        {
            Interpolation o = new Interpolation();
            Interpolation clone = o.Clone() as Interpolation;

            Assert.AreEqual(o.Max, clone.Max);
            Assert.AreEqual(o.Min, clone.Min);
            Assert.AreEqual(o.Active, clone.Active);
            Assert.AreEqual(o.Count, clone.Count);

            float x1 = 0.1f; float y1 = 0.1f;
            float x3 = 0.5f; float y3 = 2.0f;
            float x2 = 1.0f; float y2 = 1.0f;

            o.Active = true;
            o.Add(x1, y1);
            o.Add(x2, y2);
            o.Add(x3, y3);

            Assert.AreNotEqual(o.Max, clone.Max);
            Assert.AreNotEqual(o.Min, clone.Min);
            Assert.AreNotEqual(o.Active, clone.Active);
            Assert.AreNotEqual(o.Count, clone.Count);

            clone = o.Clone() as Interpolation;

            Assert.AreEqual(o.Max, clone.Max);
            Assert.AreEqual(o.Min, clone.Min);
            Assert.AreEqual(o.Active, clone.Active);
            Assert.AreEqual(o.Count, clone.Count);
        }

        [TestMethod]
        public void EqualsTest()
        {
            Interpolation o1 = new Interpolation();
            Interpolation o2 = new Interpolation();

            Assert.IsTrue(o1.Equals(o2));

            // Verify that Active is used in the equals comparison
            o1.Active = true;
            Assert.IsFalse(o1.Equals(o2));

            // Verify that a list of interpolations is used in the equals comparison
            float x1 = 0.1f; float y1 = 0.1f;
            float x3 = 0.5f; float y3 = 2.0f;
            float x2 = 1.0f; float y2 = 1.0f;
            o1.Add(x1, y1);
            o1.Add(x2, y2);
            o1.Add(x3, y3);
            Assert.IsFalse(o1.Equals(o2));

            o2.Add(x1, y1);
            o2.Add(x2, y2);
            o2.Add(x3, y3);
            o2.Active = true;
            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod]
        public void InterpolationIsDoneCorrectly()
        {
            float x1 = 0.0f; float y1 = 0.0f;
            float x3 = 0.5f; float y3 = 2.0f;
            float x2 = 1.0f; float y2 = 1.0f;

            Interpolation i = new Interpolation();
            i.Add(x1, y1);
            i.Add(x2, y2);
            Assert.AreEqual(0.5f, i.CalculateInterpolatedValue(0.5f), "Interpolation not done correctly");

            // value is on key of a inner item

            i.Add(x3, y3);
            Assert.AreEqual(2.0f, i.CalculateInterpolatedValue(0.5f), "Interpolation not done correctly");

            // value is between firts and second
            // and interpolation is positive
            Assert.AreEqual(1.0f, i.CalculateInterpolatedValue(0.25f), "Interpolation not done correctly");

            // value is between second and third 
            // and interpolation is negative
            Assert.AreEqual(1.5f, i.CalculateInterpolatedValue(0.75f), "Interpolation not done correctly");
        }

        [TestMethod]
        public void Interpolation_Apply_ActiveFalse()
        {
            var interpolation = new Interpolation
            {
                Active = false
            };

            interpolation.Add(0, 0);
            interpolation.Add(10, 20);

            var value= new ConnectorValue
            {
                type = FSUIPCOffsetType.Integer,
                Float64 = 5
            };

            var result = interpolation.Apply(value, new List<ConfigRefValue>());

            Assert.AreSame(value, result);
        }

        [TestMethod]
        public void InterpolationExceptionsAreThrownCorrectly()
        {
            float x1 = 0.0f; float y1 = 0.0f;
            float x2 = 0.0f; float y2 = 1.0f;

            Interpolation i = new Interpolation();
            i.Add(x1, y1);
            try
            {
                i.Add(x2, y2);
                Assert.Fail("The x-value was already registered but no exception was thrown.");
            }
            catch (XvalueAlreadyExistsException) { }
        }

        [TestMethod]
        public void InterpolationSerializesCorrectly()
        {
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(sw, settings);

            float x1 = 0.0f; float y1 = 0.0f;
            float x3 = 0.5f; float y3 = 2.0f;
            float x2 = 1.0f; float y2 = 1.0f;

            Interpolation i = new Interpolation();
            i.Active = true;
            i.Add(x1, y1);
            i.Add(x2, y2);
            i.Add(x3, y3);
            i.WriteXml(xmlWriter);
            xmlWriter.Flush();
            string s = sw.ToString();
            Assert.AreEqual(
"<?xml version=\"1.0\" encoding=\"utf-16\"?>" + "\r\n" +
"<interpolation active=\"True\">" + "\r\n" +
"  <value x=\"0\" y=\"0\" />" + "\r\n" +
"  <value x=\"0.5\" y=\"2\" />" + "\r\n" +
"  <value x=\"1\" y=\"1\" />" + "\r\n" +
"</interpolation>",
                s,
                "Serialization did not work correctly"
                );
        }

        [TestMethod]
        public void InterpolationDeserializesCorrectly()
        {
            String s =
"<?xml version=\"1.0\" encoding=\"utf-16\"?>" + "\r\n" +
"<interpolation active=\"True\">" + "\r\n" +
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
            Interpolation i = new Interpolation();
            xmlReader.ReadToDescendant("interpolation");
            i.ReadXml(xmlReader);
            Assert.IsTrue(i.Active, "Interpolation is not active");
            Assert.AreEqual(3, i.Count, "Number of items in Interpolation wrong");
            Assert.AreEqual(0, i.CalculateInterpolatedValue(0), "Value of interpolation is wrong");
            Assert.AreEqual(2, i.CalculateInterpolatedValue(0.5f), "Value of interpolation is wrong");
            Assert.AreEqual(1, i.CalculateInterpolatedValue(1), "Value of interpolation is wrong");
        }

        [TestMethod]
        public void Apply_String_WithFloatValue_UsesFloatParsing()
        {
            
            var interpolation = new Interpolation();

            interpolation.Add(0f, 0f);
            interpolation.Add(10f, 5f);

            Assert.AreEqual("2.75", interpolation.Apply("5.5"));
        }
    }
}
