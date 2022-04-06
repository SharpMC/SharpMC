using System.Linq;

namespace SharpMC
{
	public class ServerInfo
	{
		private MinecraftServer Server { get; }
		public ServerInfo(MinecraftServer server)
		{
			Server = server;
		}

		private const int Protocol = 316;
		private const string ProtocolName = "1.11";
		public string Motd { get; set; } = "A SharpMC Server";
		public int MaxPlayers { get; set; } = 30;
		public int Players => Server.LevelManager.GetLevels().Sum(x => x.PlayerCount);

		public string GetMotd()
		{
			return
				$"{{\"version\":{{\"name\":\"{ProtocolName}\",\"protocol\":{Protocol}}},\"players\":{{\"max\":{MaxPlayers},\"online\":{Players}}},\"description\":{{\"text\":\"{Motd}\"}}}}";
		}
	}
}
