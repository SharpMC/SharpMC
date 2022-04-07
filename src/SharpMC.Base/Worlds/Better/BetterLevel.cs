using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Better
{
	internal class BetterLevel : Level
	{
		public BetterLevel(string worldname)
		{
			Difficulty = 0;
			LvlName = worldname;
			LevelType = LvlType.Default;
			Generator = new BetterWorldProvider(worldname);
			ConsoleFunctions.WriteInfoLine("Level Type: Better (Experimental)");
			DefaultGamemode = Gamemode.Creative;
		}
	}
}