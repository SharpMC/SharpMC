namespace SharpMC.Blocks
{
    public static class BlockFactory
    {
        public static Block GetBlockById(ushort id, short metadata = 0)
        {
            if (id == 1) return new BlockStone {Metadata = (byte) metadata};
            if (id == 46) return new BlockTnt();
            if (id == 0) return new BlockAir();
            if (id == 51) return new BlockFire();
            if (id == 7) return new BlockBedrock();
            if (id == 3) return new BlockDirt();
            if (id == 2) return new BlockGrass();
            if (id == 16) return new BlockCoalOre();
            if (id == 21) return new BlockLapisLazuliOre();
            if (id == 56) return new BlockDiamondOre();
            if (id == 10) return new BlockFlowingLava();
            if (id == 8) return new BlockFlowingWater();
            if (id == 11) return new BlockFlowingLava();
            if (id == 9) return new BlockFlowingWater();
            if (id == 31 && metadata == 1) return new BlockTallGrass();
            if (id == 5 && metadata == 0) return new OakWoodPlank();
            if (id == 69) return new BlockLever();
            if (id == 55) return new BlockRedstoneDust();
            if (id == 123) return new BlockRedstoneLampInActive();
            if (id == 124) return new BlockRedstoneLampActive();
            if (id == 63) return new BlockStandingSign();

            // Doors
            if (id == 64 || id == 71 || id == 193 ||
                id == 194 || id == 195 || id == 196 ||
                id == 197 || id == 324 || id == 330 ||
                id == 427 || id == 428 || id == 429 ||
                id == 430 || id == 431) return new Door(id);

            // Stairs
            if (id == 67 || id == 53 || id == 108 ||
                id == 109 || id == 114 || id == 128 ||
                id == 134 || id == 135 || id == 136 ||
                id == 156 || id == 163 || id == 164 ||
                id == 180) return new StairsBlock(id);

            // Slabs
            if (id == 44) return new Slab((byte) metadata);
            if (id == 43) return new DoubleSlab((byte) metadata);

            return new Block(id);
        }
    }
}