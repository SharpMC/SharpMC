namespace SharpMC.World.Anvil
{
    public interface IAnvilColumn : IChunkColumn
    {
        byte[] BiomeId { get; }
        byte GetMetadata(int x, int y, int z);
        byte GetBlocklight(int x, int y, int z);
        byte GetSkylight(int x, int y, int z);
    }
}