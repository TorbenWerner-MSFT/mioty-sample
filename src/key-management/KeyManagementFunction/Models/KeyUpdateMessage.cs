using Newtonsoft.Json;

namespace KeyManagementFunction.Models
{
    public class KeyUpdateMessage
    {
        [JsonProperty("key")]
        public KeyUpdateMessagePayload Key { get; set; }

        public KeyUpdateMessage(string deviceId, string shortId, string newKey)
        {
            Key = new KeyUpdateMessagePayload
            {
                Action = "add",
                Type = "MIOTY",
                Uid = deviceId,
                ShortId = shortId,
                NwkKey = newKey
            };
        }
    }

    public class KeyUpdateMessagePayload
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("shortId")]
        public string ShortId { get; set; }

        [JsonProperty("nwkKey")]
        public string NwkKey { get; set; }
    }
}
