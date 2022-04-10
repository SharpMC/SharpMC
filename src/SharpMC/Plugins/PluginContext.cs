using Microsoft.Extensions.Logging;
using SharpMC.API.Plugins;
using SharpMC.API.Worlds;
using SharpMC.Logging;
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

        public ILevelManager LevelManager => G.Instance.LevelManager;

        public IGlobals Globals => G.Instance;

        public ILogger GetLogger(IPlugin plugin)
        {
            var caller = plugin.GetType();
            var log = LogManager.GetLogger(caller);
            return log;
        }
    }
}