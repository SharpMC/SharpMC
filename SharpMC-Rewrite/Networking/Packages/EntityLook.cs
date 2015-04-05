using SharpMCRewrite.Classes;

namespace SharpMCRewrite.Networking.Packages
{
	internal class EntityLook : Package<EntityLook>
	{
		public Player Player;

		public EntityLook(ClientWrapper client) : base(client)
		{
			SendId = 0x16;
		}

		public EntityLook(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x16;
		}

		public override void Write()
		{
			Buffer.WriteVarInt(SendId);
			Buffer.WriteVarInt(Player.UniqueServerId);
			Buffer.WriteByte((byte) Player.Yaw);
			Buffer.WriteByte((byte) Player.Pitch);
			Buffer.WriteBool(Player.OnGround);
			Buffer.FlushData();
		}
	}
}