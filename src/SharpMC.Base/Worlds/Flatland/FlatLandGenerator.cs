// Distributed under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using SharpMC.Blocks;
using SharpMC.Core.Utils;
using SharpMC.Util;
using SharpMC.Worlds;
using ChunkColumn = SharpMC.World.ChunkColumn;

namespace SharpMC.Core.Worlds.Flatland
{
	public class FlatLandGenerator : WorldProvider
	{
		private readonly string _folder = "world";
		public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();

		public FlatLandGenerator(string folder)
		{
			_folder = folder;
			IsCaching = true;

			if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
		}

		public override bool IsCaching { get; set; }

		public override void Initialize()
		{
		}

		private string GetChunkHash(double chunkX, double chunkZ)
		{
			return string.Format("{0}:{1}", chunkX, chunkZ);
		}

		public override ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
		{
			if (ChunkCache.ContainsKey(chunkCoordinates.ToTuple()))
			{
				ChunkColumn c;
				if (ChunkCache.TryGetValue(chunkCoordinates.ToTuple(), out c))
				{
					Debug.WriteLine("Chunk " + chunkCoordinates.X + ":" + chunkCoordinates.Y + " was already generated!");
					return c;
				}
			}

			if (File.Exists((_folder + "/" + chunkCoordinates.X + "." + chunkCoordinates.Y + ".cfile")))
			{
				var cd = LoadChunk((int) chunkCoordinates.X, (int) chunkCoordinates.Y);
				if (!ChunkCache.ContainsKey(new Tuple<int, int>(cd.X, cd.Z)))
					ChunkCache.Add(new Tuple<int, int>(cd.X, cd.Z), cd);
				return cd;
			}

			Debug.WriteLine("ChunkFile not found, generating...");

			var chunk = new ChunkColumn {X = (int) chunkCoordinates.X, Z = (int) chunkCoordinates.Y};
			var h = PopulateChunk(chunk);

			chunk.SetBlock(0, h + 1, 0, new Block(7));
			chunk.SetBlock(1, h + 1, 0, new Block(41));
			chunk.SetBlock(2, h + 1, 0, new Block(41));
			chunk.SetBlock(3, h + 1, 0, new Block(41));
			chunk.SetBlock(3, h + 1, 0, new Block(41));

			if (!ChunkCache.ContainsKey(chunkCoordinates.ToTuple()))
				ChunkCache.Add(chunkCoordinates.ToTuple(), chunk);

			return chunk;
		}

		public override Vector3 GetSpawnPoint()
		{
			return new Vector3(1, 1, 1);
		}

		public int PopulateChunk(ChunkColumn chunk)
		{
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					for (var y = 0; y < 4; y++)
					{
						if (y == 0)
						{
							chunk.SetBlock(x, y, z, new BlockBedrock());
						}


						if (y == 1 || y == 2)
						{
							chunk.SetBlock(x, y, z, new Block(3));
						}

						if (y == 3)
						{
							chunk.SetBlock(x, y, z, new BlockGrass());
						}

						if (chunk.X == 1 && chunk.Z == 1)
						{
							if (y == 1 || y == 2 || y == 3)
							{
								chunk.SetBlock(x, y, z, new BlockFlowingLava());
							}
						}

						if (chunk.X == 3 && chunk.Z == 1)
						{
							if (y == 3)
							{
								chunk.SetBlock(x, y, z, new BlockFlowingWater());
							}
						}
					}
				}
			}

			return 4;
		}

		public override void SaveChunks(string folder)
		{
			lock (ChunkCache)
			{
				foreach (var i in ChunkCache.Values.ToArray())
				{
					File.WriteAllBytes(_folder + "/" + i.X + "." + i.Z + ".cfile", Globals.Compress(i.Export()));
				}
			}
		}

		public override ChunkColumn LoadChunk(int x, int z)
		{
			var u = Globals.Decompress(File.ReadAllBytes(_folder + "/" + x + "." + z + ".cfile"));
			var reader = new DataBuffer(u);

			var blockLength = reader.ReadInt();
			var block = reader.ReadUShortLocal(blockLength);

			var metalength = reader.ReadInt();
			var blockmeta = reader.ReadUShortLocal(metalength);

			var skyLength = reader.ReadInt();
			var skylight = reader.Read(skyLength);

			var lightLength = reader.ReadInt();
			var blocklight = reader.Read(lightLength);

			var biomeIdLength = reader.ReadInt();
			var biomeId = reader.Read(biomeIdLength);

			var cc = new ChunkColumn
			{
				Blocks = block,
				Metadata = blockmeta,
				Blocklight = {Data = blocklight},
				Skylight = {Data = skylight},
				BiomeId = biomeId,
				X = x,
				Z = z
			};
			Debug.WriteLine("We should have loaded " + x + ", " + z);
			return cc;
		}
	}
}