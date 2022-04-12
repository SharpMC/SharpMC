using SharpMC.Network.Binary;
using SharpMC.Network.Util;
using static SharpMC.Network.Util.BinaryTool;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Login : Packet<Login>, IToClient
    {
        public byte ClientId => 0x26;

        public int EntityId { get; set; }
        public bool IsHardcore { get; set; }
        public byte GameMode { get; set; }
        public sbyte PreviousGameMode { get; set; }
        public string[] WorldNames { get; set; }
        public LoginDimCodec DimensionCodec { get; set; }
        public LoginDim Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }

        public int[] HashedSeeds
        {
            get => ToIntArray(HashedSeed);
            set => HashedSeed = ToLong(value);
        }

        public int MaxPlayers { get; set; }
        public int ViewDistance { get; set; }
        public int SimulationDistance { get; set; }
        public bool ReducedDebugInfo { get; set; }
        public bool EnableRespawnScreen { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadInt();
            IsHardcore = stream.ReadBool();
            GameMode = stream.ReadByte();
            PreviousGameMode = stream.ReadSByte();
            WorldNames = stream.ReadStringArray();
            DimensionCodec = stream.ReadNbt<LoginDimCodec>();
            Dimension = stream.ReadNbt<LoginDim>();
            WorldName = stream.ReadString();
            HashedSeed = stream.ReadLong();
            MaxPlayers = stream.ReadVarInt();
            ViewDistance = stream.ReadVarInt();
            SimulationDistance = stream.ReadVarInt();
            ReducedDebugInfo = stream.ReadBool();
            EnableRespawnScreen = stream.ReadBool();
            IsDebug = stream.ReadBool();
            IsFlat = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteInt(EntityId);
            stream.WriteBool(IsHardcore);
            stream.WriteByte(GameMode);
            stream.WriteSByte(PreviousGameMode);
            stream.WriteStringArray(WorldNames);
            stream.WriteNbt(DimensionCodec);
            stream.WriteNbt(Dimension);
            stream.WriteString(WorldName);
            stream.WriteLong(HashedSeed);
            stream.WriteVarInt(MaxPlayers);
            stream.WriteVarInt(ViewDistance);
            stream.WriteVarInt(SimulationDistance);
            stream.WriteBool(ReducedDebugInfo);
            stream.WriteBool(EnableRespawnScreen);
            stream.WriteBool(IsDebug);
            stream.WriteBool(IsFlat);
        }
    }
}