namespace SharpMC.Blocks
{
    public static class BlockFactory
    {
        public static Block GetBlock(short id, byte metadata = 0)
        {
            switch (id)
            {
                case 0:
                    return new BlockAir();
                default:
                    return new Block(id, metadata);
            }
        }
    }
}