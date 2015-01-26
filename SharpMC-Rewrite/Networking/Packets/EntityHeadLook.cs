namespace SharpMCRewrite
{
    public class EntityHeadLook : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x19;
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
            if ((int)Arguments [0] != state.Player.UniqueServerID)
            {
                buffer.WriteVarInt (PacketID);

            }

        }
    }
}

