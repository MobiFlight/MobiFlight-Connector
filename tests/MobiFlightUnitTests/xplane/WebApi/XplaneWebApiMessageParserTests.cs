using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.xplane.WebApi;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.xplane.WebApi.Tests
{
    [TestClass]
    public class XplaneWebApiMessageParserTests
    {
        [TestMethod]
        public void TryParseDatarefUpdate_ScalarValue_PopulatesFloatValues()
        {
            var json = "{\"type\":\"dataref_update_values\",\"data\":{\"123\":1.5}}";

            var result = XplaneWebApiMessageParser.TryParseDatarefUpdate(json, out var floatValues, out var stringValues);

            Assert.IsTrue(result);
            Assert.IsTrue(floatValues.ContainsKey(123));
            CollectionAssert.AreEqual(new[] { 1.5f }, floatValues[123]);
            Assert.AreEqual(0, stringValues.Count);
        }

        [TestMethod]
        public void TryParseDatarefUpdate_ArrayValue_PopulatesFloatArray()
        {
            var json = "{\"type\":\"dataref_update_values\",\"data\":{\"7\":[1.0,2.0,3.0]}}";

            var result = XplaneWebApiMessageParser.TryParseDatarefUpdate(json, out var floatValues, out var _);

            Assert.IsTrue(result);
            CollectionAssert.AreEqual(new[] { 1.0f, 2.0f, 3.0f }, floatValues[7]);
        }

        [TestMethod]
        public void TryParseDatarefUpdate_StringValue_Base64DecodesIt()
        {
            var json = "{\"type\":\"dataref_update_values\",\"data\":{\"42\":\"QjczOA==\"}}";

            var result = XplaneWebApiMessageParser.TryParseDatarefUpdate(json, out var floatValues, out var stringValues);

            Assert.IsTrue(result);
            Assert.AreEqual(0, floatValues.Count);
            Assert.AreEqual("B738", stringValues[42]);
        }

        [TestMethod]
        public void DecodeBase64Text_TrimsTrailingNulPadding()
        {
            var decoded = XplaneWebApiMessageParser.DecodeBase64Text("QjczOAAAAAA=");

            Assert.AreEqual("B738", decoded);
        }

        [TestMethod]
        public void DecodeBase64Text_NotActuallyBase64_FallsBackToRawValue()
        {
            var decoded = XplaneWebApiMessageParser.DecodeBase64Text("not-valid-base64!!!");

            Assert.AreEqual("not-valid-base64!!!", decoded);
        }

        [TestMethod]
        public void DecodeBase64Text_Empty_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, XplaneWebApiMessageParser.DecodeBase64Text(string.Empty));
        }

        [TestMethod]
        public void TryParseDatarefUpdate_WrongMessageType_ReturnsFalse()
        {
            var json = "{\"type\":\"some_other_message\",\"data\":{\"1\":1.0}}";

            var result = XplaneWebApiMessageParser.TryParseDatarefUpdate(json, out var floatValues, out var stringValues);

            Assert.IsFalse(result);
            Assert.IsNull(floatValues);
            Assert.IsNull(stringValues);
        }

        [TestMethod]
        public void TryParseDatarefUpdate_MalformedJson_ReturnsFalse()
        {
            var result = XplaneWebApiMessageParser.TryParseDatarefUpdate("not json", out var floatValues, out var stringValues);

            Assert.IsFalse(result);
            Assert.IsNull(floatValues);
            Assert.IsNull(stringValues);
        }

        [TestMethod]
        public void TryParseDatarefUpdate_MissingData_ReturnsFalse()
        {
            var json = "{\"type\":\"dataref_update_values\"}";

            var result = XplaneWebApiMessageParser.TryParseDatarefUpdate(json, out var floatValues, out var stringValues);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryParseResult_Success_PopulatesReqIdAndSuccess()
        {
            var json = "{\"req_id\":123,\"type\":\"result\",\"success\":true}";

            var result = XplaneWebApiMessageParser.TryParseResult(json, out var reqId, out var success, out var errorCode, out var errorMessage);

            Assert.IsTrue(result);
            Assert.AreEqual(123, reqId);
            Assert.IsTrue(success);
            Assert.IsNull(errorCode);
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void TryParseResult_Error_PopulatesErrorFields()
        {
            var json = "{\"req_id\":123,\"type\":\"result\",\"success\":false,\"error_code\":\"index_out_of_range\",\"error_message\":\"Index is out of range\"}";

            var result = XplaneWebApiMessageParser.TryParseResult(json, out var reqId, out var success, out var errorCode, out var errorMessage);

            Assert.IsTrue(result);
            Assert.AreEqual(123, reqId);
            Assert.IsFalse(success);
            Assert.AreEqual("index_out_of_range", errorCode);
            Assert.AreEqual("Index is out of range", errorMessage);
        }

        [TestMethod]
        public void TryParseResult_WrongMessageType_ReturnsFalse()
        {
            var json = "{\"type\":\"dataref_update_values\",\"data\":{}}";

            var result = XplaneWebApiMessageParser.TryParseResult(json, out _, out _, out _, out _);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryParseResult_MalformedJson_ReturnsFalse()
        {
            var result = XplaneWebApiMessageParser.TryParseResult("not json", out _, out _, out _, out _);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BuildSubscribeMessage_ProducesExpectedShape()
        {
            var json = XplaneWebApiMessageParser.BuildSubscribeMessage(1, new long[] { 10, 20 });

            var envelope = JObject.Parse(json);

            Assert.AreEqual(1, (int)envelope["req_id"]);
            Assert.AreEqual("dataref_subscribe_values", (string)envelope["type"]);

            var ids = envelope["params"]["datarefs"].Select(d => (long)d["id"]).ToList();
            CollectionAssert.AreEqual(new List<long> { 10, 20 }, ids);
        }

        [TestMethod]
        public void BuildUnsubscribeMessage_ProducesExpectedShape()
        {
            var json = XplaneWebApiMessageParser.BuildUnsubscribeMessage(2, new long[] { 30 });

            var envelope = JObject.Parse(json);

            Assert.AreEqual(2, (int)envelope["req_id"]);
            Assert.AreEqual("dataref_unsubscribe_values", (string)envelope["type"]);

            var ids = envelope["params"]["datarefs"].Select(d => (long)d["id"]).ToList();
            CollectionAssert.AreEqual(new List<long> { 30 }, ids);
        }

        [TestMethod]
        public void BuildSetValuesMessage_SingleValue_OmitsIndex_WhenNotIndexed()
        {
            var json = XplaneWebApiMessageParser.BuildSetValuesMessage(3, 99, 1.5f, null);

            var envelope = JObject.Parse(json);

            Assert.AreEqual(3, (int)envelope["req_id"]);
            Assert.AreEqual("dataref_set_values", (string)envelope["type"]);

            var entry = envelope["params"]["datarefs"][0];
            Assert.AreEqual(99, (long)entry["id"]);
            Assert.AreEqual(1.5f, (float)entry["value"]);
            Assert.IsNull(entry["index"]);
        }

        [TestMethod]
        public void BuildSetValuesMessage_SingleValue_IncludesIndex_WhenIndexed()
        {
            var json = XplaneWebApiMessageParser.BuildSetValuesMessage(4, 99, 2.0f, 3);

            var envelope = JObject.Parse(json);

            var entry = envelope["params"]["datarefs"][0];
            Assert.AreEqual(3, (int)entry["index"]);
        }

        [TestMethod]
        public void BuildSetValuesMessage_Batch_IncludesAllEntries()
        {
            var json = XplaneWebApiMessageParser.BuildSetValuesMessage(5, new (long, float, int?)[]
            {
                (1, 10f, null),
                (2, 20f, 1),
            });

            var envelope = JObject.Parse(json);
            var entries = envelope["params"]["datarefs"].ToList();

            Assert.AreEqual(2, entries.Count);
            Assert.AreEqual(1, (long)entries[0]["id"]);
            Assert.AreEqual(2, (long)entries[1]["id"]);
            Assert.AreEqual(1, (int)entries[1]["index"]);
        }

        [TestMethod]
        public void BuildCommandSetActiveMessage_SingleCommand_OmitsDuration_WhenNotSpecified()
        {
            var json = XplaneWebApiMessageParser.BuildCommandSetActiveMessage(6, 42, true);

            var envelope = JObject.Parse(json);

            Assert.AreEqual(6, (int)envelope["req_id"]);
            Assert.AreEqual("command_set_is_active", (string)envelope["type"]);

            var entry = envelope["params"]["commands"][0];
            Assert.AreEqual(42, (long)entry["id"]);
            Assert.IsTrue((bool)entry["is_active"]);
            Assert.IsNull(entry["duration"]);
        }

        [TestMethod]
        public void BuildCommandSetActiveMessage_SingleCommand_WithZeroDuration_ProducesPressAndReleasePulse()
        {
            var json = XplaneWebApiMessageParser.BuildCommandSetActiveMessage(8, 42, true, duration: 0);

            var envelope = JObject.Parse(json);
            var entry = envelope["params"]["commands"][0];

            Assert.IsTrue((bool)entry["is_active"]);
            Assert.AreEqual(0.0, (double)entry["duration"]);
        }

        [TestMethod]
        public void BuildCommandSetActiveMessage_Batch_IncludesAllEntries()
        {
            var json = XplaneWebApiMessageParser.BuildCommandSetActiveMessage(7, new (long, bool, double?)[]
            {
                (1, true, 0.0),
                (2, false, null),
            });

            var envelope = JObject.Parse(json);
            var entries = envelope["params"]["commands"].ToList();

            Assert.AreEqual(2, entries.Count);
            Assert.IsTrue((bool)entries[0]["is_active"]);
            Assert.AreEqual(0.0, (double)entries[0]["duration"]);
            Assert.IsFalse((bool)entries[1]["is_active"]);
            Assert.IsNull(entries[1]["duration"]);
        }
    }
}
