namespace SharpMC.Blocks
{
    public class BlockAir : Block
    {
        public BlockAir() : base(0)
        {
            IsReplacible = true;
            IsSolid = false;
        }
    }
}