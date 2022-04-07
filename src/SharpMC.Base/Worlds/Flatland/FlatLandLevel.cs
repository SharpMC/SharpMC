using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Flatland
{
	public class FlatLandLevel : Level
	{
		public FlatLandLevel(string world)
		{
			Difficulty = 0;
			LvlName = world;
			Generator = new FlatLandGenerator(world);
			LevelType = LvlType.Flat;
		}
	}
}