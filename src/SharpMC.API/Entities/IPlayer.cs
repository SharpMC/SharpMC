using System.Numerics;
using SharpMC.API.Enums;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;

namespace SharpMC.API.Entities
{
    public interface IPlayer
    {
        GameMode Gamemode { get; set; }

        ILevel Level { get; }

        ILocation KnownPosition { get; }

        string UserName { get; }

        void SendChat(string message, ChatColor? color = null);

        void Kick(string name);

        bool ToggleOperatorStatus();

        void PositionChanged(Vector3 pos, float yaw);

        void UnloadChunk(int x, int z);
    }
}