namespace SharpMC.Data.Effects
{
    public static class KnownEffects
    {
        public static readonly Effect Absorption = new() { Id = 22, DisplayName = "Absorption", Name = "Absorption", Type = EffectType.Good };
        public static readonly Effect BadLuck = new() { Id = 27, DisplayName = "Bad Luck", Name = "BadLuck", Type = EffectType.Bad };
        public static readonly Effect BadOmen = new() { Id = 31, DisplayName = "Bad Omen", Name = "BadOmen", Type = EffectType.Bad };
        public static readonly Effect Blindness = new() { Id = 15, DisplayName = "Blindness", Name = "Blindness", Type = EffectType.Bad };
        public static readonly Effect ConduitPower = new() { Id = 29, DisplayName = "Conduit Power", Name = "ConduitPower", Type = EffectType.Good };
        public static readonly Effect DolphinsGrace = new() { Id = 30, DisplayName = "Dolphin's Grace", Name = "DolphinsGrace", Type = EffectType.Good };
        public static readonly Effect FireResistance = new() { Id = 12, DisplayName = "Fire Resistance", Name = "FireResistance", Type = EffectType.Good };
        public static readonly Effect Glowing = new() { Id = 24, DisplayName = "Glowing", Name = "Glowing", Type = EffectType.Bad };
        public static readonly Effect Haste = new() { Id = 3, DisplayName = "Haste", Name = "Haste", Type = EffectType.Good };
        public static readonly Effect HealthBoost = new() { Id = 21, DisplayName = "Health Boost", Name = "HealthBoost", Type = EffectType.Good };
        public static readonly Effect HeroOfTheVillage = new() { Id = 32, DisplayName = "Hero of the Village", Name = "HeroOfTheVillage", Type = EffectType.Good };
        public static readonly Effect Hunger = new() { Id = 17, DisplayName = "Hunger", Name = "Hunger", Type = EffectType.Bad };
        public static readonly Effect InstantDamage = new() { Id = 7, DisplayName = "Instant Damage", Name = "InstantDamage", Type = EffectType.Bad };
        public static readonly Effect InstantHealth = new() { Id = 6, DisplayName = "Instant Health", Name = "InstantHealth", Type = EffectType.Good };
        public static readonly Effect Invisibility = new() { Id = 14, DisplayName = "Invisibility", Name = "Invisibility", Type = EffectType.Good };
        public static readonly Effect JumpBoost = new() { Id = 8, DisplayName = "Jump Boost", Name = "JumpBoost", Type = EffectType.Good };
        public static readonly Effect Levitation = new() { Id = 25, DisplayName = "Levitation", Name = "Levitation", Type = EffectType.Bad };
        public static readonly Effect Luck = new() { Id = 26, DisplayName = "Luck", Name = "Luck", Type = EffectType.Good };
        public static readonly Effect MiningFatigue = new() { Id = 4, DisplayName = "Mining Fatigue", Name = "MiningFatigue", Type = EffectType.Bad };
        public static readonly Effect Nausea = new() { Id = 9, DisplayName = "Nausea", Name = "Nausea", Type = EffectType.Bad };
        public static readonly Effect NightVision = new() { Id = 16, DisplayName = "Night Vision", Name = "NightVision", Type = EffectType.Good };
        public static readonly Effect Poison = new() { Id = 19, DisplayName = "Poison", Name = "Poison", Type = EffectType.Bad };
        public static readonly Effect Regeneration = new() { Id = 10, DisplayName = "Regeneration", Name = "Regeneration", Type = EffectType.Good };
        public static readonly Effect Resistance = new() { Id = 11, DisplayName = "Resistance", Name = "Resistance", Type = EffectType.Good };
        public static readonly Effect Saturation = new() { Id = 23, DisplayName = "Saturation", Name = "Saturation", Type = EffectType.Good };
        public static readonly Effect SlowFalling = new() { Id = 28, DisplayName = "Slow Falling", Name = "SlowFalling", Type = EffectType.Good };
        public static readonly Effect Slowness = new() { Id = 2, DisplayName = "Slowness", Name = "Slowness", Type = EffectType.Bad };
        public static readonly Effect Speed = new() { Id = 1, DisplayName = "Speed", Name = "Speed", Type = EffectType.Good };
        public static readonly Effect Strength = new() { Id = 5, DisplayName = "Strength", Name = "Strength", Type = EffectType.Good };
        public static readonly Effect WaterBreathing = new() { Id = 13, DisplayName = "Water Breathing", Name = "WaterBreathing", Type = EffectType.Good };
        public static readonly Effect Weakness = new() { Id = 18, DisplayName = "Weakness", Name = "Weakness", Type = EffectType.Bad };
        public static readonly Effect Wither = new() { Id = 20, DisplayName = "Wither", Name = "Wither", Type = EffectType.Bad };
        
        public static readonly Effect[] All = { Absorption, BadLuck, BadOmen, Blindness, ConduitPower, DolphinsGrace, FireResistance, Glowing, Haste, HealthBoost, HeroOfTheVillage, Hunger, InstantDamage, InstantHealth, Invisibility, JumpBoost, Levitation, Luck, MiningFatigue, Nausea, NightVision, Poison, Regeneration, Resistance, Saturation, SlowFalling, Slowness, Speed, Strength, WaterBreathing, Weakness, Wither };
    }
}