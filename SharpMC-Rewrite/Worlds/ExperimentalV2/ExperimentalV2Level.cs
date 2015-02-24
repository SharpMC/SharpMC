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

			new Task(() =>
			{
				Generator.GenerateChunks(8*12, Generator.GetSpawnPoint().X, Generator.GetSpawnPoint().Z,
					new Dictionary<Tuple<int, int>, ChunkColumn>());
			}).Start();
		}
	}
}