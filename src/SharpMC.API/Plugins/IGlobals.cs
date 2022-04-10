using System;
using SharpMC.API.Admin;
using SharpMC.API.Entities;
using SharpMC.API.Worlds;

namespace SharpMC.API.Plugins
{
    public interface IGlobals
    {
        void StopServer(string message = null);
        
        void BroadcastChat(string message, IPlayer player);
        
        ILevelManager LevelManager { get; set; }

        IPermissionManager PermissionManager { get; }
        
        IPluginManager PluginManager { get; set; }

        Random Rand { get; set; }
        
        IMessageFactory MessageFactory { get; set; }
        
        IPlayer ConsolePlayer { get; set; }
    }
}