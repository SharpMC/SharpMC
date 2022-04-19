using System;
using SharpMC.Chunky.Palette;
using SharpMC.Network.Chunky.Palette;
using SharpMC.Network.Util;

namespace SharpMC.Chunky
{
    public class DataPalette : IEquatable<DataPalette>
    {
        public const int GlobalPaletteBitsPerEntry = 14;
        public const int GlobalBiomeBitsPerEntry = 4;

        public IPalette Palette { get; set; }
        public BitStorage Storage { get; set; }
        public PaletteType PaletteType { get; }
        public int GlobalPaletteBits { get; }

        public DataPalette(IPalette palette, BitStorage storage,
            PaletteType paletteType, int globalPaletteBits)
        {
            Palette = palette ?? throw new ArgumentException("palette is marked non-null but is null");
            Storage = storage;
            PaletteType = paletteType;
            GlobalPaletteBits = globalPaletteBits;
        }

        public static DataPalette CreateForChunk(int globalPaletteBits =
            GlobalPaletteBitsPerEntry)
        {
            return CreateEmpty(PaletteType.Chunk, globalPaletteBits);
        }

        public static DataPalette CreateForBiome(int globalPaletteBits =
            GlobalBiomeBitsPerEntry)
        {
            return CreateEmpty(PaletteType.Biome, globalPaletteBits);
        }

        public static DataPalette CreateEmpty(PaletteType paletteType, int globalPaletteBits)
        {
            return new DataPalette(new ListPalette(paletteType.MinBitsPerEntry),
                new BitStorage(paletteType.MinBitsPerEntry, paletteType.StorageSize),
                paletteType, globalPaletteBits);
        }

        public static DataPalette Read(IMinecraftReader input, PaletteType paletteType,
            int globalPaletteBits)
        {
            BitStorage storage;
            int bitsPerEntry = input.ReadByte();
            var palette = ReadPalette(paletteType, bitsPerEntry, input);
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
                output.WriteByte(0);
                output.WriteVarInt(palette.Palette.IdToState(0));
                output.WriteVarInt(0);
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

        public int this[(int x, int y, int z) index]
        {
            get
            {
                if (Storage != null)
                {
                    var id = Storage[GetIndex(index)];
                    return Palette.IdToState(id);
                }
                return Palette.IdToState(0);
            }
            set
            {
                var id = Palette.StateToId(value);
                if (id == -1)
                {
                    Resize();
                    id = Palette.StateToId(value);
                }
                if (Storage != null)
                {
                    var storeIndex = GetIndex(index);
                    Storage[storeIndex] = id;
                }
            }
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
                    Storage[i] = Palette.StateToId(oldPalette.IdToState(oldData[i]));
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

        private static int GetIndex((int x, int y, int z) t)
            => GetIndex(t.x, t.y, t.z);

        private static int GetIndex(int x, int y, int z)
            => y << 8 | z << 4 | x;

        public override string ToString()
        {
            return $"DataPalette(palette={Palette}, storage={Storage}, " +
                   $"paletteType={PaletteType}, globalPaletteBits={GlobalPaletteBits})";
        }

        #region Hashcode

        public bool Equals(DataPalette other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Palette, other.Palette) && Equals(Storage, other.Storage) &&
                   PaletteType.Equals(other.PaletteType) &&
                   GlobalPaletteBits == other.GlobalPaletteBits;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DataPalette) obj);
        }

        public override int GetHashCode() 
            => HashCode.Combine(Palette, Storage, PaletteType, GlobalPaletteBits);

        public static bool operator ==(DataPalette left, DataPalette right) 
            => Equals(left, right);

        public static bool operator !=(DataPalette left, DataPalette right) 
            => !Equals(left, right);

        #endregion
    }
}