using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpMC.Chunky;
using SharpMC.Chunky.Palette;
using SharpMC.Data;
using SharpMC.Network.Util;
using Xunit;
using static SharpMC.Blocks.KnownBlocks;
using static SharpMC.Chunky.DataPalette;
using static SharpMC.Chunky.Palette.PaletteType;
using static SharpMC.Network.Test.DataBunch3;
using static SharpMC.Network.Test.TestHelper;

namespace SharpMC.Network.Test
{
    public class ChunkTests
    {
        [Theory]
        [InlineData(1, 17417, 34804)]
        [InlineData(2, 17429, 33810)]
        [InlineData(3, 18699, 36517)]
        public void ShouldReadChunk(int idx, int size, int blocks)
        {
            var input = idx switch {1 => MapChunkData1, 2 => MapChunkData2, _ => MapChunkData3};
            Assert.Equal(size, input.Length);

            const int count = 24;
            var sections = Chunks.ReadAll(input, count);
            Assert.Equal(count, sections.Length);

            WriteTexts($"{nameof(ShouldReadChunk)}{idx}", null, ToJson(sections));
            var foundBlocks = sections.Sum(s => s.BlockCount);
            Assert.Equal(blocks, foundBlocks);
        }

        [Theory]
        [InlineData(1, 17417)]
        [InlineData(2, 17429)]
        [InlineData(3, 18699)]
        public void ShouldWriteChunk(int idx, int size)
        {
            var expected = idx switch {1 => MapChunkData1, 2 => MapChunkData2, _ => MapChunkData3};
            Assert.Equal(size, expected.Length);

            const int count = 24;
            var sections = new ChunkSection[count];

            var original = Chunks.ReadAll(expected, count);
            WriteTexts($"{nameof(ShouldWriteChunk)}{idx}", ToJson(original), ToJson(sections));

            var actual = Chunks.WriteAll(sections);
            WriteBytes($"{nameof(ShouldWriteChunk)}{idx}", expected, actual);

            Assert.Equal(ToJson(original), ToJson(sections));
            Assert.Equal(ToJson(expected), ToJson(actual));
        }

        [Fact]
        public void ShouldEncodeChunkSection()
        {
            var toTest = new List<ChunkSection> {new()};
            var section = new ChunkSection {[(0, 0, 0)] = 10};
            toTest.Add(section);
            var singletonPalette = new SingletonPalette(20);
            var dataPalette = new DataPalette(singletonPalette, null, Chunk, GlobalPaletteBitsPerEntry);
            var biomePalette = new DataPalette(singletonPalette, null, Biome, GlobalBiomeBitsPerEntry);
            section = new ChunkSection(4096, dataPalette, biomePalette);
            toTest.Add(section);

            foreach (var oneSection in toTest)
            {
                using var stream = new MemoryStream();
                using var output = new MinecraftStream(stream);
                ChunkSection.Write(output, oneSection);
                using var raw = new MemoryStream(stream.ToArray());
                using var input = new MinecraftStream(raw);
                ChunkSection decoded;
                try
                {
                    decoded = ChunkSection.Read(input);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(oneSection.ToString(), e);
                }
                Assert.Equal(ToJson(oneSection), ToJson(decoded));
                Assert.Equal(oneSection, decoded);
            }
        }
    }
}