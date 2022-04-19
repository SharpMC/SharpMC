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
            var sections = Bulk.NewSections(count, 44);
            if (idx == 1)
            {
                sections[0].AddToPalette(Bedrock, Deepslate, DeepslateRedstoneOre, Tuff,
                    Gravel, Lava, DeepslateDiamondOre, DeepslateGoldOre, Air, DeepslateIronOre);
                sections[1].AddToPalette(Deepslate, DeepslateIronOre, Air, Gravel,
                    DeepslateDiamondOre, DeepslateRedstoneOre, DeepslateLapisOre);
                sections[2].AddToPalette(Deepslate, Lava + 8, Lava + 2, Lava + 4, Lava + 6,
                    Air, Lava, DeepslateGoldOre, Tuff, DeepslateRedstoneOre);
                sections[3].AddToPalette(Air, Tuff, Deepslate, DeepslateIronOre,
                    Gravel, DeepslateCopperOre, Dirt);
                sections[4].AddToMapPalette(5, Deepslate, Dirt, DeepslateIronOre,
                    DeepslateGoldOre, Air, GlowLichen - 1, Stone, GoldOre, IronOre, Diorite, CopperOre,
                    DeepslateCopperOre, LapisOre, Granite, DeepslateLapisOre, CoalOre, Gravel, Water);
                sections[5].AddToPalette(Stone, CoalOre, Gravel, CopperOre, Dirt, Andesite,
                    Water, Air, Granite, IronOre, LapisOre, Diorite);
                sections[6].AddToPalette(Andesite, Stone, IronOre, CopperOre, Diorite, Granite,
                    Dirt, LapisOre, Water, CoalOre, Gravel, Seagrass);
                sections[7].AddToPalette(Water, Gravel, Stone, Granite, Dirt, Seagrass, Sand,
                    Clay, TallSeagrass, TallSeagrass - 1, Air);
            }
            else if (idx == 2)
            {
                sections[0].AddToPalette(Bedrock, Deepslate, DeepslateDiamondOre,
                    Gravel, DeepslateRedstoneOre, Tuff);
                sections[1].AddToPalette(Deepslate, Tuff, DeepslateRedstoneOre,
                    Gravel, SmoothBasalt, Air, Calcite, DeepslateDiamondOre);
                sections[2].AddToPalette(Deepslate, Tuff, Air, Gravel,
                    DeepslateRedstoneOre, DeepslateDiamondOre, DeepslateIronOre);
                sections[3].AddToPalette(DeepslateRedstoneOre, Deepslate,
                    Tuff, Gravel, DeepslateIronOre, DeepslateGoldOre, DeepslateLapisOre,
                    Dirt, DeepslateCopperOre, Diorite, Granite);
                sections[4].AddToPalette(Diorite, Granite, Deepslate,
                    DeepslateIronOre, IronOre, Dirt, Andesite, Stone, CopperOre,
                    GoldOre, DeepslateGoldOre, DeepslateCopperOre, Gravel);
                sections[5].AddToPalette(Stone, Granite, IronOre, Dirt, Diorite,
                    Air, Water, LapisOre, GoldOre, CopperOre, Andesite, Gravel);
                sections[6].AddToPalette(Stone, Andesite, IronOre, Gravel,
                    CopperOre, Diorite, LapisOre, Dirt, Granite, CoalOre);
                sections[7].AddToMapPalette(5, Stone, Dirt, KelpPlant, CoalOre,
                    Granite, IronOre, Gravel, CopperOre, Sand, Water, TallSeagrass, TallSeagrass - 1,
                    Seagrass, Clay, Kelp + 25, Kelp + 23, Kelp + 21, Kelp + 24, Kelp + 22, Air);
            }
            else if (idx == 3)
            {
                sections[0].AddToPalette(Bedrock, Deepslate, DeepslateDiamondOre,
                    DeepslateIronOre, DeepslateRedstoneOre, DeepslateLapisOre, DeepslateGoldOre);
                sections[1].AddToPalette(Deepslate, DeepslateDiamondOre, Gravel,
                    Air, DeepslateGoldOre, DeepslateLapisOre, DeepslateRedstoneOre, Tuff);
                sections[2].AddToPalette(Tuff, Deepslate, DeepslateRedstoneOre,
                    DeepslateGoldOre, DeepslateDiamondOre);
                sections[3].AddToPalette(Deepslate, DeepslateGoldOre, DeepslateIronOre,
                    Air, Tuff, Water, MagmaBlock, GlowLichen - 4, Gravel, BubbleColumn);
                sections[4].AddToMapPalette(5, Deepslate, Water, Air, BubbleColumn,
                    Gravel, Andesite, Stone, DeepslateGoldOre, GlowLichen - 7, Diorite, GoldOre,
                    DeepslateCopperOre, CopperOre, Dirt, Granite, Water + 1, LapisOre, IronOre, CoalOre);
                sections[5].AddToPalette(Granite, Andesite, Stone, CoalOre,
                    IronOre, CopperOre, Diorite, Gravel, Water);
                sections[6].AddToPalette(Stone, Diorite, Water, IronOre, Gravel,
                    CoalOre, Dirt, CopperOre, Andesite, LapisOre, SpruceStairs + 10,
                    SprucePlanks, Seagrass, TallSeagrass, KelpPlant);
                sections[7].AddToMapPalette(6, Stone, Gravel, Water,
                    SpruceStairs + 10, KelpPlant, SprucePlanks, Seagrass, DarkOakStairs + 49,
                    DarkOakPlanks, DarkOakStairs + 29, TallSeagrass, TallSeagrass - 1, SpruceStairs + 9,
                    Chest + 5, DarkOakLog, DarkOakStairs + 39, DarkOakStairs + 59, DarkOakStairs + 9,
                    DarkOakTrapdoor + 7, DarkOakTrapdoor - 9, DarkOakTrapdoor + 23, SpruceStairs + 29,
                    SpruceStairs - 1, SpruceFence - 10, SpruceFence - 6, SpruceFence - 2,
                    DarkOakStairs + 19, Kelp + 25, DarkOakStairs - 1, SpruceFence - 22,
                    SpruceFence - 7, SpruceFence - 14, Kelp + 24, Kelp + 22, Air);
            }
            sections.CompactAirPalette();
            
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