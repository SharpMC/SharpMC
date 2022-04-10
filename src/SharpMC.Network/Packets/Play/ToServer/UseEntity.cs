using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class UseEntity : Packet<UseEntity>, IToServer
    {
        public byte ServerId => 0x0d;

        public int Target { get; set; }
        public int Mouse { get; set; }
        public bool Sneaking { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Target = stream.ReadVarInt();
            Mouse = stream.ReadVarInt();
            Sneaking = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Target);
            stream.WriteVarInt(Mouse);
            stream.WriteBool(Sneaking);
        }
    }
}
