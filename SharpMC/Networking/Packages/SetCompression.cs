using SharpMC.Classes;

namespace SharpMC.Networking.Packages
{
	internal class SetCompression : Package<SetCompression>
	{
		public int CompressionLevel = -1;

		public SetCompression(ClientWrapper client) : base(client)
		{
			SendId = 0x46;
		}

		public SetCompression(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x46;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(CompressionLevel);
				Buffer.FlushData();
			}
		}
	}
}