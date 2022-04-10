using SharpMC.Network.Util;
using System;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class Spectate : Packet<Spectate>, IToServer
    {
        public byte ServerId => 0x2d;

        public Guid Target { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Target = stream.ReadUuid();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteUuid(Target);
        }
    }
}
