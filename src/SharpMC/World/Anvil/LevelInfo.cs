using SharpMC.Network.Binary;
using SharpNBT;

namespace SharpMC.World.Anvil
{
    public class LevelInfo : NbtPoco
    {
        public LevelInfo()
        {
        }

        public LevelInfo(TagContainer dataTag)
        {
            LoadFromNbt(dataTag);
        }

        public int Version { get; set; }
        public bool Initialized { get; set; }
        public string LevelName { get; set; }
        public string GeneratorName { get; set; }
        public int GeneratorVersion { get; set; }
        public string GeneratorOptions { get; set; }
        public long RandomSeed { get; set; }
        public bool MapFeatures { get; set; }
        public long LastPlayed { get; set; }
        public bool AllowCommands { get; set; }
        public bool Hardcore { get; set; }
        private int GameType { get; set; }
        public long Time { get; set; }
        public long DayTime { get; set; }
        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
        public int SpawnZ { get; set; }
        public bool Raining { get; set; }
        public int RainTime { get; set; }
        public bool Thundering { get; set; }
        public int ThunderTime { get; set; }

        public void LoadFromNbt(TagContainer dataTag)
        {
            GetPropertyValue(dataTag, () => Version);
            GetPropertyValue(dataTag, () => Initialized);
            GetPropertyValue(dataTag, () => LevelName);
            GetPropertyValue(dataTag, () => GeneratorName);
            GetPropertyValue(dataTag, () => GeneratorVersion);
            GetPropertyValue(dataTag, () => GeneratorOptions);
            GetPropertyValue(dataTag, () => RandomSeed);
            GetPropertyValue(dataTag, () => MapFeatures);
            GetPropertyValue(dataTag, () => LastPlayed);
            GetPropertyValue(dataTag, () => AllowCommands);
            GetPropertyValue(dataTag, () => Hardcore);
            GetPropertyValue(dataTag, () => GameType);
            GetPropertyValue(dataTag, () => Time);
            GetPropertyValue(dataTag, () => DayTime);
            GetPropertyValue(dataTag, () => SpawnX);
            GetPropertyValue(dataTag, () => SpawnY);
            GetPropertyValue(dataTag, () => SpawnZ);
            GetPropertyValue(dataTag, () => Raining);
            GetPropertyValue(dataTag, () => RainTime);
            GetPropertyValue(dataTag, () => Thundering);
            GetPropertyValue(dataTag, () => ThunderTime);
        }

        public void SaveToNbt(TagContainer dataTag)
        {
            SetPropertyValue(dataTag, () => Version);
            SetPropertyValue(dataTag, () => Initialized);
            SetPropertyValue(dataTag, () => LevelName);
            SetPropertyValue(dataTag, () => GeneratorName);
            SetPropertyValue(dataTag, () => GeneratorVersion);
            SetPropertyValue(dataTag, () => GeneratorOptions);
            SetPropertyValue(dataTag, () => RandomSeed);
            SetPropertyValue(dataTag, () => MapFeatures);
            SetPropertyValue(dataTag, () => LastPlayed);
            SetPropertyValue(dataTag, () => AllowCommands);
            SetPropertyValue(dataTag, () => Hardcore);
            SetPropertyValue(dataTag, () => GameType);
            SetPropertyValue(dataTag, () => Time);
            SetPropertyValue(dataTag, () => DayTime);
            SetPropertyValue(dataTag, () => SpawnX);
            SetPropertyValue(dataTag, () => SpawnY);
            SetPropertyValue(dataTag, () => SpawnZ);
            SetPropertyValue(dataTag, () => Raining);
            SetPropertyValue(dataTag, () => RainTime);
            SetPropertyValue(dataTag, () => Thundering);
            SetPropertyValue(dataTag, () => ThunderTime);
        }
    }
}