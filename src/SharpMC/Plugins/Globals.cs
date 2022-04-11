using System;
using SharpMC.Players;
using SharpMC.Plugins.Channel;
using SharpMC.World;

namespace SharpMC.Plugins
{
    public class Globals
    {
        public void StopServer(string message = null)
        {
            throw new NotImplementedException();
        }

        public void BroadcastChat(string message, Player player)
        {
            throw new NotImplementedException();
        }

        public LevelManager LevelManager { get; set; }

        public PermissionManager PermissionManager { get; set; }

        public PluginManager PluginManager { get; set; }

        public Random Rand { get; set; }

        public MessageFactory MessageFactory { get; set; }

        public Player ConsolePlayer { get; set; }

        public static Globals Instance { get; } = new Globals();
    }
}