using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMCRewrite.Worlds.Nether
{
	class NetherLevel : Level
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
