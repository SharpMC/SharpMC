using System;
using System.Collections.Generic;
using System.Linq;
using MiNET.Utils;
using System.Diagnostics;
using MiNET.Worlds;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using Craft.Net.Anvil;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.NET;

namespace SharpMCRewrite.Worlds
{
    public class FlatLandGenerator : IWorldProvider
    {
        public Dictionary<Tuple<int,int>, ChunkColumn> _chunkCache = new Dictionary<Tuple<int,int>, ChunkColumn>();
        public override bool IsCaching { get; set; }
        private string Folder = "world";

        public FlatLandGenerator(string folder)
        {
            Folder = folder;
            IsCaching = true;
        }

		public override void Initialize()
        {
        }

		public override ChunkColumn GetChunk(int x, int z)
	    {
		    foreach (var ch in _chunkCache)
		    {
			    if (ch.Key.Item1 == x && ch.Key.Item2 == z)
			    {
				    return ch.Value;
			    }
		    }
		    throw new Exception("We couldn't find the chunk.");
	    }

		public override IEnumerable<ChunkColumn> GenerateChunks(int _viewDistance, double playerX, double playerZ, Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed, ClientWrapper wrapper)
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
		               // new Networking.Packages.ChunkData(wrapper, new MSGBuffer(wrapper))
		               // {
			           //     Chunk = new ChunkColumn() {X = chunkKey.Item1, Z = chunkKey.Item2}
		               // }.Write(); //Unload the chunk client on the client side.

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

        public override ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
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
	            ChunkColumn cd = LoadChunk(chunkCoordinates.X, chunkCoordinates.Z);
				if (!_chunkCache.ContainsKey(new Tuple<int, int>(cd.X, cd.Z)))
                _chunkCache.Add (new Tuple<int, int>(cd.X, cd.Z), cd);
                return cd;
            } 

            Debug.WriteLine ("ChunkFile not found, generating...");

	        var chunk = new ChunkColumn {X = chunkCoordinates.X, Z = chunkCoordinates.Z};
	        int h = PopulateChunk(chunk);

			chunk.SetBlock(0, h + 1, 0, new Block(7));
			chunk.SetBlock(1, h + 1, 0, new Block(41));
			chunk.SetBlock(2, h + 1, 0, new Block(41));
			chunk.SetBlock(3, h + 1, 0, new Block(41));
			chunk.SetBlock(3, h + 1, 0, new Block(41));

            _chunkCache.Add(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), chunk);

            return chunk;
        }

		public override Vector3 GetSpawnPoint()
        {
            return new Vector3(1, 1, 1);
        }

        public int PopulateChunk(ChunkColumn chunk)
        {
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
	        return 4;
        }

		public override void SaveChunks(string folder)
        {
            foreach (var i in _chunkCache)
            {
                File.WriteAllBytes (folder + "/" + i.Value.X + "." + i.Value.Z + ".cfile", Globals.Compress (i.Value.Export()));
            }
        }

		public override ChunkColumn LoadChunk(int x, int z)
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

		public override void SetBlock(Block block, Level level, bool broadcast)
        {
            ChunkColumn c;
			if (!_chunkCache.TryGetValue(new Tuple<int, int>(block.Coordinates.X / 16, block.Coordinates.Z / 16), out c)) throw new Exception("No chunk found!");

			c.SetBlock((block.Coordinates.X & 0x0f), (block.Coordinates.Y & 0x7f), (block.Coordinates.Z & 0x0f), block);
			if (!broadcast) return;

	        foreach (var player in level.OnlinePlayers)
	        {
		        new Networking.Packages.BlockChange(player.Wrapper, new MSGBuffer(player.Wrapper))
		        {
			        Block = block,
					Location = block.Coordinates				
		        }.Write();
	        }
        }
    }
}

