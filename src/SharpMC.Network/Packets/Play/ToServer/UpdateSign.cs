using SharpMC.Network.API;
using System.Numerics;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class UpdateSign : Packet<UpdateSign>, IToServer
    {
        public byte ServerId => 0x2b;

        public Vector3 Location { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
            Text1 = stream.ReadString();
            Text2 = stream.ReadString();
            Text3 = stream.ReadString();
            Text4 = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteString(Text1);
            stream.WriteString(Text2);
            stream.WriteString(Text3);
            stream.WriteString(Text4);
        }
    }
}
