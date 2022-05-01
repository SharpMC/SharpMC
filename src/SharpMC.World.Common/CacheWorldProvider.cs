using System;
using System.Collections.Generic;
using System.Linq;
using SharpMC.API.Chunks;
using SharpMC.API.Players;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;
using SharpMC.World.API;
using SharpMC.World.API.Chunks;

namespace SharpMC.World.Common
{
    public class CacheWorldProvider : IWorldGenerator, IDisposable
    {
        private readonly object _syncLock;
        private readonly Dictionary<ChunkCoordinates, IChunkColumn> _chunkCache;
        private readonly IWorldProvider _parent;
        private readonly IChunkRepository _repo;
        private readonly IChunkColumnFactory _factory;

        public CacheWorldProvider(IWorldProvider parent, IChunkRepository repo,
            IChunkColumnFactory factory)
        {
            _factory = factory;
            _syncLock = new object();
            _parent = parent;
            _repo = repo;
            _chunkCache = new Dictionary<ChunkCoordinates, IChunkColumn>();
        }

        public void PopulateChunk(IChunkColumn chunk, ChunkCoordinates pos)
            => _parent.PopulateChunk(chunk, pos);

        public PlayerLocation SpawnPoint
            => _parent.SpawnPoint;

        public IChunkColumn GenerateChunkColumn(ChunkCoordinates coordinates)
        {
            lock (_syncLock)
            {
                if (_chunkCache.TryGetValue(coordinates, out var c))
                    return c;
            }

            if (_repo.Exists(coordinates))
            {
                var cd = _repo.LoadChunk(coordinates);
                lock (_syncLock)
                {
                    if (!_chunkCache.ContainsKey(coordinates))
                        _chunkCache.Add(coordinates, cd);
                }
                return cd;
            }

            var chunk = _factory.CreateColumn();
            chunk.AddAirToPalette();
            PopulateChunk(chunk, coordinates);

            lock (_syncLock)
            {
                if (!_chunkCache.ContainsKey(coordinates))
                    _chunkCache.Add(coordinates, chunk);
            }

            return chunk;
        }

        public void ClearCache()
        {
            lock (_syncLock)
            {
                _chunkCache.Clear();
            }
        }

        public void SaveChunks()
        {
            lock (_syncLock)
            {
                foreach (var (pos, chunk) in _chunkCache.ToArray())
                {
                    if (chunk.IsDirty)
                    {
                        _repo.SaveChunk(chunk, pos);
                    }
                }
            }
        }

        public void Dispose()
        {
            SaveChunks();
            ClearCache();
        }
    }
}