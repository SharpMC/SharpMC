using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SetPassengers : Packet<SetPassengers>, IToClient
    {
        public byte ClientId => 0x54;

        public int EntityId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
        }
    }
}
