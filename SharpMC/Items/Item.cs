using SharpMC.Classes;
using SharpMC.Enums;

namespace SharpMC.Items
{
	/// <summary>
	///     Items are objects which only exist within the player's inventory and hands - which means, they cannot be placed in
	///     the game world. Some items simply place blocks or entities into the game world when used. They are thus an item
	///     when in the inventory and a block when placed. Some examples of objects which exhibit these properties are item
	///     frames, which turn into an entity when placed, and beds, which turn into a group of blocks when placed. When
	///     equipped, items (and blocks) briefly display their names above the HUD.
	/// </summary>
	public class Item
	{
		private short _fuelEfficiency;

		internal Item(int id, byte metadata)
		{
			Id = id;
			Metadata = metadata;

			ItemMaterial = ItemMaterial.None;
			ItemType = ItemType.Item;
		}

		protected Item(int id, byte metadata, short fuelEfficiency) : this(id, metadata)
		{
			FuelEfficiency = fuelEfficiency;
		}

		public int Id { get; set; }
		public ItemMaterial ItemMaterial { get; set; }
		public ItemType ItemType { get; set; }
		public byte Metadata { get; set; }

		protected short FuelEfficiency
		{
			set { _fuelEfficiency = value; }
		}

		public virtual void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
		}

		protected Vector3 GetNewCoordinatesFromFace(Vector3 target, BlockFace face)
		{
			switch (face)
			{
				case BlockFace.NegativeY:
					target.Y--;
					break;
				case BlockFace.PositiveY:
					target.Y++;
					break;
				case BlockFace.NegativeZ:
					target.Z--;
					break;
				case BlockFace.PositiveZ:
					target.Z++;
					break;
				case BlockFace.NegativeX:
					target.X--;
					break;
				case BlockFace.PositiveX:
					target.X++;
					break;
				default:
					break;
			}
			return target;
		}

		public int GetDamage()
		{
			switch (ItemType)
			{
				case ItemType.Sword:
					return GetSwordDamage(ItemMaterial);
				case ItemType.Item:
					return 1;
				case ItemType.Axe:
					return GetAxeDamage(ItemMaterial);
				case ItemType.PickAxe:
					return GetPickAxeDamage(ItemMaterial);
				case ItemType.Shovel:
					return GetShovelDamage(ItemMaterial);
				default:
					return 1;
			}
		}

		protected int GetSwordDamage(ItemMaterial itemMaterial)
		{
			switch (itemMaterial)
			{
				case ItemMaterial.None:
					return 1;
				case ItemMaterial.Gold:
				case ItemMaterial.Wood:
					return 5;
				case ItemMaterial.Stone:
					return 6;
				case ItemMaterial.Iron:
					return 7;
				case ItemMaterial.Diamond:
					return 8;
				default:
					return 1;
			}
		}

		private int GetAxeDamage(ItemMaterial itemMaterial)
		{
			return GetSwordDamage(itemMaterial) - 1;
		}

		private int GetPickAxeDamage(ItemMaterial itemMaterial)
		{
			return GetSwordDamage(itemMaterial) - 2;
		}

		private int GetShovelDamage(ItemMaterial itemMaterial)
		{
			return GetSwordDamage(itemMaterial) - 3;
		}

		public virtual short GetFuelEfficiency()
		{
			return _fuelEfficiency;
		}

		public virtual Item GetSmelt()
		{
			return null;
		}
	}
}