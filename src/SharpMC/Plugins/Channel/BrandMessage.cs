using Microsoft.Extensions.Logging;
using SharpMC.Logging;
using SharpMC.Net;

namespace SharpMC.Plugins.Channel
{
    public class BrandMessage : PluginMessage
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(BrandMessage));

        public BrandMessage() : base("Brand")
        {
        }

        public override void HandleData(IClientWrapper client, IDataBuffer buffer)
        {
            var c = buffer.ReadString();
            Log.LogInformation($"{client.Player.Username}'s client: {c}");
        }
    }
}