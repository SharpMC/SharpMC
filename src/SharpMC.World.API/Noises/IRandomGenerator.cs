namespace SharpMC.World.API.Noises
{
    public interface IRandomGenerator
    {
        int GetRandomNumber(int min, int max);
    }
}