using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class MapChunk : Packet<MapChunk>, IToClient
    {
        public byte ClientId => 0x22;

        public int X { get; set; }
        public int Z { get; set; }
        public byte[] Heightmaps { get; set; }
        public bool TrustEdges { get; set; }

        public byte[] Data { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            X = stream.ReadInt();
            Z = stream.ReadInt();
            Heightmaps = stream.ReadNbt();
            TrustEdges = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            if (Data != null)
            {
                stream.Write(Data);
                return;
            }
            stream.WriteInt(X);
            stream.WriteInt(Z);
            stream.WriteNbt(Heightmaps);
            stream.WriteBool(TrustEdges);
        }
    }
}