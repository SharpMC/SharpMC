using System;

namespace SharpMC.Admin
{
    public class ServerSettings
    {
        public string Seed { get; set; } = "SharpMC";

        public bool OnlineMode { get; set; } = false;

        public int MaxPlayers { get; set; } = 10;

        public bool DisplayPacketErrors { get; set; } = false;

        public bool Debug { get; set; } = false;

        public string Motd { get; set; } = "A SharpMC Powered Server";

        public LevelType LevelType { get; set; } = LevelType.Standard;

        public bool PluginDisabled { get; set; } = false;

        public int Port { get; set; } = 25565;

        public string WorldName { get; set; } = "world";

        public string[] Operators { get; set; } = Array.Empty<string>();
    }
}