using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Items;

namespace SharpMC.Blocks
{
	public class BlockCoalOre : Block
	{
		internal BlockCoalOre() : base(16)
		{
			Drops = new ItemCoal();
		}
	}
}
