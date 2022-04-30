using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntityEffect : Packet<EntityEffect>, IToClient
    {
        public byte ClientId => 0x65;

        public int EntityId { get; set; }
        public int EffectId { get; set; }
        public sbyte Amplifier { get; set; }
        public int Duration { get; set; }
        public sbyte HideParticles { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            EffectId = stream.ReadVarInt();
            Amplifier = stream.ReadSByte();
            Duration = stream.ReadVarInt();
            HideParticles = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteVarInt(EffectId);
            stream.WriteSByte(Amplifier);
            stream.WriteVarInt(Duration);
            stream.WriteSByte(HideParticles);
        }
    }
}
