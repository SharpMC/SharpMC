using SharpMC.Enums;

namespace SharpMC.Worlds.Flatland
{
	public class FlatLandLevel : Level
	{
		public FlatLandLevel(string world)
		{
			Difficulty = 0;
			LvlName = world;
			Generator = new FlatLandGenerator(world);
			LevelType = LVLType.flat;
		}
	}
}