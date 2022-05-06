using SharpMC.Network.Binary.Model;

namespace SharpMC.Data
{
    public static class Defaults
    {
        public static WorldBiome[] Biomes =
        {
            new()
            {
                Id = 0, Name = "minecraft:the_void", Precipitation = "none", SkyColor = 8103167, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "none",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 1, Name = "minecraft:plains", Precipitation = "rain", SkyColor = 7907327, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.8f, Downfall = 0.4f, Category = "plains",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 2, Name = "minecraft:sunflower_plains", Precipitation = "rain", SkyColor = 7907327,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.8f, Downfall = 0.4f,
                Category = "plains", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 3, Name = "minecraft:snowy_plains", Precipitation = "snow", SkyColor = 8364543,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0f, Downfall = 0.5f,
                Category = "icy", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 4, Name = "minecraft:ice_spikes", Precipitation = "snow", SkyColor = 8364543,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0f, Downfall = 0.5f,
                Category = "icy", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 5, Name = "minecraft:desert", Precipitation = "none", SkyColor = 7254527, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f, Category = "desert",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 6, Name = "minecraft:swamp", Precipitation = "rain", SkyColor = 7907327, WaterFogColor = 2302743,
                FogColor = 12638463, WaterColor = 6388580, Temperature = 0.8f, Downfall = 0.9f, Category = "swamp",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8, GrassColorMod = "swamp",
                FoliageColor = 6975545
            },
            new()
            {
                Id = 7, Name = "minecraft:forest", Precipitation = "rain", SkyColor = 7972607, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.7f, Downfall = 0.8f, Category = "forest",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 8, Name = "minecraft:flower_forest", Precipitation = "rain", SkyColor = 7972607,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.7f, Downfall = 0.8f,
                Category = "forest", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 9, Name = "minecraft:birch_forest", Precipitation = "rain", SkyColor = 8037887,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.6f, Downfall = 0.6f,
                Category = "forest", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 10, Name = "minecraft:dark_forest", Precipitation = "rain", SkyColor = 7972607,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.7f, Downfall = 0.8f,
                Category = "forest", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                GrassColorMod = "dark_forest"
            },
            new()
            {
                Id = 11, Name = "minecraft:old_growth_birch_forest", Precipitation = "rain", SkyColor = 8037887,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.6f, Downfall = 0.6f,
                Category = "forest", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 12, Name = "minecraft:old_growth_pine_taiga", Precipitation = "rain", SkyColor = 8168447,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.3f, Downfall = 0.8f,
                Category = "taiga", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 13, Name = "minecraft:old_growth_spruce_taiga", Precipitation = "rain", SkyColor = 8233983,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.25f, Downfall = 0.8f,
                Category = "taiga", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 14, Name = "minecraft:taiga", Precipitation = "rain", SkyColor = 8233983, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.25f, Downfall = 0.8f, Category = "taiga",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 15, Name = "minecraft:snowy_taiga", Precipitation = "snow", SkyColor = 8625919,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4020182, Temperature = -0.5f, Downfall = 0.4f,
                Category = "taiga", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 16, Name = "minecraft:savanna", Precipitation = "none", SkyColor = 7254527, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f, Category = "savanna",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 17, Name = "minecraft:savanna_plateau", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "savanna", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 18, Name = "minecraft:windswept_hills", Precipitation = "rain", SkyColor = 8233727,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.2f, Downfall = 0.3f,
                Category = "extreme_hills", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 19, Name = "minecraft:windswept_gravelly_hills", Precipitation = "rain", SkyColor = 8233727,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.2f, Downfall = 0.3f,
                Category = "extreme_hills", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 20, Name = "minecraft:windswept_forest", Precipitation = "rain", SkyColor = 8233727,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.2f, Downfall = 0.3f,
                Category = "extreme_hills", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 21, Name = "minecraft:windswept_savanna", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "savanna", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 22, Name = "minecraft:jungle", Precipitation = "rain", SkyColor = 7842047, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.95f, Downfall = 0.9f, Category = "jungle",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 23, Name = "minecraft:sparse_jungle", Precipitation = "rain", SkyColor = 7842047,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.95f, Downfall = 0.8f,
                Category = "jungle", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 24, Name = "minecraft:bamboo_jungle", Precipitation = "rain", SkyColor = 7842047,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.95f, Downfall = 0.9f,
                Category = "jungle", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 25, Name = "minecraft:badlands", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "mesa", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                GrassColor = 9470285, FoliageColor = 10387789
            },
            new()
            {
                Id = 26, Name = "minecraft:eroded_badlands", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "mesa", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                GrassColor = 9470285, FoliageColor = 10387789
            },
            new()
            {
                Id = 27, Name = "minecraft:wooded_badlands", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "mesa", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                GrassColor = 9470285, FoliageColor = 10387789
            },
            new()
            {
                Id = 28, Name = "minecraft:meadow", Precipitation = "rain", SkyColor = 8103167, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 937679, Temperature = 0.5f, Downfall = 0.8f, Category = "mountain",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8, MinDelay = 12000,
                ReplaceMusic = 0, MaxDelay = 24000, MusicSound = "minecraft:music.overworld.meadow"
            },
            new()
            {
                Id = 29, Name = "minecraft:grove", Precipitation = "snow", SkyColor = 8495359, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = -0.2f, Downfall = 0.8f, Category = "forest",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8, MinDelay = 12000,
                ReplaceMusic = 0, MaxDelay = 24000, MusicSound = "minecraft:music.overworld.grove"
            },
            new()
            {
                Id = 30, Name = "minecraft:snowy_slopes", Precipitation = "snow", SkyColor = 8560639,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = -0.3f, Downfall = 0.9f,
                Category = "mountain", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MusicSound = "minecraft:music.overworld.snowy_slopes",
                MinDelay = 12000
            },
            new()
            {
                Id = 31, Name = "minecraft:frozen_peaks", Precipitation = "snow", SkyColor = 8756735,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = -0.7f, Downfall = 0.9f,
                Category = "mountain", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MusicSound = "minecraft:music.overworld.frozen_peaks",
                MinDelay = 12000
            },
            new()
            {
                Id = 32, Name = "minecraft:jagged_peaks", Precipitation = "snow", SkyColor = 8756735,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = -0.7f, Downfall = 0.9f,
                Category = "mountain", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.overworld.jagged_peaks"
            },
            new()
            {
                Id = 33, Name = "minecraft:stony_peaks", Precipitation = "rain", SkyColor = 7776511,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 1f, Downfall = 0.3f,
                Category = "mountain", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.overworld.stony_peaks"
            },
            new()
            {
                Id = 34, Name = "minecraft:river", Precipitation = "rain", SkyColor = 8103167, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "river",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 35, Name = "minecraft:frozen_river", Precipitation = "snow", SkyColor = 8364543,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 3750089, Temperature = 0f, Downfall = 0.5f,
                Category = "river", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 36, Name = "minecraft:beach", Precipitation = "rain", SkyColor = 7907327, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.8f, Downfall = 0.4f, Category = "beach",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 37, Name = "minecraft:snowy_beach", Precipitation = "snow", SkyColor = 8364543,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4020182, Temperature = 0.05f, Downfall = 0.3f,
                Category = "beach", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 38, Name = "minecraft:stony_shore", Precipitation = "rain", SkyColor = 8233727,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.2f, Downfall = 0.3f,
                Category = "beach", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 39, Name = "minecraft:warm_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 270131, FogColor = 12638463, WaterColor = 4445678, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 40, Name = "minecraft:lukewarm_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 267827, FogColor = 12638463, WaterColor = 4566514, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 41, Name = "minecraft:deep_lukewarm_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 267827, FogColor = 12638463, WaterColor = 4566514, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 42, Name = "minecraft:ocean", Precipitation = "rain", SkyColor = 8103167, WaterFogColor = 329011,
                FogColor = 12638463, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "ocean",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 43, Name = "minecraft:deep_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 44, Name = "minecraft:cold_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4020182, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 45, Name = "minecraft:deep_cold_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4020182, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 46, Name = "minecraft:frozen_ocean", Precipitation = "snow", SkyColor = 8364543,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 3750089, Temperature = 0f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                TemperatureMod = "frozen"
            },
            new()
            {
                Id = 47, Name = "minecraft:deep_frozen_ocean", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 3750089, Temperature = 0.5f, Downfall = 0.5f,
                Category = "ocean", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                TemperatureMod = "frozen"
            },
            new()
            {
                Id = 48, Name = "minecraft:mushroom_fields", Precipitation = "rain", SkyColor = 7842047,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.9f, Downfall = 1f,
                Category = "mushroom", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 49, Name = "minecraft:dripstone_caves", Precipitation = "rain", SkyColor = 7907327,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.8f, Downfall = 0.4f,
                Category = "underground", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.overworld.dripstone_caves"
            },
            new()
            {
                Id = 50, Name = "minecraft:lush_caves", Precipitation = "rain", SkyColor = 8103167,
                WaterFogColor = 329011, FogColor = 12638463, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f,
                Category = "underground", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8,
                ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.overworld.lush_caves"
            },
            new()
            {
                Id = 51, Name = "minecraft:nether_wastes", Precipitation = "none", SkyColor = 7254527,
                WaterFogColor = 329011, FogColor = 3344392, WaterColor = 4159204, Temperature = 2f, Downfall = 0f,
                Category = "nether", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.nether_wastes.mood",
                Extent = 8, ReplaceMusic = 0, MaxDelay = 24000, MinDelay = 12000,
                MusicSound = "minecraft:music.nether.nether_wastes",
                AmbientSound = "minecraft:ambient.nether_wastes.loop",
                AddedSound = "minecraft:ambient.nether_wastes.additions", TickChance = 0.0111
            },
            new()
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
            new()
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
            new()
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
            new()
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
            new()
            {
                Id = 56, Name = "minecraft:the_end", Precipitation = "none", SkyColor = 0, WaterFogColor = 329011,
                FogColor = 10518688, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "the_end",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 57, Name = "minecraft:end_highlands", Precipitation = "none", SkyColor = 0, WaterFogColor = 329011,
                FogColor = 10518688, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "the_end",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 58, Name = "minecraft:end_midlands", Precipitation = "none", SkyColor = 0, WaterFogColor = 329011,
                FogColor = 10518688, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "the_end",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 59, Name = "minecraft:small_end_islands", Precipitation = "none", SkyColor = 0,
                WaterFogColor = 329011, FogColor = 10518688, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f,
                Category = "the_end", TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            },
            new()
            {
                Id = 60, Name = "minecraft:end_barrens", Precipitation = "none", SkyColor = 0, WaterFogColor = 329011,
                FogColor = 10518688, WaterColor = 4159204, Temperature = 0.5f, Downfall = 0.5f, Category = "the_end",
                TickDelay = 6000, Offset = 2, Sound = "minecraft:ambient.cave", Extent = 8
            }
        };

        public static WorldCodec[] Realms =
        {
            new()
            {
                Id = 0, Name = "minecraft:overworld", Effects = "minecraft:overworld",
                Infiniburn = "#minecraft:infiniburn_overworld", Skylight = 1,
                BedWorks = 1, Raids = 1, LogicalHeight = 384, Scale = 1,
                MinY = -64, Height = 384, Natural = 1
            },
            new()
            {
                Id = 1, Name = "minecraft:overworld_caves", Effects = "minecraft:overworld",
                Natural = 1, Infiniburn = "#minecraft:infiniburn_overworld",
                Raids = 1, LogicalHeight = 384, Scale = 1, MinY = -64,
                Height = 384, HasCeiling = 1, Skylight = 1, BedWorks = 1
            },
            new()
            {
                Id = 2, Name = "minecraft:the_nether", PiglinSafe = 1,
                AmbientLight = 0.10000000149011612f, Infiniburn = "#minecraft:infiniburn_nether",
                Respawn = 1, Effects = "minecraft:the_nether", FixedTime = 18000,
                LogicalHeight = 128, Scale = 8, HasCeiling = 1, UltraWarm = 1, Height = 256
            },
            new()
            {
                Id = 3, Name = "minecraft:the_end", Effects = "minecraft:the_end",
                Infiniburn = "#minecraft:infiniburn_end", FixedTime = 6000,
                Raids = 1, LogicalHeight = 256, Scale = 1, Height = 256
            }
        };

        public static string[] WorldNames =
        {
            "minecraft:overworld", "minecraft:the_nether", "minecraft:the_end"
        };

        public static string WorldName = "minecraft:overworld";

        public static LoginDim CurrentDim => new()
        {
            BedWorks = 1,
            CoordinateScale = 1,
            Effects = "minecraft:overworld",
            HasRaids = 1,
            HasSkylight = 1,
            Height = 384,
            InfiniBurn = "#minecraft:infiniburn_overworld",
            LogicalHeight = 384,
            MinY = -64,
            Natural = 1
        };
    }
}