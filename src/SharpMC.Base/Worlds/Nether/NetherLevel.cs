using SharpMC.Core;
using SharpMC.Core.Worlds.Nether;
using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Worlds.Nether
{
	internal class NetherLevel : Level
	{
		public NetherLevel(string worldname)
		{
			Difficulty = 0;
			LvlName = worldname;
			LevelType = LvlType.Default;
			Generator = new NetherWorldProvider(worldname);
			ConsoleFunctions.WriteInfoLine("Level Type: Nether");
			Dimension = -1;
		}
	}
}