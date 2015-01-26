namespace SharpMCRewrite
{
    public class HeldItemChange : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x09;
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
            short slot = buffer.ReadShort ();
            state.Player.PlayerInventory.HeldItem = slot;
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

