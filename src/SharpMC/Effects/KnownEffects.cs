namespace SharpMC.Effects
{
    public static class KnownEffects
    {
        public static readonly Effect Absorption = new Effect { Id = 22, DisplayName = "Absorption", Name = "Absorption", Type = EffectType.Good };
        public static readonly Effect BadLuck = new Effect { Id = 27, DisplayName = "Bad Luck", Name = "BadLuck", Type = EffectType.Bad };
        public static readonly Effect BadOmen = new Effect { Id = 31, DisplayName = "Bad Omen", Name = "BadOmen", Type = EffectType.Bad };
        public static readonly Effect Blindness = new Effect { Id = 15, DisplayName = "Blindness", Name = "Blindness", Type = EffectType.Bad };
        public static readonly Effect ConduitPower = new Effect { Id = 29, DisplayName = "Conduit Power", Name = "ConduitPower", Type = EffectType.Good };
        public static readonly Effect DolphinsGrace = new Effect { Id = 30, DisplayName = "Dolphin's Grace", Name = "DolphinsGrace", Type = EffectType.Good };
        public static readonly Effect FireResistance = new Effect { Id = 12, DisplayName = "Fire Resistance", Name = "FireResistance", Type = EffectType.Good };
        public static readonly Effect Glowing = new Effect { Id = 24, DisplayName = "Glowing", Name = "Glowing", Type = EffectType.Bad };
        public static readonly Effect Haste = new Effect { Id = 3, DisplayName = "Haste", Name = "Haste", Type = EffectType.Good };
        public static readonly Effect HealthBoost = new Effect { Id = 21, DisplayName = "Health Boost", Name = "HealthBoost", Type = EffectType.Good };
        public static readonly Effect HeroOfTheVillage = new Effect { Id = 32, DisplayName = "Hero of the Village", Name = "HeroOfTheVillage", Type = EffectType.Good };
        public static readonly Effect Hunger = new Effect { Id = 17, DisplayName = "Hunger", Name = "Hunger", Type = EffectType.Bad };
        public static readonly Effect InstantDamage = new Effect { Id = 7, DisplayName = "Instant Damage", Name = "InstantDamage", Type = EffectType.Bad };
        public static readonly Effect InstantHealth = new Effect { Id = 6, DisplayName = "Instant Health", Name = "InstantHealth", Type = EffectType.Good };
        public static readonly Effect Invisibility = new Effect { Id = 14, DisplayName = "Invisibility", Name = "Invisibility", Type = EffectType.Good };
        public static readonly Effect JumpBoost = new Effect { Id = 8, DisplayName = "Jump Boost", Name = "JumpBoost", Type = EffectType.Good };
        public static readonly Effect Levitation = new Effect { Id = 25, DisplayName = "Levitation", Name = "Levitation", Type = EffectType.Bad };
        public static readonly Effect Luck = new Effect { Id = 26, DisplayName = "Luck", Name = "Luck", Type = EffectType.Good };
        public static readonly Effect MiningFatigue = new Effect { Id = 4, DisplayName = "Mining Fatigue", Name = "MiningFatigue", Type = EffectType.Bad };
        public static readonly Effect Nausea = new Effect { Id = 9, DisplayName = "Nausea", Name = "Nausea", Type = EffectType.Bad };
        public static readonly Effect NightVision = new Effect { Id = 16, DisplayName = "Night Vision", Name = "NightVision", Type = EffectType.Good };
        public static readonly Effect Poison = new Effect { Id = 19, DisplayName = "Poison", Name = "Poison", Type = EffectType.Bad };
        public static readonly Effect Regeneration = new Effect { Id = 10, DisplayName = "Regeneration", Name = "Regeneration", Type = EffectType.Good };
        public static readonly Effect Resistance = new Effect { Id = 11, DisplayName = "Resistance", Name = "Resistance", Type = EffectType.Good };
        public static readonly Effect Saturation = new Effect { Id = 23, DisplayName = "Saturation", Name = "Saturation", Type = EffectType.Good };
        public static readonly Effect SlowFalling = new Effect { Id = 28, DisplayName = "Slow Falling", Name = "SlowFalling", Type = EffectType.Good };
        public static readonly Effect Slowness = new Effect { Id = 2, DisplayName = "Slowness", Name = "Slowness", Type = EffectType.Bad };
        public static readonly Effect Speed = new Effect { Id = 1, DisplayName = "Speed", Name = "Speed", Type = EffectType.Good };
        public static readonly Effect Strength = new Effect { Id = 5, DisplayName = "Strength", Name = "Strength", Type = EffectType.Good };
        public static readonly Effect WaterBreathing = new Effect { Id = 13, DisplayName = "Water Breathing", Name = "WaterBreathing", Type = EffectType.Good };
        public static readonly Effect Weakness = new Effect { Id = 18, DisplayName = "Weakness", Name = "Weakness", Type = EffectType.Bad };
        public static readonly Effect Wither = new Effect { Id = 20, DisplayName = "Wither", Name = "Wither", Type = EffectType.Bad };
        
        public static readonly Effect[] All = { Absorption, BadLuck, BadOmen, Blindness, ConduitPower, DolphinsGrace, FireResistance, Glowing, Haste, HealthBoost, HeroOfTheVillage, Hunger, InstantDamage, InstantHealth, Invisibility, JumpBoost, Levitation, Luck, MiningFatigue, Nausea, NightVision, Poison, Regeneration, Resistance, Saturation, SlowFalling, Slowness, Speed, Strength, WaterBreathing, Weakness, Wither };
    }
}