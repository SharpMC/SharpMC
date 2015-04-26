using SharpMC.Classes;

namespace SharpMC.Worlds.Standard
{
	internal class StandardLevel : Level
	{
		public StandardLevel(string worldname)
		{
			Difficulty = 0;
			LVLName = worldname;
			LevelType = LVLType.Default;
			Generator = new StandardWorldProvider(worldname);
			ConsoleFunctions.WriteInfoLine("Level Type: Standard");
		}
	}
}