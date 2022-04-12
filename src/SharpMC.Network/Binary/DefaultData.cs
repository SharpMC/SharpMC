using System.Linq;
using SharpMC.Network.Binary;
using SharpNBT;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public static class DefaultData
    {
        public static CompoundTag GenerateDimCodec()
        {
            const string ow = "minecraft:overworld";
            const string infi = "#minecraft:infiniburn_overworld";

            var tag = new CompoundTag(null)
            {
                new CompoundTag("minecraft:dimension_type")
                {
                    new StringTag("type", "minecraft:dimension_type"),
                    new ListTag("value", TagType.Compound)
                    {
                        new WorldCodec
                        {
                            Id = 0, Name = "minecraft:overworld", Effects = ow, Natural = 1,
                            Infiniburn = infi, Skylight = 1,
                            BedWorks = 1, Raids = 1, LogicalHeight = 384, Scale = 1,
                            MinY = -64, Height = 384
                        }.ToCompound(),
                        new WorldCodec
                        {
                            Id = 1, Name = "minecraft:overworld_caves", Effects = ow,
                            Natural = 1, Infiniburn = infi, Skylight = 1, BedWorks = 1,
                            Raids = 1, LogicalHeight = 384, Scale = 1, MinY = -64,
                            Height = 384, HasCeiling = 1
                        }.ToCompound(),
                        new WorldCodec
                        {
                            Id = 2, Name = "minecraft:the_nether", PiglinSafe = 1,
                            AmbientLight = 0.10000000149011612f, Infiniburn = "#minecraft:infiniburn_nether",
                            Respawn = 1, Effects = "minecraft:the_nether", FixedTime = 18000,
                            LogicalHeight = 128, Scale = 8, HasCeiling = 1, UltraWarm = 1, Height = 256
                        }.ToCompound(),
                        new WorldCodec
                        {
                            Id = 3, Name = "minecraft:the_end", Effects = "minecraft:the_end",
                            Infiniburn = "#minecraft:infiniburn_end", FixedTime = 6000,
                            Raids = 1, LogicalHeight = 256, Scale = 1, Height = 256
                        }.ToCompound()
                    }
                },
                new CompoundTag("minecraft:worldgen/biome")
                {
                    new StringTag("type", "minecraft:worldgen/biome"),
                    new ListTag("value", TagType.Compound, biomes.Select(b => b.ToCompound()))
                }
            };
            return tag;
        }

        private static WorldBiome[] biomes =
        {
            new WorldBiome
            {
                Id = 0, Name = "minecraft:the_void", Precipitation = "none", SkyColor = 8103167, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "none",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 1, Name = "minecraft:plains", Precipitation = "rain", SkyColor = 7907327, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.8f, Downfall = 0.4f, Category = "plains",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 2, Name = "minecraft:sunflower_plains", Precipitation = "rain", SkyColor = 7907327,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.8f, Downfall = 0.4f,
                Category = "plains", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 3, Name = "minecraft:snowy_plains", Precipitation = "snow", SkyColor = 8364543,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0f, Downfall = 0.5f,
                Category = "icy", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 4, Name = "minecraft:ice_spikes", Precipitation = "snow", SkyColor = 8364543,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0f, Downfall = 0.5f,
                Category = "icy", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 5, Name = "minecraft:desert", Precipitation = "none", SkyColor = 7254527, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f, Category = "desert",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 6, Name = "minecraft:swamp", Precipitation = "rain", SkyColor = 7907327, WaterFogColor = 2302743,
                FogColor = 12638463, WaterColor = 6388580, Temperature = 0.8f, Downfall = 0.9f, Category = "swamp",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8, GrassColorMod = "swamp",
                FoliageColor = 6975545
            },
            new WorldBiome
            {
                Id = 7, Name = "minecraft:forest", Precipitation = "rain", SkyColor = 7972607, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.7f, Downfall = 0.8f, Category = "forest",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 8, Name = "minecraft:flower_forest", Precipitation = "rain", SkyColor = 7972607,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.7f, Downfall = 0.8f,
                Category = "forest", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 9, Name = "minecraft:birch_forest", Precipitation = "rain", SkyColor = 8037887,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.6f, Downfall = 0.6f,
                Category = "forest", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 10, Name = "minecraft:dark_forest", Precipitation = "rain", SkyColor = 7972607,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.7f, Downfall = 0.8f,
                Category = "forest", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                GrassColorMod = "dark_forest"
            },
            new WorldBiome
            {
                Id = 11, Name = "minecraft:old_growth_birch_forest", Precipitation = "rain", SkyColor = 8037887,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.6f, Downfall = 0.6f,
                Category = "forest", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 12, Name = "minecraft:old_growth_pine_taiga", Precipitation = "rain", SkyColor = 8168447,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.3f, Downfall = 0.8f,
                Category = "taiga", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 13, Name = "minecraft:old_growth_spruce_taiga", Precipitation = "rain", SkyColor = 8233983,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.25f, Downfall = 0.8f,
                Category = "taiga", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 14, Name = "minecraft:taiga", Precipitation = "rain", SkyColor = 8233983, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.25f, Downfall = 0.8f, Category = "taiga",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 15, Name = "minecraft:snowy_taiga", Precipitation = "snow", SkyColor = 8625919,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4020182, Temperature = -0.5f, Downfall = 0.4f,
                Category = "taiga", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 16, Name = "minecraft:savanna", Precipitation = "none", SkyColor = 7254527, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f, Category = "savanna",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 17, Name = "minecraft:savanna_plateau", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "savanna", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 18, Name = "minecraft:windswept_hills", Precipitation = "rain", SkyColor = 8233727,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.2f, Downfall = 0.3f,
                Category = "extreme_hills", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 19, Name = "minecraft:windswept_gravelly_hills", Precipitation = "rain", SkyColor = 8233727,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.2f, Downfall = 0.3f,
                Category = "extreme_hills", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 20, Name = "minecraft:windswept_forest", Precipitation = "rain", SkyColor = 8233727,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.2f, Downfall = 0.3f,
                Category = "extreme_hills", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 21, Name = "minecraft:windswept_savanna", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "savanna", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 22, Name = "minecraft:jungle", Precipitation = "rain", SkyColor = 7842047, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.95f, Downfall = 0.9f, Category = "jungle",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 23, Name = "minecraft:sparse_jungle", Precipitation = "rain", SkyColor = 7842047,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.95f, Downfall = 0.8f,
                Category = "jungle", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 24, Name = "minecraft:bamboo_jungle", Precipitation = "rain", SkyColor = 7842047,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.95f, Downfall = 0.9f,
                Category = "jungle", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 25, Name = "minecraft:badlands", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "mesa", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                GrassColor = 9470285, FoliageColor = 10387789
            },
            new WorldBiome
            {
                Id = 26, Name = "minecraft:eroded_badlands", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "mesa", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                GrassColor = 9470285, FoliageColor = 10387789
            },
            new WorldBiome
            {
                Id = 27, Name = "minecraft:wooded_badlands", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "mesa", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                GrassColor = 9470285, FoliageColor = 10387789
            },
            new WorldBiome
            {
                Id = 28, Name = "minecraft:meadow", Precipitation = "rain", SkyColor = 8103167, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 937679, Temperature = 0.5f, Downfall = 0.8f, Category = "mountain",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8, MinDelay = 12000,
                ReplaceMusic = 0, MaxDelay = 24000, MusicSound = "minecraft:music.overworld.meadow"
            },
            new WorldBiome
            {
                Id = 29, Name = "minecraft:grove", Precipitation = "snow", SkyColor = 8495359, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = -0.2f, Downfall = 0.8f, Category = "forest",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8, MinDelay = 12000,
                ReplaceMusic = 0, MaxDelay = 24000, MusicSound = "minecraft:music.overworld.grove"
            },
            new WorldBiome
            {
                Id = 30, Name = "minecraft:snowy_slopes", Precipitation = "snow", SkyColor = 8560639,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = -0.3f, Downfall = 0.9f,
                Category = "mountain", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MusicSound = "minecraft:music.overworld.snowy_slopes",
                MinDelay = 12000
            },
            new WorldBiome
            {
                Id = 31, Name = "minecraft:frozen_peaks", Precipitation = "snow", SkyColor = 8756735,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = -0.7f, Downfall = 0.9f,
                Category = "mountain", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MusicSound = "minecraft:music.overworld.frozen_peaks",
                MinDelay = 12000
            },
            new WorldBiome
            {
                Id = 32, Name = "minecraft:jagged_peaks", Precipitation = "snow", SkyColor = 8756735,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = -0.7f, Downfall = 0.9f,
                Category = "mountain", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.overworld.jagged_peaks"
            },
            new WorldBiome
            {
                Id = 33, Name = "minecraft:stony_peaks", Precipitation = "rain", SkyColor = 7776511,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 1f, Downfall = 0.3f,
                Category = "mountain", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.overworld.stony_peaks"
            },
            new WorldBiome
            {
                Id = 34, Name = "minecraft:river", Precipitation = "rain", SkyColor = 8103167, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "river",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 35, Name = "minecraft:frozen_river", Precipitation = "snow", SkyColor = 8364543,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 3750089, Temperature = 0f, Downfall = 0.5f,
                Category = "river", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 36, Name = "minecraft:beach", Precipitation = "rain", SkyColor = 7907327, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.8f, Downfall = 0.4f, Category = "beach",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 37, Name = "minecraft:snowy_beach", Precipitation = "snow", SkyColor = 8364543,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4020182, Temperature = 0.05f, Downfall = 0.3f,
                Category = "beach", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 38, Name = "minecraft:stony_shore", Precipitation = "rain", SkyColor = 8233727,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.2f, Downfall = 0.3f,
                Category = "beach", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 39, Name = "minecraft:warm_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 270131, FogColor = 12638463, WaterColor = 4445678, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 40, Name = "minecraft:lukewarm_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 267827, FogColor = 12638463, WaterColor = 4566514, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 41, Name = "minecraft:deep_lukewarm_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 267827, FogColor = 12638463, WaterColor = 4566514, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 42, Name = "minecraft:ocean", Precipitation = "rain", SkyColor = 8103167, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "ocean",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 43, Name = "minecraft:deep_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 44, Name = "minecraft:cold_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4020182, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 45, Name = "minecraft:deep_cold_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4020182, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 46, Name = "minecraft:frozen_ocean", Precipitation = "snow", SkyColor = 8364543,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 3750089, Temperature = 0f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                TemperatureMod = "frozen"
            },
            new WorldBiome
            {
                Id = 47, Name = "minecraft:deep_frozen_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 3750089, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                TemperatureMod = "frozen"
            },
            new WorldBiome
            {
                Id = 48, Name = "minecraft:mushroom_fields", Precipitation = "rain", SkyColor = 7842047,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.9f, Downfall = 1f,
                Category = "mushroom", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 49, Name = "minecraft:dripstone_caves", Precipitation = "rain", SkyColor = 7907327,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.8f, Downfall = 0.4f,
                Category = "underground", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.overworld.dripstone_caves"
            },
            new WorldBiome
            {
                Id = 50, Name = "minecraft:lush_caves", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f,
                Category = "underground", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.overworld.lush_caves"
            },
            new WorldBiome
            {
                Id = 51, Name = "minecraft:nether_wastes", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 3344392, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "nether", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.nether_wastes.mood",
                Extent = 8, ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.nether.nether_wastes",
                AmbientSound = "minecraft:ambient.nether_wastes.loop",
                AddedSound = "minecraft:ambient.nether_wastes.additions", TickChance = 0.0111
            },
            new WorldBiome
            {
                Id = 52, Name = "minecraft:warped_forest", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 1705242, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "nether", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.warped_forest.mood",
                Extent = 8, ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.nether.warped_forest",
                AmbientSound = "minecraft:ambient.warped_forest.loop",
                AddedSound = "minecraft:ambient.warped_forest.additions", TickChance = 0.0111,
                ParticleType = "minecraft:warped_spore", Probability = 0.014279999770224094f
            },
            new WorldBiome
            {
                Id = 53, Name = "minecraft:crimson_forest", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 3343107, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "nether", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.crimson_forest.mood",
                Extent = 8, ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.nether.crimson_forest",
                AmbientSound = "minecraft:ambient.crimson_forest.loop",
                AddedSound = "minecraft:ambient.crimson_forest.additions", TickChance = 0.0111,
                Probability = 0.02500000037252903f, ParticleType = "minecraft:crimson_spore"
            },
            new WorldBiome
            {
                Id = 54, Name = "minecraft:soul_sand_valley", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 1787717, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "nether", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.soul_sand_valley.mood",
                Extent = 8, ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.nether.soul_sand_valley",
                AmbientSound = "minecraft:ambient.soul_sand_valley.loop",
                AddedSound = "minecraft:ambient.soul_sand_valley.additions", TickChance = 0.0111,
                Probability = 0.0062500000931322575f, ParticleType = "minecraft:ash"
            },
            new WorldBiome
            {
                Id = 55, Name = "minecraft:basalt_deltas", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 6840176, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "nether", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.basalt_deltas.mood",
                Extent = 8, ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.nether.basalt_deltas",
                AmbientSound = "minecraft:ambient.basalt_deltas.loop",
                AddedSound = "minecraft:ambient.basalt_deltas.additions", TickChance = 0.0111,
                Probability = 0.1180933341383934f, ParticleType = "minecraft:white_ash"
            },
            new WorldBiome
            {
                Id = 56, Name = "minecraft:the_end", Precipitation = "none", SkyColor = 0, WaterFogColor = 329011,
                FogColor = 10518688, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "the_end",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 57, Name = "minecraft:end_highlands", Precipitation = "none", SkyColor = 0, WaterFogColor = 329011,
                FogColor = 10518688, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "the_end",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 58, Name = "minecraft:end_midlands", Precipitation = "none", SkyColor = 0, WaterFogColor = 329011,
                FogColor = 10518688, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "the_end",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 59, Name = "minecraft:small_end_islands", Precipitation = "none", SkyColor = 0,
                WaterFogColor = 329011, FogColor = 10518688, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f,
                Category = "the_end", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new WorldBiome
            {
                Id = 60, Name = "minecraft:end_barrens", Precipitation = "none", SkyColor = 0, WaterFogColor = 329011,
                FogColor = 10518688, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "the_end",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            }
        };

        public static CompoundTag GenerateDim()
        {
            var tag = new CompoundTag(null)
            {
                new ByteTag("piglin_safe", 0),
                new ByteTag("natural", 1),
                new FloatTag("ambient_light", 0),
                new StringTag("infiniburn", "#minecraft:infiniburn_overworld"),
                new ByteTag("respawn_anchor_works", 0),
                new ByteTag("has_skylight", 1),
                new ByteTag("bed_works", 1),
                new StringTag("effects", "minecraft:overworld"),
                new ByteTag("has_raids", 1),
                new IntTag("logical_height", 384),
                new DoubleTag("coordinate_scale", 1),
                new IntTag("min_y", -64),
                new ByteTag("has_ceiling", 0),
                new ByteTag("ultrawarm", 0),
                new IntTag("height", 384)
            };
            return tag;
        }
    }
}