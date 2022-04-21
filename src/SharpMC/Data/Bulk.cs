using System;
using System.Collections.Generic;
using System.Linq;
using SharpMC.Blocks;
using SharpMC.Chunky;
using SharpMC.Chunky.Palette;
using ChunkSection = SharpMC.Chunky.ChunkSection;

namespace SharpMC.Data
{
    public static class Bulk
    {
        public static void SetBlocks(this ChunkSection section, int x, int z, params (int y, Block block)[] pairs)
        {
            foreach (var (y, block) in pairs)
            {
                var state = block.DefaultState;
                section[(x, y, z)] = state;
            }
        }

        private static IEnumerable<ChunkSection> Allocate(int sections, Func<DataPalette> biome = null)
        {
            for (var i = 0; i < sections; i++)
            {
                var section = new ChunkSection();
                if (biome != null)
                    section = new ChunkSection(section.BlockCount, section.ChunkData, biome());
                yield return section;
            }
        }

        public static void AddToMapPalette(this ChunkSection section, int bitsPerEntry, params Block[] blocks)
            => AddToMyPalette(section, forceMap: true, bitsPerEntry, blocks);

        public static void AddToPalette(this ChunkSection section, params Block[] blocks)
            => AddToMyPalette(section, forceMap: false, null, blocks);

        private static void AddToMyPalette(this ChunkSection section, bool forceMap,
            int? bitsPerEntry = null, params Block[] blocks)
        {
            var data = section.ChunkData;
            if (forceMap && data.Palette?.BitsPerEntry != bitsPerEntry)
            {
                var bits = bitsPerEntry ?? data.Palette.BitsPerEntry;
                data.Palette = new MapPalette(bits);
                data.Storage = new BitStorage(bits, data.Storage.Size);
            }
            foreach (var block in blocks)
            {
                var state = block.DefaultState;
                data.Palette!.StateToId(state);
            }
        }

        public static Block[] ReadPalette(this ChunkSection section, out int[] keys)
        {
            keys = null;
            var palette = section.ChunkData.Palette;
            if (palette == null)
            {
                return Array.Empty<Block>();
            }
            if (palette is ListPalette l)
            {
                var data = l.Data;
                return data.Select(Finder.FindBlockByState).ToArray();
            }
            if (palette is MapPalette m)
            {
                var data = m.StatesToId.Keys;
                keys = data.Select(w => m.StateToId(w)).ToArray();
                return data.Select(Finder.FindBlockByState).ToArray();
            }
            if (palette is SingletonPalette s)
            {
                var data = new[] {s.State};
                return data.Select(Finder.FindBlockByState).ToArray();
            }
            throw new InvalidOperationException(palette.GetType().FullName);
        }

        public static void CompactAirPalette(this ChunkSection[] sections)
        {
            foreach (var section in sections)
            {
                var blocks = ReadPalette(section, out _);
                if (blocks.Any(block => block.DefaultState != 0))
                    continue;
                section.ChunkData.Palette = new SingletonPalette(0);
                section.ChunkData.Storage = null;
            }
        }

        public static DataPalette CreateBiome(int state = 44, int bits = 4)
        {
            var singleton = new SingletonPalette(state);
            return new DataPalette(singleton, null, PaletteType.Biome, bits);
        }

        public static void SetSingleton(this DataPalette data, int state)
        {
            data.Storage = null;
            data.Palette = new SingletonPalette(state);
        }

        public static ChunkSection NewSection(int? singleBiome = null)
        {
            var section = new ChunkSection();
            if (singleBiome != null)
                SetSingleton(section.BiomeData, singleBiome.Value);
            return section;
        }

        public static ChunkSection[] NewSections(int count, int? singleBiome = null)
        {
            var sections = new ChunkSection[count];
            for (var i = 0; i < sections.Length; i++)
                sections[i] = NewSection(singleBiome);
            return sections;
        }
    }
}