using SharpMC.Net;

namespace SharpMC.Players
{
    public class DefaultPlayerFactory : IPlayerFactory
    {
        private MinecraftServer Server { get; }

        public DefaultPlayerFactory(MinecraftServer server)
        {
            Server = server;
        }

        public Player CreatePlayer(McNetConnection connection, string username)
        {
            return new Player(connection, Server, username);
        }
    }
}