using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Login.ToServer
{
    public class LoginStart : Packet<LoginStart>, IToServer
    {
        public byte ServerId => 0x00;

        public string Username { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Username = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Username);
        }
    }
}
