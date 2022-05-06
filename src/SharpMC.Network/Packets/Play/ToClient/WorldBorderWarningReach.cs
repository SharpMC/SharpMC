using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class WorldBorderWarningReach : Packet<WorldBorderWarningReach>, IToClient
    {
        public byte ClientId => 0x46;

        public int WarningBlocks { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WarningBlocks = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(WarningBlocks);
        }
    }
}
