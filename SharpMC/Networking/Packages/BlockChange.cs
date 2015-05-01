using SharpMC.Blocks;
using SharpMC.Entity;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
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

		public BlockChange(ClientWrapper client, MSGBuffer buffer)
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

		public static void Broadcast(Block block, bool self = true, Player source = null)
		{
			lock (Globals.Level.OnlinePlayers)
			{
				foreach (var i in Globals.Level.OnlinePlayers.ToArray())
				{
					if (!self && i == source)
					{
						continue;
					}
					//Client = i.Wrapper;
					//Buffer = new MSGBuffer(i.Wrapper);
					//_stream = i.Wrapper.TCPClient.GetStream();
					//Write();
					new BlockChange(i.Wrapper, new MSGBuffer(i.Wrapper))
					{
						BlockId = block.Id,
						MetaData = block.Metadata,
						Location = block.Coordinates
					}.Write();

				}
			}
		}
	}
}