using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace SharpMC.Generator
{
    internal static class PakkitReader
    {
        public static void ReadAndWrite(string file, string outDir)
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
                Console.WriteLine("   => {0}", outPath);
                var parent = Path.GetDirectoryName(outPath);
                Directory.CreateDirectory(parent);
                File.WriteAllText(outPath, code);
            }
        }

        private static string? ToTitleCase(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;
            var bld = new StringBuilder();
            foreach (var part in text.Split('_'))
            {
                var big = part.Substring(0, 1)
                    .ToUpperInvariant() + part.Substring(1);
                bld.Append(big);
            }
            var txt = bld.ToString();
            return txt;
        }
    }
}