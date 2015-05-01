using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class PlayerAbilities : Package<PlayerAbilities>
	{
		public PlayerAbilities(ClientWrapper client) : base(client)
		{
			ReadId = 0x13;
		}

		public PlayerAbilities(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x13;
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
	}
}