using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Login.ToClient
{
    public class Disconnect : Packet<Disconnect>, IToClient
    {
        public byte ClientId => 0x00;

        public string Reason { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Reason = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Reason);
        }
    }
}
