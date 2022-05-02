using SharpMC.API;
using SharpMC.API.Enums;

namespace SharpMC.Config
{
    public sealed class LevelSettings
    {
        public string? Seed { get; set; }
        public LevelType Type { get; set; }
        public string? WorldName { get; set; }
        public GameMode GameMode { get; set; }
    }
}