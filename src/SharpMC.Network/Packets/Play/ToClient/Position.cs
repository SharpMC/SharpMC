using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Position : Packet<Position>, IToClient
    {
        public byte ClientId => 0x38;

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public sbyte Flags { get; set; }
        public int TeleportId { get; set; }
        public bool DismountVehicle { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            Yaw = stream.ReadFloat();
            Pitch = stream.ReadFloat();
            Flags = stream.ReadSByte();
            TeleportId = stream.ReadVarInt();
            DismountVehicle = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteFloat(Yaw);
            stream.WriteFloat(Pitch);
            stream.WriteSByte(Flags);
            stream.WriteVarInt(TeleportId);
            stream.WriteBool(DismountVehicle);
        }
    }
}
