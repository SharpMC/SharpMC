using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class PlayerLook : Package<PlayerLook>
	{
		public PlayerLook(ClientWrapper client) : base(client)
		{
			ReadId = 0x05;
		}

		public PlayerLook(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x05;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				Client.Player.KnownPosition.Yaw = Buffer.ReadFloat();
				Client.Player.KnownPosition.Pitch = Buffer.ReadFloat();
				Client.Player.KnownPosition.OnGround = Buffer.ReadBool();

				Client.Player.LookChanged();
			}
		}
	}
}