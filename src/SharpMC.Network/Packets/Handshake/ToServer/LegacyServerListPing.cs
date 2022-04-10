using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Handshake.ToServer
{
    public class LegacyServerListPing : Packet<LegacyServerListPing>, IToServer
    {
        public byte ServerId => 0xfe;

        public byte Payload { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Payload = stream.ReadByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteByte(Payload);
        }
    }
}
