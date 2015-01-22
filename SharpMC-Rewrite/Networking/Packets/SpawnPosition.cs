namespace SharpMCRewrite
{
    public class SpawnPosition : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x05;
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
            Vector3 D = Globals.Level.Generator.GetSpawnPoint ();
            long Data = (((long)D.X & 0x3FFFFFF) << 38) | (((long)D.Y & 0xFFF) << 26) | ((long)D.Z & 0x3FFFFFF);
            buffer.WriteVarInt (PacketID);
            buffer.WriteLong (Data);
            buffer.FlushData ();
        }
    }
}

