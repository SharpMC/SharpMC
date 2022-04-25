namespace SharpMC.World.Standard.Settings
{
    public interface IWorldSettings
    {
        int LeafRadius { get; }
        bool UseVanillaTrees { get; }
        int WaterLevel { get; }
    }
}