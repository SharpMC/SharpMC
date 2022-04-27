using System;
using System.Numerics;
using SharpMC.API.Enums;
using SharpMC.API.Plugins;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;

namespace SharpMC.API.Entities
{
    public interface IPlayer : ICommandSender
    {
        void Kick(string message);

        void SendChat(string message, ChatColor color = null);

        bool ToggleOperatorStatus();

        string Username { get; }

        ILevel Level { get; set; }

        GameMode Gamemode { get; set; }

        IPosition KnownPosition { get; }

        Guid Uuid { get; }

        bool IsOperator { get; set; }

        ILocation Location { get; }

        IWorld World { get; }

        void PositionChanged(Vector3 vector, double value);

        void UnloadChunk(int x, int z);
    }
}