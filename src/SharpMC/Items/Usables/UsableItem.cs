using System;
using System.Collections;
using System.Numerics;
using SharpMC.API.Enums;
using Material = SharpMC.Items.ItemMaterial;
using Type = SharpMC.Items.ItemType;

namespace SharpMC.Items.Usables
{
    /// <summary>
    /// Items are objects which only exist within the player's inventory and hands - which
    /// means, they cannot be placed in the game world. Some items simply place blocks or
    /// entities into the game world when used. They are thus an item when in the inventory
    /// and a block when placed. Some examples of objects which exhibit these properties
    /// are item frames, which turn into an entity when placed, and beds, which turn into
    /// a group of blocks when placed. When equipped, items (and blocks) briefly display
    /// their names above the HUD.
    /// </summary>
    public abstract class UsableItem : Item, IUsableItem
    {
        public bool IsUsable { get; protected set; }

        public abstract void UseItem(IItemLevel world, ILevelPlayer player,
            Vector3 coordinates, BlockFace face);

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
            }
            return target;
        }

        public byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

        public virtual Item GetSmelt()
        {
            return null;
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

        protected int GetSwordDamage(ItemMaterial itemMaterial)
        {
            switch (itemMaterial)
            {
                case Material.None:
                    return 1;
                case Material.Gold:
                case Material.Wood:
                    return 5;
                case Material.Stone:
                    return 6;
                case Material.Iron:
                    return 7;
                case Material.Diamond:
                    return 8;
                default:
                    return 1;
            }
        }

        private int GetAxeDamage(ItemMaterial itemMaterial)
        {
            return GetSwordDamage(itemMaterial) - 1;
        }

        public int GetDamage()
        {
            var material = ItemMaterial.GetValueOrDefault();
            switch (ItemType)
            {
                case Type.Sword:
                    return GetSwordDamage(material);
                case Type.Item:
                    return 1;
                case Type.Axe:
                    return GetAxeDamage(material);
                case Type.PickAxe:
                    return GetPickAxeDamage(material);
                case Type.Shovel:
                    return GetShovelDamage(material);
                default:
                    return 1;
            }
        }

        protected UsableItem(ushort id, byte metadata, short fuelEfficiency)
            : this(id, metadata)
        {
            FuelEfficiency = fuelEfficiency;
        }

        public byte Metadata { get; set; }
        public int MaxStackSize { get; set; }
        public bool IsBlock { get; set; }

        protected short FuelEfficiency
        {
            set { _fuelEfficiency = value; }
        }

        private short _fuelEfficiency;

        internal UsableItem(ushort id, byte metadata)
        {
            Id = id;
            Metadata = metadata;

            ItemMaterial = Material.None;
            ItemType = Type.Item;
            IsUsable = false;
            MaxStackSize = 64;
            IsBlock = false;
        }

        internal UsableItem() : this(0, 0)
        {
        }
    }
}