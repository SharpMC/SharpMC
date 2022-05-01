using SharpMC.API;
using SharpMC.API.Entities;
using SharpMC.API.Net;
using SharpMC.API.Players;

namespace SharpMC.Players
{
    internal sealed class PlayerFactory : IPlayerFactory
    {
        private IServer Server { get; }

        public PlayerFactory(IServer server)
        {
            Server = server;
        }

        public IPlayer CreatePlayer(INetConnection connection, string username)
        {
            return new Player(connection, Server, username);
        }
    }
}