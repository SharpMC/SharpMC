using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Collect : Packet<Collect>, IToClient
    {
        public byte ClientId => 0x61;

        public int CollectedEntityId { get; set; }
        public int CollectorEntityId { get; set; }
        public int PickupItemCount { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            CollectedEntityId = stream.ReadVarInt();
            CollectorEntityId = stream.ReadVarInt();
            PickupItemCount = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(CollectedEntityId);
            stream.WriteVarInt(CollectorEntityId);
            stream.WriteVarInt(PickupItemCount);
        }
    }
}
