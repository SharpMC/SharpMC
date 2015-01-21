using Craft.Net.Anvil;
using System.IO;
using MiNET.Utils;
using System.Net;
using System;

namespace SharpMCRewrite.Worlds
{
    public class ChunkColumn
    {
        public int X { get; set; }
        public int Y { get; set; }
        public byte[] BiomeId = ArrayOf<byte>.Create(256, 1);
        public int[] BiomeColor = ArrayOf<int>.Create(256, 1);

        public byte[] Blocks = new byte[16*16*256]; // two bytes per block (USHORT'S)
        //public NibbleArray Blocklight = new NibbleArray(16*16*256);
       // public NibbleArray Skylight = new NibbleArray(16*16*256);
        public byte[] Skylight = new byte[16*16*256];
        public byte[] Blocklight = new byte[16*16*256];

        private byte[] _cache = null;

        public ChunkColumn()
        {
            for (int i = 0; i < Skylight.Length; i ++)
                Skylight [i] = 0xff;

            for (int i = 0; i < BiomeColor.Length; i++)
                BiomeColor[i] = 8761930; // Grass color?
        }

        public ushort GetBlock(int x, int y, int z)
        {
            return BitConverter.ToUInt16(new byte[2] {Blocks[(x*2048) + (z*256) + y], Blocks[(x*2048) + (z*256) + (y + 1)]}, 0);
        }

        public void SetBlock(int x, int y, int z, ushort BlockID)
        {
            byte[] bID = BitConverter.GetBytes (BlockID);
            _cache = null;
            Blocks[(x*2048) + (z*256) + y] = bID[0];
            Blocks [(x * 2048) + (z * 256) + (y + 1)] = bID [1];
        }

        public void SetBlocklight(int x, int y, int z, byte data)
        {
            _cache = null;
            Blocklight[(x*2048) + (z*256) + y] = data;
        }

        public void SetSkylight(int x, int y, int z, byte data)
        {
            _cache = null;
            Skylight[(x*2048) + (z*256) + y] = data;
        }

        public byte[] GetBytes()
        {

            using (var stream = new MemoryStream ())
            {
                using (var writer = new NbtBinaryWriter (stream, true))
                {
                    writer.Write (IPAddress.HostToNetworkOrder (X));
                    writer.Write (IPAddress.HostToNetworkOrder (Y));
                    writer.Write (true);
                    writer.Write ((ushort)0xffff); // bitmap
                    writer.WriteVarInt (Blocks.Length + Skylight.Length + 33024 + 32768);

                    writer.Write (Blocks);

                    writer.Write (Blocklight);
                    writer.Write (Skylight);

                    writer.Write (BiomeId); //OK

                    writer.Flush ();
                    writer.Close ();
                }
                return stream.ToArray();
            }
        }
    }

    public static class ArrayOf<T> where T : new()
    {
        public static T[] Create(int size, T initialValue)
        {
            var array = new T[size];
            for (int i = 0; i < array.Length; i++)
                array[i] = initialValue;
            return array;
        }

        public static T[] Create(int size)
        {
            var array = new T[size];
            for (int i = 0; i < array.Length; i++)
                array[i] = new T();
            return array;
        }
    }
}

