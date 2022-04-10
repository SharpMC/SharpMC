using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class Chat : Packet<Chat>, IToServer
    {
        public byte ServerId => 0x03;

        public string Message { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Message = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Message);
        }
    }
}
