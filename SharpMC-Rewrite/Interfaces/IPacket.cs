using System;
using SharpMCRewrite.Networking;

namespace SharpMCRewrite
{
    public interface IPacket
    {
        int PacketID
        {
            get;
        }

        void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments);
        void Write(ClientWrapper state, object[] Arguments);
    }
}

