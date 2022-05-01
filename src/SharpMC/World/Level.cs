using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using SharpMC.API.Chunks;
using SharpMC.API.Entities;
using SharpMC.API.Enums;
using SharpMC.API.Net;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;
using SharpMC.World.API;

namespace SharpMC.World
{
    internal sealed class Level : ILevel, IDisposable
    {
        private readonly ILogger<Level> _log;
        private readonly IWorldGenerator _worldGenerator;
        private readonly IEntityManager _entityManager;

        private ConcurrentDictionary<int, IPlayer> Players { get; }

        public Level(ILogger<Level> log, IWorldGenerator worldGenerator, 
            IEntityManager entityManager)
        {
            _log = log;
            _worldGenerator = worldGenerator;
            _entityManager = entityManager;
            Players = new ConcurrentDictionary<int, IPlayer>();
        }

        public int TimeToRain { get; set; }
        public int WorldTime { get; set; }

        public void SaveChunks()
        {
            throw new NotImplementedException();
        }

        public void CalculateTps(IPlayer player)
        {
            throw new NotImplementedException();
        }

        public int PlayerCount => Players.Count;

        public GameMode DefaultGameMode { get; }
        public IEnumerable<IPlayer> GetPlayers() => Players.Values;

        public ILocation SpawnPoint => _worldGenerator.SpawnPoint;

        public void Initialize()
        {
            var pos = new ChunkCoordinates(SpawnPoint);
            var dict = new Dictionary<Tuple<int, int>, byte[]>();
            var count = 0;
            var chunkLoading = Stopwatch.StartNew();
            foreach (var _ in GenerateChunks(null, pos, dict, 8))
            {
                count++;
            }
            chunkLoading.Stop();
            _log.LogInformation("World pre-cache {0} chunks completed in {1}ms", count,
                chunkLoading.ElapsedMilliseconds);
            TickTimer = new Timer(OnTick, null, 50, 50);
        }

        public IEnumerable<(byte[], ChunkCoordinates)> GenerateChunks(
           IPlayer player, ChunkCoordinates chunkPosition,
           Dictionary<Tuple<int, int>, byte[]> chunksUsed, double radius)
        {
            lock (chunksUsed)
            {
                var newOrders = new Dictionary<Tuple<int, int>, double>();
                var radiusSquared = Math.Pow(radius, 2);
                var centerX = chunkPosition.X;
                var centerZ = chunkPosition.Z;
                for (var x = -radius; x <= radius; ++x)
                {
                    for (var z = -radius; z <= radius; ++z)
                    {
                        var distance = x * x + z * z;
                        if (distance > radiusSquared)
                        {
                            // TODO ?! : continue;
                        }
                        var chunkX = (int)(x + centerX);
                        var chunkZ = (int)(z + centerZ);
                        var index = new Tuple<int, int>(chunkX, chunkZ);
                        newOrders[index] = distance;
                    }
                }
                foreach (var chunkKey in chunksUsed.Keys.ToArray())
                {
                    if (!newOrders.ContainsKey(chunkKey))
                    {
                        if (player != null)
                        {
                            var pos = new ChunkCoordinates(chunkKey.Item1, chunkKey.Item2);
                            player.UnloadChunk(pos);
                        }
                        chunksUsed.Remove(chunkKey);
                    }
                }
                foreach (var pair in newOrders.OrderBy(pair => pair.Value))
                {
                    if (chunksUsed.ContainsKey(pair.Key))
                        continue;
                    if (_worldGenerator == null)
                        continue;
                    var coordinates = new ChunkCoordinates(pair.Key.Item1, pair.Key.Item2);
                    var chunkColumn = _worldGenerator.GenerateChunkColumn(coordinates);
                    byte[]? chunk = null;
                    if (chunkColumn != null)
                    {
                        chunk = chunkColumn.ToArray();
                    }
                    chunksUsed.Add(pair.Key, chunk);
                    yield return (chunk, coordinates);
                }
            }
        }

        public void RelayBroadcast(IPlayer[] players, INetPacket packet)
        {
            foreach (var i in players)
            {
                i.Connection.SendPacket(packet);
            }
        }

        public void AddPlayer(IPlayer newPlayer, bool spawn)
        {
            _entityManager.AddEntity(newPlayer);
            if (Players.TryAdd(newPlayer.EntityId, newPlayer))
            {
                SpawnToAll(newPlayer);
                foreach (var entity in _entityManager.Entities.ToArray())
                {
                    entity.SpawnToPlayers(new[] { newPlayer });
                }
            }
            newPlayer.IsSpawned = spawn;
        }

        public void SpawnToAll(IPlayer newPlayer)
        {
            var players = Players.Values.ToArray();
            newPlayer.SpawnToPlayers(players);
            foreach (var player in Players.Values.ToArray())
            {
                player.SpawnToPlayers(new[] { newPlayer });
            }
        }

        public void RemovePlayer(IPlayer player)
            => RemovePlayer(player, true);

        public void RemovePlayer(IPlayer player, bool despawn)
        {
            if (Players.TryRemove(player.EntityId, out var p))
            {
                DespawnFromAll(player);
                foreach (var entity in _entityManager.Entities.ToArray())
                {
                    entity.DespawnFromPlayers(new[] {player});
                }
                _entityManager.RemoveEntity(default, player);
            }
            player.IsSpawned = !despawn;
        }

        public void DespawnFromAll(IPlayer newPlayer)
        {
            var players = Players.Values.ToArray();
            newPlayer.DespawnFromPlayers(players);
            foreach (var player in Players.Values.ToArray())
            {
                player.DespawnFromPlayers(new[] { newPlayer });
            }
        }

        public void RelayBroadcast(INetPacket packet)
        {
            var players = Players.Values.ToArray();
            RelayBroadcast(players, packet);
        }

        private readonly object _tickLock = new();
        public long GameTick { get; set; }

        private void OnTick(object? _)
        {
            if (!Monitor.TryEnter(_tickLock))
            {
                return;
            }
            try
            {
                foreach (var player in Players.Values.ToArray())
                {
                    player.OnTick();
                }
            }
            finally
            {
                Monitor.Exit(_tickLock);
                GameTick++;
            }
        }

        private Timer? TickTimer { get; set; }

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_disposed)
                    return;
                _disposed = true;
                TickTimer?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Level()
        {
            Dispose(false);
        }
    }
}