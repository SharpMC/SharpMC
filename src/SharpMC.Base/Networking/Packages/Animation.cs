using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class Animation : Package<Animation>
	{
		public byte AnimationId;
		public int EntityId;
		public byte Hand;

		public Animation(ClientWrapper client) : base(client)
		{
			SendId = 0x0B;
			ReadId = 0x0B;
		}

		public Animation(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x0B;
			ReadId = 0x0B;
		}

		public override void Read()
		{
			var hand = Buffer.ReadVarInt();
			Client.Player.PlayerHandSwing((byte)hand);
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				Buffer.WriteByte(AnimationId);
				Buffer.FlushData();
			}
		}
	}
}