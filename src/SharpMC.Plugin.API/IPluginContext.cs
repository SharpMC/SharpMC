using Microsoft.Extensions.Logging;
using SharpMC.API.Worlds;

namespace SharpMC.Plugin.API
{
    public interface IPluginContext
    {
        ILevelManager LevelManager { get; }

        IGlobal Globals { get; }

        ILogger GetLogger(IPlugin plugin);
    }
}