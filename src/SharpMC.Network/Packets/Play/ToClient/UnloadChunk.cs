using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class UnloadChunk : Packet<UnloadChunk>, IToClient
    {
        public byte ClientId => 0x1d;

        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ChunkX = stream.ReadInt();
            ChunkZ = stream.ReadInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteInt(ChunkX);
            stream.WriteInt(ChunkZ);
        }
    }
}
