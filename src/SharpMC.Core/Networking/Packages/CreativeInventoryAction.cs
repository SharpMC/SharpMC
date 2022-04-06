using SharpMC.Core.Enums;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class CreativeInventoryAction : Package<CreativeInventoryAction>
	{
		public CreativeInventoryAction(ClientWrapper client) : base(client)
		{
			ReadId = 0x11;
		}

		public CreativeInventoryAction(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x11;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				if (Client.Player.Gamemode == Gamemode.Creative)
				{
					var slot = Buffer.ReadShort();
					var itemId = Buffer.ReadShort();

					byte itemCount = 1;
					short itemDamage = 0;
					byte nbt = 0;

					if (itemId != -1)
					{
						itemCount = (byte) Buffer.ReadByte();
						itemDamage = Buffer.ReadShort();
						nbt = (byte) Buffer.ReadByte();
						if (nbt != 0)
						{
							//NBT Data found
						}

						Client.Player.Inventory.SetSlot(slot, itemId, (byte) itemDamage, itemCount);
					}
					else
					{
						Client.Player.Inventory.SetSlot(slot, -1, 0, 0);
					}
				}
			}
		}
	}
}
