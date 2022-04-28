namespace SharpMC.Blocks
{
    public interface IBlock
    {
        int DefaultState { get; }

        int Id { get; }
    }
}