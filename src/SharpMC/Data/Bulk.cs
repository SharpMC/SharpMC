using System;
using System.Collections.Generic;
using System.Linq;
using SharpMC.Blocks;
using SharpMC.Chunky;
using SharpMC.Chunky.Palette;
using SharpNBT;

namespace SharpMC.Data
{
    public static class Bulk
    {
        public static void SetBlocks(this ChunkSection section, int x, int z, params (int y, MiBlock block)[] pairs)
        {
            foreach (var (y, block) in pairs)
            {
                var state = block.DefaultState;
                section.SetBlock(x, y, z, state);
            }
        }

        public static void SetBlocks(this ChunkCache cache, int x, int z, params (int y, MiBlock block)[] pairs)
        {
            foreach (var (y, block) in pairs)
            {
                var state = block.DefaultState;
                if (cache.UpdateBlock(x, y, z, state))
                    continue;
                throw new InvalidOperationException($"Could not update {(x, y, z)} !");
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

        public static ChunkSection[] Allocate(this ChunkCache cache, int x, int z, int count,
            Func<DataPalette> biome = null)
        {
            var sections = Allocate(count, biome).ToArray();
            var data = sections.Select(s => s.ChunkData).ToArray();
            cache.AddToCache(x, z, data);
            return sections;
        }

        public static void AddToMapPalette(this ChunkSection section, int bitsPerEntry, params MiBlock[] blocks)
            => AddToMyPalette(section, forceMap: true, bitsPerEntry, blocks);

        public static void AddToPalette(this ChunkSection section, params MiBlock[] blocks)
            => AddToMyPalette(section, forceMap: false, null, blocks);

        private static void AddToMyPalette(this ChunkSection section, bool forceMap,
            int? bitsPerEntry = null, params MiBlock[] blocks)
        {
            var data = section.ChunkData;
            if (forceMap && data.Palette is ListPalette lp)
            {
                data.Palette = new MapPalette(bitsPerEntry ?? lp.BitsPerEntry);
            }
            foreach (var block in blocks)
            {
                var state = block.DefaultState;
                data.Palette.StateToId(state);
            }
        }

        public static MiBlock[] ReadPalette(this ChunkSection section, out int[] keys)
        {
            keys = null;
            var palette = section.ChunkData.Palette;
            if (palette == null)
            {
                return Array.Empty<MiBlock>();
            }
            if (palette is ListPalette l)
            {
                var data = l.Data;
                return data.Select(Finder.FindBlockByState).ToArray();
            }
            if (palette is MapPalette m)
            {
                var data = m.States;
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
                if (blocks.All(block => block.DefaultState == 0))
                    section.ChunkData.Palette = new SingletonPalette(0);
            }
        }

        public static DataPalette CreateBiome(int state = 44, int bits = 4)
        {
            var singleton = new SingletonPalette(state);
            return new DataPalette(singleton, null, PaletteType.Biome, bits);
        }
    }
}