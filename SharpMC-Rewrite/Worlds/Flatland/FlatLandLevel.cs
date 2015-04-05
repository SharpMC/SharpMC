using SharpMCRewrite.Classes;

namespace SharpMCRewrite.Worlds.Flatland
{
	public class FlatLandLevel : Level
	{
		public FlatLandLevel(string world)
		{
			Difficulty = 0;
			LVLName = world;
			Generator = new FlatLandGenerator(world);
			LevelType = LVLType.flat;
		}
	}
}