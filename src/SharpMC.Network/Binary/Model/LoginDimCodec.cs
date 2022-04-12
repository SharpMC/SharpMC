using System.Linq;
using SharpNBT;

namespace SharpMC.Network.Binary
{
    public class LoginDimCodec : INbtSerializable
    {
        public WorldCodec[] Realms { get; set; }
        public WorldBiome[] Biomes { get; set; }

        public CompoundTag ToCompound()
        {
            var tag = new CompoundTag(null)
            {
                new CompoundTag("minecraft:dimension_type")
                {
                    new StringTag("type", "minecraft:dimension_type"),
                    new ListTag("value", TagType.Compound, Realms.Select(b => b.ToCompound()))
                },
                new CompoundTag("minecraft:worldgen/biome")
                {
                    new StringTag("type", "minecraft:worldgen/biome"),
                    new ListTag("value", TagType.Compound, Biomes.Select(b => b.ToCompound()))
                }
            };
            return tag;
        }

        public void ToObject(CompoundTag tag)
        {
            // TODO ?!
        }
    }
}