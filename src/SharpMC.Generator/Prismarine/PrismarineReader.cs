using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static SharpMC.Generator.Tools.Helpers;

#pragma warning disable CS0618

namespace SharpMC.Generator.Prismarine
{
    internal static class PrismarineReader
    {
        internal static void Start(string source, string target)
        {
            const string version = "1.18.2";
            Start(version, source, target);
        }

        private static void Start(string version, string source, string target)
        {
            const string repo = "https://raw.githubusercontent.com/PrismarineJS/minecraft-data";
            var url = $"{repo}/master/data/pc/{version}/protocol.json";
            Console.WriteLine();
            var dest = Path.Combine(source, "protocol.json");
            if (!File.Exists(dest))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                using var client = new WebClient();
                client.DownloadFile(url, dest);
            }
            ReadJsonFile(dest, target);
            Console.WriteLine();
        }

        private static void ReadJsonFile(string file, string target)
        {
            var txt = File.ReadAllText(file, Encoding.UTF8);
            var cfg = new JsonSerializerSettings();
            var json = JsonConvert.DeserializeObject<JObject>(txt, cfg);
            if (json == null)
            {
                throw new InvalidOperationException("Empty JSON!");
            }
            foreach (var pair in json)
            {
                if (pair.Key == "types")
                    continue;
                var pairName = pair.Key;
                pairName = ToTitleCase(pairName);
                Console.WriteLine($" * {pairName}");
                var client = pair.Value["toClient"].ToObject<JObject>();
                var c = ReadJsonStruct($"to_{nameof(client)}", client, pairName).ToArray();
                var cKeys = c.Select(ToKey).ToArray();
                var server = pair.Value["toServer"].ToObject<JObject>();
                var s = ReadJsonStruct($"to_{nameof(server)}", server, pairName).ToArray();
                var sKeys = s.Select(ToKey).ToArray();
                var cOnly = cKeys.Except(sKeys).ToArray();
                var sOnly = sKeys.Except(cKeys).ToArray();
                var cOnlyList = new List<OneUnit>(c.Where(y => cOnly.Contains(ToKey(y))).OrderBy(ToKey));
                var sOnlyList = new List<OneUnit>(s.Where(y => sOnly.Contains(ToKey(y))).OrderBy(ToKey));
                var cBothList = c.Except(cOnlyList).OrderBy(ToKey);
                var sBothList = s.Except(sOnlyList).OrderBy(ToKey);
                var xBothList = cBothList.Zip(sBothList).ToArray();
                if (xBothList.Length >= 1)
                {
                    Console.WriteLine("   # toBoth");
                    foreach (var item in xBothList)
                    {
                        Console.WriteLine($"     - {item}");
                        var first = Collect(item.First);
                        var second = Collect(item.Second);
                        if (first != second)
                        {
                            cOnlyList.Add(item.First);
                            sOnlyList.Add(item.Second);
                            continue;
                        }
                        CodeGen.WriteDown(item, target, "ToBoth");
                    }
                }
                if (cOnlyList.Count >= 1)
                {
                    Console.WriteLine("   # toClient");
                    foreach (var item in cOnlyList)
                    {
                        Console.WriteLine($"     - {item}");
                        CodeGen.WriteDown(item, target, "ToClient");
                    }
                }
                if (sOnlyList.Count >= 1)
                {
                    Console.WriteLine("   # toServer");
                    foreach (var item in sOnlyList)
                    {
                        Console.WriteLine($"     - {item}");
                        CodeGen.WriteDown(item, target, "ToServer");
                    }
                }
            }
        }

        private static string Collect(OneUnit x) => string.Join(", ", x.Fields.Select(ToKey));

        private static string ToKey(OneField x) => $"{x.TypeName}:{x.Name}";

        private static string ToKey(OneUnit x) => $"{x.Namespace}:{x.Class}";

        private static IEnumerable<OneUnit> ReadJsonStruct(string name, JObject obj, string category)
        {
            name = ToTitleCase(name);
            var types = obj["types"].ToObject<JObject>();
            var meta = types["packet"].ToObject<JArray>();
            var metaKey = meta[0].ToObject<string>();
            if (metaKey != "container")
                throw new InvalidOperationException("No meta key match!");
            var metaArray = meta[1].ToObject<JArray>();
            var mappinObj = metaArray[0]["type"].ToObject<JArray>()[1]["mappings"];
            var nameObj = mappinObj.ToObject<Dictionary<string, string>>();
            var fieldObj = metaArray[1]["type"].ToObject<JArray>()[1]["fields"];
            var paramsObj = fieldObj.ToObject<Dictionary<string, string>>();
            foreach (var item in types)
            {
                var itemName = item.Key;
                const string tmp = "packet_";
                if (!itemName.StartsWith(tmp))
                    continue;
                itemName = itemName.Replace(tmp, string.Empty);
                itemName = ToTitleCase(itemName);
                var foundKey = paramsObj.SingleOrDefault(s => s.Value == item.Key).Key;
                var foundId = nameObj.SingleOrDefault(s => s.Value == foundKey).Key;
                var nsp = $"{nameof(SharpMC)}.Network.Packets.{category}";
                var fields = item.Value.ToObject<JArray>()[1];
                var fRes = new List<OneField>();
                foreach (var field in fields)
                {
                    var fieldName = ToTitleCase(field["name"].ToObject<string>());
                    var fieldType = field["type"];
                    if (fieldType is JValue)
                    {
                        var simpleFiType = fieldType.ToObject<string>();
                        var nativeFiType = FieldMapping.GetNative(simpleFiType);
                        if (fieldName == itemName)
                            fieldName = $"_{fieldName}";
                        var f = new OneField
                        {
                            TypeName = simpleFiType, NativeType = nativeFiType, Name = fieldName
                        };
                        fRes.Add(f);
                        continue;
                    }
                    // TODO: Something complex ?!
                }
                yield return new OneUnit
                {
                    Id = foundId, Namespace = nsp, Direction = name, Class = itemName, Fields = fRes
                };
            }
        }
    }
}