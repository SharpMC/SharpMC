using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMCRewrite.Worlds.Experimental
{
	class ExperimentalLevel : Level
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
