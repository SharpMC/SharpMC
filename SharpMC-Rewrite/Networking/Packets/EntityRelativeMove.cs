namespace SharpMCRewrite
{
    public class EntityRelativeMove : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x15;
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
            Player g = (Player)Arguments [0];

            buffer.WriteVarInt (0x15);
            buffer.WriteVarInt (g.UniqueServerID);
            //buffer.WriteByte (g.Coordinates.X * 32);
        }
    }
}

