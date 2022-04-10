using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class WorldBorderWarningDelay : Packet<WorldBorderWarningDelay>, IToClient
    {
        public byte ClientId => 0x45;

        public int WarningTime { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WarningTime = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(WarningTime);
        }
    }
}
