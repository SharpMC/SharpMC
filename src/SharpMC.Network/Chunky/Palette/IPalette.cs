namespace SharpMC.Network.Chunky.Palette
{
    public interface IPalette
    {
        int Size { get; }

        int StateToId(int state);

        int IdToState(int id);

        int BitsPerEntry { get; }
    }
}