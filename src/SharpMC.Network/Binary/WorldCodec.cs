using System.Collections.Generic;
using SharpNBT;

namespace SharpMC.Network.Binary
{
    public class WorldCodec : INbtSerializable
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public byte PiglinSafe { get; set; }
        public int Height { get; set; }
        public byte UltraWarm { get; set; }
        public byte HasCeiling { get; set; }
        public int MinY { get; set; }
        public double Scale { get; set; }
        public int LogicalHeight { get; set; }
        public long? FixedTime { get; set; }
        public byte Raids { get; set; }
        public string Effects { get; set; }
        public byte BedWorks { get; set; }
        public byte Skylight { get; set; }
        public byte Respawn { get; set; }
        public string Infiniburn { get; set; }
        public float AmbientLight { get; set; }
        public byte Natural { get; set; }

        public CompoundTag ToCompound()
        {
            var fields = new List<Tag>
            {
                new ByteTag("piglin_safe", PiglinSafe),
                new ByteTag("natural", Natural),
                new FloatTag("ambient_light", AmbientLight),
                new StringTag("infiniburn", Infiniburn),
                new ByteTag("respawn_anchor_works", Respawn),
                new ByteTag("has_skylight", Skylight),
                new ByteTag("bed_works", BedWorks),
                new StringTag("effects", Effects),
                new ByteTag("has_raids", Raids),
                new IntTag("logical_height", LogicalHeight),
                new DoubleTag("coordinate_scale", Scale),
                new IntTag("min_y", MinY),
                new ByteTag("has_ceiling", HasCeiling),
                new ByteTag("ultrawarm", UltraWarm),
                new IntTag("height", Height)
            };
            if (FixedTime.HasValue)
            {
                fields.Insert(8, new LongTag("fixed_time", FixedTime.Value));
            }
            var tag = new CompoundTag(null)
            {
                new StringTag("name", Name),
                new IntTag("id", Id),
                new CompoundTag("element", fields)
            };
            return tag;
        }
    }
}