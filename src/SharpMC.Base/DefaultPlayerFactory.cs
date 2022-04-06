using SharpMC.API;

namespace SharpMC
{
	public sealed class DefaultPlayerFactory : IPlayerFactory
	{
		private MinecraftServer Server { get; }
		public DefaultPlayerFactory(MinecraftServer server)
		{
			Server = server;
		}

		public Player CreatePlayer(MCNetConnection connection, string username)
		{
			return new Player(connection, Server, username);
		}
	}
}
