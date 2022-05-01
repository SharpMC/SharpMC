using System.IO;
using SharpMC.API;
using SharpMC.World.API;
using SharpMC.World.API.Chunks;
using SharpMC.World.API.Storage;
using SharpMC.World.Common.Storage;

namespace SharpMC.World.Common
{
    internal sealed class CommonPackager : IWorldPackager
    {
        private readonly IHostEnv _host;
        private readonly ICompression _compression;
        private readonly IChunkColumnFactory _factory;

        public CommonPackager(IHostEnv host, ICompression compression,
            IChunkColumnFactory factory)
        {
            _host = host;
            _compression = compression;
            _factory = factory;
        }

        public IWorldGenerator Wrap(IWorldProvider parent)
        {
            var pType = parent.GetType().Name;
            var folder = pType
                .Replace("WorldProvider", string.Empty)
                .Replace("WorldGenerator", string.Empty);
            var root = _host.ContentRoot;
            folder = Path.Combine(root, nameof(World), folder);
            IChunkRepository repo = new ChunkRepository(folder, _compression, _factory);
            IWorldGenerator cache = new CacheWorldProvider(parent, repo, _factory);
            return cache;
        }
    }
}