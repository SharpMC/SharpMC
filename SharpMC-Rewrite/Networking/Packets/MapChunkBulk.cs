namespace SharpMCRewrite
{
    public class MapChunkBulk : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x26;
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
            buffer.WriteBool (true);
            buffer.WriteVarInt (49);
            for (int i = 0; i < 49; i++)
            {
                buffer.Write (Globals.ChunkColums [i].GetBytes ());
            }
            buffer.FlushData ();
        }
    }
}

