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
        /// Argument 1: Block ID (INT)
        /// Argument 2: Meta (INT)
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="buffer">Buffer.</param>
        /// <param name="Arguments">Arguments.</param>
        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
            buffer.WriteVarInt (PacketID);
            buffer.WritePosition ((Vector3)Arguments [0]);
            buffer.WriteVarInt ((int)Arguments [1] << 4 | (int)Arguments[2]);
            buffer.FlushData ();
        }
    }
}

