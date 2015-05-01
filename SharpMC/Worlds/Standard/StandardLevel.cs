using SharpMC.Enums;

namespace SharpMC.Worlds.Standard
{
	internal class StandardLevel : Level
	{
		public StandardLevel(string worldname)
		{
			Difficulty = 0;
			LvlName = worldname;
			LevelType = LVLType.Default;
			Generator = new StandardWorldProvider(worldname);
			ConsoleFunctions.WriteInfoLine("Level Type: Standard");
		}
	}
}