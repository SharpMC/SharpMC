using SharpMC.Core;
using SharpMC.World;

namespace SharpMC.Plugins.API
{
	public class PluginContext
	{
		public PluginContext(PluginManager pluginManager)
		{
			PluginManager = pluginManager;
		}

		public PluginManager PluginManager { get; private set; }

		public LevelManager LevelManager
		{
			get { return Globals.LevelManager; }
		}
	}
}