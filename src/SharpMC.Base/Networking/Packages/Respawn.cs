using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class Respawn : Package<Respawn>
	{
		public byte Difficulty = 0;
		public int Dimension = 0;
		public byte GameMode = 1;

		public Respawn(ClientWrapper client)
			: base(client)
		{
			SendId = 0x07;
		}

		public Respawn(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			SendId = 0x07;
		}

		public override void Write()
		{
			Buffer.WriteVarInt(SendId);
			Buffer.WriteInt(Dimension);
			Buffer.WriteByte(Difficulty);
			Buffer.WriteByte(GameMode);
			Buffer.WriteString("flat");
			Buffer.FlushData();
		}
	}
}