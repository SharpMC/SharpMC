using Microsoft.Extensions.Logging;
using SharpMC.API.Worlds;

namespace SharpMC.API.Plugins
{
    public interface IPluginContext
    {
        ILevelManager LevelManager { get; }
        
        IGlobals Globals { get; }

        ILogger GetLogger(IPlugin plugin);
    }
}