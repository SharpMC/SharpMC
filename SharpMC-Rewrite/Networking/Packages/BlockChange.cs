using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	class BlockChange : Package<BlockChange>
	{
		public IntVector3 Location;
		public Block Block;
		public BlockChange(ClientWrapper client) : base(client)
		{
			SendId = 0x23;
		}

		public BlockChange(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x23;
		}

		public override void Write()
		{
			Buffer.WriteVarInt(SendId);
			Buffer.WritePosition(Location);
			Buffer.WriteVarInt(Block.Id << 4 | Block.Metadata);
			Buffer.FlushData(true);
		}
	}
}
