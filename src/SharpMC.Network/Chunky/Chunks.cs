using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpMC.Network.Util;

namespace SharpMC.Chunky
{
    public static class Chunks
    {
        public static ChunkSection[] ReadAll(byte[] chunkData, int chunkSize)
        {
            using var mem = new MemoryStream(chunkData);
            using var input = new MinecraftStream(mem);
            var sections = ReadAll(input, chunkSize);
            return sections.ToArray();
        }

        public static IEnumerable<ChunkSection> ReadAll(IMinecraftReader input, int chunkSize)
        {
            for (var sectionY = 0; sectionY < chunkSize; sectionY++)
            {
                var section = ChunkSection.Read(input);
                yield return section;
            }
        }
    }
}