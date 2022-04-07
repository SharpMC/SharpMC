using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Standard
{
	internal class StandardLevel : Level
	{
		public StandardLevel(string worldname)
		{
			Difficulty = 0;
			LvlName = worldname;
			LevelType = LvlType.Default;
			Generator = new StandardWorldProvider(worldname);
			ConsoleFunctions.WriteInfoLine("Level Type: Standard");
			DefaultGamemode = Gamemode.Creative;
		}
	}
}