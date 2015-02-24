namespace SharpMCRewrite.Worlds.Experimental
{
	internal class ExperimentalLevel : Level
	{
		public ExperimentalLevel(string worldname)
		{
			Difficulty = 0;
			LVLName = worldname;
			LevelType = LVLType.Default;
			Generator = new ExperimentalGenerator(worldname);
		}
	}
}