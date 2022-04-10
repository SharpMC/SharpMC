using Newtonsoft.Json;

namespace SharpMC.Net
{
    public sealed class AuthResponse
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("properties")]
        public Property[] Properties;

        public struct Property
        {
            [JsonProperty("name")]
            public string Name;

            [JsonProperty("value")]
            public string Value;

            [JsonProperty("signature")]
            public string Signature;
        }
    }
}