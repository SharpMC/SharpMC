using SharpMC.Core.Utils;
using SharpMC.Util;

namespace SharpMC.Core.Networking.Packages
{
	internal class PlayerPosition : Package<PlayerPosition>
	{
		public PlayerPosition(ClientWrapper client) : base(client)
		{
			ReadId = 0x04;
		}

		public PlayerPosition(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x04;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var x = Buffer.ReadDouble();
				var feetY = Buffer.ReadDouble();
				var z = Buffer.ReadDouble();
				var onGround = Buffer.ReadBool();

				Client.Player.PositionChanged(
                    Vectors.Create(x, feetY, z), 0.0f, 0.0f, onGround);
			}
		}
	}
}