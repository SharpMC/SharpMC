using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Status.ToClient
{
    public class ServerInfo : Packet<ServerInfo>, IToClient
    {
        public byte ClientId => 0x00;

        public string Response { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Response = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Response);
        }
    }
}