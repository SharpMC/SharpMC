using System.Collections.Generic;
using System.IO;
using SharpMC.Network.Util;

namespace SharpMC.Chunky
{
    public static class Chunks
    {
        public static IEnumerable<ChunkSection> ReadAll(byte[] chunkData, int chunkSize)
        {
            var mem = new MemoryStream(chunkData);
            var input = new MinecraftStream(mem);
            return ReadAll(input, chunkSize);
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