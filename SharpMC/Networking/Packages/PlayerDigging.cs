using System.Runtime.InteropServices;
using SharpMC.Entity;
using SharpMC.Enums;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class PlayerDigging : Package<PlayerDigging>
	{
		public PlayerDigging(ClientWrapper client) : base(client)
		{
			ReadId = 0x07;
		}

		public PlayerDigging(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x07;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var status = Buffer.ReadByte();

				if (status == 3)
				{
					//Drop item stack
					Client.Player.Inventory.DropCurrentItemStack();
					return;
				}

				if (status == 4)
				{
					//Drop item
					Client.Player.Inventory.DropCurrentItem();
					return;
				}

				if (status == 2 || Client.Player.Gamemode == Gamemode.Creative)
				{
					var Position = Buffer.ReadPosition();
					var Face = Buffer.ReadByte();

					var block = Client.Player.Level.GetBlock(Position);
					block.BreakBlock(Client.Player.Level);
					//Globals.Level.SetBlock(new BlockAir() {Coordinates = intVector});
					Client.Player.Digging = false;
					
					//new SpawnObject(Client) {EntityId = EntityManager.GetEntityId(), X = Position.X, Y = Position.Y, Z = Position.Z, Type = ObjectType.ItemStack, Data = new ItemStack((short)block.Id, 1, (byte)block.Metadata)}.Write();
					new ItemEntity(Client.Player.Level, new ItemStack((short) block.Drops.Id, 1, block.Drops.Metadata)) {KnownPosition = new PlayerLocation(Position.X, Position.Y, Position.Z)}.SpawnEntity();	
				}
				else if (status == 0)
				{
					Client.Player.Digging = true;
				}
				else if (status == 1)
				{
					Client.Player.Digging = false;
				}
			}
		}
	}
}