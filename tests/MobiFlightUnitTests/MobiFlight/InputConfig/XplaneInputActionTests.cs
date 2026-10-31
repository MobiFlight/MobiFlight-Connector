using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass()]
    public class XplaneInputActionTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            XplaneInputAction o = generateTestObject();
            XplaneInputAction c = (XplaneInputAction)o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(c.InputType, o.InputType, "InputType not the same");
            Assert.AreEqual(c.Path, o.Path, "Path not the same");
            Assert.AreEqual(c.Expression, o.Expression, "Expression not the same");
        }

        private XplaneInputAction generateTestObject()
        {
            XplaneInputAction o = new XplaneInputAction();
            o.InputType = XplaneInputAction.INPUT_TYPE_DATAREF;
            o.Path = "/my/test/path";
            o.Expression = "$+1";

            return o;
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            XplaneInputAction o = new XplaneInputAction();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\XplaneInputAction\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("onPress");
            o.ReadXml(xmlReader);

            Assert.AreEqual("DataRef", o.InputType, "InputType not the same");
            Assert.AreEqual("/my/test/path1", o.Path, "Path not the same");
            Assert.AreEqual("$+2", o.Expression);
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

            XplaneInputAction o = generateTestObject();
            xmlWriter.WriteStartElement("onPress");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\XplaneInputAction\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void executeTest()
        {
            XplaneInputAction o = generateTestObject();
            MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock mock = new MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock();
            MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock simConnectMock = new MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock();
            MobiFlightUnitTests.mock.xplane.XplaneCacheMock xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            CacheCollection cacheCollection = new CacheCollection()
            {
                fsuipcCache = mock,
                simConnectCache = simConnectMock,
                moduleCache = null,
                xplaneCache = xplaneCacheMock
            };

            o.execute(cacheCollection, null, new List<ConfigRefValue>());
            Assert.HasCount(1, xplaneCacheMock.Writes, "The message count is not as expected");

            xplaneCacheMock.Clear();
            // validate config references work
            o.Expression = "1+#";
            List<ConfigRefValue> configrefs = new List<ConfigRefValue>();
            configrefs.Add(new ConfigRefValue() { ConfigRef = new Base.ConfigRef() { Active = true, Placeholder = "#" }, Value = "1" });
            o.execute(cacheCollection, null, configrefs);

            Assert.HasCount(1, xplaneCacheMock.Writes, "The message count is not as expected");
            Assert.AreEqual(2, xplaneCacheMock.Writes[0].Value, "The Write Value is wrong");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            XplaneInputAction o1 = new XplaneInputAction();
            XplaneInputAction o2 = new XplaneInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void TargetsSameCommandAsTest()
        {
            var a = new XplaneInputAction { InputType = XplaneInputAction.INPUT_TYPE_COMMAND, Path = "sim/flight_controls/pitch_trim_up" };
            var sameCommand = new XplaneInputAction { InputType = XplaneInputAction.INPUT_TYPE_COMMAND, Path = "sim/flight_controls/pitch_trim_up" };
            var differentCommand = new XplaneInputAction { InputType = XplaneInputAction.INPUT_TYPE_COMMAND, Path = "sim/flight_controls/pitch_trim_down" };
            var dataRefInstead = new XplaneInputAction { InputType = XplaneInputAction.INPUT_TYPE_DATAREF, Path = "sim/flight_controls/pitch_trim_up" };

            Assert.IsTrue(a.TargetsSameCommandAs(sameCommand), "Same InputType and Path must be considered the same command.");
            Assert.IsFalse(a.TargetsSameCommandAs(differentCommand), "Distinct on/off style command paths must not be paired.");
            Assert.IsFalse(a.TargetsSameCommandAs(dataRefInstead), "A DataRef action is never a command pair, even with a matching Path.");
            Assert.IsFalse(a.TargetsSameCommandAs(null), "No sibling action means nothing to pair with.");
        }

        private static CacheCollection CreateCacheCollectionWith(MobiFlightUnitTests.mock.xplane.XplaneCacheMock xplaneCacheMock)
        {
            return new CacheCollection()
            {
                fsuipcCache = new MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock(),
                simConnectCache = new MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock(),
                moduleCache = null,
                xplaneCache = xplaneCacheMock
            };
        }

        private static InputEventArgs CreateButtonArgs(MobiFlightButton.InputEvent value, bool hasPairedReleaseAction)
        {
            return new InputEventArgs
            {
                InputType = DeviceType.Button,
                Value = (int)value,
                HasPairedReleaseAction = hasPairedReleaseAction
            };
        }

        [TestMethod()]
        public void HandleCommand_Press_Paired_BeginsCommand()
        {
            var o = new XplaneInputAction { InputType = XplaneInputAction.INPUT_TYPE_COMMAND, Path = "sim/flight_controls/pitch_trim_up" };
            var xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            o.execute(CreateCacheCollectionWith(xplaneCacheMock), CreateButtonArgs(MobiFlightButton.InputEvent.PRESS, hasPairedReleaseAction: true), new List<ConfigRefValue>());

            Assert.HasCount(1, xplaneCacheMock.Commands);
            Assert.AreEqual("begin", xplaneCacheMock.Commands[0].Description);
            Assert.AreEqual(o.Path, xplaneCacheMock.Commands[0].Command);
        }

        [TestMethod()]
        public void HandleCommand_Press_Unpaired_SendsOneShotPulse()
        {
            // No onRelease/onLongRelease targeting the same command - e.g. onPress-only, or a distinct
            // on/off command pair - so this must pulse instead of beginning something that never ends.
            var o = new XplaneInputAction { InputType = XplaneInputAction.INPUT_TYPE_COMMAND, Path = "toliss_airbus/eleccommands/Bat1On" };
            var xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            o.execute(CreateCacheCollectionWith(xplaneCacheMock), CreateButtonArgs(MobiFlightButton.InputEvent.PRESS, hasPairedReleaseAction: false), new List<ConfigRefValue>());

            Assert.HasCount(1, xplaneCacheMock.Commands);
            Assert.IsNull(xplaneCacheMock.Commands[0].Description);
            Assert.AreEqual(o.Path, xplaneCacheMock.Commands[0].Command);
        }

        [TestMethod()]
        public void HandleCommand_Release_Paired_EndsCommand()
        {
            var o = new XplaneInputAction { InputType = XplaneInputAction.INPUT_TYPE_COMMAND, Path = "sim/flight_controls/pitch_trim_up" };
            var xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            o.execute(CreateCacheCollectionWith(xplaneCacheMock), CreateButtonArgs(MobiFlightButton.InputEvent.RELEASE, hasPairedReleaseAction: true), new List<ConfigRefValue>());

            Assert.HasCount(1, xplaneCacheMock.Commands);
            Assert.AreEqual("end", xplaneCacheMock.Commands[0].Description);
        }

        [TestMethod()]
        public void HandleCommand_Release_Unpaired_SendsOneShotPulse()
        {
            // The release side of a distinct on/off pair (e.g. Bat1Off) must pulse, not deactivate a
            // command that was never begun.
            var o = new XplaneInputAction { InputType = XplaneInputAction.INPUT_TYPE_COMMAND, Path = "toliss_airbus/eleccommands/Bat1Off" };
            var xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            o.execute(CreateCacheCollectionWith(xplaneCacheMock), CreateButtonArgs(MobiFlightButton.InputEvent.RELEASE, hasPairedReleaseAction: false), new List<ConfigRefValue>());

            Assert.HasCount(1, xplaneCacheMock.Commands);
            Assert.IsNull(xplaneCacheMock.Commands[0].Description);
        }

        [TestMethod()]
        public void HandleCommand_LongRelease_Paired_EndsCommand()
        {
            var o = new XplaneInputAction { InputType = XplaneInputAction.INPUT_TYPE_COMMAND, Path = "sim/flight_controls/pitch_trim_up" };
            var xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            o.execute(CreateCacheCollectionWith(xplaneCacheMock), CreateButtonArgs(MobiFlightButton.InputEvent.LONG_RELEASE, hasPairedReleaseAction: true), new List<ConfigRefValue>());

            Assert.HasCount(1, xplaneCacheMock.Commands);
            Assert.AreEqual("end", xplaneCacheMock.Commands[0].Description);
        }

        [TestMethod()]
        public void HandleCommand_NonButtonDevice_AlwaysSendsOneShotPulse()
        {
            var o = new XplaneInputAction { InputType = XplaneInputAction.INPUT_TYPE_COMMAND, Path = "sim/flight_controls/pitch_trim_up" };
            var xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();
            var args = new InputEventArgs { InputType = DeviceType.Encoder, Value = 1 };

            o.execute(CreateCacheCollectionWith(xplaneCacheMock), args, new List<ConfigRefValue>());

            Assert.HasCount(1, xplaneCacheMock.Commands);
            Assert.IsNull(xplaneCacheMock.Commands[0].Description);
        }
    }
}