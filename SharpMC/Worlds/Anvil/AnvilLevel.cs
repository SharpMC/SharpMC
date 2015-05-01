using SharpMC.Enums;

namespace SharpMC.Worlds.Anvil
{
	internal class AnvilLevel : Level
	{
		public AnvilLevel(string worldname)
		{
			Difficulty = 0;
			LvlName = worldname;
			LevelType = LVLType.Default;
			Generator = new AnvilWorldProvider(worldname);
			ConsoleFunctions.WriteInfoLine("Level Type: Anvil");
		}
	}
}