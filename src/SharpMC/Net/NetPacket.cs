using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.API.Net;
using SharpMC.Network.Packets;

namespace SharpMC.Net
{
    internal record NetPacket(Packet Payload) : INetPacket;
}