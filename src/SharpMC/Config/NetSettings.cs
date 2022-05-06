using System;
using System.Net;
using SharpMC.Network.API;

namespace SharpMC.Config
{
    public sealed class NetSettings : INetConfiguration
    {
        public ProtocolType Protocol { get; set; }

        public string? Host { get; set; }

        public int Port { get; set; }

        public IPAddress HostObj
        {
            get
            {
                switch (Host)
                {
                    case nameof(IPAddress.Any):
                        return IPAddress.Any;
                    case nameof(IPAddress.Loopback):
                        return IPAddress.Loopback;
                    default:
                        throw new InvalidOperationException(Host);
                }
            }
        }

        public override string ToString()
        {
            var proto = Protocol.ToString().ToLowerInvariant();
            return $"{HostObj} {proto}/{Port}";
        }

        IPAddress INetConfiguration.Host => HostObj;
    }
}