using System;
using Microsoft.Extensions.Logging;
using SharpMC.Entities;
using SharpMC.Logging;
using SharpMC.Net;
using SharpMC.Util;
using System.Collections.Generic;
using SharpMC.API.Enums;
using SharpMC.Network.Packets.Play.ToClient;

namespace SharpMC.Players
{
    public class Player : Entity
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(Player));

        public McNetConnection Connection { get; }
        public string Username { get; set; }
        public Guid UUID { get; set; }
        public string DisplayName { get; set; }
        public AuthResponse AuthResponse { get; set; } = null;
        public GameMode Gamemode { get; set; }
        public bool IsConnected => Connection.IsConnected;
        public int ViewDistance { get; set; } = 8;

        private MinecraftServer Server { get; }
        private Dictionary<Tuple<int, int>, byte[]> ChunksUsed = new Dictionary<Tuple<int, int>, byte[]>();
        private long _timeSinceLastKeepAlive;
        private ChunkCoordinates _prevChunkCoordinates;

        public Player(McNetConnection connection, MinecraftServer server, string username) : base(null)
        {
            Server = server;
            Connection = connection;
            Username = username;
            Uuid = Guid.NewGuid();
        }

        public void InitiateGame()
        {
            Level = Server.LevelManager.GetLevel(this, "default");
            if (Level == null)
            {
                Disconnect("No level assigned to player!");
                return;
            }
            KnownPosition = Level.SpawnPoint;
            Gamemode = Level.DefaultGamemode;
            SendJoinGame();
            _prevChunkCoordinates = new ChunkCoordinates(KnownPosition);
            SendChunksForKnownPosition(_prevChunkCoordinates);
            SendPlayerPositionAndLook();
            Level.AddPlayer(this, true);
        }

        public override void OnTick()
        {
            var cur = new ChunkCoordinates(KnownPosition);
            if (cur.DistanceTo(_prevChunkCoordinates) >= 2)
            {
                _prevChunkCoordinates = cur;
                SendChunksForKnownPosition(cur);
            }
            _timeSinceLastKeepAlive++;
            if (Level.GameTick % 20 == 0)
            {
                if (Connection.KeepAliveReady || _timeSinceLastKeepAlive >= 100)
                {
                    Connection.SendKeepAlive();
                    _timeSinceLastKeepAlive = 0;
                }
            }
        }

        private void SendChunksForKnownPosition(ChunkCoordinates coords)
        {
            foreach (var i in Level.GenerateChunks(this, coords, ChunksUsed, ViewDistance))
            {
                Connection.SendPacket(new MapChunk
                {
                    Data = i
                });
            }
        }

        private void SendJoinGame()
        {
            var joinGame = new Login
            {
                EntityId = EntityId,
                GameMode = (byte) Gamemode,
                IsHardcore = false,
                Dimension = 0,
                WorldName = "flat",
                ReducedDebugInfo = false
            };
            Connection.SendPacket(joinGame);
        }

        public void SendPlayerPositionAndLook()
        {
            var loc = (PlayerLocation) KnownPosition.Clone();
            var packet = new Position
            {
                Flags = 0,
                TeleportId = 0,
                X = loc.X,
                Y = loc.Y,
                Z = loc.Z,
                Yaw = loc.Yaw,
                Pitch = loc.Pitch
            };
            Connection.SendPacket(packet);
        }

        public void UnloadChunk(ChunkCoordinates coordinates)
        {
            Connection.SendPacket(new UnloadChunk
            {
                ChunkX = coordinates.X,
                ChunkZ = coordinates.Z
            });
        }

        public void Disconnect(string reason)
        {
            Log.LogWarning("Kicking player {0} with reason: {1}", Username, reason);
        }

        public Guid Uuid { get; set; }
        public bool IsOperator { get; set; }
        public Guid UniqueId { get; set; }
        public string Name { get; set; }
        public double Health { get; set; }

        public override void DespawnFromPlayers(Player[] players)
        {
            var packet = new PlayerInfo
            {
                Action = PlayerListAction.RemovePlayer,
                UUID = UUID
            };
            Level.RelayBroadcast(players, packet);
        }

        public override void DespawnEntity()
        {
            IsSpawned = false;
            Level.DespawnFromAll(this);
        }

        public override void SpawnToPlayers(Player[] players)
        {
            PlayerListProperty p = null;
            if (AuthResponse != null)
            {
                foreach (var i in AuthResponse.Properties)
                {
                    if (i.Name.Equals("textures", StringComparison.InvariantCultureIgnoreCase))
                    {
                        p = new PlayerListProperty
                        {
                            Name = i.Name,
                            Value = i.Value,
                            IsSigned = true,
                            Signature = i.Signature
                        };
                        break;
                    }
                }
            }
            var packet = new PlayerInfo
            {
                Action = PlayerListAction.AddPlayer,
                Ping = 0,
                Gamemode = (int) Gamemode,
                Name = Username,
                UUID = UUID
            };
            if (p != null)
            {
                packet.Properties = new[] {p};
            }
            Level.RelayBroadcast(players, packet);
            var spp = new NamedEntitySpawn
            {
                EntityId = EntityId,
                Pitch = (sbyte) (KnownPosition.Pitch.ToRadians()),
                Yaw = (sbyte) (KnownPosition.Yaw.ToRadians()),
                X = KnownPosition.X,
                Y = KnownPosition.Y,
                Z = KnownPosition.Z,
                PlayerUUID = UUID
            };
            Level.RelayBroadcast(players, spp);
        }

        public void SendChat(string name, ChatColor color = null)
        {
            throw new NotImplementedException();
        }
    }
}