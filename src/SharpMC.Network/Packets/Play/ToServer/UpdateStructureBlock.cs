using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class UpdateStructureBlock : Packet<UpdateStructureBlock>, IToServer
    {
        public byte ServerId => 0x2a;

        public System.Numerics.Vector3 Location { get; set; }
        public int Action { get; set; }
        public int Mode { get; set; }
        public string Name { get; set; }
        public byte OffsetX { get; set; }
        public byte OffsetY { get; set; }
        public byte OffsetZ { get; set; }
        public byte SizeX { get; set; }
        public byte SizeY { get; set; }
        public byte SizeZ { get; set; }
        public int Mirror { get; set; }
        public int Rotation { get; set; }
        public string Metadata { get; set; }
        public float Integrity { get; set; }
        public int Seed { get; set; }
        public byte Flags { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
            Action = stream.ReadVarInt();
            Mode = stream.ReadVarInt();
            Name = stream.ReadString();
            OffsetX = stream.ReadByte();
            OffsetY = stream.ReadByte();
            OffsetZ = stream.ReadByte();
            SizeX = stream.ReadByte();
            SizeY = stream.ReadByte();
            SizeZ = stream.ReadByte();
            Mirror = stream.ReadVarInt();
            Rotation = stream.ReadVarInt();
            Metadata = stream.ReadString();
            Integrity = stream.ReadFloat();
            Seed = stream.ReadVarInt();
            Flags = stream.ReadByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteVarInt(Action);
            stream.WriteVarInt(Mode);
            stream.WriteString(Name);
            stream.WriteByte(OffsetX);
            stream.WriteByte(OffsetY);
            stream.WriteByte(OffsetZ);
            stream.WriteByte(SizeX);
            stream.WriteByte(SizeY);
            stream.WriteByte(SizeZ);
            stream.WriteVarInt(Mirror);
            stream.WriteVarInt(Rotation);
            stream.WriteString(Metadata);
            stream.WriteFloat(Integrity);
            stream.WriteVarInt(Seed);
            stream.WriteByte(Flags);
        }
    }
}
