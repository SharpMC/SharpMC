using System.Net;

namespace SharpMC.Network.API
{
    public interface INetConfiguration
    {
        ProtocolType Protocol { get; }

        IPAddress Host { get; }

        int Port { get; }
    }
}