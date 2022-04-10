using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class StopSound : Packet<StopSound>, IToClient
    {
        public byte ClientId => 0x5e;

        public sbyte Flags { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Flags = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteSByte(Flags);
        }
    }
}
