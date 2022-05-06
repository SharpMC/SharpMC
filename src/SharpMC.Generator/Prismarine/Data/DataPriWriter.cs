using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static SharpMC.Generator.Prismarine.CodeGen;
using static SharpMC.Generator.Tools.Helpers;

// ReSharper disable UseObjectOrCollectionInitializer

namespace SharpMC.Generator.Prismarine.Data
{
    internal static class DataPriWriter
    {
        public static void WriteBlocks(Block[] blocks, BlockLoot[] loots, string target)
        {
            const string fieldType = "Block";
            var f = new List<OneField>();
            foreach (var block in blocks)
            {
                var fieldName = ToTitleCase(block.Name);
                var v = $" = new {fieldType} {{ Id = {block.Id}, " +
                        $"DisplayName = \"{block.DisplayName}\", Name = \"{block.Name}\", " +
                        $"MinStateId = {block.MinStateId}, MaxStateId = {block.MaxStateId}, " +
                        $"Diggable = {block.Diggable.ToStr()}, Hardness = {block.Hardness}, " +
                        $"Resistance = {block.Resistance}, StackSize = {block.StackSize}, " +
                        $"DefaultState = {block.DefaultState}, Material = \"{block.Material}\" ";
                var loot = loots.SingleOrDefault(l => l.Block == block.Name);
                if (loot != null && loot.Drops.Count >= 1)
                {
                    var dropsTxt = string.Join(", ", loot.Drops.Select(ToStr));
                    v += $", Drops = new [] {{ {dropsTxt} }} ";
                }
                v += "}";
                f.Add(new OneField
                {
                    Name = fieldName, TypeName = $"readonly {fieldType}", Constant = v
                });
            }
            (f = f.SortByName()).AddAllField(fieldType);
            var item = new OneUnit
            {
                Class = "KnownBlocks", Namespace = $"{nameof(SharpMC)}.Blocks", Fields = f
            };
            Console.WriteLine($" * {item.Class}");
            Write(item, target);
        }

        private static void Write(OneUnit item, string target)
        {
            var nsp = item.Namespace;
            var nspDir = nsp.Replace('.', Path.DirectorySeparatorChar);
            var className = item.Class;
            var outPath = Path.Combine(target, nspDir, $"{className}.cs");
            var outDir = Path.GetDirectoryName(outPath);
            Directory.CreateDirectory(outDir);
            var lines = new List<string>();
            lines.Add("using System;");
            lines.Add(string.Empty);
            lines.Add($"namespace {nsp}");
            lines.Add("{");
            var ext = new List<string>();
            var extStr = ext.Count == 0 ? string.Empty : $" : {string.Join(", ", ext)}";
            lines.Add($"{Sp}public class {className}{extStr}");
            lines.Add($"{Sp}{{");
            foreach (var field in item.Fields)
            {
                var fName = field.Name;
                var fType = field.TypeName;
                var fInit = field.Constant;
                lines.Add($"{Sp}{Sp}public static {fType} {fName}{fInit};");
            }
            lines.Add($"{Sp}}}");
            lines.Add("}");
            File.WriteAllLines(outPath, lines, Encoding.UTF8);
        }

        public static void WriteEntities(Entity[] entity, EntityLoot[] loots, string target)
        {
            const string fieldType = "Entity";
            var f = new List<OneField>();
            foreach (var one in entity)
            {
                var fieldName = ToTitleCase(one.Name);
                var v = $" = new {fieldType} {{ Id = {one.Id}, " +
                        $"Type = EntityType.{one.Type}, " +
                        $"DisplayName = \"{one.DisplayName}\", Name = \"{one.Name}\", " +
                        $"Width = {one.Width}, Height = {one.Height} ";
                var loot = loots.SingleOrDefault(l => l.Entity == one.Name);
                if (loot != null && loot.Drops.Count >= 1)
                {
                    var dropsTxt = string.Join(", ", loot.Drops.Select(ToStr));
                    v += $", Drops = new [] {{ {dropsTxt} }} ";
                }
                v += "}";
                f.Add(new OneField
                {
                    Name = fieldName, TypeName = $"readonly {fieldType}", Constant = v
                });
            }
            (f = f.SortByName()).AddAllField(fieldType);
            var item = new OneUnit
            {
                Class = "KnownEntities", Namespace = $"{nameof(SharpMC)}.Entities", Fields = f
            };
            Console.WriteLine($" * {item.Class}");
            Write(item, target);
        }

        public static void WriteParticles(Particle[] items, string target)
        {
            const string fieldType = "Particle";
            var f = new List<OneField>();
            foreach (var one in items)
            {
                var fieldName = ToTitleCase(one.Name);
                var v = $" = new {fieldType} {{ Id = {one.Id}, " +
                        $"Name = \"{one.Name}\" " +
                        "}";
                f.Add(new OneField
                {
                    Name = fieldName, TypeName = $"readonly {fieldType}", Constant = v
                });
            }
            (f = f.SortByName()).AddAllField(fieldType);
            var item = new OneUnit
            {
                Class = "KnownParticles", Namespace = $"{nameof(SharpMC)}.Particles", Fields = f
            };
            Console.WriteLine($" * {item.Class}");
            Write(item, target);
        }

        public static void WriteItems(Item[] items, string target)
        {
            const string fieldType = "Item";
            var f = new List<OneField>();
            foreach (var one in items)
            {
                var fieldName = ToTitleCase(one.Name);
                var v = $" = new {fieldType} {{ Id = {one.Id}, " +
                        $"DisplayName = \"{one.DisplayName}\", Name = \"{one.Name}\", " +
                        $"StackSize = {one.StackSize} " +
                        "}";
                f.Add(new OneField
                {
                    Name = fieldName, TypeName = $"readonly {fieldType}", Constant = v
                });
            }
            (f = f.SortByName()).AddAllField(fieldType);
            var item = new OneUnit
            {
                Class = "KnownItems", Namespace = $"{nameof(SharpMC)}.Items", Fields = f
            };
            Console.WriteLine($" * {item.Class}");
            Write(item, target);
        }

        public static void WriteBiomes(Biome[] biomes, string target)
        {
            const string fieldType = "Biome";
            var f = new List<OneField>();
            foreach (var one in biomes)
            {
                var v = $" = new {fieldType} {{ Id = {one.Id}, " +
                        $"DisplayName = \"{one.DisplayName}\", Name = \"{one.Name}\", " +
                        $"Category = BiomeCategory.{one.Category}, Temperature = {one.Temperature}, " +
                        $"Precipitation = BiomePrecipitation.{one.Precipitation}, " +
                        $"Depth = {one.Depth}, Color = {one.Color}, " +
                        $"Rainfall = {one.Rainfall}, Dimension = BiomeDim.{one.Dimension} " +
                        "}";
                f.Add(new OneField
                {
                    Name = ToTitleCase(one.Name), TypeName = $"readonly {fieldType}", Constant = v
                });
            }
            (f = f.SortByName()).AddAllField(fieldType);
            var item = new OneUnit
            {
                Class = "KnownBiomes", Namespace = $"{nameof(SharpMC)}.Biomes", Fields = f
            };
            Console.WriteLine($" * {item.Class}");
            Write(item, target);
        }

        private static string ToStr(LootItem item)
        {
            var dv = $"new LootItem {{ Item = \"{item.Item}\", " +
                     $"DropChance = {item.DropChance}, " +
                     $"StackSizeRange = new [] {{ {string.Join(", ", item.StackSizeRange)} }} ";
            if (item.BlockAge != null)
                dv += $", BlockAge = {item.BlockAge} ";
            if (item.PlayerKill != null)
                dv += $", PlayerKill = {item.PlayerKill.ToStr()} ";
            if (item.SilkTouch != null)
                dv += $", SilkTouch = {item.SilkTouch} ";
            if (item.NoSilkTouch != null)
                dv += $", NoSilkTouch = {item.NoSilkTouch} ";
            dv += "}";
            return dv;
        }

        public static void WriteFoods(Food[] foods, string target)
        {
            const string fieldType = "Food";
            var f = new List<OneField>();
            foreach (var one in foods)
            {
                var v = $" = new {fieldType} {{ Id = {one.Id}, " +
                        $"DisplayName = \"{one.DisplayName}\", Name = \"{one.Name}\", " +
                        $"StackSize = {one.StackSize}, FoodPoints = {one.FoodPoints}, " +
                        $"Saturation = {one.Saturation}, EffectiveQuality = {one.EffectiveQuality}, " +
                        $"SaturationRatio = {one.SaturationRatio} " +
                        "}";
                f.Add(new OneField
                {
                    Name = ToTitleCase(one.Name), TypeName = $"readonly {fieldType}", Constant = v
                });
            }
            (f = f.SortByName()).AddAllField(fieldType);
            var item = new OneUnit
            {
                Class = "KnownFoods", Namespace = $"{nameof(SharpMC)}.Foods", Fields = f
            };
            Console.WriteLine($" * {item.Class}");
            Write(item, target);
        }

        public static void WriteAttributes(Attribute[] attributes, string target)
        {
            const string fieldType = "Attribute";
            var f = new List<OneField>();
            foreach (var one in attributes)
            {
                var v = $" = new {fieldType} {{ " +
                        $"Name = \"{one.Name}\", Resource = \"{one.Resource}\", " +
                        "}";
                f.Add(new OneField
                {
                    Name = ToTitleCase(one.Name), TypeName = $"readonly {fieldType}", Constant = v
                });
            }
            (f = f.SortByName()).AddAllField(fieldType);
            var item = new OneUnit
            {
                Class = "KnownAttributes", Namespace = $"{nameof(SharpMC)}.Attributes", Fields = f
            };
            Console.WriteLine($" * {item.Class}");
            Write(item, target);
        }

        public static void WriteEffects(Effect[] effects, string target)
        {
            const string fieldType = "Effect";
            var f = new List<OneField>();
            foreach (var one in effects)
            {
                var v = $" = new {fieldType} {{ Id = {one.Id}, " +
                        $"DisplayName = \"{one.DisplayName}\", Name = \"{one.Name}\", " +
                        $"Type = EffectType.{one.Type} " +
                        "}";
                f.Add(new OneField
                {
                    Name = ToTitleCase(one.Name), TypeName = $"readonly {fieldType}", Constant = v
                });
            }
            (f = f.SortByName()).AddAllField(fieldType);
            var item = new OneUnit
            {
                Class = "KnownEffects", Namespace = $"{nameof(SharpMC)}.Effects", Fields = f
            };
            Console.WriteLine($" * {item.Class}");
            Write(item, target);
        }

        public static void WriteEnchantments(Enchantment[] enchantments, string target)
        {
            const string fieldType = "Enchantment";
            var f = new List<OneField>();
            foreach (var one in enchantments)
            {
                var cat = ToTitleCase(one.Category.ToString());
                var v = $" = new {fieldType} {{ Id = {one.Id}, " +
                        $"DisplayName = \"{one.DisplayName}\", Name = \"{one.Name}\", " +
                        $"MaxLevel = {one.MaxLevel}, TreasureOnly = {one.TreasureOnly.ToStr()}, " +
                        $"Curse = {one.Curse.ToStr()}, Weight = {one.Weight}, Tradeable = {one.Tradeable.ToStr()}, " +
                        $"Discoverable = {one.Discoverable.ToStr()}, Category = EnchantCategory.{cat} ";
                if (one.Exclude.Length >= 1)
                    v += $", Exclude = new [] {{ {string.Join(", ", one.Exclude.Select(e => $"\"{e}\""))} }} ";
                v += "}";
                f.Add(new OneField
                {
                    Name = ToTitleCase(one.Name), TypeName = $"readonly {fieldType}", Constant = v
                });
            }
            (f = f.SortByName()).AddAllField(fieldType);
            var item = new OneUnit
            {
                Class = "KnownEnchantments", Namespace = $"{nameof(SharpMC)}.Enchantments", Fields = f
            };
            Console.WriteLine($" * {item.Class}");
            Write(item, target);
        }
    }
}