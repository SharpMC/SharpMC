using SharpMC.Classes;

namespace SharpMC.Networking.Packages
{
	internal class WindowItems : Package<WindowItems>
	{
		public Slot[] Slots;
		public byte WindowId = 0;

		public WindowItems(ClientWrapper client) : this(client, new MSGBuffer(client))
		{
		}

		public WindowItems(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x30;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteByte(WindowId);
				Buffer.WriteShort((short) (Slots.Length));
				foreach (var i in Slots)
				{
					Buffer.WriteShort(i.ItemId);
					if (i.ItemId != -1)
					{
						Buffer.WriteByte(i.ItemCount);
						Buffer.WriteShort(i.ItemDamage);
						Buffer.WriteByte(0);
					}
				}
				Buffer.FlushData();
			}
		}
	}
}