using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class FacePlayer : Packet<FacePlayer>, IToClient
    {
        public byte ClientId => 0x37;

        public int FeetEyes { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool IsEntity { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            FeetEyes = stream.ReadVarInt();
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            IsEntity = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(FeetEyes);
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteBool(IsEntity);
        }
    }
}
