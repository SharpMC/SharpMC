using System.IO;
using SharpMC.Storage.API;
using SharpMC.Util;
using SharpMC.World;

namespace SharpMC.Storage
{
    public class ChunkRepository : IChunkRepository
    {
        private readonly string _folder;
        private readonly ICompression _compression;

        public ChunkRepository(string folder, ICompression compression)
        {
            _compression = compression;
            _folder = folder;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public void SaveChunk(IChunkColumn chunk, ChunkCoordinates pos)
        {
            var file = GetFileName(_folder, pos.X, pos.Z);
            var raw = chunk.ToArray();
            var output = _compression.Compress(raw);
            File.WriteAllBytes(file, output);
        }

        public bool Exists(ChunkCoordinates coordinates)
        {
            var x = coordinates.X;
            var z = coordinates.Z;
            return File.Exists(GetFileName(_folder, x, z));
        }

        public IChunkColumn LoadChunk(ChunkCoordinates pos)
        {
            var file = GetFileName(_folder, pos.X, pos.Z);
            var raw = File.ReadAllBytes(file);
            var input = _compression.Decompress(raw);
            var column = new ChunkColumn();
            column.FromArray(input);
            return column;
        }

        private static string GetFileName(string folder, int x, int z)
        {
            return $"{folder}/{x}.{z}.cfile";
        }
    }
}