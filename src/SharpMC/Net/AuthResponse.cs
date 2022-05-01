using Newtonsoft.Json;
using SharpMC.API.Net;

namespace SharpMC.Net
{
    public sealed class AuthResponse : IAuthResponse
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