using System.Collections.Generic;
using SharpMC.API.Entities;
using SharpMC.API.Net;

namespace SharpMC.API.Worlds
{
    public interface ILevel
    {
        void SaveChunks();

        void CalculateTps(IPlayer player);

        int TimeToRain { get; set; }

        int WorldTime { get; set; }

        int PlayerCount { get; }

        IEnumerable<IPlayer> GetPlayers();

        void RelayBroadcast(INetPacket packet);

        void RemovePlayer(IPlayer player);
    }
}