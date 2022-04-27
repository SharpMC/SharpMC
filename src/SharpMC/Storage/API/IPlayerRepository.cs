using SharpMC.API.Entities;

namespace SharpMC.Storage.API
{
    public interface IPlayerRepository
    {
        void SavePlayer(IPlayer player);

        bool Exists(string saveName);

        IPlayer LoadPlayer((string uuid, string userName) id, dynamic level);
    }
}