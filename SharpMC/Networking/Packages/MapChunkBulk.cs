using SharpMC.Classes;
using SharpMC.Worlds;

namespace SharpMC.Networking.Packages
{
	internal class MapChunkBulk : Package<MapChunkBulk>
	{
		public ChunkColumn[] Chunks;
		public bool SkyLightSend = true;

		public MapChunkBulk(ClientWrapper client) : base(client)
		{
			SendId = 0x26;
		}

		public MapChunkBulk(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x26;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteBool(SkyLightSend);
				Buffer.WriteVarInt(Chunks.Length);

				foreach (var chunk in Chunks)
				{
					Buffer.Write(chunk.GetMeta());
				}

				foreach (var chunk in Chunks)
				{
					Buffer.Write(chunk.GetChunkData());
				}

				Buffer.FlushData();
			}
		}
	}
}