// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015
using System;
using SharpMC.Entity;
using SharpMC.Enums;
using SharpMC.Items;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
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

			if (type == 2)
			{
				//Interact at
				targetX = Buffer.ReadFloat();
				targetY = Buffer.ReadFloat();
				targetZ = Buffer.ReadFloat();
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
			}
		}

		private int CalculateDamage(Player target)
		{
			double armorValue = 0;

			for (byte i = 5; i <= 8; i++)
			{
				var id = target.Inventory.GetSlot(i);
				var armorPiece = ItemFactory.GetItemById(id.ItemId);

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