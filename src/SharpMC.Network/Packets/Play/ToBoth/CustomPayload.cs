using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToBoth
{
    public class CustomPayload : Packet<CustomPayload>, IToClient, IToServer
    {
        public byte ClientId => 0x18;
        public byte ServerId => 0x0a;

        public string Channel { get; set; }
        public byte[] Data { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Channel = stream.ReadString();
            Data = stream.ReadBuffer();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Channel);
            stream.WriteBuffer(Data);
        }
    }
}
