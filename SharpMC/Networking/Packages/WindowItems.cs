using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class WindowItems : Package<WindowItems>
	{
		public ItemStack[] ItemStacks;
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
				Buffer.WriteShort((short) (ItemStacks.Length));
				foreach (var i in ItemStacks)
				{
					Buffer.WriteShort(i.ItemId);
					if (i.ItemId != -1)
					{
						Buffer.WriteByte(i.ItemCount);
						Buffer.WriteShort(i.MetaData);
						Buffer.WriteByte(0);
					}
				}
				Buffer.FlushData();
			}
		}
	}
}