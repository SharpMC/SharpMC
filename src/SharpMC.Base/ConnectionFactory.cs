using System.Net.Sockets;
using SharpMC.Network;

namespace SharpMC
{
	public class ConnectionFactory : NetConnectionFactory
    {
		private MinecraftServer Server { get; }

        public ConnectionFactory(MinecraftServer server)
        {
			Server = server;
		}

		protected override NetConnection Create(Direction direction, Socket socket, ConnectionConfirmed confirmedAction = null)
		{
			return new MCNetConnection(Server, direction, socket);
		}
	}
}
