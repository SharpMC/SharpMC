using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using static SharpMC.Generator.Tools.Helpers;

namespace SharpMC.Generator.Pakkit
{
    internal static class PakkitReader
    {
        private static void ReadAndWrite(string file, string outDir)
        {
            var txt = File.ReadAllText(file, Encoding.UTF8);
            var cfg = new JsonSerializerSettings();
            var array = JsonConvert.DeserializeObject<JArray>(txt, cfg);
            if (array == null)
            {
                throw new InvalidOperationException("Could not read!");
            }
            var count = array.Count;
            Console.WriteLine("   - {0} recorded messages found!", count);
            foreach (var item in array)
            {
                var packetValid = item["packetValid"].ToObject<bool>();
                if (!packetValid)
                    continue;
                var meta = item["meta"];
                var name = meta["name"].ToObject<string>();
                var state = meta["state"].ToObject<string>();
                var direction = item["direction"].ToObject<string>();
                var dir = ToTitleCase(direction.Replace("bound", string.Empty));
                var sta = ToTitleCase(state);
                var className = ToTitleCase(name);
                var nsp = $"{nameof(SharpMC)}.Network.Packets.{sta}.{dir}";
                var nspDir = nsp.Replace('.', Path.DirectorySeparatorChar);
                var outPath = Path.Combine(outDir, nspDir, $"{className}.cs");
                if (File.Exists(outPath))
                    continue;
                var hexIdString = item["hexIdString"].ToObject<string>();
                var data = item["data"];
                var json = JsonConvert.SerializeObject(data, cfg);
                var schema = JsonSchema.FromSampleJson(json);
                schema.Title = className;
                var gen = new CSharpGeneratorSettings
                {
                    Namespace = nsp, ClassStyle = CSharpClassStyle.Poco
                };
                var generator = new CSharpGenerator(schema, gen);
                var code = generator.GenerateFile();
                var fqn = $"{nsp}.{className}";
                var head = $"// {fqn} | PacketId = {hexIdString}";
                code = head + Environment.NewLine + code;
                Console.WriteLine("   => {0}", outPath);
                var parent = Path.GetDirectoryName(outPath);
                Directory.CreateDirectory(parent);
                File.WriteAllText(outPath, code);
            }
        }
        
        public static void Start(string source, string target)
        {
            Console.WriteLine();
            const string p = "*.pakkit-json";
            const SearchOption o = SearchOption.AllDirectories;
            var files = Directory.GetFiles(source, p, o);
            foreach (var file in files)
            {
                Console.WriteLine(" * {0}", file);
                ReadAndWrite(file, target);
            }
            Console.WriteLine();
        }
    }
}