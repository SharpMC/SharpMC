using SharpMC.World;

namespace SharpMC.Blocks.Logic
{
    public class RedstoneDevice : Block
    {
        public ushort Id { get; }

        protected RedstoneDevice(ushort id)
        {
            Id = id;
        }

        public virtual void RedstoneTick(Level level)
        {
        }

        public virtual void BreakBlock(Level world)
        {
        }
    }
}