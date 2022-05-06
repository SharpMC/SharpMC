using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Respawn : Packet<Respawn>, IToClient
    {
        public byte ClientId => 0x3d;

        public byte[] Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public byte Gamemode { get; set; }
        public byte PreviousGamemode { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public bool CopyMetadata { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            // TODO Dimension = stream.ReadNbt();
            WorldName = stream.ReadString();
            HashedSeed = stream.ReadLong();
            Gamemode = stream.ReadByte();
            PreviousGamemode = stream.ReadByte();
            IsDebug = stream.ReadBool();
            IsFlat = stream.ReadBool();
            CopyMetadata = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            // TODO stream.WriteNbt(Dimension);
            stream.WriteString(WorldName);
            stream.WriteLong(HashedSeed);
            stream.WriteByte(Gamemode);
            stream.WriteByte(PreviousGamemode);
            stream.WriteBool(IsDebug);
            stream.WriteBool(IsFlat);
            stream.WriteBool(CopyMetadata);
        }
    }
}
