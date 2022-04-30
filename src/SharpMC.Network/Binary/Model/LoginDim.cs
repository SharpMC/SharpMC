using SharpNBT;
using static SharpMC.Network.Binary.NbtUtil;

namespace SharpMC.Network.Binary.Model
{
    public class LoginDim : INbtSerializable
    {
        public byte PiglinSafe { get; set; }
        public int Height { get; set; }
        public byte UltraWarm { get; set; }
        public byte HasCeiling { get; set; }
        public int MinY { get; set; }
        public double CoordinateScale { get; set; }
        public int LogicalHeight { get; set; }
        public byte HasRaids { get; set; }
        public string Effects { get; set; }
        public byte BedWorks { get; set; }
        public byte HasSkylight { get; set; }
        public byte RespawnWorks { get; set; }
        public string InfiniBurn { get; set; }
        public float AmbientLight { get; set; }
        public byte Natural { get; set; }

        public void ToObject(CompoundTag tag)
        {
            var d = ToDict(tag);
            PiglinSafe = (byte) d["piglin_safe"];
            Natural = (byte) d["natural"];
            AmbientLight = (float) d["ambient_light"];
            InfiniBurn = (string) d["infiniburn"];
            RespawnWorks = (byte) d["respawn_anchor_works"];
            HasSkylight = (byte) d["has_skylight"];
            BedWorks = (byte) d["bed_works"];
            Effects = (string) d["effects"];
            HasRaids = (byte) d["has_raids"];
            LogicalHeight = (int) d["logical_height"];
            CoordinateScale = (double) d["coordinate_scale"];
            MinY = (int) d["min_y"];
            HasCeiling = (byte) d["has_ceiling"];
            UltraWarm = (byte) d["ultrawarm"];
            Height = (int) d["height"];
        }

        public CompoundTag ToCompound()
        {
            var tag = new CompoundTag(null)
            {
                new ByteTag("piglin_safe", PiglinSafe),
                new ByteTag("natural", Natural),
                new FloatTag("ambient_light", AmbientLight),
                new StringTag("infiniburn", InfiniBurn),
                new ByteTag("respawn_anchor_works", RespawnWorks),
                new ByteTag("has_skylight", HasSkylight),
                new ByteTag("bed_works", BedWorks),
                new StringTag("effects", Effects),
                new ByteTag("has_raids", HasRaids),
                new IntTag("logical_height", LogicalHeight),
                new DoubleTag("coordinate_scale", CoordinateScale),
                new IntTag("min_y", MinY),
                new ByteTag("has_ceiling", HasCeiling),
                new ByteTag("ultrawarm", UltraWarm),
                new IntTag("height", Height)
            };
            return tag;
        }
    }
}