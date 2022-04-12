using System.Collections.Generic;
using SharpNBT;

namespace SharpMC.Network.Binary
{
    public class WorldBiome : INbtSerializable
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Precipitation { get; set; }
        public string GrassColorMod { get; set; }
        public int SkyColor { get; set; }
        public float Temperature { get; set; }
        public float Downfall { get; set; }
        public string Category { get; set; }
        public int? FoliageColor { get; set; }
        public int WaterFogColor { get; set; }
        public int FogColor { get; set; }
        public int? GrassColor { get; set; }
        public int WaterColor { get; set; }
        public int TickDelay { get; set; }
        public double Offset { get; set; }
        public string Sound { get; set; }
        public int Extent { get; set; }

        public byte? ReplaceMusic { get; set; }
        public int? MaxDelay { get; set; }
        public int? MinDelay { get; set; }
        public string MusicSound { get; set; }

        public string AmbientSound { get; set; }
        public string AddedSound { get; set; }
        public double TickChance { get; set; }

        public float Probability { get; set; }
        public string ParticleType { get; set; }
        public string TemperatureMod { get; set; }

        public CompoundTag ToCompound()
        {
            var effects = new List<Tag>
            {
                new IntTag("sky_color", SkyColor),
                new IntTag("water_fog_color", WaterFogColor),
                new IntTag("fog_color", FogColor),
                new IntTag("water_color", WaterColor),
                new CompoundTag("mood_sound")
                {
                    new IntTag("tick_delay", TickDelay),
                    new DoubleTag("offset", Offset),
                    new StringTag("sound", Sound),
                    new IntTag("block_search_extent", Extent)
                }
            };
            if (!string.IsNullOrWhiteSpace(ParticleType))
            {
                effects.Insert(1, new CompoundTag("particle")
                {
                    new FloatTag("probability", Probability),
                    new CompoundTag("options")
                    {
                        new StringTag("type", ParticleType)
                    }
                });
            }
            if (!string.IsNullOrWhiteSpace(AddedSound))
            {
                effects.Insert(1, new CompoundTag("additions_sound")
                {
                    new StringTag("sound", AddedSound),
                    new DoubleTag("tick_chance", TickChance)
                });
            }
            if (!string.IsNullOrWhiteSpace(AmbientSound))
            {
                effects.Insert(1, new StringTag("ambient_sound", AmbientSound));
            }
            if (ReplaceMusic != null)
            {
                effects.Insert(0, new CompoundTag("music")
                {
                    new ByteTag("replace_current_music", ReplaceMusic.Value),
                    new IntTag("max_delay", MaxDelay.GetValueOrDefault()),
                    new StringTag("sound", MusicSound),
                    new IntTag("min_delay", MinDelay.GetValueOrDefault())
                });
            }
            if (FoliageColor != null)
            {
                effects.Insert(1, new IntTag("foliage_color", FoliageColor.Value));
            }
            if (!string.IsNullOrWhiteSpace(GrassColorMod))
            {
                effects.Insert(0, new StringTag("grass_color_modifier", GrassColorMod));
            }
            if (GrassColor != null)
            {
                effects.Insert(1, new IntTag("grass_color", GrassColor.Value));
            }
            var fields = new List<Tag>
            {
                new StringTag("precipitation", Precipitation),
                new CompoundTag("effects", effects),
                new FloatTag("temperature", Temperature),
                new FloatTag("downfall", Downfall),
                new StringTag("category", Category)
            };
            if (!string.IsNullOrWhiteSpace(TemperatureMod))
            {
                fields.Add(new StringTag("temperature_modifier", TemperatureMod));
            }
            var tag = new CompoundTag(null)
            {
                new StringTag("name", Name),
                new IntTag("id", Id),
                new CompoundTag("element", fields)
            };
            return tag;
        }

        public void ToObject(CompoundTag tag)
        {
            throw new System.NotImplementedException();
        }
    }
}