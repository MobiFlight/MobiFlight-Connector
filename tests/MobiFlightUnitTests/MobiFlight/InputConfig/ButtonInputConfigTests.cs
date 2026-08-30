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
        [DataRow((int)MobiFlightButton.InputEvent.PRESS)]
        [DataRow((int)MobiFlightButton.InputEvent.RELEASE)]
        public void MatchesSyntheticDelay_NoDelayOnEvent_AlwaysMatches(int rawEvent)
        {
            var cfg = new ButtonInputConfig { HoldDelay = 300, RepeatDelay = 100, LongReleaseDelay = 300 };

            var result = cfg.MatchesSyntheticDelay(new InputEventArgs { Value = rawEvent, SyntheticDelayMs = null });

            Assert.IsTrue(result, "PRESS/RELEASE carry no delay to match - always applies.");
        }

        [TestMethod()]
        public void MatchesSyntheticDelay_Hold_ComparesAgainstOwnHoldDelay()
        {
            var cfg = new ButtonInputConfig { onHold = new RecordingInputAction(), HoldDelay = 300 };

            Assert.IsTrue(cfg.MatchesSyntheticDelay(new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.HOLD, SyntheticDelayMs = 300 }));
            Assert.IsFalse(cfg.MatchesSyntheticDelay(new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.HOLD, SyntheticDelayMs = 900 }),
                "A different config's HoldDelay must not match this one.");
        }

        [TestMethod()]
        public void MatchesSyntheticDelay_Repeat_ComparesAgainstOwnHoldAndRepeatDelay()
        {
            var cfg = new ButtonInputConfig { onHold = new RecordingInputAction(), HoldDelay = 300, RepeatDelay = 10 }; // RepeatDelay below the traditional floor - honored as-is, no clamping

            Assert.IsTrue(cfg.MatchesSyntheticDelay(new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.REPEAT, SyntheticDelayMs = 10, SyntheticHoldDelayMs = 300 }));
            Assert.IsFalse(cfg.MatchesSyntheticDelay(new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.REPEAT, SyntheticDelayMs = 100, SyntheticHoldDelayMs = 300 }),
                "A different config's RepeatDelay must not match this one.");
            Assert.IsFalse(cfg.MatchesSyntheticDelay(new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.REPEAT, SyntheticDelayMs = 10, SyntheticHoldDelayMs = 900 }),
                "Same RepeatDelay but a different HoldDelay tier must not match - it's a different config's binding.");
        }

        [TestMethod()]
        public void MatchesSyntheticDelay_NoOnHold_NeverMatchesEvenWithCoincidingDelay()
        {
            // A config with no onHold at all must never match HOLD/REPEAT, even if its leftover
            // (unused) HoldDelay/RepeatDelay field values happen to coincide with another config's
            // real delay - those fields are meaningless without onHold to dispatch to.
            var cfg = new ButtonInputConfig { HoldDelay = 300, RepeatDelay = 10 };

            Assert.IsFalse(cfg.MatchesSyntheticDelay(new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.HOLD, SyntheticDelayMs = 300 }));
            Assert.IsFalse(cfg.MatchesSyntheticDelay(new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.REPEAT, SyntheticDelayMs = 10, SyntheticHoldDelayMs = 300 }));
        }

        [TestMethod()]
        public void MatchesSyntheticDelay_NonHoldRepeatEvent_AlwaysMatches()
        {
            // MatchesSyntheticDelay only gates HOLD/REPEAT - RELEASE's LONG_RELEASE decision is made
            // in ResolveDispatchedEvent instead, from HeldDurationMs, not SyntheticDelayMs.
            var cfg = new ButtonInputConfig { LongReleaseDelay = 200 };

            Assert.IsTrue(cfg.MatchesSyntheticDelay(new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.RELEASE, SyntheticDelayMs = 999 }));
        }

        [TestMethod()]
        public void Execute_ConfigWithoutOnLongRelease_AlwaysDispatchesReleaseRegardlessOfHeldDuration()
        {
            // A latching switch held a long time still arrives as plain RELEASE (see
            // SyntheticButtonEventGenerator.Observe) - without onLongRelease, it never upgrades to
            // LONG_RELEASE, however long HeldDurationMs says it was held. A config that only defines
            // onRelease (the common case before onLongRelease existed) must still fire it.
            var release = new RecordingInputAction();
            ButtonInputConfig cfg = new ButtonInputConfig { onRelease = release, LongReleaseDelay = 300 };

            cfg.execute(new CacheCollection(), new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.RELEASE, HeldDurationMs = 5000 }, new List<ConfigRefValue>());

            Assert.AreEqual(1, release.ExecuteCount);
        }

        [TestMethod()]
        public void Execute_ConfigWithOnLongRelease_FiresLongReleaseNotReleaseWhenDurationExceedsDelay()
        {
            var release = new RecordingInputAction();
            var longRelease = new RecordingInputAction();
            ButtonInputConfig cfg = new ButtonInputConfig { onRelease = release, onLongRelease = longRelease, LongReleaseDelay = 300 };

            cfg.execute(new CacheCollection(), new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.RELEASE, HeldDurationMs = 500 }, new List<ConfigRefValue>());

            Assert.AreEqual(1, longRelease.ExecuteCount);
            Assert.AreEqual(0, release.ExecuteCount, "Held past its own LongReleaseDelay with onLongRelease defined - it, not onRelease, must handle this.");
        }

        [TestMethod()]
        public void Execute_ConfigWithOnLongRelease_DoesNotFireWhenDurationIsUnderDelay()
        {
            var release = new RecordingInputAction();
            var longRelease = new RecordingInputAction();
            ButtonInputConfig cfg = new ButtonInputConfig { onRelease = release, onLongRelease = longRelease, LongReleaseDelay = 300 };

            cfg.execute(new CacheCollection(), new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.RELEASE, HeldDurationMs = 100 }, new List<ConfigRefValue>());

            Assert.AreEqual(1, release.ExecuteCount, "Under its own LongReleaseDelay - a plain release, handled by onRelease.");
            Assert.AreEqual(0, longRelease.ExecuteCount);
        }

        [TestMethod()]
        public void Execute_ConfigWithOnLongRelease_NormalizesDispatchedValueToLongReleaseWithoutMutatingCallerArgs()
        {
            // Several InputAction implementations substitute args.Value into their command (the "@"
            // placeholder) - onLongRelease must see LONG_RELEASE, not the raw RELEASE it upgraded from.
            var longRelease = new RecordingInputAction();
            ButtonInputConfig cfg = new ButtonInputConfig { onLongRelease = longRelease, LongReleaseDelay = 300 };
            var originalArgs = new InputEventArgs { Value = (int)MobiFlightButton.InputEvent.RELEASE, HeldDurationMs = 500 };

            cfg.execute(new CacheCollection(), originalArgs, new List<ConfigRefValue>());

            Assert.AreEqual((double)MobiFlightButton.InputEvent.LONG_RELEASE, longRelease.LastArgs.Value,
                "onLongRelease must see the normalized LONG_RELEASE value.");
            Assert.AreEqual((double)MobiFlightButton.InputEvent.RELEASE, originalArgs.Value,
                "The caller's own args instance must not be mutated - normalization must happen on a clone.");
            Assert.AreNotSame(originalArgs, longRelease.LastArgs, "onLongRelease must receive a clone, not the caller's own args instance.");
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