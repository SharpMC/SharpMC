using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class ResourcePackSend : Packet<ResourcePackSend>, IToClient
    {
        public byte ClientId => 0x3c;

        public string Url { get; set; }
        public string Hash { get; set; }
        public bool Forced { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Url = stream.ReadString();
            Hash = stream.ReadString();
            Forced = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Url);
            stream.WriteString(Hash);
            stream.WriteBool(Forced);
        }
    }
}
