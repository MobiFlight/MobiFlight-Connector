using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MobiFlight.xplane.WebApi
{
    internal static class XplaneWebApiMessageParser
    {
        public static bool TryParseDatarefUpdate(string json, out Dictionary<long, float[]> floatValues, out Dictionary<long, string> stringValues)
        {
            floatValues = null;
            stringValues = null;

            JObject envelope;
            try
            {
                envelope = JObject.Parse(json);
            }
            catch (JsonException)
            {
                return false;
            }

            if ((string)envelope["type"] != XplaneWebApiConstants.MessageTypeDatarefUpdateValues)
                return false;

            if (!(envelope["data"] is JObject data))
                return false;

            floatValues = new Dictionary<long, float[]>();
            stringValues = new Dictionary<long, string>();

            foreach (var property in data.Properties())
            {
                if (!long.TryParse(property.Name, out var id)) continue;

                switch (property.Value.Type)
                {
                    case JTokenType.String:
                        stringValues[id] = DecodeBase64Text(property.Value.Value<string>());
                        break;
                    case JTokenType.Array:
                        floatValues[id] = property.Value.ToObject<float[]>();
                        break;
                    case JTokenType.Integer:
                    case JTokenType.Float:
                        floatValues[id] = new[] { property.Value.ToObject<float>() };
                        break;
                }
            }

            return true;
        }

        internal static string DecodeBase64Text(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return string.Empty;

            const int StackAllocCharLimit = 256;
            Span<byte> buffer = base64.Length <= StackAllocCharLimit
                ? stackalloc byte[StackAllocCharLimit / 4 * 3]
                : new byte[(base64.Length / 4 * 3) + 3];

            if (!Convert.TryFromBase64Chars(base64, buffer, out var bytesWritten))
            {
                return base64;
            }

            ReadOnlySpan<byte> decoded = buffer[..bytesWritten];
            var nulIndex = decoded.IndexOf((byte)0);
            if (nulIndex >= 0) decoded = decoded[..nulIndex];

            return Encoding.UTF8.GetString(decoded);
        }

        public static bool TryParseResult(string json, out int reqId, out bool success, out string errorCode, out string errorMessage)
        {
            reqId = 0;
            success = false;
            errorCode = null;
            errorMessage = null;

            JObject envelope;
            try
            {
                envelope = JObject.Parse(json);
            }
            catch (JsonException)
            {
                return false;
            }

            if ((string)envelope["type"] != XplaneWebApiConstants.MessageTypeResult)
                return false;

            reqId = (int?)envelope["req_id"] ?? 0;
            success = (bool?)envelope["success"] ?? false;
            errorCode = (string)envelope["error_code"];
            errorMessage = (string)envelope["error_message"];

            return true;
        }

        public static string BuildSubscribeMessage(int reqId, IEnumerable<long> ids)
        {
            return BuildSubscriptionMessage(XplaneWebApiConstants.MessageTypeDatarefSubscribeValues, reqId, ids);
        }

        public static string BuildUnsubscribeMessage(int reqId, IEnumerable<long> ids)
        {
            return BuildSubscriptionMessage(XplaneWebApiConstants.MessageTypeDatarefUnsubscribeValues, reqId, ids);
        }

        public static string BuildSetValuesMessage(int reqId, long id, float value, int? index)
        {
            return BuildSetValuesMessage(reqId, new[] { (id, value, index) });
        }

        public static string BuildSetValuesMessage(int reqId, IEnumerable<(long Id, float Value, int? Index)> values)
        {
            var message = new DatarefSetMessage
            {
                ReqId = reqId,
                Type = XplaneWebApiConstants.MessageTypeDatarefSetValues,
                Params = new DatarefSetParams()
            };

            foreach (var (id, value, index) in values)
            {
                message.Params.Datarefs.Add(new DatarefSetEntry { Id = id, Value = value, Index = index });
            }

            return JsonConvert.SerializeObject(message);
        }

        public static string BuildCommandSetActiveMessage(int reqId, long id, bool isActive, double? duration = null)
        {
            return BuildCommandSetActiveMessage(reqId, new[] { (id, isActive, duration) });
        }

        public static string BuildCommandSetActiveMessage(int reqId, IEnumerable<(long Id, bool IsActive, double? Duration)> commands)
        {
            var message = new CommandSetMessage
            {
                ReqId = reqId,
                Type = XplaneWebApiConstants.MessageTypeCommandSetIsActive,
                Params = new CommandSetParams()
            };

            foreach (var (id, isActive, duration) in commands)
            {
                message.Params.Commands.Add(new CommandSetEntry { Id = id, IsActive = isActive, Duration = duration });
            }

            return JsonConvert.SerializeObject(message);
        }

        private static string BuildSubscriptionMessage(string type, int reqId, IEnumerable<long> ids)
        {
            var message = new DatarefSubscribeMessage
            {
                ReqId = reqId,
                Type = type,
                Params = new DatarefSubscribeParams()
            };

            foreach (var id in ids)
            {
                message.Params.Datarefs.Add(new DatarefSubscribeEntry { Id = id });
            }

            return JsonConvert.SerializeObject(message);
        }
    }
}
