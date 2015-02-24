using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpMCRewrite.Worlds.ExperimentalV2
{
	internal class ExperimentalV2Level : Level
	{
		public ExperimentalV2Level(string worldname)
		{
			Difficulty = 0;
			LVLName = worldname;
			LevelType = LVLType.Default;
			Generator = new ExperimentalV2Generator(worldname);
		}
	}
}