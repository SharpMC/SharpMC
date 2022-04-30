using SharpMC.API.Entities;

namespace SharpMC.API.Worlds
{
    public interface ILevel
    {
        void SaveChunks();

        void CalculateTps(IPlayer player);

        int TimeToRain { get; set; }

        int WorldTime { get; set; }

        int PlayerCount { get; }
    }
}