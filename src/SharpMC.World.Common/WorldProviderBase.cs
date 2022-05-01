using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using SharpMC.API.Chunks;
using SharpMC.API.Entities;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;
using SharpMC.World.API;

namespace SharpMC.World.Common
{
    public abstract class WorldProviderBase : IWorldProvider
    {
        public IEnumerable<IChunkColumn> GenerateChunks(int viewDistance,
            List<Tuple<int, int>> chunksUsed, IPlayer player)
        {
            lock (chunksUsed)
            {
                var newOrders = new Dictionary<Tuple<int, int>, double>();
                var radiusSquared = viewDistance / Math.PI;
                var radius = Math.Ceiling(Math.Sqrt(radiusSquared));
                var centerX = (int) player.KnownPosition.X >> 4;
                var centerZ = (int) player.KnownPosition.Z >> 4;

                for (var x = -radius; x <= radius; ++x)
                {
                    for (var z = -radius; z <= radius; ++z)
                    {
                        var distance = x * x + z * z;
                        if (distance > radiusSquared)
                        {
                            continue;
                        }
                        var chunkX = (int) (x + centerX);
                        var chunkZ = (int) (z + centerZ);
                        var index = new Tuple<int, int>(chunkX, chunkZ);
                        newOrders[index] = distance;
                    }
                }

                if (newOrders.Count > viewDistance)
                {
                    foreach (var pair in newOrders.OrderByDescending(pair => pair.Value))
                    {
                        if (newOrders.Count <= viewDistance) break;
                        newOrders.Remove(pair.Key);
                    }
                }

                foreach (var chunkKey in chunksUsed.ToArray())
                {
                    if (!newOrders.ContainsKey(chunkKey))
                    {
                        chunksUsed.Remove(chunkKey);
                        var pos = new ChunkCoordinates(chunkKey.Item1, chunkKey.Item2);
                        new Task(() => player.UnloadChunk(pos)).Start();
                    }
                }

                foreach (var pair in newOrders.OrderBy(pair => pair.Value))
                {
                    if (chunksUsed.Contains(pair.Key)) continue;

                    var chunk = GenerateChunkColumn(new Vector2(pair.Key.Item1, pair.Key.Item2));
                    chunksUsed.Add(pair.Key);

                    yield return chunk;
                }
            }
        }

        protected abstract IChunkColumn GenerateChunkColumn(Vector2 vector);

        public abstract void PopulateChunk(IChunkColumn chunk, ICoordinates pos);

        public abstract ILocation SpawnPoint { get; }
    }
}