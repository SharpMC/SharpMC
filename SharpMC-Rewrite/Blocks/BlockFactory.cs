using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMCRewrite.Blocks
{
	class BlockFactory
	{
		public static Block GetBlockById(ushort id)
		{
			return new Block(id);
		}
	}
}
