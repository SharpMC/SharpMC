using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Classes;
using SharpMC.Enums;

namespace SharpMC.Networking.Packages
{
	public class EntityMetadata : Package<EntityMetadata>
	{
		public int EntityId = 0;
		public ObjectType type;
		public object data;

		public EntityMetadata(ClientWrapper client) : base(client)
		{
			SendId = 0x1c;
		}

		public EntityMetadata(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x1c;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				if (type == ObjectType.ItemStack)
				{
					var item = (ItemStack) data;
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
