using System.Collections.Generic;
using SharpMC.API.Entities;

namespace SharpMC.API.Worlds
{
    public interface ILevelManager
    {
        ILevel? MainLevel { get; }

        ILevel[] GetLevels();

        void TeleportToMain(IPlayer player);

        void TeleportToLevel(IPlayer player, string levelName);

        IEnumerable<IPlayer> GetAllPlayers();

        ILevel GetLevel(LevelType type);
        
        ILevel? GetLevel(IPlayer player);
    }
}