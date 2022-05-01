namespace SharpMC.World.API
{
    public interface IWorldPackager
    {
        IWorldGenerator Wrap(IWorldProvider parent);
    }
}