namespace SharpMC.Core.Utils
{
	public class StatusRequestMessage
	{
		public StatusRequestMessage(string version, int protocol, int maxPlayers, int onlinePlayers, string description)
		{
			this.version = new StatusVersionClass(version, protocol);
			this.players = new StatusPlayersClass(maxPlayers, onlinePlayers);
			this.description = new McChatMessage(description);
		}

		public StatusVersionClass version;
		public StatusPlayersClass players;
		public McChatMessage description;
	}

	public class StatusVersionClass
	{
		public StatusVersionClass(string name, int protocol)
		{
			this.name = name;
			this.protocol = protocol;
		}

		public string name = string.Empty;
		public int protocol = 0;
	}

	public class StatusPlayersClass
	{
		public StatusPlayersClass(int max, int online)
		{
			this.max = max;
			this.online = online;
		}

		public int max;
		public int online;
	}
}
