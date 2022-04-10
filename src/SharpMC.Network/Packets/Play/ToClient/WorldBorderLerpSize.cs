using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class WorldBorderLerpSize : Packet<WorldBorderLerpSize>, IToClient
    {
        public byte ClientId => 0x43;

        public double OldDiameter { get; set; }
        public double NewDiameter { get; set; }
        public int Speed { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            OldDiameter = stream.ReadDouble();
            NewDiameter = stream.ReadDouble();
            Speed = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteDouble(OldDiameter);
            stream.WriteDouble(NewDiameter);
            stream.WriteVarInt(Speed);
        }
    }
}
