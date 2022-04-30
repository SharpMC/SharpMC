using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class AttachEntity : Packet<AttachEntity>, IToClient
    {
        public byte ClientId => 0x4e;

        public int EntityId { get; set; }
        public int VehicleId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadInt();
            VehicleId = stream.ReadInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteInt(EntityId);
            stream.WriteInt(VehicleId);
        }
    }
}
