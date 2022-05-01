using SharpMC.Network.Packets.API;

namespace SharpMC.API.Net
{
    public interface INetConnection
    {
        void SendPacket(IPacket packet);

        void SendKeepAlive();

        bool KeepAliveReady { get; }
    }
}