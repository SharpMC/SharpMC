using System.Security.Cryptography.X509Certificates;
using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	class Animation : Package<Animation>
	{
		public byte AnimationId;
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
			new Animation(Client, new MSGBuffer(Client)) { AnimationId = 0 }.Broadcast(false, Client.Player);
		}

		public override void Write()
		{
			Buffer.WriteVarInt(SendId);
			Buffer.WriteVarInt(Client.Player.UniqueServerID);
			Buffer.WriteByte(AnimationId);
			Buffer.FlushData();
		}
	}
}
