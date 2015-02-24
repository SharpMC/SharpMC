namespace SharpMCRewrite.Worlds.Nether
{
	internal class NetherLevel : Level
	{
		public NetherLevel(string worldname)
		{
			Difficulty = 0;
			LVLName = worldname;
			LevelType = LVLType.Default;
			Generator = new NetherGenerator(worldname);
		}
	}
}