using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Camera : Packet<Camera>, IToClient
    {
        public byte ClientId => 0x47;

        public int CameraId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            CameraId = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(CameraId);
        }
    }
}
