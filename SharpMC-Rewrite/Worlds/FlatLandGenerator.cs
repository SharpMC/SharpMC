using System;
using System.Collections.Generic;
using System.Linq;
using MiNET.Utils;
using System.Diagnostics;
using MiNET.Worlds;
using System.IO;
using System.Reflection;
using Craft.Net.Anvil;

namespace SharpMCRewrite.Worlds
{
    public class FlatLandGenerator : IWorldProvider
    {
        public Dictionary<Tuple<int,int>, ChunkColumn> _chunkCache = new Dictionary<Tuple<int,int>, ChunkColumn>();
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

        public IEnumerable<ChunkColumn> GenerateChunks(int _viewDistance, double playerX, double playerZ, Dictionary<Tuple<int,int>, ChunkColumn> chunksUsed)
        {
            lock (chunksUsed)
            {
                Dictionary<Tuple<int, int>, double> newOrders = new Dictionary<Tuple<int,int>, double>();
                double radiusSquared = _viewDistance/Math.PI;
                double radius = Math.Ceiling(Math.Sqrt(radiusSquared));
                double centerX = Math.Floor((playerX)/16);
                double centerZ = Math.Floor((playerZ)/16);

                for (double x = -radius; x <= radius; ++x)
                {
                    for (double z = -radius; z <= radius; ++z)
                    {
                        var distance = (x*x) + (z*z);
                        if (distance > radiusSquared)
                        {
                            continue;
                        }
                        int chunkX = (int)Math.Floor(x + centerX);
                        int chunkZ = (int)Math.Floor(z + centerZ);

                        Tuple<int,int> index = new Tuple<int, int> ((int)chunkX, (int)chunkZ);
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

                    int x = pair.Key.Item1;
                    int z = pair.Key.Item2;

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
            if (_chunkCache.ContainsKey (new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z)))
            {
                ChunkColumn c;
                if (_chunkCache.TryGetValue (new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), out c))
                {
                    Debug.WriteLine ("Chunk " + chunkCoordinates.X + ":" + chunkCoordinates.Z  + " was already generated!");
                    return c;
                }
            }

            if (File.Exists ((Folder + "/" + chunkCoordinates.X + "." + chunkCoordinates.Z + ".cfile")))
            {
                ChunkColumn cd = LoadChunk (chunkCoordinates.X, chunkCoordinates.Z);
                _chunkCache.Add (new Tuple<int, int>(cd.X, cd.Z), cd);
                return cd;
            } 

            Debug.WriteLine ("ChunkFile not found, generating...");
            var generator = new FlatLandGenerator(Folder);

            var chunk = new ChunkColumn();
            chunk.X = chunkCoordinates.X;
            chunk.Z = chunkCoordinates.Z;
            generator.PopulateChunk(chunk);
            _chunkCache.Add(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), chunk);

            return chunk;
        }

        public Vector3 GetSpawnPoint()
        {
            return new Vector3(1, 1, 1);
        }

        public void PopulateChunk(ChunkColumn chunk)
        {
            var random = new CryptoRandom();
            var blocks = new ushort[16 * 16 * 256];
            int Last = 0;
            for (int x = 0; x < 256; x ++)
            {
                blocks[x] = (7 << 4) | 0; // Bedrock
                Last++;
            }

            for (int x = Last; x < (256 * 3); x ++)
            {
                blocks[x] = (3 << 4) | 0; // Dirt?
                Last++;
            }

            for (int x = Last; x < (256 * 4); x ++)
            {
                blocks[x] = (2 << 4) | 0; // Grass??
                Last++;
            }

            chunk.Blocks = blocks;
        }

        public void SaveChunks (string folder)
        {
            foreach (var i in _chunkCache)
            {
                File.WriteAllBytes (folder + "/" + i.Value.X + "." + i.Value.Z + ".cfile", Globals.Compress (i.Value.Export()));
            }
        }

        public ChunkColumn LoadChunk (int x, int z)
        {
                byte[] u = Globals.Decompress(File.ReadAllBytes (Folder + "/" + x + "." + z + ".cfile"));
                MSGBuffer reader = new MSGBuffer (u);

                int BlockLength = reader.ReadInt ();
                ushort[] Block = reader.ReadUShortLocal (BlockLength);

                int SkyLength = reader.ReadInt ();
                byte[] Skylight = reader.Read (SkyLength);

                int LightLength = reader.ReadInt ();
                byte[] Blocklight = reader.Read (LightLength);

                int BiomeIDLength = reader.ReadInt ();
                byte[] BiomeID = reader.Read (BiomeIDLength);

                ChunkColumn CC = new ChunkColumn ();
                CC.Blocks = Block;
                CC.Blocklight.Data = Blocklight;
                CC.Skylight.Data = Skylight;
                CC.BiomeId = BiomeID;
                CC.X = x;
                CC.Z = z;
                Debug.WriteLine ("We should have loaded " + x + ", " + z);
                return CC;
        }

        public void SetBlock(Vector3 cords, ushort blockID)
        {
            ChunkColumn c;
            if (_chunkCache.TryGetValue(new Tuple<int, int>((int)Math.Floor(cords.X / 16), (int)Math.Floor(cords.Z / 16)), out c))
            {
                c.SetBlock (((int)Math.Floor(cords.X) & 0x0f), ((int)Math.Floor(cords.Y) & 0x7f), ((int)Math.Floor(cords.Z) & 0x0f), blockID);
                return;
            }
            throw new Exception ("No chunk found!");
        }
    }
}

