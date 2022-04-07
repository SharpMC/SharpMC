namespace SharpMC.Blocks
{
    public class RedstonePowerSource : RedstoneDevice
    {
        internal RedstonePowerSource(ushort id) : base(id)
        {
        }

        public virtual bool IsActive()
        {
            return true;
        }
    }
}