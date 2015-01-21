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
            Vector3 D = Globals.WorldGen.GetSpawnPoint ();
            long Data = (long)(((int)D.X & 0x3FFFFFF) << 38) | (((int)D.Y & 0xFFF) << 26) | ((int)D.Z & 0x3FFFFFF);
            buffer.WriteVarInt (PacketID);
            buffer.WriteLong (Data);
            buffer.FlushData ();
        }
    }
}

