using SharpMCRewrite.NET;
using SharpMCRewrite.Worlds;

namespace SharpMCRewrite.Networking.Packages
{
	internal class ChunkData : Package<ChunkData>
	{
		public ChunkColumn Chunk;

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
			Buffer.WriteVarInt(SendId);
			Buffer.Write(Chunk.GetBytes());
			Buffer.FlushData(true);
		}

		public static void Broadcast(ChunkColumn chunk, bool self = true, Player source = null)
		{
			foreach (var i in Globals.Level.OnlinePlayers)
			{
				if (!self && i == source)
				{
					continue;
				}
				//Client = i.Wrapper;
				//Buffer = new MSGBuffer(i.Wrapper);
				//_stream = i.Wrapper.TCPClient.GetStream();
				//Write();
				new ChunkData(i.Wrapper, new MSGBuffer(i.Wrapper)) {Chunk = chunk}.Write();
			}
		}
	}
}