using SharpMC.Network.Packets.API;
using System;
using SharpMC.Network.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class BossBar : Packet<BossBar>, IToClient
    {
        public byte ClientId => 0x0d;

        public Guid EntityUUID { get; set; }
        public int Action { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityUUID = stream.ReadUuid();
            Action = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteUuid(EntityUUID);
            stream.WriteVarInt(Action);
        }
    }
}
