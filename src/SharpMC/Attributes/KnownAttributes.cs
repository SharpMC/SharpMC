namespace SharpMC.Attributes
{
    public static class KnownAttributes
    {
        public static readonly Attribute Armor = new Attribute { Name = "armor", Resource = "minecraft:generic.armor", };
        public static readonly Attribute ArmorToughness = new Attribute { Name = "armorToughness", Resource = "minecraft:generic.armor_toughness", };
        public static readonly Attribute AttackDamage = new Attribute { Name = "attackDamage", Resource = "minecraft:generic.attack_damage", };
        public static readonly Attribute AttackKnockback = new Attribute { Name = "attackKnockback", Resource = "minecraft:generic.attack_knockback", };
        public static readonly Attribute AttackSpeed = new Attribute { Name = "attackSpeed", Resource = "minecraft:generic.attack_speed", };
        public static readonly Attribute FlyingSpeed = new Attribute { Name = "flyingSpeed", Resource = "minecraft:generic.flying_speed", };
        public static readonly Attribute FollowRange = new Attribute { Name = "followRange", Resource = "minecraft:generic.follow_range", };
        public static readonly Attribute HorseJumpStrength = new Attribute { Name = "horseJumpStrength", Resource = "minecraft:horse.jump_strength", };
        public static readonly Attribute KnockbackResistance = new Attribute { Name = "knockbackResistance", Resource = "minecraft:generic.knockback_resistance", };
        public static readonly Attribute Luck = new Attribute { Name = "luck", Resource = "minecraft:generic.luck", };
        public static readonly Attribute MaxHealth = new Attribute { Name = "maxHealth", Resource = "minecraft:generic.max_health", };
        public static readonly Attribute MovementSpeed = new Attribute { Name = "movementSpeed", Resource = "minecraft:generic.movement_speed", };
        public static readonly Attribute ZombieSpawnReinforcements = new Attribute { Name = "zombieSpawnReinforcements", Resource = "minecraft:zombie.spawn_reinforcements", };
        
        public static readonly Attribute[] All = { Armor, ArmorToughness, AttackDamage, AttackKnockback, AttackSpeed, FlyingSpeed, FollowRange, HorseJumpStrength, KnockbackResistance, Luck, MaxHealth, MovementSpeed, ZombieSpawnReinforcements };
    }
}