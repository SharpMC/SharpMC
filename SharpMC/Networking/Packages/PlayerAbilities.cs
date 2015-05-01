using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class PlayerAbilities : Package<PlayerAbilities>
	{
		public byte Flags;
		public float FlyingSpeed = 1.0f;
		public float WalkingSpeed = 1.0f;

		public PlayerAbilities(ClientWrapper client) : base(client)
		{
			ReadId = 0x13;
			SendId = 0x39;
		}

		public PlayerAbilities(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x13;
			SendId = 0x39;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var flags = (byte) Buffer.ReadByte();
				var flyingSpeed = Buffer.ReadFloat();
				var walkingSpeed = Buffer.ReadFloat();
				//We don't do anything with this for now. Not really needed.
			}
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteByte(Flags);
				Buffer.WriteFloat(FlyingSpeed);
				Buffer.WriteFloat(WalkingSpeed);
				Buffer.FlushData();
			}
		}
	}
}