using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class DeathCombatEvent : Packet<DeathCombatEvent>, IToClient
    {
        public byte ClientId => 0x35;

        public int PlayerId { get; set; }
        public int EntityId { get; set; }
        public string Message { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            PlayerId = stream.ReadVarInt();
            EntityId = stream.ReadInt();
            Message = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(PlayerId);
            stream.WriteInt(EntityId);
            stream.WriteString(Message);
        }
    }
}
