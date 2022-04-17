using System;
using SharpMC.Chunky.Palette;
using SharpMC.Network.Chunky.Palette;
using SharpMC.Network.Util;

namespace SharpMC.Chunky
{
    public class DataPalette
    {
        public const int GlobalPaletteBitsPerEntry = 14;

        public IPalette Palette { get; set; }
        public BitStorage Storage { get; private set; }

        public PaletteType PaletteType { get; }
        public int GlobalPaletteBits { get; }

        public DataPalette(IPalette palette, BitStorage storage, 
            PaletteType paletteType, int globalPaletteBits)
        {
            Palette = palette;
            Storage = storage;
            PaletteType = paletteType;
            GlobalPaletteBits = globalPaletteBits;
        }

        public static DataPalette CreateForChunk(int globalPaletteBits
            = GlobalPaletteBitsPerEntry)
        {
            return CreateEmpty(PaletteType.Chunk, globalPaletteBits);
        }

        public static DataPalette CreateForBiome(int globalPaletteBits = 4)
        {
            return CreateEmpty(PaletteType.Biome, globalPaletteBits);
        }

        public static DataPalette CreateEmpty(PaletteType paletteType, int globalPaletteBits)
        {
            return new DataPalette(new ListPalette(paletteType.MinBitsPerEntry),
                new BitStorage(paletteType.MinBitsPerEntry,
                    paletteType.StorageSize), paletteType, globalPaletteBits);
        }

        public static DataPalette Read(IMinecraftReader input, PaletteType paletteType,
            int globalPaletteBits)
        {
            var bitsPerEntry = input.ReadByte();
            var palette = ReadPalette(paletteType, bitsPerEntry, input);
            BitStorage storage;
            if (!(palette is SingletonPalette))
            {
                storage = new BitStorage(bitsPerEntry, paletteType.StorageSize,
                    input.ReadLongArray());
            }
            else
            {
                input.ReadVarInt();
                storage = null;
            }
            return new DataPalette(palette, storage, paletteType, globalPaletteBits);
        }

        public static void Write(IMinecraftWriter output, DataPalette palette)
        {
            if (palette.Palette is SingletonPalette)
            {
                output.WriteByte(0); // Bits per entry
                output.WriteVarInt(palette.Palette.IdToState(0));
                output.WriteVarInt(0); // Data length
                return;
            }
            output.WriteByte((byte) palette.Storage.BitsPerEntry);
            if (!(palette.Palette is GlobalPalette))
            {
                var paletteLength = palette.Palette.Size;
                output.WriteVarInt(paletteLength);
                for (var i = 0; i < paletteLength; i++)
                {
                    output.WriteVarInt(palette.Palette.IdToState(i));
                }
            }
            var data = palette.Storage.Data;
            output.WriteLongArray(data);
        }

        public int Get(int x, int y, int z)
        {
            if (Storage != null)
            {
                var id = Storage.Get(Index(x, y, z));
                return Palette.IdToState(id);
            }
            return Palette.IdToState(0);
        }

        public int Set(int x, int y, int z, int state)
        {
            var id = Palette.StateToId(state);
            if (id == -1)
            {
                Resize();
                id = Palette.StateToId(state);
            }
            if (Storage != null)
            {
                var index = Index(x, y, z);
                var curr = Storage.Get(index);
                Storage.Set(index, id);
                return curr;
            }
            return state;
        }

        private static IPalette ReadPalette(PaletteType paletteType, int bitsPerEntry,
            IMinecraftReader input)
        {
            if (bitsPerEntry > paletteType.MaxBitsPerEntry)
            {
                return new GlobalPalette();
            }
            if (bitsPerEntry == 0)
            {
                return new SingletonPalette(input);
            }
            if (bitsPerEntry <= paletteType.MinBitsPerEntry)
            {
                return new ListPalette(bitsPerEntry, input);
            }
            return new MapPalette(bitsPerEntry, input);
        }

        private int SanitizeBitsPerEntry(int bitsPerEntry)
        {
            if (bitsPerEntry <= PaletteType.MaxBitsPerEntry)
            {
                return Math.Max(PaletteType.MinBitsPerEntry, bitsPerEntry);
            }
            return GlobalPaletteBitsPerEntry;
        }

        private void Resize()
        {
            var oldPalette = Palette;
            var oldData = Storage;
            var bitsPerEntry = SanitizeBitsPerEntry(oldPalette is SingletonPalette
                ? 1
                : oldData.BitsPerEntry + 1);
            Palette = CreatePalette(bitsPerEntry, PaletteType);
            Storage = new BitStorage(bitsPerEntry, PaletteType.StorageSize);
            if (oldPalette is SingletonPalette)
            {
                Palette.StateToId(oldPalette.IdToState(0));
            }
            else
            {
                for (var i = 0; i < PaletteType.StorageSize; i++)
                {
                    Storage.Set(i, Palette.StateToId(
                        oldPalette.IdToState(oldData.Get(i))));
                }
            }
        }

        private static IPalette CreatePalette(int bitsPerEntry, PaletteType paletteType)
        {
            if (bitsPerEntry <= paletteType.MinBitsPerEntry)
            {
                return new ListPalette(bitsPerEntry);
            }
            if (bitsPerEntry <= paletteType.MaxBitsPerEntry)
            {
                return new MapPalette(bitsPerEntry);
            }
            return new GlobalPalette();
        }

        private static int Index(int x, int y, int z) => y << 8 | z << 4 | x;
    }
}