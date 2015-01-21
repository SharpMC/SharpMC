using System;

namespace SharpMCRewrite
{
    public class KeepAlive : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x00;
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
            int id = new Random().Next (0, 100);
            buffer.WriteVarInt (PacketID);
            buffer.WriteVarInt (id);
            buffer.FlushData ();
        }
    }
}

