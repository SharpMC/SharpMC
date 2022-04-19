using System.Linq;
using SharpMC.Data;
using SharpMC.Network.Binary;
using SharpMC.Network.Binary.Model;
using SharpMC.Network.Binary.Special;
using SharpMC.Network.Packets.Play.ToBoth;
using SharpMC.Network.Packets.Play.ToClient;
using SharpMC.Network.Packets.Play.ToServer;
using SharpNBT;
using Xunit;
using static SharpMC.Network.Test.DataBunch;
using static SharpMC.Network.Test.DataBunch2;
using static SharpMC.Network.Test.DataBunch3;
using static SharpMC.Network.Test.DataBunch4;
using static SharpMC.Network.Test.TestHelper;
using static SharpMC.Util.Numbers;

namespace SharpMC.Network.Test
{
    public class BinaryTests
    {
        [Theory]
        [InlineData(4, new byte[] {0x28, 0x00, 0x29, 0x00})]
        [InlineData(7, new byte[] {0x28, 0x00, 0x29, 0x01, 0x0D, 0x01, 0x00})]
        [InlineData(19, new byte[]
        {
            0x28, 0x00, 0x29, 0x01, 0x0D, 0x01, 0x0a, 0x00, 0x00, 0x09,
            0x00, 0x01, 0x45, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00
        })]
        public void ShouldReadCreativeSlot(int count, byte[] input)
        {
            Assert.Equal(count, input.Length);

            var packet = Read<SetCreativeSlot>(input, out var packetId);
            Assert.Equal(0x28, packetId);

            Assert.Equal(0x28, packet.ServerId);
            Assert.Equal(41, packet.Slot);
            if (count == 4)
            {
                Assert.False(packet.Item.Present);
                return;
            }
            Assert.True(packet.Item.Present);
            Assert.Equal(13, packet.Item.ItemId);
            Assert.Equal(1, packet.Item.ItemCount.GetValueOrDefault());
            if (count == 7)
            {
                Assert.Null(packet.Item.Optional);
                return;
            }
            var meta = (CompoundTag) packet.Item.Optional;
            Assert.Single(meta);
            var opt = (ListTag) meta[0];
            Assert.Equal("E", opt.Name);
            Assert.Equal(TagType.Short, opt.ChildType);
            Assert.Empty(opt);
        }

        [Fact]
        public void ShouldReadCustomPayload()
        {
            var input = PlayCustomPayload;
            Assert.Equal(25, input.Length);

            var packet = Read<CustomPayload>(input, out var packetId);
            Assert.Equal(0x0a, packetId);

            Assert.Equal(0x0a, packet.ServerId);
            Assert.Equal("minecraft:brand", packet.Channel);
            Assert.Equal(new byte[] {7, 118, 97, 110, 105, 108, 108, 97}, packet.Data);
        }

        [Theory]
        [InlineData(1, 32457)]
        [InlineData(2, 26319)]
        [InlineData(3, 27594)]
        public void ShouldReadMapChunk(int idx, int size)
        {
            var input = idx switch {1 => PlayMapChunk1, 2 => PlayMapChunk2, _ => PlayMapChunk3};
            Assert.Equal(size, input.Length);

            var packet = Read<MapChunk>(input, out var packetId);
            WriteTexts($"{nameof(ShouldReadMapChunk)}{idx}", null, ToJson(packet));
            Assert.Equal(0x22, packetId);

            Assert.Equal(0x22, packet.ClientId);
            var entity = packet.BlockEntities?.SingleOrDefault();
            if (entity == null)
                return;
            Assert.Equal(49, entity.Y);
            Assert.Equal(1, entity.Type);
            Assert.Null(entity.Optional);
            Assert.Equal(4, entity.Coordinates.BlockX);
            Assert.Equal(8, entity.Coordinates.BlockZ);
        }

        [Fact]
        public void ShouldReadPlayLogin()
        {
            var input = PlayLogin;
            Assert.Equal(23992, input.Length);

            var packet = Read<Login>(input, out var packetId);
            WriteTexts(nameof(ShouldReadPlayLogin), null, ToJson(packet));
            Assert.Equal(0x26, packetId);

            Assert.Equal(0x26, packet.ClientId);
            Assert.Equal(167, packet.EntityId);
            Assert.False(packet.IsHardcore);
            Assert.Equal(0, packet.GameMode);
            Assert.Equal(-1, packet.PreviousGameMode);
            Assert.Equal(new[]
            {
                "minecraft:overworld", "minecraft:the_nether", "minecraft:the_end"
            }, packet.WorldNames);
            Assert.Equal("minecraft:overworld", packet.WorldName);
            Assert.Equal(-2837111331551244922, packet.HashedSeed);
            Assert.Equal(new[] {-660566458, -1901654650}, packet.HashedSeeds);
            Assert.Equal(0, packet.Dimension.PiglinSafe);
            Assert.Equal(384, packet.Dimension.Height);
            Assert.Equal(0, packet.Dimension.UltraWarm);
            Assert.Equal(0, packet.Dimension.HasCeiling);
            Assert.Equal(-64, packet.Dimension.MinY);
            Assert.Equal(1, packet.Dimension.CoordinateScale);
            Assert.Equal(384, packet.Dimension.LogicalHeight);
            Assert.Equal(1, packet.Dimension.HasRaids);
            Assert.Equal("minecraft:overworld", packet.Dimension.Effects);
            Assert.Equal(1, packet.Dimension.BedWorks);
            Assert.Equal(1, packet.Dimension.HasSkylight);
            Assert.Equal(0, packet.Dimension.RespawnWorks);
            Assert.Equal("#minecraft:infiniburn_overworld", packet.Dimension.InfiniBurn);
            Assert.Equal(0, packet.Dimension.AmbientLight);
            Assert.Equal(1, packet.Dimension.Natural);
            Assert.Equal(20, packet.MaxPlayers);
            Assert.Equal(10, packet.ViewDistance);
            Assert.Equal(10, packet.SimulationDistance);
            Assert.False(packet.ReducedDebugInfo);
            Assert.True(packet.EnableRespawnScreen);
            Assert.False(packet.IsDebug);
            Assert.False(packet.IsFlat);
            Assert.Equal(-1, packet.PacketId);
        }

        [Theory]
        [InlineData(4, new byte[] {0x28, 0x00, 0x29, 0x00})]
        [InlineData(7, new byte[] {0x28, 0x00, 0x29, 0x01, 0x0D, 0x01, 0x00})]
        [InlineData(19, new byte[]
        {
            0x28, 0x00, 0x29, 0x01, 0x0D, 0x01, 0x0a, 0x00, 0x00, 0x09,
            0x00, 0x01, 0x45, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00
        })]
        public void ShouldWriteCreativeSlot(int count, byte[] expected)
        {
            Assert.Equal(count, expected.Length);

            var packet = new SetCreativeSlot
            {
                Slot = 41,
                Item = count switch
                {
                    4 => new SlotData(),
                    7 => new SlotData {Present = true, ItemId = 13, ItemCount = 1},
                    _ => new SlotData
                    {
                        Present = true, ItemId = 13, ItemCount = 1, Optional = new CompoundTag(null)
                        {
                            new ListTag("E", TagType.Short)
                        }
                    }
                }
            };

            var actual = Write(packet, 0x28);
            WriteBytes($"{nameof(ShouldWriteCreativeSlot)}{count}", expected, actual);
            Assert.Equal(ToJson(expected), ToJson(actual));
        }

        [Fact]
        public void ShouldWriteCustomPayload()
        {
            var expected = PlayCustomPayload;
            Assert.Equal(25, expected.Length);

            var packet = new CustomPayload
            {
                Channel = "minecraft:brand",
                Data = new byte[] {7, 118, 97, 110, 105, 108, 108, 97}
            };

            var actual = Write(packet, 0x0a);
            WriteBytes(nameof(ShouldWriteCustomPayload), expected, actual);
            Assert.Equal(ToJson(expected), ToJson(actual));
        }

        [Theory]
        [InlineData(1, 32457)]
        [InlineData(2, 26319)]
        [InlineData(3, 27594)]
        public void ShouldWriteMapChunk(int idx, int size)
        {
            var expected = idx switch {1 => PlayMapChunk1, 2 => PlayMapChunk2, _ => PlayMapChunk3};
            Assert.Equal(size, expected.Length);

            var packet = idx switch
            {
                1 => new MapChunk
                {
                    X = -9, Z = -13, TrustEdges = true, SkyLightMask = new long[] {768},
                    BlockLightMask = new long[] {62}, EmptySkyLightMask = new long[] {255},
                    EmptyBlockLightMask = new long[] {961}, ChunkData = MapChunkData1,
                    HeightMaps = new HeightMaps
                    {
                        MotionBlocking = GetLongs(36, 2292305770412047999, 17079008895),
                        WorldSurface = GetLongs(36, 2292305770412047999, 17079008895)
                    },
                    SkyLight = new[] { Chunk1Sky1, GetBytes(2048, 255) },
                    BlockLight = new[] { Chunk1Block1, Chunk1Block2, Chunk1Block3, Chunk1Block4, Chunk1Block5 }
                },
                2 => new MapChunk
                {
                    X = -7, Z = -12, TrustEdges = true, SkyLightMask = new long[] {768},
                    BlockLightMask = new long[] {12}, EmptySkyLightMask = new long[] {255},
                    EmptyBlockLightMask = new long[] {1011}, ChunkData = MapChunkData2,
                    HeightMaps = new HeightMaps
                    {
                        MotionBlocking = GetLongs(36, 2292305770412047999, 17079008895),
                        WorldSurface = GetLongs(36, 2292305770412047999, 17079008895)
                    },
                    SkyLight = new[] { Chunk2Sky1, GetBytes(2048, 255) },
                    BlockLight = new[] { Chunk2Block1, Chunk2Block2 }
                },
                _ => new MapChunk
                {
                    X = -6, Z = -5, TrustEdges = true, SkyLightMask = new long[] {768},
                    BlockLightMask = new long[] {48}, EmptySkyLightMask = new long[] {255},
                    EmptyBlockLightMask = new long[] {975}, ChunkData = MapChunkData3,
                    HeightMaps = new HeightMaps
                    {
                        MotionBlocking = GetLongs(36, 2292305770412047999, 17079008895),
                        WorldSurface = GetLongs(36, 2292305770412047999, 17079008895)
                    },
                    BlockEntities = new ChunkBlockEntity[]
                    {
                        new() {Coordinates = new PackedCoordinates {BlockX = 4, BlockZ = 8}, Y = 49, Type = 1}
                    },
                    SkyLight = new[] {Chunk3Sky1, GetBytes(2048, 255)},
                    BlockLight = new[] {Chunk3Block1, Chunk3Block2}
                }
            };

            var actual = Write(packet, 0x22);
            WriteBytes($"{nameof(ShouldWriteMapChunk)}{idx}", expected, actual);
            Assert.Equal(ToJson(expected), ToJson(actual));
        }

        [Fact]
        public void ShouldWritePlayLogin()
        {
            var expected = PlayLogin;
            Assert.Equal(23992, expected.Length);

            var packet = new Login
            {
                EntityId = 167,
                IsHardcore = false,
                GameMode = 0,
                PreviousGameMode = -1,
                WorldNames = Defaults.WorldNames,
                WorldName = Defaults.WorldName,
                HashedSeeds = new[] {-660566458, -1901654650},
                MaxPlayers = 20,
                ViewDistance = 10,
                SimulationDistance = 10,
                ReducedDebugInfo = false,
                EnableRespawnScreen = true,
                IsDebug = false,
                IsFlat = false,
                DimensionCodec = new LoginDimCodec
                {
                    Realms = Defaults.Realms, Biomes = Defaults.Biomes
                },
                Dimension = Defaults.CurrentDim
            };

            var actual = Write(packet, 0x26);
            WriteBytes(nameof(ShouldWritePlayLogin), expected, actual);
            Assert.Equal(ToJson(expected), ToJson(actual));
        }
    }
}