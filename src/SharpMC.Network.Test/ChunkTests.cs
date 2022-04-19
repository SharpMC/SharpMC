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
using static SharpMC.Network.Test.DataBunch3;
using static SharpMC.Network.Test.TestHelper;

namespace SharpMC.Network.Test
{
    public class ChunkTests
    {
        [Theory]
        [InlineData(1, 17417, -9, -13, new[] {33, 18684, 18684, 0, 18684, 1, 6, 34},
            new[]
            {
                "bedrock", "deepslate", "deepslate", "air",
                "deepslate", "stone", "andesite", "water"
            })]
        /* [InlineData(2, 17429, -7, -12, new[] {33, 18684, 18684, 3955, 4, 1, 1, 1},
            new[] { "bedrock", "deepslate", "deepslate", "deepslate_redstone_ore",
                "diorite", "stone", "stone", "stone" })] 
        [InlineData(3, 18699, -6, -5, new[] {33, 18684, 17714, 18684, 18684, 2, 1, 1},
            new[] { "bedrock", "deepslate", "tuff", "deepslate", 
                "deepslate", "granite", "stone", "stone" })] */
        public void ShouldReadChunk(int idx, int size, int x, int z, int[] array, string[] n)
        {
            var input = idx switch {1 => MapChunkData1, 2 => MapChunkData2, _ => MapChunkData3};
            Assert.Equal(size, input.Length);

            using var cache = new ChunkCache {ChunkHeightY = 384, ChunkMinY = -64};
            var chunkSize = cache.ChunkHeightY;

            var sections = Chunks.ReadAll(input, chunkSize).ToArray();
            File.WriteAllText($"{nameof(ShouldReadChunk)}_s_{idx}.txt", ToDebugStr(sections));
            Assert.Equal(24, sections.Length);

            foreach (var section in sections)
            {
                Assert.Equal(64, section.BiomeData.PaletteType.StorageSize);
            }
            var javaChunks = sections.Select(c => c.ChunkData).ToArray();
            cache.AddToCache(x, z, javaChunks);

            const int offset = 4;
            var states = new int[array.Length];
            var blocks = new string[array.Length];
            for (var i = -offset; i < array.Length - offset; i++)
            {
                var block = cache.GetBlockAt(x * 16, i * 16, z * 16, out _);
                states[i + offset] = block;
                blocks[i + offset] = Finder.FindBlockByState(block).Name;
            }

            Assert.Equal(AsList(array), AsList(states));
            Assert.Equal(AsList(n), AsList(blocks));
        }

        [Theory]
        [InlineData(1, 17417, -9, -13)]
        // [InlineData(2, 17429)]
        // [InlineData(3, 18699)]
        public void ShouldWriteChunk(int idx, int size, int x, int z)
        {
            var expected = idx switch {1 => MapChunkData1, 2 => MapChunkData2, _ => MapChunkData3};
            Assert.Equal(size, expected.Length);

            var cache = new ChunkCache {ChunkHeightY = 384, ChunkMinY = -64};

            var sections = cache.Allocate(x, z, cache.ChunkHeightY, () => Bulk.CreateBiome());

            sections[0].AddToPalette(Bedrock, Deepslate, DeepslateRedstoneOre, Tuff,
                Gravel, Lava, DeepslateDiamondOre, DeepslateGoldOre, Air, DeepslateIronOre);
            sections[1].AddToPalette(Deepslate, DeepslateIronOre, Air, Gravel, DeepslateDiamondOre,
                DeepslateRedstoneOre, DeepslateLapisOre);
            sections[2].AddToPalette(Deepslate, Lava + 8, Lava + 2, Lava + 4, Lava + 6, Air, Lava, DeepslateGoldOre,
                Tuff, DeepslateRedstoneOre);
            sections[3].AddToPalette(Air, Tuff, Deepslate, DeepslateIronOre, Gravel, DeepslateCopperOre, Dirt);
            sections[4].AddToMapPalette(5, Deepslate, Dirt, DeepslateIronOre, DeepslateGoldOre, Air, GlowLichen - 1,
                Stone, GoldOre, IronOre, Diorite, CopperOre, DeepslateCopperOre, LapisOre, Granite, DeepslateLapisOre,
                CoalOre,
                Gravel, Water);
            sections[5].AddToPalette(Stone, CoalOre, Gravel, CopperOre, Dirt, Andesite, Water, Air, Granite, IronOre,
                LapisOre, Diorite);
            sections[6].AddToPalette(Andesite, Stone, IronOre, CopperOre, Diorite, Granite, Dirt, LapisOre, Water,
                CoalOre, Gravel, Seagrass);
            sections[7].AddToPalette(Water, Gravel, Stone, Granite, Dirt, Seagrass, Sand, Clay, TallSeagrass,
                TallSeagrass - 1, Air);

            sections[0].BlockCount = 4058;
            sections[1].BlockCount = 3975;
            sections[2].BlockCount = 3523;
            sections[3].BlockCount = 4082;
            sections[4].BlockCount = 4060;
            sections[5].BlockCount = 4097;
            sections[6].BlockCount = 4418;
            sections[7].BlockCount = 6591;

            cache.SetBlocks(x * 16, z * 16,
                (-4 * 16, Bedrock),
                (-3 * 16, Deepslate),
                (-2 * 16, Deepslate),
                (-1 * 16, Deepslate),
                (0 * 16, Stone),
                (1 * 16, Andesite),
                (2 * 16, Water)
            );

            sections.CompactAirPalette();

            File.WriteAllText($"{nameof(ShouldWriteChunk)}_s_{idx}.txt", ToDebugStr(sections));
            Assert.Equal(24, sections.Length);

            using var stream = new MemoryStream();
            using var output = new MinecraftStream(stream);
            foreach (var section in sections)
                ChunkSection.Write(output, section);

            var actual = stream.ToArray();
            WriteBytes($"{nameof(ShouldWriteChunk)}{idx}", expected, actual);
            Assert.Equal(ToJson(expected), ToJson(actual));
        }

        private static IList<ChunkSection> Setup()
        {
            var chunkSectionsToTest = new List<ChunkSection> {new()};
            var section = new ChunkSection
            {
                [(0, 0, 0)] = 10
            };
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