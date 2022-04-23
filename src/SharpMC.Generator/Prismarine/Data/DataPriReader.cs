using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static SharpMC.Generator.Tools.Helpers;

namespace SharpMC.Generator.Prismarine.Data
{
    internal static class DataPriReader
    {
        internal static void Start(string source, string target)
        {
            const string version = "1.18";
            Start(version, source, target);

            const string oldVersion = "1.17";
            StartOld(oldVersion, source, target);
        }

        private static void StartOld(string version, string source, string target)
        {
            Console.WriteLine();
            var foods =  Load<Food>(version, source, "foods");
            var enchantments = Load<Enchantment>(version, source, "enchantments");
            var effects = Load<Effect>(version, source, "effects");
            var blockCollisionShapes = LoadSimple<JObject>(version, source, "blockCollisionShapes");
            var attributes = Load<Attribute>(version, source, "attributes");
            
            /* DataPriWriter.WriteFoods(foods, target);
            DataPriWriter.WriteEnchantments(enchantments, target);
            DataPriWriter.WriteEffects(effects, target);
            DataPriWriter.WriteBlockShapes(blockCollisionShapes, target);
            DataPriWriter.WriteAttributes(attributes, target); */
            
            Console.WriteLine();
        }

        private static void Start(string version, string source, string target)
        {
            Console.WriteLine();
            var biomes = Load<Biome>(version, source, "biomes");
            var blockLoots = Load<BlockLoot>(version, source, "blockLoot");
            var blocks = Load<Block>(version, source, "blocks");
            var entity = Load<Entity>(version, source, "entities");
            var entityLoots = Load<EntityLoot>(version, source, "entityLoot");
            var items = Load<Item>(version, source, "items");
            var lang = LoadSimple<Dictionary<string, string>>(version, source, "language");
            var materials = LoadSimple<Dictionary<string, Dictionary<int, double>>>(version, source, "materials");
            var particles = Load<Particle>(version, source, "particles");
            var recipes = LoadSimple<Dictionary<int, Recipe[]>>(version, source, "recipes");

            DataPriWriter.WriteBlocks(blocks, blockLoots, target);
            DataPriWriter.WriteEntities(entity, entityLoots, target);
            DataPriWriter.WriteItems(items, target);
            DataPriWriter.WriteBiomes(biomes, target);
            DataPriWriter.WriteParticles(particles, target);
            
            Console.WriteLine();
        }

        private static string Download(string version, string source, string name)
        {
            const string repo = "https://raw.githubusercontent.com/PrismarineJS/minecraft-data";
            var url = $"{repo}/master/data/pc/{version}/{name}.json";
            var dest = Path.Combine(source, $"{name}.json");
            if (!File.Exists(dest))
            {
                CreateDir(dest);
                using var client = new WebClient();
                client.DownloadFile(url, dest);
            }
            return dest;
        }

        private static T[] Load<T>(string version, string source, string name)
        {
            var dest = Download(version, source, name);
            return ReadJsonFile<T>(dest);
        }

        private static T LoadSimple<T>(string version, string source, string name)
        {
            var dest = Download(version, source, name);
            return ReadSimpleJson<T>(dest);
        }

        private static T[] ReadJsonFile<T>(string file)
        {
            var txt = File.ReadAllText(file, Encoding.UTF8);
            var cfg = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.DeserializeObject<T[]>(txt, cfg);
            if (json == null)
            {
                throw new InvalidOperationException("Empty JSON!");
            }
            return json;
        }

        private static T ReadSimpleJson<T>(string file)
        {
            var txt = File.ReadAllText(file, Encoding.UTF8);
            var cfg = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.DeserializeObject<T>(txt, cfg);
            if (json == null)
            {
                throw new InvalidOperationException("Empty JSON!");
            }
            return json;
        }
    }
}