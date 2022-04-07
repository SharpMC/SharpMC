namespace SharpMC.Items
{
    internal class ItemFactory
    {
        public static Item GetItemById(ushort id, byte metadata = 0)
        {
            return GetItemById((short) id, metadata);
        }

        public static Item GetItemById(short id, byte metadata = 0)
        {
            if (id == 286)
            {
                // Debugging item, prints metadata into Console.
                return new InfoTool();
            }

            if (id == 259) return new ItemFlintAndSteel();
            if (id == 263) return new ItemCoal();
            if (id == 276) return new ItemDiamondSword();
            if (id == 310) return new ItemDiamondHelmet();
            if (id == 311) return new ItemDiamondChestplate();
            if (id == 312) return new ItemDiamondLeggings();
            if (id == 313) return new ItemDiamondBoots();
            if (id == 306) return new ItemIronHelmet();
            if (id == 307) return new ItemIronChestplate();
            if (id == 308) return new ItemIronLeggings();
            if (id == 309) return new ItemIronBoots();
            if (id == 267) return new ItemIronSword();
            if (id == 326) return new ItemWaterBucket();
            if (id == 327) return new ItemLavaBucket();
            if (id == 325) return new ItemBucket();
            if (id == 280) return new ItemStick();
            if (id == 332) return new ItemSnowball();
            if (id == 295) return new ItemWheatSeeds();
            if (id == 331) return new ItemRedstone();
            if (id == 323) return new ItemSign();

            return new Item((ushort) id, metadata);
        }
    }
}