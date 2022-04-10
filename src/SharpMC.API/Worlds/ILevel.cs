using SharpMC.API.Entities;

namespace SharpMC.API.Worlds
{
    public interface ILevel
    {
        int Timetorain { get; set; }

        int WorldTime { get; set; }

        string LvlName { get; }

        void CalculateTps(IPlayer player);

        void SaveChunks();

        void RemovePlayer(IPlayer player);
        
        void RelayBroadcast(object packet);
    }
}