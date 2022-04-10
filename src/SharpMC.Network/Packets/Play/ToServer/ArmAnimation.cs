using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class ArmAnimation : Packet<ArmAnimation>, IToServer
    {
        public byte ServerId => 0x2c;

        public int Hand { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Hand = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Hand);
        }
    }
}
