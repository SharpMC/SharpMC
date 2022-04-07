using SharpMC.Core.Networking;
using SharpMC.Core.Utils;
using SharpMC.World;

namespace SharpMC.Networking.Packages
{
	internal class ChunkData : Package<ChunkData>
	{
		public ChunkColumn Chunk;
		public bool Queee = false;
		public bool Unloader = false;

		public ChunkData(ClientWrapper client) : base(client)
		{
			SendId = 0x21;
		}

		public ChunkData(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
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