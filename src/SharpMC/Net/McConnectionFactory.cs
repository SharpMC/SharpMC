using System;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using SharpMC.API;
using SharpMC.Network;
using SharpMC.Network.API;
using SharpMC.Network.Events;

namespace SharpMC.Net
{
    internal sealed class McConnectionFactory : NetConnectionFactory
    {
        private readonly ILoggerFactory _factory;
        private IServer Server { get; }

        public McConnectionFactory(IServer server, ILoggerFactory factory)
            : base(factory)
        {
            Server = server;
            _factory = factory;
        }

        protected override NetConnection Create(Direction direction, Socket socket,
            EventHandler<ConnectionConfirmedArgs>? confirmedAction = null)
        {
            var log = _factory.CreateLogger<NetConnection>();
            return new McNetConnection(log, Server, direction, socket);
        }
    }
}