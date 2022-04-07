using SharpMC.Core.Utils;
using SharpMC.Enums;

namespace SharpMC.Core.Networking.Packages
{
	public class EntityMetadata : Package<EntityMetadata>
	{
		public object Data;
		public int EntityId = 0;
		public ObjectType Type;

		public EntityMetadata(ClientWrapper client) : base(client)
		{
			SendId = 0x1c;
		}

		public EntityMetadata(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x1c;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				if (Type == ObjectType.ItemStack)
				{
					var item = (ItemStack) Data;
					Buffer.WriteByte((5 << 5 | 10 & 0x1F) & 0xFF);
					Buffer.WriteShort((short) (item.ItemId != 0 ? item.ItemId : 1));
					Buffer.WriteByte(1);
					Buffer.WriteShort(item.MetaData);
					Buffer.WriteByte(0); //nbt shit starting
					Buffer.WriteByte(127);
				}
				Buffer.FlushData();
			}
		}
	}
}