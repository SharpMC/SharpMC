using SharpMC.Network.Packets.API;
using System;
using SharpMC.Network.API;
using System.Numerics;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SpawnEntityPainting : Packet<SpawnEntityPainting>, IToClient
    {
        public byte ClientId => 0x03;

        public int EntityId { get; set; }
        public Guid EntityUUID { get; set; }
        public int Title { get; set; }
        public Vector3 Location { get; set; }
        public byte Direction { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            EntityUUID = stream.ReadUuid();
            Title = stream.ReadVarInt();
            Location = stream.ReadPosition();
            Direction = stream.ReadByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteUuid(EntityUUID);
            stream.WriteVarInt(Title);
            stream.WritePosition(Location);
            stream.WriteByte(Direction);
        }
    }
}
