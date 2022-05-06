using Microsoft.Extensions.Logging;
using SharpMC.API;
using SharpMC.API.Entities;
using SharpMC.API.Net;
using SharpMC.API.Players;

namespace SharpMC.Players
{
    internal sealed class PlayerFactory : IPlayerFactory
    {
        private readonly ILoggerFactory _factory;
        private IServer Server { get; }

        public PlayerFactory(IServer server, ILoggerFactory factory)
        {
            Server = server;
            _factory = factory;
        }

        public IPlayer CreatePlayer(INetConnection connection, string username)
        {
            var log = _factory.CreateLogger<Player>();
            return new Player(log, connection, Server, username);
        }
    }
}