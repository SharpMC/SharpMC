using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class DeclareCommands : Packet<DeclareCommands>, IToClient
    {
        public byte ClientId => 0x12;

        public int RootIndex { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            RootIndex = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(RootIndex);
        }
    }
}
