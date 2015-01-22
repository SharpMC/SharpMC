using System;
using System.Collections.Generic;
using System.Linq;
using MiNET.Utils;
using System.Diagnostics;
using MiNET.Worlds;
using System.IO;
using System.Reflection;

namespace SharpMCRewrite.Worlds
{
    public class FlatLandGenerator : IWorldProvider
    {
        public Dictionary<string, ChunkColumn> _chunkCache = new Dictionary<string, ChunkColumn>();
        public bool IsCaching { get; private set; }
        private string Folder = "world";

        public FlatLandGenerator(string folder)
        {
            Folder = folder;
            IsCaching = true;
        }

        public void Initialize()
        {
        }

        public IEnumerable<ChunkColumn> GenerateChunks(int _viewDistance, double playerX, double playerZ, Dictionary<string, ChunkColumn> chunksUsed)
        {
            lock (chunksUsed)
            {
                Dictionary<string, double> newOrders = new Dictionary<string, double>();
                double radiusSquared = _viewDistance/Math.PI;
                double radius = Math.Ceiling(Math.Sqrt(radiusSquared));
                var centerX = ((int)playerX)/16;
                var centerZ = ((int)playerZ)/16;

                for (double x = -radius; x <= radius; ++x)
                {
                    for (double z = -radius; z <= radius; ++z)
                    {
                        var distance = (x*x) + (z*z);
                        if (distance > radiusSquared)
                        {
                            continue;
                        }
                        var chunkX = x + centerX;
                        var chunkZ = z + centerZ;
                        string index = GetChunkHash(chunkX, chunkZ);
                        newOrders[index] = distance;
                    }
                }

                if (newOrders.Count > _viewDistance)
                {
                    foreach (var pair in newOrders.OrderByDescending(pair => pair.Value))
                    {
                        if (newOrders.Count <= _viewDistance) break;
                        newOrders.Remove(pair.Key);
                    }
                }


                foreach (var chunkKey in chunksUsed.Keys.ToArray())
                {
                    if (!newOrders.ContainsKey(chunkKey))
                    {
                        chunksUsed.Remove(chunkKey);
                    }
                }

                Stopwatch stopwatch = new Stopwatch();
                long avarageLoadTime = -1;
                foreach (var pair in newOrders.OrderBy(pair => pair.Value))
                {
                    if (chunksUsed.ContainsKey(pair.Key)) continue;

                    stopwatch.Restart();

                    int x = Int32.Parse(pair.Key.Split(new[] {':'})[0]);
                    int z = Int32.Parse(pair.Key.Split(new[] {':'})[1]);

                    ChunkColumn chunk = GenerateChunkColumn(new Vector2(x, z));
                    chunksUsed.Add(pair.Key, chunk);

                    long elapsed = stopwatch.ElapsedMilliseconds;
                    if (avarageLoadTime == -1) avarageLoadTime = elapsed;
                    else avarageLoadTime = (avarageLoadTime + elapsed)/2;
                    Debug.WriteLine("Chunk {2} generated in: {0} ms (Avarage: {1} ms)", elapsed, avarageLoadTime, pair.Key);

                    yield return chunk;
                }
            }
        }
            
        private string GetChunkHash(double chunkX, double chunkZ)
        {
            return string.Format("{0}:{1}", chunkX, chunkZ);
        }

        public ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
        {
            if (_chunkCache.ContainsKey (chunkCoordinates.X + ":" + chunkCoordinates.Y))
            {
                ChunkColumn c;
                if (_chunkCache.TryGetValue (chunkCoordinates.X + ":" + chunkCoordinates.Y, out c))
                {
                    Debug.WriteLine ("Chunk " + chunkCoordinates.X + ":" + chunkCoordinates.Y  + " was already generated!");
                    return c;
                }
            }

            ChunkColumn cd = LoadChunk (chunkCoordinates.X, chunkCoordinates.Y);
            if (cd.Blocks.Length != 0)
            {
                _chunkCache.Add (cd.X + ":" + cd.Y, (ChunkColumn)cd);
                return (ChunkColumn)cd;
            } 

            Debug.WriteLine ("ChunkFile not found, generating...");
            var generator = new FlatLandGenerator(Folder);

            var chunk = new ChunkColumn();
            chunk.X = chunkCoordinates.X;
            chunk.Y = chunkCoordinates.Y;
            generator.PopulateChunk(chunk);
            _chunkCache.Add(chunkCoordinates.X + ":" + chunkCoordinates.Y, chunk);

            return chunk;
        }

        public Vector3 GetSpawnPoint()
        {
            return new Vector3(1, 1, 1);
        }

        public void PopulateChunk(ChunkColumn chunk)
        {
            var random = new CryptoRandom();
            var blocks = new byte[16 * 16 * 256];
            int Last = 0;

            //for (int i = 0; i < (256 * 2); i += 2)
            for (int i = 0; i < blocks.Length; i += 2)
            {
                if (i < (256 * 2))
                {
                    byte[] Bedrock = BitConverter.GetBytes ((ushort)(7 << 4) | 0);
                    blocks [i] = Bedrock [0];
                    blocks [i + 1] = Bedrock [1];
                }

                if (i >= (256 * 2) && i < (256 * 4))
                {
                    byte[] Grass = BitConverter.GetBytes ((ushort)(3 << 4) | 0);
                    blocks [i] = Grass [0];
                    blocks [i + 1] = Grass [1];
                }

                if (i >= (256 * 4) && i < (256 * 6))
                {
                    byte[] Grass = BitConverter.GetBytes ((ushort)(3 << 4) | 0);
                    blocks [i] = Grass [0];
                    blocks [i + 1] = Grass [1];
                }

                if (i >= (256 * 6) && i < (256 * 8))
                {
                    byte[] Grass = BitConverter.GetBytes ((ushort)(2 << 4) | 0);
                    blocks [i] = Grass [0];
                    blocks [i + 1] = Grass [1];
                }
            }

            chunk.Blocks = blocks.ToArray();
        }

        public void SaveChunks (string folder)
        {
            foreach (var i in _chunkCache)
            {
                File.WriteAllBytes (folder + "/" + i.Value.X + "." + i.Value.Y + ".cfile", i.Value.GetBytes());
            }
        }

        public ChunkColumn LoadChunk (int x, int y)
        {
            if (File.Exists ((Folder + "/" + x + "." + y + ".cfile")))
            {
                byte[] u = File.ReadAllBytes (Folder + "/" + x + "." + y + ".cfile");
                MSGBuffer reader = new MSGBuffer (u);
                int X = reader.ReadInt ();
                int Y = reader.ReadInt ();
                bool t = reader.ReadBool ();
                int Length = reader.ReadVarInt ();
                byte[] Block = reader.Read (16 * 16 * 256);
                byte[] BlockLight = reader.Read (16 * 16 * 256);
                byte[] SkyLight = reader.Read (16 * 16 * 256);
                byte[] BiomeID = reader.Read (256);

                ChunkColumn CC = new ChunkColumn ();
                CC.Blocks = Block;
                CC.Blocklight = BlockLight;
                CC.Skylight = SkyLight;
                CC.BiomeId = BiomeID;
                CC.X = X;
                CC.Y = Y;
                Debug.WriteLine ("We should have loaded " + X + ", " + Y);
                return CC;
            }
            else
            {
                Debug.WriteLine ("We couldn't find the file :|");
                return new ChunkColumn() { Blocks = new byte[0] {  } };
            }
        }
    }
}

