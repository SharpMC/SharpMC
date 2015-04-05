using SharpMCRewrite.Classes;

namespace SharpMCRewrite.Networking.Packages
{
	internal class Animation : Package<Animation>
	{
		public byte AnimationId;
		public Player TargetPlayer;

		public Animation(ClientWrapper client) : base(client)
		{
			SendId = 0x0B;
			ReadId = 0x0A;
		}

		public Animation(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x0B;
			ReadId = 0x0A;
		}

		public override void Read()
		{
			new Animation(Client) {AnimationId = 0, TargetPlayer = Client.Player}.Broadcast(false, Client.Player);
		}

		public override void Write()
		{
			Buffer.WriteVarInt(SendId);
			Buffer.WriteVarInt(TargetPlayer.UniqueServerId);
			Buffer.WriteByte(AnimationId);
			Buffer.FlushData();
		}
	}
}