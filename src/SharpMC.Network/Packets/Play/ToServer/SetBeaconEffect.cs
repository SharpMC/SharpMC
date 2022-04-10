using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class SetBeaconEffect : Packet<SetBeaconEffect>, IToServer
    {
        public byte ServerId => 0x24;

        public int PrimaryEffect { get; set; }
        public int SecondaryEffect { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            PrimaryEffect = stream.ReadVarInt();
            SecondaryEffect = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(PrimaryEffect);
            stream.WriteVarInt(SecondaryEffect);
        }
    }
}
