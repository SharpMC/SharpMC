namespace SharpMCRewrite
{
    public class TimeUpdate : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x03;
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
            buffer.WriteLong (Globals.Level.Day);
            buffer.WriteLong (Globals.Level.Tick);
            buffer.FlushData ();
        }
    }
}

