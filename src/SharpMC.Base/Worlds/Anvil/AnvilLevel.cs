using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Anvil
{
	internal class AnvilLevel : Level
	{
		public AnvilLevel(string worldname)
		{
			Difficulty = 0;
			LvlName = worldname;
			LevelType = LvlType.Default;
			Generator = new AnvilWorldProvider(worldname);
			ConsoleFunctions.WriteInfoLine("Level Type: Anvil");
		}
	}
}