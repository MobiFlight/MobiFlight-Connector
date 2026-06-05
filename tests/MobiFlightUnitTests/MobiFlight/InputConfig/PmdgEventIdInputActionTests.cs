using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages.Incoming.Converter;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MobiFlight.InputConfig.Tests
{
    [TestClass()]
    public class PmdgEventIdInputActionTests
    {
        private JsonSerializerSettings _serializerSettings;

        [TestInitialize]
        public void Setup()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new InputActionConverter() }
            };
        }

        [TestMethod()]
        public void CloneTest()
        {
            PmdgEventIdInputAction o = generateTestObject();
            PmdgEventIdInputAction c = (PmdgEventIdInputAction)o.Clone();
            Assert.AreNotSame(o, c, "Clone is the same object");
            Assert.AreEqual(c.EventId, o.EventId, "EventId not the same");
            Assert.AreEqual(c.Param, o.Param, "Param not the same");
            Assert.AreEqual(c.AircraftType, o.AircraftType, "Param not the same");
        }

        private PmdgEventIdInputAction generateTestObject()
        {
            PmdgEventIdInputAction o = new PmdgEventIdInputAction();
            o.EventId = Int32.MaxValue;
            o.Param = (UInt32.MaxValue - 1).ToString();
            o.AircraftType = PmdgEventIdInputAction.PmdgAircraftType.B747;

            return o;
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            PmdgEventIdInputAction o = new PmdgEventIdInputAction();
            String s = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\PmdgEventIdInputAction\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("onPress");
            o.ReadXml(xmlReader);

            Assert.AreEqual(Int32.MaxValue, o.EventId, "EventId not the same");
            Assert.AreEqual((UInt32.MaxValue - 1).ToString(), o.Param, "Param not the same");
            Assert.AreEqual(PmdgEventIdInputAction.PmdgAircraftType.B777, o.AircraftType);
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

            PmdgEventIdInputAction o = generateTestObject();
            xmlWriter.WriteStartElement("onPress");
            o.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\MobiFlight\InputConfig\PmdgEventIdInputAction\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void executeTest()
        {
            PmdgEventIdInputAction o = generateTestObject();
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
            Assert.HasCount(1, mock.Writes, "The message count is not as expected");
            Assert.AreEqual("SetEventID>" + o.EventId + ">-2", mock.Writes[0].Value, "The Write Value is wrong");

            mock.Clear();
            // validate config references work
            o.Param = "1+#";
            List<ConfigRefValue> configrefs = new List<ConfigRefValue>();
            configrefs.Add(new ConfigRefValue() { ConfigRef = new Base.ConfigRef() { Active = true, Placeholder = "#" }, Value = "1" });
            o.execute(cacheCollection, null, configrefs);

            Assert.HasCount(1, mock.Writes, "The message count is not as expected");
            Assert.AreEqual("SetEventID>" + o.EventId + ">" + 2, mock.Writes[0].Value, "The Write Value is wrong");
        }

        [TestMethod()]
        public void executeTest_WithParamNull_LogsAndSkips()
        {
            PmdgEventIdInputAction o = generateTestObject();
            MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock mock = new MobiFlightUnitTests.mock.FSUIPC.FSUIPCCacheMock();
            MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock simConnectMock = new MobiFlightUnitTests.mock.SimConnectMSFS.SimConnectCacheMock();
            MobiFlightUnitTests.mock.xplane.XplaneCacheMock xplaneCacheMock = new MobiFlightUnitTests.mock.xplane.XplaneCacheMock();

            // Arrange
            var mockLogAppender = new Mock<ILogAppender>();
            Log.Instance.ClearAppenders();
            Log.Instance.AddAppender(mockLogAppender.Object);
            Log.Instance.Enabled = true;
            Log.Instance.Severity = LogSeverity.Error;

            CacheCollection cacheCollection = new CacheCollection()
            {
                fsuipcCache = mock,
                simConnectCache = simConnectMock,
                moduleCache = null,
                xplaneCache = xplaneCacheMock
            };

            o.Param = null;
            o.execute(cacheCollection, null, new List<ConfigRefValue>());
            Assert.HasCount(0, mock.Writes, "The message count is not as expected");

            // Assert
            var expectedLogMessage = $"Unable to execute {PmdgEventIdInputAction.Label} action for eventId {o.EventId} because the parameter is not defined.";
            mockLogAppender.Verify(
                appender => appender.log(expectedLogMessage, LogSeverity.Error),
                Times.Once,
                "Expected log message should be logged once with Info severity"
            );
        }

        [TestMethod()]
        public void EqualsTest()
        {
            PmdgEventIdInputAction o1 = new PmdgEventIdInputAction();
            PmdgEventIdInputAction o2 = new PmdgEventIdInputAction();

            Assert.IsTrue(o1.Equals(o2));

            o1 = generateTestObject();
            Assert.IsFalse(o1.Equals(o2));

            o2 = generateTestObject();

            Assert.IsTrue(o1.Equals(o2));
        }

        #region JSON Serialization

        [TestMethod()]
        public void JsonSerialize_AircraftType_SerializesAsString()
        {
            // Arrange
            var o = generateTestObject(); // AircraftType = B747

            // Act
            var json = JsonConvert.SerializeObject(o, _serializerSettings);

            // Assert
            Assert.Contains("\"B747\"", json, $"Expected AircraftType to be serialized as \"B747\" but got: {json}");
        }

        [TestMethod()]
        public void JsonDeserialize_AircraftTypeAsString_DeserializesCorrectly_B737()
        {
            // Arrange
            var json = $"{{\"Type\":\"PmdgEventIdInputAction\",\"AircraftType\":\"B737\",\"EventId\":100,\"Param\":\"42\"}}";

            // Act
            var o = JsonConvert.DeserializeObject<PmdgEventIdInputAction>(json, _serializerSettings);

            // Assert
            Assert.AreEqual(PmdgEventIdInputAction.PmdgAircraftType.B737, o.AircraftType);
        }

        [TestMethod()]
        public void JsonDeserialize_AircraftTypeAsString_DeserializesCorrectly_B777()
        {
            // Arrange
            var json = $"{{\"Type\":\"PmdgEventIdInputAction\",\"AircraftType\":\"B777\",\"EventId\":100,\"Param\":\"42\"}}";

            // Act
            var o = JsonConvert.DeserializeObject<PmdgEventIdInputAction>(json, _serializerSettings);

            // Assert
            Assert.AreEqual(PmdgEventIdInputAction.PmdgAircraftType.B777, o.AircraftType);
        }

        [TestMethod()]
        public void JsonDeserialize_AircraftTypeAsString_DeserializesCorrectly_B747()
        {
            // Arrange
            var json = $"{{\"Type\":\"PmdgEventIdInputAction\",\"AircraftType\":\"B747\",\"EventId\":100,\"Param\":\"42\"}}";

            // Act
            var o = JsonConvert.DeserializeObject<PmdgEventIdInputAction>(json, _serializerSettings);

            // Assert
            Assert.AreEqual(PmdgEventIdInputAction.PmdgAircraftType.B747, o.AircraftType);
        }

        [TestMethod()]
        public void JsonDeserialize_AircraftTypeAsInteger_BackwardCompatibility_B737()
        {
            // Arrange - legacy format where enum was stored as integer
            var json = $"{{\"Type\":\"PmdgEventIdInputAction\",\"AircraftType\":0,\"EventId\":100,\"Param\":\"42\"}}";

            // Act
            var o = JsonConvert.DeserializeObject<PmdgEventIdInputAction>(json, _serializerSettings);

            // Assert
            Assert.AreEqual(PmdgEventIdInputAction.PmdgAircraftType.B737, o.AircraftType, "Integer 0 should deserialize to B737 for backward compatibility");
        }

        [TestMethod()]
        public void JsonDeserialize_AircraftTypeAsInteger_BackwardCompatibility_B777()
        {
            // Arrange - legacy format where enum was stored as integer
            var json = $"{{\"Type\":\"PmdgEventIdInputAction\",\"AircraftType\":1,\"EventId\":100,\"Param\":\"42\"}}";

            // Act
            var o = JsonConvert.DeserializeObject<PmdgEventIdInputAction>(json, _serializerSettings);

            // Assert
            Assert.AreEqual(PmdgEventIdInputAction.PmdgAircraftType.B777, o.AircraftType, "Integer 1 should deserialize to B777 for backward compatibility");
        }

        [TestMethod()]
        public void JsonDeserialize_AircraftTypeAsInteger_BackwardCompatibility_B747()
        {
            // Arrange - legacy format where enum was stored as integer
            var json = $"{{\"Type\":\"PmdgEventIdInputAction\",\"AircraftType\":2,\"EventId\":100,\"Param\":\"42\"}}";

            // Act
            var o = JsonConvert.DeserializeObject<PmdgEventIdInputAction>(json, _serializerSettings);

            // Assert
            Assert.AreEqual(PmdgEventIdInputAction.PmdgAircraftType.B747, o.AircraftType, "Integer 2 should deserialize to B747 for backward compatibility");
        }

        [TestMethod()]
        public void JsonRoundTrip_PreservesAllProperties()
        {
            // Arrange
            var original = generateTestObject();

            // Act
            var json = JsonConvert.SerializeObject(original, _serializerSettings);
            var deserialized = JsonConvert.DeserializeObject<PmdgEventIdInputAction>(json, _serializerSettings);

            // Assert
            Assert.AreEqual(original.EventId, deserialized.EventId, "EventId should survive round-trip");
            Assert.AreEqual(original.Param, deserialized.Param, "Param should survive round-trip");
            Assert.AreEqual(original.AircraftType, deserialized.AircraftType, "AircraftType should survive round-trip");
        }

        #endregion
    }
}