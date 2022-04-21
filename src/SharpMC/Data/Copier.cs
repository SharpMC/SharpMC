using System;
using SharpMC.Chunky;

namespace SharpMC.Data
{
    public static class Copier
    {
        public enum CopyMode
        {
            Direct = 0,
            Indexed
        }

        private const int Pixel = 16;

        public static void CopyBlocks(ChunkSection[] sources, ChunkSection[] targets,
            CopyMode mode)
        {
            for (var i = 0; i < sources.Length && i < targets.Length; i++)
            {
                var source = sources[i];
                var sourceSect = source.ChunkData?.Storage;
                var target = targets[i];
                var targetSect = target.ChunkData?.Storage;
                if (sourceSect == null || targetSect == null)
                    continue;
                switch (mode)
                {
                    case CopyMode.Direct:
                        CopyBlocksDirect(sourceSect, targetSect);
                        break;
                    case CopyMode.Indexed:
                        CopyBlocksIndexed(source, target);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }
        }

        private static void CopyBlocksIndexed(ChunkSection source, ChunkSection target)
        {
            for (var x = 0; x < Pixel; x++)
            for (var y = 0; y < Pixel; y++)
            for (var z = 0; z < Pixel; z++)
            {
                var idx = (x, y, z);
                var sourceBlock = Finder.FindBlockByState(source[idx]);
                target[idx] = sourceBlock.DefaultState;
            }
        }

        private static void CopyBlocksDirect(BitStorage source, BitStorage target)
        {
            for (var j = 0; j < Pixel * Pixel * Pixel; j++)
            {
                var sourceBlock = Finder.FindBlockByState(source[j]);
                target[j] = sourceBlock.DefaultState;
            }
        }

        public static void RecountBlocks(this ChunkSection[] sections)
        {
            foreach (var section in sections)
            {
                var storage = section.ChunkData.Storage;
                if (storage == null)
                    continue;
                var count = 0;
                for (var j = 0; j < Pixel * Pixel * Pixel; j++)
                {
                    var state = storage[j];
                    if (state == 0)
                        continue;
                    count++;
                }
                if (section.BlockCount == count)
                    continue;
                section.BlockCount = count;
            }
        }
    }
}