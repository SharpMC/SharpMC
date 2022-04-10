using System.IO;
using SharpMC.Network.Util;
using SharpMC.Util;

namespace SharpMC.World
{
    public class ChunkColumn
    {
        public const int Height = 256;
        public const int Width = 16;
        public const int Depth = 16;

        public ChunkCoordinates Coordinates { get; }
        public bool IsDirty { get; private set; } = false;
        public byte[] HeightMap = new byte[Width * Depth];

        private ChunkSection[] Sections { get; }
        private byte[] Biomes { get; }
        private byte[] Cache { get; set; } = null;

        public ChunkColumn(ChunkCoordinates coordinates)
        {
            Coordinates = coordinates;
            Sections = new ChunkSection[16];
            Biomes = new byte[Depth * Width];

            for (var i = 0; i < Sections.Length; i++)
            {
                Sections[i] = new ChunkSection();
            }

            for (var i = 0; i < Biomes.Length; i++)
            {
                Biomes[i] = 1; //Plains
            }

            for (var i = 0; i < HeightMap.Length; i++)
            {
                HeightMap[i] = 0;
            }
        }

        private ChunkSection GetChunkSection(int y)
        {
            return Sections[y >> 4];
        }

        public short GetBlockId(int x, int y, int z)
        {
            return GetChunkSection(y).GetBlockId(x, y - 16 * (y >> 4), z);
        }

        public void SetBlockId(int x, int y, int z, short id)
        {
            GetChunkSection(y).SetBlockId(x, y - 16 * (y >> 4), z, id);

            Cache = null;
            IsDirty = true;
        }

        public byte GetBlockData(int x, int y, int z)
        {
            return GetChunkSection(y).GetBlockData(x, y - 16 * (y >> 4), z);
        }

        public void SetBlockData(int x, int y, int z, byte meta)
        {
            GetChunkSection(y).SetBlockData(x, y - 16 * (y >> 4), z, meta);

            Cache = null;
            IsDirty = true;
        }

        public void SetBiome(int x, int z, byte biome)
        {
            Biomes[(z << 4) + (x)] = biome;

            Cache = null;
            IsDirty = true;
        }

        public byte GetBiome(int x, int z)
        {
            return Biomes[(z << 4) + (x)];
        }

        public void RecalcHeight()
        {
            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    for (byte y = 127; y > 0; y--)
                    {
                        if (GetBlockId(x, y, z) != 0)
                        {
                            HeightMap[(x << 4) + z] = (byte) (y + 1);
                            //..SetHeight(x, z, (byte)(y + 1));
                            break;
                        }
                    }
                }
            }
        }

        public byte[] ToArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var m = new MinecraftStream(ms))
                {
                    WriteTo(m);
                }
                return ms.ToArray();
            }
        }

        public void WriteTo(MinecraftStream stream)
        {
            if (Cache != null)
            {
                stream.Write(Cache);
                return;
            }

            byte[] sectionData;
            var sectionBitmask = 0;
            using (var ms = new MemoryStream())
            {
                using (var mc = new MinecraftStream(ms))
                {
                    for (var i = 0; i < Sections.Length; i++)
                    {
                        var section = Sections[i];
                        if (section.IsAllAir) continue;

                        sectionBitmask |= 1 << i;

                        section.WriteTo(mc, true);
                    }
                }
                sectionData = ms.ToArray();
            }

            using (var ms = new MemoryStream())
            {
                using (var mc = new MinecraftStream(ms))
                {
                    mc.WriteInt(Coordinates.X);
                    mc.WriteInt(Coordinates.Z);

                    mc.WriteBool(true);

                    mc.WriteVarInt(sectionBitmask); //Primary Bitmask

                    mc.WriteVarInt(sectionData.Length + 256);
                    mc.Write(sectionData, 0, sectionData.Length);
                    mc.Write(Biomes);

                    mc.WriteVarInt(0); //No block entities for now
                }

                Cache = ms.ToArray();
            }

            stream.Write(Cache);
        }
    }
}