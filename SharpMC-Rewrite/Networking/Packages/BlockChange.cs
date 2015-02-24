using SharpMCRewrite.Blocks;
using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	internal class BlockChange : Package<BlockChange>
	{
		public Block Block;
		public IntVector3 Location;

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