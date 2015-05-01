using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class SetSlot : Package<SetSlot>
	{
		public byte ItemCount;
		public short ItemDamage;
		public short ItemId;
		public byte MetaData;
		public short Slot;
		public byte WindowId;

		public SetSlot(ClientWrapper client) : base(client)
		{
			SendId = 0x2F;
		}

		public SetSlot(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x2F;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteByte(WindowId);
				Buffer.WriteShort(Slot);
				//Buffer.WriteShort((short) ((ItemId << 4) | MetaData));
				Buffer.WriteShort(ItemId);
				if (ItemId != -1)
				{
					Buffer.WriteByte(ItemCount);
					Buffer.WriteShort(MetaData);
					Buffer.WriteByte(0);
				}
				Buffer.FlushData();
			}
		}
	}
}