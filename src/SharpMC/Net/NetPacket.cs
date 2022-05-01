using SharpMC.API.Net;
using SharpMC.Network.Packets;

namespace SharpMC.Net
{
    internal record NetPacket(Packet Payload) : INetPacket;
}