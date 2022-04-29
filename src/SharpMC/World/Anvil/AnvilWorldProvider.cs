using SharpMC.Players;
using SharpMC.Util;
using SharpMC.World.Generators;
using SharpNBT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using SharpMC.Plugins;
using SharpMC.World.Common;
using SharpMC.World.Standard;
using static SharpMC.World.Anvil.AnvilTool;

namespace SharpMC.World.Anvil
{
    internal class AnvilWorldProvider : WorldProviderBase
    {
        private readonly IWorldGenerator _backEndGenerator;
        private readonly Dictionary<Tuple<int, int>, ChunkColumn> _chunkCache;

        private string _basePath;
        private LevelInfo _level;
        private byte _waterOffsetY;

        public AnvilWorldProvider()
        {
            _backEndGenerator = (IWorldGenerator) new StandardWorldProvider(default, default);
            _chunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();
        }

        public AnvilWorldProvider(string basePath) : this()
        {
            _basePath = basePath;
            Init();
        }

        public void Init()
        {
            _basePath = _basePath ?? Config.Custom.GetProperty("PCWorldFolder", "World").Trim();

            if (File.Exists(Path.Combine(_basePath, "level.dat")))
            {
                var path = Path.Combine(_basePath, "level.dat");
                var file = NbtFile.Read(path, FormatOptions.Java);
                var dataTag = file["Data"];
                _level = new LevelInfo((TagContainer) dataTag);
            }
            else
            {
                throw new Exception(@"Could not load Anvil world!");
            }

            _waterOffsetY = (byte) Config.Custom.GetProperty("PCWaterOffset", 0);
        }

        public override void PopulateChunk(IChunkColumn chunk, ChunkCoordinates pos)
        {
            throw new System.NotImplementedException();
        }

        public override PlayerLocation SpawnPoint
        {
            get
            {
                var spawnPoint = new Vector3(_level.SpawnX, _level.SpawnY, _level.SpawnZ);
                spawnPoint.Y += 2; // Compensate for point being at head
                spawnPoint.Y += _waterOffsetY; // Compensate for offset
                if (spawnPoint.Y > 256) spawnPoint.Y = 256;
                return new PlayerLocation(spawnPoint);
            }
        }

        private static CompoundTag CreateNbtFromChunkColumn(IAnvilColumn chunk, ChunkCoordinates pos, int yoffset)
        {
            var levelTag = new CompoundTag("Level");

            levelTag.Add(new IntTag("xPos", pos.X));
            levelTag.Add(new IntTag("zPos", pos.Z));
            levelTag.Add(new ByteArrayTag("Biomes", chunk.BiomeId));

            var sectionsTag = new ListTag("Sections", TagType.Compound);
            levelTag.Add(sectionsTag);

            for (var i = 0; i < 8; i++)
            {
                var sectionTag = new CompoundTag(null);
                sectionsTag.Add(sectionTag);
                sectionTag.Add(new ByteTag("Y", (byte) i));
                var sy = i * 16;

                var blocks = new byte[4096];
                var data = new byte[2048];
                var blockLight = new byte[2048];
                var skyLight = new byte[2048];

                for (var x = 0; x < 16; x++)
                {
                    for (var z = 0; z < 16; z++)
                    {
                        for (var y = 0; y < 16; y++)
                        {
                            var yi = sy + y;
                            if (yi < 0 || yi >= 256) 
                                continue;

                            var anvilIndex = (y + yoffset) * 16 * 16 + z * 16 + x;
                            var blockId = chunk.GetBlock(x, yi, z);

                            blocks[anvilIndex] = (byte) blockId.DefaultState;
                            SetNibble4(data, anvilIndex, chunk.GetMetadata(x, yi, z));
                            SetNibble4(blockLight, anvilIndex, chunk.GetBlocklight(x, yi, z));
                            SetNibble4(skyLight, anvilIndex, chunk.GetSkylight(x, yi, z));
                        }
                    }
                }

                sectionTag.Add(new ByteArrayTag("Blocks", blocks));
                sectionTag.Add(new ByteArrayTag("Data", data));
                sectionTag.Add(new ByteArrayTag("BlockLight", blockLight));
                sectionTag.Add(new ByteArrayTag("SkyLight", skyLight));
            }

            levelTag.Add(new ListTag("Entities", TagType.Compound));
            levelTag.Add(new ListTag("TileEntities", TagType.Compound));
            levelTag.Add(new ListTag("TileTicks", TagType.Compound));

            return levelTag;
        }

        protected override IChunkColumn GenerateChunkColumn(Vector2 vector)
        {
            throw new NotImplementedException();
        }
    }
}