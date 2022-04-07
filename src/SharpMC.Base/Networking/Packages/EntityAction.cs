using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class EntityAction : Package<EntityAction>
	{
		public EntityAction(ClientWrapper client) : base(client)
		{
			ReadId = 0x0C;
		}

		public EntityAction(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x0C;
		}

		public override void Read()
		{
			var entityId = Buffer.ReadVarInt();
			var actionId = Buffer.ReadVarInt();
			var jumpBoost = Buffer.ReadVarInt();
			
			Client.Player.LastEntityAction = (Enums.EntityAction) actionId;
			if (actionId == 0)
			{
				Client.Player.IsCrouching = true;
			}
			else if (actionId == 1)
			{
				Client.Player.IsCrouching = false;
			}
		}
	}
}