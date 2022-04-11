using System;
using Microsoft.Extensions.Logging;
using SharpMC.API.Plugins;
using SharpMC.API.Worlds;
using SharpMC.Logging;
using SharpMC.World;
using G = SharpMC.Plugins.Globals;

namespace SharpMC.Plugins
{
    public class PluginContext : IPluginContext
    {
        public PluginContext(PluginManager pluginManager)
        {
            PluginManager = pluginManager;
        }

        public PluginManager PluginManager { get; }

        ILevelManager IPluginContext.LevelManager => throw new NotImplementedException();
        IGlobals IPluginContext.Globals => throw new NotImplementedException();

        public LevelManager LevelManager => G.Instance.LevelManager;
        public Globals Globals => G.Instance;

        public ILogger GetLogger(IPlugin plugin)
        {
            var caller = plugin.GetType();
            var log = LogManager.GetLogger(caller);
            return log;
        }
    }
}