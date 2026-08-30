using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages.Incoming.Converter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass()]
    public class ButtonInputConfigTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            ButtonInputConfig o = generateTestObject();
            ButtonInputConfig c = (ButtonInputConfig)o.Clone();

            Assert.AreNotSame(o, c, "Cloned object is the same");
            Assert.AreEqual((o.onPress as EventIdInputAction).EventId, (c.onPress as EventIdInputAction).EventId, "OnPress is not correct");
            Assert.AreEqual((o.onRelease as JeehellInputAction).EventId, (c.onRelease as JeehellInputAction).EventId, "OnRelease is not correct");
            Assert.AreEqual((o.onLongRelease as MSFS2020CustomInputAction).PresetId, (c.onLongRelease as MSFS2020CustomInputAction).PresetId, "OnLongRelase is not correct");
            Assert.AreEqual((o.onHold as XplaneInputAction).Path, (c.onHold as XplaneInputAction).Path, "onHold is not correct");
            Assert.AreEqual(c.RepeatDelay, o.RepeatDelay, "RepeatDelay is not correct");
            Assert.AreEqual(c.HoldDelay, o.HoldDelay, "HoldDelay is not correct");
            Assert.AreEqual(c.LongReleaseDelay, o.LongReleaseDelay, "LongReleaseDelay is not correct");
        }

        private ButtonInputConfig generateTestObject()
        {            
            ButtonInputConfig o = new ButtonInputConfig();
            o.onPress = new EventIdInputAction() { EventId = 12345 };
            o.onRelease = new JeehellInputAction() { EventId = 127, Param = "123" };
            o.onLongRelease = new MSFS2020CustomInputAction() { Command = "(A:EXTERNAL POWER AVAILABLE:1, Bool)", PresetId = "c1cb32b4-fd35-41ab-8ff7-c407bd407998" };
            o.onHold = new XplaneInputAction() { Expression = "", InputType = "Command", Path = "sim/autopilot/autothrottle_toggle" };
            o.RepeatDelay = 1000;
            o.HoldDelay = 2000;
            o.LongReleaseDelay = 1234;
            return o;
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            ButtonInputConfig o = new ButtonInputConfig();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            ButtonInputConfig o = new ButtonInputConfig();
            String s = File.ReadAllText(@"assets\MobiFlight\InputConfig\ButtonInputConfig\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            XmlReader xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("button");
            o.ReadXml(xmlReader);

            Assert.AreEqual(12345, (o.onPress as EventIdInputAction).EventId, "EventId not the same");
            Assert.AreEqual(127, (o.onRelease as JeehellInputAction).EventId, "EventId not the same");
            Assert.AreEqual("(A:EXTERNAL POWER AVAILABLE:1, Bool)", (o.onLongRelease as MSFS2020CustomInputAction).Command, "Command not the same");

            o = new ButtonInputConfig();
            s = File.ReadAllText(@"assets\MobiFlight\InputConfig\ButtonInputConfig\ReadXmlTest.2.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("button");
            o.ReadXml(xmlReader);

            Assert.AreEqual(12345, (o.onPress as EventIdInputAction).EventId, "EventId not the same");
            Assert.IsNull(o.onRelease, "onRelease not null");
            Assert.IsNull(o.onLongRelease, "onLongRelease not null");

            o = new ButtonInputConfig();
            s = File.ReadAllText(@"assets\MobiFlight\InputConfig\ButtonInputConfig\ReadXmlTest.3.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("button");
            o.ReadXml(xmlReader);

            Assert.AreEqual(12345, (o.onPress as EventIdInputAction).EventId, "EventId not the same");
            Assert.IsNull(o.onRelease, "onRelease not null");
            Assert.IsNull(o.onLongRelease, "onLongRelease not null");

            o = new ButtonInputConfig();
            s = File.ReadAllText(@"assets\MobiFlight\InputConfig\ButtonInputConfig\ReadXmlTest.4.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("button");
            o.ReadXml(xmlReader);

            Assert.AreEqual(12345, (o.onPress as EventIdInputAction).EventId, "EventId not the same");
            Assert.IsNull(o.onRelease, "onRelease not null");
            Assert.AreEqual("c1cb32b4-fd35-41ab-8ff7-c407bd407998", (o.onLongRelease as MSFS2020CustomInputAction).PresetId, "PresetId not the same");

            o = new ButtonInputConfig();
            s = File.ReadAllText(@"assets\MobiFlight\InputConfig\ButtonInputConfig\ReadXmlTest.5.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("button");
            o.ReadXml(xmlReader);

            Assert.IsNull(o.onPress, "onPress not null");
            Assert.IsNull(o.onRelease, "onRelease not null");
            Assert.IsNull(o.onLongRelease, "onLongRelease not null");
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            XmlWriter xmlWriter = XmlWriter.Create(sw, settings);

            ButtonInputConfig o = generateTestObject();
            xmlWriter.WriteStartElement("button");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = File.ReadAllText(@"assets\MobiFlight\InputConfig\ButtonInputConfig\WriteXmlTest.1.xml");

            Assert.AreEqual(result, s, "The both strings are not equal");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            ButtonInputConfig o1 = new ButtonInputConfig();
            ButtonInputConfig o2 = new ButtonInputConfig();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void GetInputActionsByTypeTest()
        {
            ButtonInputConfig cfg = new ButtonInputConfig();
            cfg.onPress = new VariableInputAction();
            cfg.onRelease = new MSFS2020CustomInputAction();
            cfg.onLongRelease = new XplaneInputAction();

            var result = cfg.GetInputActionsByType(typeof(VariableInputAction));
            Assert.HasCount(1, result);
            Assert.AreEqual(typeof(VariableInputAction), result[0].GetType());

            cfg.onPress = new MSFS2020CustomInputAction();
            cfg.onRelease = new VariableInputAction();
            cfg.onLongRelease = new XplaneInputAction();

            result = cfg.GetInputActionsByType(typeof(VariableInputAction));
            Assert.HasCount(1, result);
            Assert.AreEqual(typeof(VariableInputAction), result[0].GetType());

            cfg.onPress = new MSFS2020CustomInputAction();
            cfg.onRelease = new MSFS2020CustomInputAction();
            cfg.onLongRelease = new XplaneInputAction();

            result = cfg.GetInputActionsByType(typeof(VariableInputAction));
            Assert.HasCount(0, result);

            cfg.onPress = new VariableInputAction();
            cfg.onRelease = new VariableInputAction();
            cfg.onLongRelease = new VariableInputAction();

            result = cfg.GetInputActionsByType(typeof(VariableInputAction));
            Assert.HasCount(3, result);
            Assert.AreEqual(typeof(VariableInputAction), result[0].GetType());
            Assert.AreEqual(typeof(VariableInputAction), result[1].GetType());
            Assert.AreEqual(typeof(VariableInputAction), result[2].GetType());
        }

        // execute() is now pure dispatch to the matching InputAction - see SyntheticButtonEventGeneratorTests for detection.

        private class RecordingInputAction : InputAction
        {
            public int ExecuteCount = 0;
            public InputEventArgs LastArgs;

            public override void execute(CacheCollection cacheCollection, InputEventArgs args, List<ConfigRefValue> configRefs)
            {
                ExecuteCount++;
                LastArgs = args;
            }

            public override object Clone() => new RecordingInputAction();
            public override void ReadXml(XmlReader reader) { }
            public override void WriteXml(XmlWriter writer) { }
        }

        [TestMethod()]
        public void Execute_DispatchesToMatchingActionOnly()
        {
            var press = new RecordingInputAction();
            var release = new RecordingInputAction();
            var longRelease = new RecordingInputAction();
            var hold = new RecordingInputAction();

            ButtonInputConfig cfg = new ButtonInputConfig
            {
                onPress = press,
                onRelease = release,
                onLongRelease = longRelease,
                onHold = hold
            };

            void ExecuteWith(MobiFlightButton.InputEvent value)
            {
                cfg.execute(new CacheCollection(), new InputEventArgs { Value = (int)value }, new List<ConfigRefValue>());
            }

            ExecuteWith(MobiFlightButton.InputEvent.PRESS);
            Assert.AreEqual(1, press.ExecuteCount);
            Assert.AreEqual(0, release.ExecuteCount + longRelease.ExecuteCount + hold.ExecuteCount);

            ExecuteWith(MobiFlightButton.InputEvent.RELEASE);
            Assert.AreEqual(1, release.ExecuteCount);

            ExecuteWith(MobiFlightButton.InputEvent.LONG_RELEASE);
            Assert.AreEqual(1, longRelease.ExecuteCount);

            ExecuteWith(MobiFlightButton.InputEvent.HOLD);
            Assert.AreEqual(1, hold.ExecuteCount);

            // REPEAT has no binding of its own - it dispatches to onHold, same as HOLD.
            ExecuteWith(MobiFlightButton.InputEvent.REPEAT);
            Assert.AreEqual(2, hold.ExecuteCount);
        }

        [TestMethod()]
        public void Execute_LongReleaseWithoutOwnAction_FallsBackToRelease()
        {
            // A latching switch held past LongReleaseDelay is reclassified LONG_RELEASE. A config
            // that only defines onRelease (the common case before onLongRelease existed) must still
            // fire it - it must not go silent just because onLongRelease isn't configured.
            var release = new RecordingInputAction();
            ButtonInputConfig cfg = new ButtonInputConfig { onRelease = release };

            cfg.execute(new CacheCollection(), new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.LONG_RELEASE }, new List<ConfigRefValue>());

            Assert.AreEqual(1, release.ExecuteCount);
        }

        [TestMethod()]
        public void Execute_LongReleaseWithOwnAction_DoesNotAlsoFireRelease()
        {
            var release = new RecordingInputAction();
            var longRelease = new RecordingInputAction();
            ButtonInputConfig cfg = new ButtonInputConfig { onRelease = release, onLongRelease = longRelease };

            cfg.execute(new CacheCollection(), new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.LONG_RELEASE }, new List<ConfigRefValue>());

            Assert.AreEqual(1, longRelease.ExecuteCount);
            Assert.AreEqual(0, release.ExecuteCount, "onLongRelease is configured, so it - not onRelease - must handle this.");
        }

        [TestMethod()]
        public void Execute_LongReleaseWithoutOwnAction_NormalizesDispatchedValueToRelease()
        {
            // Several InputAction implementations substitute args.Value into their command (the "@"
            // placeholder) - onRelease must see RELEASE, not the raw LONG_RELEASE it fell back from.
            var release = new RecordingInputAction();
            ButtonInputConfig cfg = new ButtonInputConfig { onRelease = release };
            var originalArgs = new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.LONG_RELEASE };

            cfg.execute(new CacheCollection(), originalArgs, new List<ConfigRefValue>());

            Assert.AreEqual((double)MobiFlightButton.InputEvent.RELEASE, release.LastArgs.Value,
                "onRelease must see a normalized RELEASE value, not the raw LONG_RELEASE it fell back from.");
            Assert.AreEqual((double)MobiFlightButton.InputEvent.LONG_RELEASE, originalArgs.Value,
                "The caller's own args instance must not be mutated - normalization must happen on a clone.");
            Assert.AreNotSame(originalArgs, release.LastArgs, "onRelease must receive a clone, not the caller's own args instance.");
        }

        [TestMethod()]
        public void Execute_WithNoMatchingAction_DoesNotThrow()
        {
            ButtonInputConfig cfg = new ButtonInputConfig();
            cfg.execute(new CacheCollection(), new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.HOLD }, new List<ConfigRefValue>());
        }

        [TestMethod()]
        public void JsonSerializationTest()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<JsonConverter> { new InputActionConverter() }
            };

            var original = generateTestObject();
            string json = JsonConvert.SerializeObject(original, serializerSettings);
            var deserialized = JsonConvert.DeserializeObject<ButtonInputConfig>(json, serializerSettings);

            Assert.AreEqual(original, deserialized);
        }
    }
}