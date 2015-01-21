namespace SharpMCRewrite
{
    public interface IPacket
    {
        int PacketID
        {
            get;
        }

        bool IsPlayePacket
        {
            get;
        }

        void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments);
        void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments);
    }
}

