namespace SharpMCRewrite
{
    public class Disconnect : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x40;
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
            buffer.WriteString ("{ \"text\": \"" + (string)Arguments[0] + "\" }");
            buffer.FlushData ();
        }
    }
}
    