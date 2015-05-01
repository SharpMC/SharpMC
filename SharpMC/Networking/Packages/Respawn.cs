using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	class Respawn : Package<Respawn>
	{
		public int Dimension = 0;
		public byte GameMode = 1;
		public byte Difficulty = 0;

		public Respawn(ClientWrapper client)
			: base(client)
		{
			SendId = 0x07;
		}

		public Respawn(ClientWrapper client, MSGBuffer buffer)
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
