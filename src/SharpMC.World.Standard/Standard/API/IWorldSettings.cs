namespace SharpMC.World.Standard.API
{
    public interface IWorldSettings
    {
        int LeafRadius { get; }
        bool UseVanillaTrees { get; }
        int WaterLevel { get; }
    }
}