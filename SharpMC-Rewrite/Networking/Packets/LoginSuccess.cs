using System;

namespace SharpMCRewrite
{
    public class LoginSuccess : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x02;
            }
        }

        public void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
        }

        public void Write(ClientWrapper state, object[] Arguments)
        {
            state.MinecraftStream.WriteVarInt (PacketID);
            state.MinecraftStream.WriteString ((string)Arguments[0]);
            state.MinecraftStream.WriteString ((string)Arguments[1]);
            state.MinecraftStream.FlushData ();
        }
    }
}

