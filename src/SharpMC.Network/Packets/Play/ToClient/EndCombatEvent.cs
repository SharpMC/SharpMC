using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EndCombatEvent : Packet<EndCombatEvent>, IToClient
    {
        public byte ClientId => 0x33;

        public int Duration { get; set; }
        public int EntityId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Duration = stream.ReadVarInt();
            EntityId = stream.ReadInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Duration);
            stream.WriteInt(EntityId);
        }
    }
}
