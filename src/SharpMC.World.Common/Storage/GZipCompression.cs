using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using SharpMC.World.API.Storage;

namespace SharpMC.World.Common.Storage
{
    public class GZipCompression : ICompression
    {
        public byte[] Compress(byte[] input)
        {
            using var output = new MemoryStream();
            using (var zip = new GZipStream(output, CompressionMode.Compress))
            {
                zip.Write(input, 0, input.Length);
            }
            return output.ToArray();
        }

        public byte[] Decompress(byte[] input)
        {
            using var output = new MemoryStream(input);
            using var zip = new GZipStream(output, CompressionMode.Decompress);
            var bytes = new List<byte>();
            var b = zip.ReadByte();
            while (b != -1)
            {
                bytes.Add((byte) b);
                b = zip.ReadByte();
            }
            return bytes.ToArray();
        }
    }
}