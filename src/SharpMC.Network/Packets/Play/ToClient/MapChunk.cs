using SharpMC.Network.API;
using SharpMC.Network.Binary.Model;
using SharpMC.Network.Binary.Special;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class MapChunk : Packet<MapChunk>, IToClient
    {
        public byte ClientId => 0x22;

        public int X { get; set; }
        public int Z { get; set; }
        public HeightMaps HeightMaps { get; set; }
        public byte[] ChunkData { get; set; }
        public ChunkBlockEntity[] BlockEntities { get; set; }
        public bool TrustEdges { get; set; }
        public long[] SkyLightMask { get; set; }
        public long[] BlockLightMask { get; set; }
        public long[] EmptySkyLightMask { get; set; }
        public long[] EmptyBlockLightMask { get; set; }
        public byte[][] SkyLight { get; set; }
        public byte[][] BlockLight { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            X = stream.ReadInt();
            Z = stream.ReadInt();
            HeightMaps = stream.ReadNbt<HeightMaps>();
            ChunkData = stream.ReadByteArray();
            BlockEntities = stream.ReadBitFieldArray<ChunkBlockEntity>();
            TrustEdges = stream.ReadBool();
            SkyLightMask = stream.ReadLongArray();
            BlockLightMask = stream.ReadLongArray();
            EmptySkyLightMask = stream.ReadLongArray();
            EmptyBlockLightMask = stream.ReadLongArray();
            SkyLight = stream.ReadByteArrays();
            BlockLight = stream.ReadByteArrays();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteInt(X);
            stream.WriteInt(Z);
            stream.WriteNbt(HeightMaps);
            stream.WriteByteArray(ChunkData);
            stream.WriteBitFieldArray(BlockEntities);
            stream.WriteBool(TrustEdges);
            stream.WriteLongArray(SkyLightMask);
            stream.WriteLongArray(BlockLightMask);
            stream.WriteLongArray(EmptySkyLightMask);
            stream.WriteLongArray(EmptyBlockLightMask);
            stream.WriteByteArrays(SkyLight);
            stream.WriteByteArrays(BlockLight);
        }
    }
}