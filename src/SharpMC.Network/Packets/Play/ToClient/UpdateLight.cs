using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class UpdateLight : Packet<UpdateLight>, IToClient
    {
        public byte ClientId => 0x25;

        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }
        public bool TrustEdges { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ChunkX = stream.ReadVarInt();
            ChunkZ = stream.ReadVarInt();
            TrustEdges = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(ChunkX);
            stream.WriteVarInt(ChunkZ);
            stream.WriteBool(TrustEdges);
        }
    }
}
