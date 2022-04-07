using System;
using SharpMC.Core.Utils;
using SharpMC.Enums;
using SharpMC.Items;

namespace SharpMC.Core.Networking.Packages
{
	internal class UseEntity : Package<UseEntity>
	{
		public UseEntity(ClientWrapper client)
			: base(client)
		{
			ReadId = 0x02;
		}

		public UseEntity(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			ReadId = 0x02;
		}

		public override void Read()
		{
			var target = Buffer.ReadVarInt();
			var type = Buffer.ReadVarInt();
			float targetX;
			float targetY;
			float targetZ;
			byte hand;

			if (type == 2)
			{
				//Interact at
				targetX = Buffer.ReadFloat();
				targetY = Buffer.ReadFloat();
				targetZ = Buffer.ReadFloat();
				hand = (byte)Buffer.ReadByte();
			}
			else if (type == 1)
			{
				//Player hit
				var targ = Client.Player.Level.GetPlayer(target);
				if (targ != null)
				{
					if (!targ.HealthManager.IsInvulnerable)
					{
						targ.HealthManager.TakeHit(Client.Player, CalculateDamage(targ), DamageCause.EntityAttack);
					}
				}
			}
			else if (type == 0)
			{
				//Interact

				//I guess this is for stuff like villagers & horses for openening inventory's?
				//Or maybe to get on a horse or somethin
				hand = (byte)Buffer.ReadByte();
			}
		}

		private int CalculateDamage(Player target)
		{
			double armorValue = 0;

			for (byte i = 5; i <= 8; i++)
			{
				var id = target.Inventory.GetSlot(i);
				var armorPiece = ItemFactory.GetItemById((short) id.ItemId);

				if (armorPiece.ItemType == ItemType.Helmet)
				{
					switch (armorPiece.ItemMaterial)
					{
						case ItemMaterial.Leather:
							armorValue += 1;
							break;
						case ItemMaterial.Gold:
							armorValue += 2;
							break;
						case ItemMaterial.Chain:
							armorValue += 2;
							break;
						case ItemMaterial.Iron:
							armorValue += 2;
							break;
						case ItemMaterial.Diamond:
							armorValue += 3;
							break;
					}
				}

				if (armorPiece.ItemType == ItemType.Chestplate)
				{
					switch (armorPiece.ItemMaterial)
					{
						case ItemMaterial.Leather:
							armorValue += 3;
							break;
						case ItemMaterial.Gold:
							armorValue += 5;
							break;
						case ItemMaterial.Chain:
							armorValue += 5;
							break;
						case ItemMaterial.Iron:
							armorValue += 6;
							break;
						case ItemMaterial.Diamond:
							armorValue += 8;
							break;
					}
				}

				if (armorPiece.ItemType == ItemType.Leggings)
				{
					switch (armorPiece.ItemMaterial)
					{
						case ItemMaterial.Leather:
							armorValue += 2;
							break;
						case ItemMaterial.Gold:
							armorValue += 3;
							break;
						case ItemMaterial.Chain:
							armorValue += 4;
							break;
						case ItemMaterial.Iron:
							armorValue += 5;
							break;
						case ItemMaterial.Diamond:
							armorValue += 6;
							break;
					}
				}

				if (armorPiece.ItemType == ItemType.Boots)
				{
					switch (armorPiece.ItemMaterial)
					{
						case ItemMaterial.Leather:
							armorValue += 1;
							break;
						case ItemMaterial.Gold:
							armorValue += 1;
							break;
						case ItemMaterial.Chain:
							armorValue += 1;
							break;
						case ItemMaterial.Iron:
							armorValue += 2;
							break;
						case ItemMaterial.Diamond:
							armorValue += 3;
							break;
					}
				}
			}

			armorValue *= 0.04;

			var itemInHandSlot = 36 + Client.Player.Inventory.CurrentSlot;
			var damage = ItemFactory.GetItemById(Client.Player.Inventory.GetSlot(itemInHandSlot).ItemId).GetDamage();
				//Item Damage.

			damage = (int) Math.Floor(damage*(1.0 - armorValue));
			if (damage <= 0) damage = 1;

			return damage;
		}
	}
}