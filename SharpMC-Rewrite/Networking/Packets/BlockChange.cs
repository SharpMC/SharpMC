using SharpMCRewrite.Blocks;

namespace SharpMCRewrite
{
    public class BlockChange : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x23;
            }
        }

        public bool IsPlayePacket
        {
            get
            {
                return true;
            }
        }

        public void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }

        /// <summary>
        /// Argument 0: Block Position (VECTOR3)
        /// Argument 1: Block
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="buffer">Buffer.</param>
        /// <param name="Arguments">Arguments.</param>
        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
	        Block b = (Block)Arguments[1];
            buffer.WriteVarInt (PacketID);
            buffer.WritePosition ((Vector3)Arguments [0]);
            buffer.WriteVarInt (b.Id << 4 | b.Metadata);
            buffer.FlushData ();
        }
    }
}

