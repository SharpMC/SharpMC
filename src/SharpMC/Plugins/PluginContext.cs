using Microsoft.Extensions.Logging;
using SharpMC.API.Worlds;
using SharpMC.Plugin.API;

namespace SharpMC.Plugins
{
    internal sealed class PluginContext : IPluginContext
    {
        private readonly ILoggerFactory _factory;

        public PluginContext(ILevelManager levelManager, IGlobal globals,
            ILoggerFactory factory)
        {
            _factory = factory;
            LevelManager = levelManager;
            Globals = globals;
        }

        public ILevelManager LevelManager { get; }
        public IGlobal Globals { get; }

        public ILogger GetLogger(IPlugin plugin)
        {
            var caller = plugin.GetType();
            var log = _factory.CreateLogger(caller);
            return log;
        }
    }
}