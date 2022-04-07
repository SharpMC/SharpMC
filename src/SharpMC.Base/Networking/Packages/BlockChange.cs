using System.Numerics;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class BlockChange : Package<BlockChange>
	{
		public int BlockId;
		public Vector3 Location;
		public int MetaData;

		public BlockChange(ClientWrapper client)
			: base(client)
		{
			SendId = 0x23;
		}

		public BlockChange(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			SendId = 0x23;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WritePosition(Location);
				Buffer.WriteVarInt(BlockId << 4 | MetaData);
				Buffer.FlushData();
			}
		}
	}
}