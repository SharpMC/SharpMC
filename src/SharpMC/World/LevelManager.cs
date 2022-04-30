using System;
using System.Collections.Generic;
using SharpMC.API.Entities;
using SharpMC.API.Worlds;

namespace SharpMC.World
{
    internal sealed class LevelManager : ILevelManager
    {
        public ILevel MainLevel { get; }

        public IEnumerable<ILevel> GetLevels()
        {
            throw new NotImplementedException();
        }

        public void TeleportToMain(IPlayer player)
        {
            throw new NotImplementedException();
        }

        public void TeleportToLevel(IPlayer player, string levelName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPlayer> GetAllPlayers()
        {
            throw new NotImplementedException();
        }
    }
}