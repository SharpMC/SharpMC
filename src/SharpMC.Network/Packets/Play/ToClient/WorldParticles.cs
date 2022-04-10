using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class WorldParticles : Packet<WorldParticles>, IToClient
    {
        public byte ClientId => 0x24;

        public int ParticleId { get; set; }
        public bool LongDistance { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float OffsetZ { get; set; }
        public float ParticleData { get; set; }
        public int Particles { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ParticleId = stream.ReadInt();
            LongDistance = stream.ReadBool();
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            OffsetX = stream.ReadFloat();
            OffsetY = stream.ReadFloat();
            OffsetZ = stream.ReadFloat();
            ParticleData = stream.ReadFloat();
            Particles = stream.ReadInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteInt(ParticleId);
            stream.WriteBool(LongDistance);
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteFloat(OffsetX);
            stream.WriteFloat(OffsetY);
            stream.WriteFloat(OffsetZ);
            stream.WriteFloat(ParticleData);
            stream.WriteInt(Particles);
        }
    }
}
