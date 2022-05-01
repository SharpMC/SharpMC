using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Extensions.Logging;
using SharpMC.API;
using SharpMC.API.Chunks;
using SharpMC.API.Entities;
using SharpMC.API.Enums;
using SharpMC.API.Net;
using SharpMC.API.Players;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;
using SharpMC.Data;
using SharpMC.Net;
using SharpMC.Network.Binary.Model;
using SharpMC.Network.Binary.Special;
using SharpMC.Network.Core;
using SharpMC.Network.Packets.Play.ToBoth;
using SharpMC.Network.Packets.Play.ToClient;
using static SharpMC.Util.Numbers;

namespace SharpMC.Players
{
    public sealed class Player : IPlayer
    {
        private readonly ILogger<Player> _log;

        public INetConnection Connection { get; }
        private IServer Server { get; }
        public string UserName { get; set; }

        public Player(ILogger<Player> log, INetConnection connection, 
            IServer server, string userName)
        {
            _log = log;
            Connection = connection;
            Server = server;
            UserName = userName;
            ChunksUsed = new Dictionary<Tuple<int, int>, byte[]>();
        }

        public GameMode Gamemode { get; set; }
        public ILevel Level { get; set; }
        public PlayerLocation KnownPosition { get; set; }
        public string DisplayName { get; set; }
        public IAuthResponse AuthResponse { get; set; }

        public int ViewDistance { get; set; }
        private Dictionary<Tuple<int, int>, byte[]> ChunksUsed;
        private long _timeSinceLastKeepAlive;
        private ChunkCoordinates _prevChunkCoordinates;

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

        public void OnTick()
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

        public void InitiateGame()
        {
            var level = Server.LevelManager.GetLevel(this);
            if (level == null)
            {
                Disconnect("No level assigned to player!");
                return;
            }
            Level = level;
            KnownPosition = Level.SpawnPoint;
            Gamemode = Level.DefaultGameMode;
            SendJoinGame();
            _prevChunkCoordinates = new ChunkCoordinates(KnownPosition);
            SendChunksForKnownPosition(_prevChunkCoordinates);
            SendPlayerPositionAndLook();
            Level.AddPlayer(this, true);
        }

        private void SendChunksForKnownPosition(ChunkCoordinates coords)
        {
            foreach (var (b, c) in Level.GenerateChunks(this, coords, ChunksUsed, ViewDistance))
            {
                var map = new MapChunk
                {
                    X = c.X,
                    Z = c.Z,
                    TrustEdges = true,
                    ChunkData = b,
                    HeightMaps = new HeightMaps
                    {
                        MotionBlocking = GetLongs(36, 2292305770412047999, 17079008895),
                        WorldSurface = GetLongs(36, 2292305770412047999, 17079008895)
                    },
                    BlockEntities = Array.Empty<ChunkBlockEntity>()
                };
                Connection.SendPacket(map);
            }
        }

        public void Disconnect(string reason)
        {
            _log.LogWarning("Kicking player {0} with reason: {1}", UserName, reason);
        }

        public void SendPlayerPositionAndLook()
        {
            var loc = KnownPosition with { };
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
            var obj = new UnloadChunk
            {
                ChunkX = coordinates.X,
                ChunkZ = coordinates.Z
            };
            Connection.SendPacket(obj);
        }

        public void DespawnFromPlayers(IPlayer[] players)
        {
            var packet = new PlayerInfo
            {
                Action = PlayerListAction.RemovePlayer,
                UUID = Uuid
            };
            Level.RelayBroadcast(players, packet);
        }

        public void DespawnEntity()
        {
            IsSpawned = false;
            Level.DespawnFromAll(this);
        }

        private void SendJoinGame()
        {
            var joinGame = new Login
            {
                EntityId = 167,
                IsHardcore = false,
                GameMode = (byte)Gamemode,
                PreviousGameMode = -1,
                WorldNames = Defaults.WorldNames,
                WorldName = Defaults.WorldName, // WorldName = "flat"
                HashedSeeds = new[] { -660566458, -1901654650 },
                MaxPlayers = 20,
                ViewDistance = 10,
                SimulationDistance = 10,
                ReducedDebugInfo = false,
                EnableRespawnScreen = true,
                IsDebug = false,
                IsFlat = false,
                DimensionCodec = new LoginDimCodec
                {
                    Realms = Defaults.Realms,
                    Biomes = Defaults.Biomes
                },
                Dimension = Defaults.CurrentDim
            };
            Connection.SendPacket(joinGame);

            SendJoinSuffix();
        }

        private void SendJoinSuffix()
        {
            var diff = new Difficulty
            {
                _Difficulty = 1,
                DifficultyLocked = false
            };
            Connection.SendPacket(diff);

            var able = new Abilities
            {
                Flags = 0,
                FlyingSpeed = 0.05000000074505806f,
                WalkingSpeed = 0.10000000149011612f
            };
            Connection.SendPacket(able);

            var slot = new HeldItemSlot
            {
                Slot = 4
            };
            Connection.SendPacket(slot);

            var brand = new CustomPayload
            {
                Channel = "minecraft:brand",
                Data = new byte[] { 7, 118, 97, 110, 105, 108, 108, 97 }
            };
            Connection.SendPacket(brand);
        }

        public int EntityId { get; set; }

        public bool IsSpawned { get; set; }

        public Guid Uuid { get; set; }

        public void SendChat(string message, ChatColor? color = null)
        {
            throw new System.NotImplementedException();
        }

        public void SpawnToPlayers(IPlayer[] players)
        {
            PlayerListProperty p = null;
            if (AuthResponse != null)
            {
                foreach (var i in ((AuthResponse)AuthResponse).Properties)
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
                Gamemode = (int)Gamemode,
                Name = UserName,
                UUID = Uuid
            };
            if (p != null)
            {
                packet.Properties = new[] { p };
            }
            Level.RelayBroadcast(players, packet);
            var spp = new NamedEntitySpawn
            {
                EntityId = EntityId,
                Pitch = (sbyte)KnownPosition.Pitch.ToRadians(),
                Yaw = (sbyte)KnownPosition.Yaw.ToRadians(),
                X = KnownPosition.X,
                Y = KnownPosition.Y,
                Z = KnownPosition.Z,
                PlayerUUID = Uuid
            };
            Level.RelayBroadcast(players, spp);
        }
    }
}