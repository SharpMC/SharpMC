using System;
using System.Numerics;
using SharpMC.API;
using SharpMC.API.Entities;
using SharpMC.API.Enums;
using SharpMC.API.Net;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;

namespace SharpMC.Players
{
    public sealed class Player : IPlayer
    {
        public Player(INetConnection connection, IServer server, string username)
        {
            throw new System.NotImplementedException();
        }

        public GameMode Gamemode { get; set; }
        public ILevel Level { get; set; }
        public ILocation KnownPosition { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public INetConnection Connection { get; }
        public IAuthResponse AuthResponse { get; set; }
        public int ViewDistance { get; set; }

        public void SendChat(string message, ChatColor? color = null)
        {
            throw new System.NotImplementedException();
        }

        public void Kick(string name)
        {
            throw new System.NotImplementedException();
        }

        public bool ToggleOperatorStatus()
        {
            throw new System.NotImplementedException();
        }

        public void PositionChanged(Vector3 pos, float yaw)
        {
            throw new System.NotImplementedException();
        }

        public void UnloadChunk(ICoordinates pos)
        {
            throw new System.NotImplementedException();
        }

        public void OnTick()
        {
            throw new System.NotImplementedException();
        }

        public void InitiateGame()
        {
            throw new NotImplementedException();
        }

        public int EntityId { get; set; }

        public bool IsSpawned { get; set; }

        public Guid Uuid { get; set; }

        public void SpawnToPlayers(IPlayer[] players)
        {
            throw new System.NotImplementedException();
        }

        public void DespawnFromPlayers(IPlayer[] players)
        {
            throw new System.NotImplementedException();
        }
    }
}