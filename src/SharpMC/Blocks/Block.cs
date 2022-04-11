using SharpMC.Util;

namespace SharpMC.Blocks
{
    public class Block
    {
        public short Id { get; }
        public byte Metadata { get; }
        public bool IsSolid { get; protected set; }
        public BlockCoordinates Coordinates { get; set; }

        public Block(short id, byte metadata)
        {
            Id = id;
            Metadata = metadata;
            IsSolid = true;
        }

        public Block(short id) : this(id, 0)
        {
        }

        public virtual void OnPlace()
        {
        }
    }
}