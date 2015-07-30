using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class EntityHeadLook : Package<EntityHeadLook>
	{
		public int EntityId;
		public byte HeadYaw;
		public EntityHeadLook(ClientWrapper client) : base(client)
		{
			SendId = 0x19;
		}

		public EntityHeadLook(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x19;
		}

		public override void Write()
		{
			if (Client != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				Buffer.WriteByte(HeadYaw);
				Buffer.FlushData();
			}
		}
	}
}
