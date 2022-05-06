using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class UpdateViewDistance : Packet<UpdateViewDistance>, IToClient
    {
        public byte ClientId => 0x4a;

        public int ViewDistance { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ViewDistance = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(ViewDistance);
        }
    }
}
