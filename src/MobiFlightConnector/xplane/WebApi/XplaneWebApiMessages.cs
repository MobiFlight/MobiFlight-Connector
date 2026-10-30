using System.Collections.Generic;
using Newtonsoft.Json;

namespace MobiFlight.xplane.WebApi
{
    internal class DatarefListEntry
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value_type")]
        public string ValueType { get; set; }
    }

    internal class CommandListEntry
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    internal class PagedListResponse<T>
    {
        [JsonProperty("data")]
        public List<T> Data { get; set; } = new List<T>();
    }

    internal class DatarefSubscribeEntry
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }

    internal class DatarefSubscribeParams
    {
        [JsonProperty("datarefs")]
        public List<DatarefSubscribeEntry> Datarefs { get; set; } = new List<DatarefSubscribeEntry>();
    }

    internal class DatarefSubscribeMessage
    {
        [JsonProperty("req_id")]
        public int ReqId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("params")]
        public DatarefSubscribeParams Params { get; set; }
    }

    internal class DatarefSetEntry
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("value")]
        public float Value { get; set; }

        [JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
        public int? Index { get; set; }
    }

    internal class DatarefSetParams
    {
        [JsonProperty("datarefs")]
        public List<DatarefSetEntry> Datarefs { get; set; } = new List<DatarefSetEntry>();
    }

    internal class DatarefSetMessage
    {
        [JsonProperty("req_id")]
        public int ReqId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("params")]
        public DatarefSetParams Params { get; set; }
    }

    internal class CommandSetEntry
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public double? Duration { get; set; }
    }

    internal class CommandSetParams
    {
        [JsonProperty("commands")]
        public List<CommandSetEntry> Commands { get; set; } = new List<CommandSetEntry>();
    }

    internal class CommandSetMessage
    {
        [JsonProperty("req_id")]
        public int ReqId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("params")]
        public CommandSetParams Params { get; set; }
    }
}
