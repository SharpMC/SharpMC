using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SharpMC.Network.Packets;
using SharpMC.Network.Util;

namespace SharpMC.Network.Test
{
    internal static class TestHelper
    {
        internal static T Read<T>(byte[] data, out int packetId)
            where T : Packet, new()
        {
            using var ms = new MemoryStream(data);
            using var mc = new MinecraftStream(ms);
            packetId = mc.ReadVarInt();
            var packet = new T();
            packet.Decode(mc);
            return packet;
        }

        internal static byte[] Write(Packet packet, int? packetId = null)
        {
            using var ms = new MemoryStream();
            using var mc = new MinecraftStream(ms);
            var id = packetId ?? packet.PacketId;
            mc.WriteVarInt(id);
            packet.Encode(mc);
            return ms.ToArray();
        }

        public static string ToJson(Packet packet)
        {
            var cfg = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = {new StringEnumConverter()}
            };
            return JsonConvert.SerializeObject(packet, cfg);
        }

        public static string ToJson(byte[] bytes)
        {
            return JsonConvert.SerializeObject(bytes);
        }

        public static void WriteBytes(string prefix, byte[] expected, byte[] actual)
        {
            File.WriteAllBytes($"{prefix}_e.bin", expected);
            File.WriteAllBytes($"{prefix}_a.bin", actual);
        }

        public static void WriteTexts(string prefix, string actual)
        {
            File.WriteAllText($"{prefix}_a.json", actual, Encoding.UTF8);
        }

        public static byte[] GetBytes(int count, byte num, params byte[] suffix)
            => Enumerable.Range(1, count)
                .Select(_ => num)
                .Concat(suffix)
                .ToArray();
    }
}