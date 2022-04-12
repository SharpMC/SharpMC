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
                    new ListTag("value", TagType.Compound, Defaults.Biomes.Select(b => b.ToCompound()))
                }
            };
            return tag;
        }
    }
}