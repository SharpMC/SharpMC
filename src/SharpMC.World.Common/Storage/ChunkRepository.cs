using System.IO;
using SharpMC.API.Worlds;
using SharpMC.World.API.Chunks;
using SharpMC.World.API.Storage;

namespace SharpMC.World.Common.Storage
{
    public class ChunkRepository : IChunkRepository
    {
        private readonly string _folder;
        private readonly ICompression _compression;
        private readonly IChunkColumnFactory _factory;

        public ChunkRepository(string folder, ICompression compression, 
            IChunkColumnFactory factory)
        {
            _factory = factory;
            _compression = compression;
            _folder = folder;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public void SaveChunk(IChunkColumn chunk, ICoordinates pos)
        {
            var file = GetFileName(_folder, pos.X, pos.Z);
            var raw = chunk.ToArray();
            var output = _compression.Compress(raw);
            File.WriteAllBytes(file, output);
        }

        public bool Exists(ICoordinates coordinates)
        {
            var x = coordinates.X;
            var z = coordinates.Z;
            return File.Exists(GetFileName(_folder, x, z));
        }

        public IChunkColumn LoadChunk(ICoordinates pos)
        {
            var file = GetFileName(_folder, pos.X, pos.Z);
            var raw = File.ReadAllBytes(file);
            var input = _compression.Decompress(raw);
            var column = _factory.CreateColumn();
            column.FromArray(input);
            return column;
        }

        private static string GetFileName(string folder, int x, int z)
        {
            return $"{folder}/{x}.{z}.cfile";
        }
    }
}