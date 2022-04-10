using System;
using System.Net.Sockets;
using SharpMC.Network;
using SharpMC.Network.Events;

namespace SharpMC.Net
{
    public class McConnectionFactory : NetConnectionFactory
    {
        private MinecraftServer Server { get; }

        public McConnectionFactory(MinecraftServer server)
        {
            Server = server;
        }

        protected override NetConnection Create(Direction direction, Socket socket,
            EventHandler<ConnectionConfirmedArgs> confirmedAction = null)
        {
            return new McNetConnection(Server, direction, socket);
        }
    }
}