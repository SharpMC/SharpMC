namespace SharpMCRewrite
{
    public class PlayerPositionAndLook : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x08;
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
            buffer.WriteDouble (Globals.WorldGen.GetSpawnPoint ().X);
            buffer.WriteDouble (Globals.WorldGen.GetSpawnPoint ().Y);
            buffer.WriteDouble (Globals.WorldGen.GetSpawnPoint ().Z);
            buffer.WriteFloat (0f);
            buffer.WriteFloat (0f);
            buffer.WriteByte (111);
            buffer.FlushData ();
        }
    }
}

