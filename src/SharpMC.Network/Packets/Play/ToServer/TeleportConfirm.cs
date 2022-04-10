using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class TeleportConfirm : Packet<TeleportConfirm>, IToServer
    {
        public byte ServerId => 0x00;

        public int TeleportId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            TeleportId = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(TeleportId);
        }
    }
}
