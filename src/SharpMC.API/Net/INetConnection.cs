namespace SharpMC.API.Net
{
    public interface INetConnection
    {
        void SendPacket(INetPacket packet);

        void SendKeepAlive();

        bool KeepAliveReady { get; }
    }
}