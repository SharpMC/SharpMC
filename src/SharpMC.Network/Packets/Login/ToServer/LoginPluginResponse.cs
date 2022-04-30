using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Login.ToServer
{
    public class LoginPluginResponse : Packet<LoginPluginResponse>, IToServer
    {
        public byte ServerId => 0x02;

        public int MessageId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            MessageId = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(MessageId);
        }
    }
}
