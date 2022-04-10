using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToBoth
{
    public class CloseWindow : Packet<CloseWindow>, IToClient, IToServer
    {
        public byte ClientId => 0x13;
        public byte ServerId => 0x09;

        public byte WindowId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteByte(WindowId);
        }
    }
}
