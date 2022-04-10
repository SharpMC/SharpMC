using SharpMC.Net;

namespace SharpMC.Plugins.Channel
{
    public class PluginMessage
    {
        public string Command { get; private set; }

        public PluginMessage(string command)
        {
            Command = command;
        }

        public virtual void HandleData(IClientWrapper client, IDataBuffer buffer)
        {
        }
    }
}