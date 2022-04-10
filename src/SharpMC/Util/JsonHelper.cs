using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SharpMC.Meta;

namespace SharpMC.Util
{
    internal static class JsonHelper
    {
        private static readonly JsonSerializerSettings Config = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static string ToJson(MetaServer message)
            => JsonConvert.SerializeObject(message, Config);
    }
}