using System;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using SharpMC.Network.API;
using SharpMC.Network.Events;

namespace SharpMC.Network
{
    public class NetConnectionFactory
    {
        public EventHandler<ConnectionCreatedArgs>? OnConnectionCreated;

        private readonly ILoggerFactory _factory;

        public NetConnectionFactory(ILoggerFactory factory)
        {
            _factory = factory;
        }

        internal NetConnection CreateConnection(Direction direction, Socket socket,
            EventHandler<ConnectionConfirmedArgs>? confirmedAction = null)
        {
            var connection = Create(direction, socket, confirmedAction);
            if (connection == null)
                return null;
            OnConnectionCreated?.Invoke(null, new ConnectionCreatedArgs(connection));
            return connection;
        }

        protected virtual NetConnection Create(Direction direction, Socket socket,
            EventHandler<ConnectionConfirmedArgs>? confirmedAction = null)
        {
            var log = _factory.CreateLogger<NetConnection>();
            var conn = new NetConnection(log, direction, socket, confirmedAction);
            return conn;
        }
    }
}