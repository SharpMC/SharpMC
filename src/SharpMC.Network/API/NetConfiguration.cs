using System.Net;

namespace SharpMC.Network.API
{
    public sealed class NetConfiguration : INetConfiguration
    {
        public ProtocolType Protocol { get; set; } = ProtocolType.Tcp;

        public IPAddress Host { get; set; } = IPAddress.Any;

        public int Port { get; set; } = 8181;

        public override string ToString()
        {
            var proto = Protocol.ToString().ToLowerInvariant();
            return $"{Host} {proto}/{Port}";
        }
    }
}