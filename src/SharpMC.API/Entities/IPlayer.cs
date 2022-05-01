using System.Numerics;
using SharpMC.API.Enums;
using SharpMC.API.Net;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;

namespace SharpMC.API.Entities
{
    public interface IPlayer : IEntity
    {
        GameMode Gamemode { get; set; }

        ILevel Level { get; }

        ILocation KnownPosition { get; }

        string UserName { get; }

        INetConnection Connection { get; }

        void SendChat(string message, ChatColor? color = null);

        void Kick(string name);

        bool ToggleOperatorStatus();

        void PositionChanged(Vector3 pos, float yaw);

        void UnloadChunk(ICoordinates pos);

        void OnTick();
    }
}