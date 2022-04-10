using System.Collections.Generic;
using SharpMC.API.Entities;

namespace SharpMC.API.Worlds
{
    public interface ILevelManager
    {
        void TeleportToLevel(IPlayer player, string name);
        
        void TeleportToMain(IPlayer player);
        
        ILevel MainLevel { get; }
        
        IEnumerable<ILevel> GetLevels();
        
        IEnumerable<IPlayer> GetAllPlayers();
    }
}