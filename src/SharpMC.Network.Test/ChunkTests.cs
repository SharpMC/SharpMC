using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpMC.Chunky;
using SharpMC.Chunky.Palette;
using SharpMC.Network.Util;
using Xunit;
using static SharpMC.Network.Test.DataBunch3;
using static SharpMC.Network.Test.TestHelper;

namespace SharpMC.Network.Test
{
    public class ChunkTests
    {
        [Theory]
        [InlineData(1, 17417, -9, -13, new[] {33, 18684, 18684, 0, 18684, 1, 6, 34})]
        [InlineData(2, 17429, -7, -12, new[] {33, 18684, 18684, 3955, 4, 1, 1, 1})]
        [InlineData(3, 18699, -6, -5, new[] {33, 18684, 17714, 18684, 18684, 2, 1, 1})]
        public void ShouldReadChunk(int idx, int size, int x, int z, int[] array)
        {
            var input = idx switch {1 => MapChunkData1, 2 => MapChunkData2, _ => MapChunkData3};
            Assert.Equal(size, input.Length);

            using var cache = new ChunkCache {ChunkHeightY = 384, ChunkMinY = -64};
            var chunkSize = cache.ChunkHeightY;
            var sections = Chunks.ReadAll(input, chunkSize).ToArray();
            Assert.Equal(24, sections.Length);
            var javaChunks = sections.Select(c => c.ChunkData).ToArray();
            cache.AddToCache(x, z, javaChunks);

            const int offset = 4;
            var stateIds = new int[array.Length];
            for (var i = -offset; i < array.Length - offset; i++)
            {
                var block = cache.GetBlockAt(x * 16, i * 16, z * 16);
                stateIds[i + offset] = block;
            }
            Assert.Equal(array, stateIds);
        }

        [Theory]
        [InlineData(1, 17417)]
        [InlineData(2, 17429)]
        [InlineData(3, 18699)]
        public void ShouldWriteChunk(int idx, int size)
        {
            var expected = idx switch { 1 => MapChunkData1, 2 => MapChunkData2, _ => MapChunkData3 };
            Assert.Equal(size, expected.Length);

            var b = PaletteType.Chunk;

            // TODO
        }

        private static IList<ChunkSection> Setup()
        {
            var chunkSectionsToTest = new List<ChunkSection> { new() };
            var section = new ChunkSection();
            section.SetBlock(0, 0, 0, 10);
            chunkSectionsToTest.Add(section);
            var singletonPalette = new SingletonPalette(20);
            var dataPalette = new DataPalette(singletonPalette, null,
                PaletteType.Chunk, DataPalette.GlobalPaletteBitsPerEntry);
            var biomePalette = new DataPalette(singletonPalette, null,
                PaletteType.Biome, 4);
            section = new ChunkSection(4096, dataPalette, biomePalette);
            chunkSectionsToTest.Add(section);
            return chunkSectionsToTest;
        }

        [Fact]
        public void ShouldEncodeChunkSection()
        {
            var chunkSectionsToTest = Setup();
            foreach (var section in chunkSectionsToTest)
            {
                using var stream = new MemoryStream();
                using var output = new MinecraftStream(stream);
                ChunkSection.Write(output, section);
                using var raw = new MemoryStream(stream.ToArray());
                using var input = new MinecraftStream(raw);
                ChunkSection decoded;
                try
                {
                    decoded = ChunkSection.Read(input);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(section.ToString(), e);
                }
                Assert.Equal(ToJson(section), ToJson(decoded));
            }
        }
    }
}