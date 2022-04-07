using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class OnGround : Package<OnGround>
	{
		public OnGround(ClientWrapper client) : base(client)
		{
			ReadId = 0x03;
		}

		public OnGround(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x03;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				Client.Player.KnownPosition.OnGround = Buffer.ReadBool();
			}
		}
	}
}