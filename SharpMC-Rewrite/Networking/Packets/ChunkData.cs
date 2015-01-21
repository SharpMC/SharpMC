namespace SharpMCRewrite
{
    public class ChunkData : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x21;
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

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
            buffer.WriteVarInt (PacketID);
            buffer.Write ((byte[])Arguments [0]);
            buffer.FlushData ();
        }
    }
}

