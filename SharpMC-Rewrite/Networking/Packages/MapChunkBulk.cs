using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMCRewrite.Classes;
using SharpMCRewrite.Worlds;

namespace SharpMCRewrite.Networking.Packages
{
	class MapChunkBulk : Package<MapChunkBulk>
	{
		public bool SkyLightSend = true;
		public ChunkColumn[] Chunks;

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
