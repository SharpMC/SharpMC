namespace SharpMCRewrite
{
    public class PlayerDigging : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x07;
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
            int Status = buffer.ReadByte ();
            Vector3 Position = buffer.ReadPosition ();
            int Face = buffer.ReadByte ();
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

