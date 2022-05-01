using System;
using System.Numerics;
using SharpMC.API.Chunks;
using SharpMC.API.Enums;
using SharpMC.API.Net;
using SharpMC.API.Players;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;

namespace SharpMC.API.Entities
{
    public interface IPlayer : IEntity
    {
        GameMode Gamemode { get; set; }

        ILevel Level { get; set; }

        PlayerLocation KnownPosition { get; set; }

        string UserName { get; set; }
        Guid Uuid { get; set; }
        string DisplayName { get; set; }

        INetConnection Connection { get; }

        IAuthResponse AuthResponse { get; set; }

        int ViewDistance { get; set; }

        void SendChat(string message, ChatColor? color = null);

        void Kick(string name);

        bool ToggleOperatorStatus();

        void PositionChanged(Vector3 pos, float yaw);

        void UnloadChunk(ChunkCoordinates pos);

        void OnTick();

        void InitiateGame();
    }
}