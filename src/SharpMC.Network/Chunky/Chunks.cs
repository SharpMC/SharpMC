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

        private static IEnumerable<ChunkSection> ReadAll(IMinecraftReader input, int chunkSize)
        {
            for (var sectionY = 0; sectionY < chunkSize; sectionY++)
            {
                var section = ChunkSection.Read(input);
                yield return section;
            }
        }

        public static byte[] WriteAll(IEnumerable<ChunkSection> sections)
        {
            using var mem = new MemoryStream();
            using var output = new MinecraftStream(mem);
            WriteAll(output, sections);
            WriteZeroChunk(output);
            return mem.ToArray();
        }

        private static void WriteAll(IMinecraftWriter output, IEnumerable<ChunkSection> sections)
        {
            foreach (var section in sections)
                if (section != null)
                    ChunkSection.Write(output, section);
        }

        private static void WriteZeroChunk(IMinecraftWriter output)
        {
            for (var i = 0; i < 16; i++)
                output.WriteByte(0);
        }
    }
}