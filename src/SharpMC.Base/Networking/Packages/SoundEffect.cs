using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class SoundEffect : Package<SoundEffect>
	{
		public string SoundName = "random.explode";
		public int X = 0;
		public int Y = 0;
		public int Z = 0;

		public SoundEffect(ClientWrapper client) : base(client)
		{
			SendId = 0x29;
		}

		public SoundEffect(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x29;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteString(SoundName);
				Buffer.WriteInt(X*8);
				Buffer.WriteInt(Y*8);
				Buffer.WriteInt(Z*8);
				Buffer.WriteFloat(1f);
				Buffer.WriteByte(63);
				Buffer.FlushData();
			}
		}
	}
}