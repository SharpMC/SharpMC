using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class RemoveEntityEffect : Packet<RemoveEntityEffect>, IToClient
    {
        public byte ClientId => 0x3b;

        public int EntityId { get; set; }
        public int EffectId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            EffectId = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteVarInt(EffectId);
        }
    }
}
