using System;
using Microsoft.Extensions.Logging;
using SharpMC.Entities;
using SharpMC.Logging;
using SharpMC.Players;
using SharpMC.World.Generators;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using SharpMC.API.Enums;
using SharpMC.Network.Packets;
using SharpMC.Util;

namespace SharpMC.World
{
    public class Level : IDisposable
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(Level));

        public string Name { get; set; }
        public PlayerLocation SpawnPoint { get; set; }
        
        public void CalculateTps(Player player)
        {
            throw new NotImplementedException();
        }

        public void SaveChunks()
        {
            throw new NotImplementedException();
        }

        public void RemovePlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public void RelayBroadcast(object packet)
        {
            throw new NotImplementedException();
        }
        
        public int Timetorain { get; set; }
        public int WorldTime { get; set; }
        public string LvlName { get; }
        public GameMode DefaultGamemode { get; set; } = GameMode.Creative;
        
        public int PlayerCount => Players.Count;
        public IWorldGenerator WorldGenerator { get; }

        private EntityManager EntityManager { get; }
        private ConcurrentDictionary<int, Player> Players { get; }
        private ConcurrentDictionary<int, Entity> Entities { get; }

        public Level(string name, IWorldGenerator worldGenerator)
        {
            Name = name;
            WorldGenerator = worldGenerator;
            EntityManager = new EntityManager();
            Players = new ConcurrentDictionary<int, Player>();
            Entities = new ConcurrentDictionary<int, Entity>();
            SpawnPoint = worldGenerator.GetSpawnPoint();
        }

        private Timer TickTimer { get; set; }
        private object _tickLock = new object();
        public long GameTick { get; set; }

        private void OnTick(object state)
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

        public void Initialize()
        {
            var chunkLoading = Stopwatch.StartNew();
            var count = 0;
            foreach (var i in GenerateChunks(null, new ChunkCoordinates(SpawnPoint),
                         new Dictionary<Tuple<int, int>, byte[]>(), 8))
            {
                count++;
            }
            chunkLoading.Stop();
            Log.LogInformation("World pre-cache {0} chunks completed in {1}ms", count,
                chunkLoading.ElapsedMilliseconds);
            TickTimer = new Timer(OnTick, null, 50, 50);
        }

        public void RelayBroadcast(Packet packet)
        {
            var players = Players.Values.ToArray();
            RelayBroadcast(players, packet);
        }

        public void RelayBroadcast(IEnumerable<Player> players, Packet packet)
        {
            foreach (var i in players)
            {
                i.Connection.SendPacket(packet);
            }
        }

        public void AddEntity(Entity entity)
        {
        }

        public void RemoveEntity(Entity entity)
        {
        }

        public virtual void AddPlayer(Player newPlayer, bool spawn)
        {
            EntityManager.AddEntity(newPlayer);
            if (Players.TryAdd(newPlayer.EntityId, newPlayer))
            {
                SpawnToAll(newPlayer);
                foreach (var entity in Entities.Values.ToArray())
                {
                    entity.SpawnToPlayers(new[] {newPlayer});
                }
            }
            newPlayer.IsSpawned = spawn;
        }

        public void SpawnToAll(Player newPlayer)
        {
            var players = Players.Values.ToArray();
            newPlayer.SpawnToPlayers(players);
            foreach (var player in Players.Values.ToArray())
            {
                player.SpawnToPlayers(new[] {newPlayer});
            }
        }

        public virtual void RemovePlayer(Player player, bool despawn = true)
        {
            if (Players.TryRemove(player.EntityId, out var p))
            {
                DespawnFromAll(player);
                foreach (var entity in Entities.Values.ToArray())
                {
                    entity.DespawnFromPlayers(new[] {player});
                }
                EntityManager.RemoveEntity(null, player);
            }
            player.IsSpawned = !despawn;
        }

        public void DespawnFromAll(Player newPlayer)
        {
            var players = Players.Values.ToArray();
            newPlayer.DespawnFromPlayers(players);
            foreach (var player in Players.Values.ToArray())
            {
                player.DespawnFromPlayers(new[] {newPlayer});
            }
        }

        public IEnumerable<byte[]> GenerateChunks(Player player, ChunkCoordinates chunkPosition,
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
                        var distance = (x * x) + (z * z);
                        if (distance > radiusSquared)
                        {
                            // TODO ?! : continue;
                        }
                        var chunkX = (int) (x + centerX);
                        var chunkZ = (int) (z + centerZ);
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
                            player.UnloadChunk(new ChunkCoordinates(chunkKey.Item1, chunkKey.Item2));
                        }
                        chunksUsed.Remove(chunkKey);
                    }
                }
                foreach (var pair in newOrders.OrderBy(pair => pair.Value))
                {
                    if (chunksUsed.ContainsKey(pair.Key))
                        continue;
                    if (WorldGenerator == null)
                        continue;
                    var coordinates = new ChunkCoordinates(pair.Key.Item1, pair.Key.Item2);
                    var chunkColumn = WorldGenerator.GenerateChunkColumn(coordinates);
                    byte[] chunk = null;
                    if (chunkColumn != null)
                    {
                        chunk = chunkColumn.ToArray();
                    }
                    chunksUsed.Add(pair.Key, chunk);
                    yield return chunk;
                }
            }
        }

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_disposed)
                    return;
                _disposed = true;
                TickTimer.Dispose();
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