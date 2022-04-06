#region Imports

using System.Net;

#endregion

namespace SharpMC.Network
{
    public sealed class NetConfiguration
    {
        public ProtocolType Protocol { get; set; } = ProtocolType.Tcp;
		public IPAddress Host { get; set; } = IPAddress.Any;
        public int Port { get; set; } = 8181;
    }
}
