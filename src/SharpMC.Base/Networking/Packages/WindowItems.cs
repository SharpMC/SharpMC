using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class WindowItems : Package<WindowItems>
	{
		public ItemStack[] ItemStacks = null;
		public byte WindowId = 0;

		public WindowItems(ClientWrapper client) : base(client)
		{
		}

		public WindowItems(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x30;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteByte(WindowId);
				Buffer.WriteShort((short) ItemStacks.Length);
				foreach (var i in ItemStacks)
				{
					Buffer.WriteShort((short) i.ItemId);
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