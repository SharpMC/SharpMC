using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SharpMC.Meta;

namespace SharpMC.Util
{
    internal static class JsonHelper
    {
        private static readonly JsonSerializerSettings Config = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string ToJson(MetaServer message)
            => JsonConvert.SerializeObject(message, Config);
    }
}