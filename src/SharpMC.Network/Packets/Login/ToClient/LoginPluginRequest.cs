using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Login.ToClient
{
    public class LoginPluginRequest : Packet<LoginPluginRequest>, IToClient
    {
        public byte ClientId => 0x04;

        public int MessageId { get; set; }
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            MessageId = stream.ReadVarInt();
            Channel = stream.ReadString();
            Data = stream.ReadBuffer();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(MessageId);
            stream.WriteString(Channel);
            stream.WriteBuffer(Data);
        }
    }
}
