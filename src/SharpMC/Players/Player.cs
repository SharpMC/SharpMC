using System;
using SharpMC.API.Entities;
using SharpMC.API.Enums;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;
using SharpMC.Entities;
using SharpMC.Net;

namespace SharpMC.Players
{
    public class Player : Entity, IPlayer
    {
        public McNetConnection Connection { get; }
        private MinecraftServer Server { get; }
        
        public Player(McNetConnection connection, MinecraftServer server, string username) : base(null)
        {
            Server = server;
            Connection = connection;
            Username = username;
            Uuid = Guid.NewGuid();
        }
        
        public void Kick(string message)
        {
            throw new NotImplementedException();
        }

        public void SendChat(string message, ChatColor color = null)
        {
            throw new NotImplementedException();
        }

        public bool ToggleOperatorStatus()
        {
            throw new NotImplementedException();
        }

        public string Username { get; set; }
        public ILevel Level { get; set; }
        public GameMode Gamemode { get; set; }
        public IPosition KnownPosition { get; }
        public Guid Uuid { get; set; }
        public bool IsOperator { get; set; }
        public ILocation Location { get; }
        public IWorld World { get; }
        public string DisplayName { get; set; }
        public AuthResponse AuthResponse { get; set; }
        public int ViewDistance { get; set; }

        public void PositionChanged(Vector3 vector, double value)
        {
            throw new NotImplementedException();
        }

        public void InitiateGame()
        {
            throw new NotImplementedException();
        }
    }
}