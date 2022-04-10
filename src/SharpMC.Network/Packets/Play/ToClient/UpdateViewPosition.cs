using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class UpdateViewPosition : Packet<UpdateViewPosition>, IToClient
    {
        public byte ClientId => 0x49;

        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ChunkX = stream.ReadVarInt();
            ChunkZ = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(ChunkX);
            stream.WriteVarInt(ChunkZ);
        }
    }
}
