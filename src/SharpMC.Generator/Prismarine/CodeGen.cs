using System.Collections.Generic;
using System.IO;
using System.Text;

// ReSharper disable UseObjectOrCollectionInitializer

namespace SharpMC.Generator.Prismarine
{
    internal static class CodeGen
    {
        private const string Sp = "    ";

        public static void WriteDown((OneUnit c, OneUnit s) t, string target, string dir)
        {
            var clientId = t.c.Id;
            var serverId = t.s.Id;
            WriteDown(t.s, target, dir, clientId, serverId);
        }

        public static void WriteDown(OneUnit item, string target, string dir)
        {
            var clientId = item.Direction == "ToClient" ? item.Id : null;
            var serverId = item.Direction == "ToServer" ? item.Id : null;
            WriteDown(item, target, dir, clientId, serverId);
        }

        private static void WriteDown(OneUnit item, string target, string dir, string clientId, string serverId)
        {
            var nsp = $"{item.Namespace}.{dir}";
            var nspDir = nsp.Replace('.', Path.DirectorySeparatorChar);
            var className = item.Class;
            var outPath = Path.Combine(target, nspDir, $"{className}.cs");
            var outDir = Path.GetDirectoryName(outPath);
            Directory.CreateDirectory(outDir);
            var lines = new List<string>();
            lines.Add("using SharpMC.Network.Util;");
            lines.Add("using System;");
            lines.Add(string.Empty);
            lines.Add($"namespace {nsp}");
            lines.Add("{");
            var ext = new List<string>();
            ext.Add($"Packet<{className}>");
            var head = new List<string>();
            if (clientId != null)
            {
                const string ci = "ToClient";
                ext.Add($"I{ci}");
                head.Add(GetIdField(ci, clientId));
            }
            if (serverId != null)
            {
                const string si = "ToServer";
                ext.Add($"I{si}");
                head.Add(GetIdField(si, serverId));
            }
            var extStr = ext.Count == 0 ? string.Empty : $" : {string.Join(", ", ext)}";
            lines.Add($"{Sp}public class {className}{extStr}");
            lines.Add($"{Sp}{{");
            if (head.Count >= 1)
            {
                foreach (var m in head)
                    lines.Add(m);
                lines.Add(string.Empty);
            }
            var encodes = new List<string>();
            var decodes = new List<string>();
            foreach (var field in item.Fields)
            {
                var shortName = FieldMapping.GetShort(field.NativeType.ToString());
                lines.Add($"{Sp}{Sp}public {shortName} {field.Name} {{ get; set; }}");
                var write = MethodMapping.GetWriter(field.TypeName, field.Name);
                encodes.Add($"stream.{write};");
                var read = MethodMapping.GetReader(field.TypeName);
                decodes.Add($"{field.Name} = stream.{read};");
            }
            if (decodes.Count >= 0)
            {
                lines.Add(string.Empty);
                lines.Add($"{Sp}{Sp}public override void Decode(IMinecraftStream stream)");
                lines.Add($"{Sp}{Sp}{{");
                foreach (var decode in decodes)
                    lines.Add($"{Sp}{Sp}{Sp}{decode}");
                lines.Add($"{Sp}{Sp}}}");
            }
            if (encodes.Count >= 0)
            {
                lines.Add(string.Empty);
                lines.Add($"{Sp}{Sp}public override void Encode(IMinecraftStream stream)");
                lines.Add($"{Sp}{Sp}{{");
                foreach (var encode in encodes)
                    lines.Add($"{Sp}{Sp}{Sp}{encode}");
                lines.Add($"{Sp}{Sp}}}");
            }
            lines.Add($"{Sp}}}");
            lines.Add("}");
            File.WriteAllLines(outPath, lines, Encoding.UTF8);
        }

        private static string GetIdField(string itemDir, string itemId)
        {
            var idName = $"{itemDir.Replace("To", string.Empty)}Id";
            return $"{Sp}{Sp}public byte {idName} => {itemId};";
        }
    }
}