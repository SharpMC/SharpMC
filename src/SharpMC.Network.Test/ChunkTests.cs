using System;
using System.Collections.Generic;
using System.IO;
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
        [InlineData(1, 17417)]
        [InlineData(2, 17429)]
        [InlineData(3, 18699)]
        public void ShouldReadChunk(int idx, int size)
        {
            var input = idx switch {1 => MapChunkData1, 2 => MapChunkData2, _ => MapChunkData3};
            Assert.Equal(size, input.Length);

            var b = PaletteType.Biome;

            // TODO
        }

        [Theory]
        [InlineData(1, 17417)]
        [InlineData(2, 17429)]
        [InlineData(3, 18699)]
        public void ShouldWriteChunk(int idx, int size)
        {
            var expected = idx switch {1 => MapChunkData1, 2 => MapChunkData2, _ => MapChunkData3};
            Assert.Equal(size, expected.Length);

            var b = PaletteType.Chunk;

            // TODO
        }

        private static IList<ChunkSection> Setup()
        {
            var chunkSectionsToTest = new List<ChunkSection> {new()};
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
                var stream = new MemoryStream();
                var output = new MinecraftStream(stream);
                ChunkSection.Write(output, section, 4);
                var input = new MinecraftStream(new MemoryStream(stream.ToArray()));
                ChunkSection decoded;
                try
                {
                    decoded = ChunkSection.Read(input, 4);
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