using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Networking.Packages
{
	internal class ChunkData : Package<ChunkData>
	{
		public ChunkColumn Chunk;
		public bool Queee = true;
		public bool Unloader = false;

		public ChunkData(ClientWrapper client) : base(client)
		{
			SendId = 0x21;
		}

		public ChunkData(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x21;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.Write(Chunk.GetBytes(Unloader));
				Buffer.FlushData(Queee);
			}
		}
	}
}