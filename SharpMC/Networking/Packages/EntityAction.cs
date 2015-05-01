using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class EntityAction : Package<EntityAction>
	{
		public EntityAction(ClientWrapper client) : base(client)
		{
			ReadId = 0x0B;
		}

		public EntityAction(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x0B;
		}

		public override void Read()
		{
			var EntityID = Buffer.ReadVarInt();
			var ActionID = Buffer.ReadVarInt();
			var JumpBoost = Buffer.ReadVarInt();

			Client.Player.LastEntityAction = (Enums.EntityAction) ActionID;
		}
	}
}