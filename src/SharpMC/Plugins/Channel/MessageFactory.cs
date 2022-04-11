using System.Collections.Generic;
using SharpMC.API.Plugins;
using SharpMC.Net;

namespace SharpMC.Plugins.Channel
{
    public class MessageFactory : IMessageFactory
    {
        private Dictionary<string, PluginMessage> Messages { get; set; }

        public MessageFactory()
        {
            Messages = new Dictionary<string, PluginMessage>
            {
                {"MC", new BrandMessage()}
            };
        }

        public bool AddMessage(PluginMessage pm, string channel)
        {
            if (Messages.ContainsKey(channel))
                return false;
            Messages.Add(channel, pm);
            return true;
        }

        public bool HandleMessage(IClientWrapper client, IDataBuffer buffer)
        {
            var raw = buffer.ReadString();
            var channel = raw.Split('|')[0];
            var command = raw.Split('|')[1];
            foreach (var msg in Messages)
            {
                if (msg.Key == channel)
                {
                    if (msg.Value.Command == command)
                    {
                        msg.Value.HandleData(client, buffer);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}