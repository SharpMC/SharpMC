namespace SharpMCRewrite
{
    public class WindowItems : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x30;
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
            byte windowID = (byte)Arguments [0];
            byte[] Data = (byte[])Arguments [1];
            short Items = (short)Arguments [2];
            buffer.WriteVarInt (PacketID);
            buffer.WriteByte (windowID);
            buffer.WriteShort (Items);
            buffer.Write (Data, 0, Data.Length);
            buffer.FlushData ();
        }
    }
}

