using System;
using SharpMC.API.Admin;
using SharpMC.API.Entities;
using SharpMC.API.Plugins;
using SharpMC.API.Worlds;

namespace SharpMC.Plugins
{
    public class Globals : IGlobals
    {
        public void StopServer(string message)
        {
            throw new System.NotImplementedException();
        }

        public void BroadcastChat(string message, IPlayer player)
        {
            throw new System.NotImplementedException();
        }

        public ILevelManager LevelManager { get; set; }

        public IPermissionManager PermissionManager { get; set; }

        public IPluginManager PluginManager { get; set; }

        public Random Rand { get; set; }

        public IMessageFactory MessageFactory { get; set; }

        public IPlayer ConsolePlayer { get; set; }

        public static IGlobals Instance { get; } = new Globals();
    }
}