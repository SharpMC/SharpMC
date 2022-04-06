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
using SharpMC.Core.Blocks;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds.Standard;
using SharpMC.Core.Worlds.Standard.BiomeSystem;

namespace SharpMC.Core.Worlds.Nether
{
	internal class NetherWorldProvider : WorldProvider
	{
		//World Tweaking settings
		//These settings can be tweaked to changed the look of the terrain.

		private const double BottomOffset = 96.0; //Old value: 96.0  || Changes the offset from y level 0
		private const double BottomsMagnitude = 32.0; //Old value: 32.0
		private const double Groundscale = 256.0; //Old value: 256.0   || Changes the scale of the ground.
		private const double BottomsFrequency = 2.2; //Original 0.5
		private const double BottomsAmplitude = 0.5; //Original 0.5
		private const double TopOffset = 32.0;
		private const double TopMagnitude = 32.0; //Old value: 32.0
		private const double Topscale = 256.0; //Old value: 256.0   || Changes the scale of the ground.
		private const double TopFrequency = 2.2; //Original 0.5
		private const double TopAmplitude = 0.5; //Original 0.5
		private static readonly Random Getrandom = new Random();
		private static readonly object SyncLock = new object();
		public static int WaterLevel = 82;
		private readonly CaveGenerator _cavegen = new CaveGenerator(ServerSettings.Seed.GetHashCode());
		private readonly string _folder;
		public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();

		public NetherWorldProvider(string folder)
		{
			_folder = folder;
			IsCaching = true;

			if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
		}

		public override sealed bool IsCaching { get; set; }

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

		public override ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
		{
			ChunkColumn c;
			if (ChunkCache.TryGetValue(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), out c)) return c;

			if (File.Exists((_folder + "/" + chunkCoordinates.X + "." + chunkCoordinates.Z + ".cfile")))
			{
				var cd = LoadChunk(chunkCoordinates.X, chunkCoordinates.Z);
				lock (ChunkCache)
				{
					if (!ChunkCache.ContainsKey(new Tuple<int, int>(cd.X, cd.Z)))
						ChunkCache.Add(new Tuple<int, int>(cd.X, cd.Z), cd);
				}
				return cd;
			}

			var chunk = new ChunkColumn {X = chunkCoordinates.X, Z = chunkCoordinates.Z};
			PopulateChunk(chunk);

			if (!ChunkCache.ContainsKey(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z)))
				ChunkCache.Add(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), chunk);

			return chunk;
		}

		public static int GetRandomNumber(int min, int max)
		{
			lock (SyncLock)
			{
				return Getrandom.Next(min, max);
			}
		}

		private void PopulateChunk(ChunkColumn chunk)
		{
			var bottom = new SimplexOctaveGenerator(ServerSettings.Seed.GetHashCode(), 8);
			var top = new SimplexOctaveGenerator(ServerSettings.Seed.GetHashCode(), 8);
			bottom.SetScale(1/Groundscale);
			top.SetScale(1/Topscale);

			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					float ox = x + chunk.X*16;
					float oz = z + chunk.Z*16;

					var bottomHeight =
						(int) ((bottom.Noise(ox, oz, BottomsFrequency, BottomsAmplitude)*BottomsMagnitude) + BottomOffset);
					var topHeight = (int) ((top.Noise(ox, oz, TopFrequency, TopAmplitude)*TopMagnitude) + TopOffset);

					for (var y = 0; y < 256; y++)
					{
						if (y == 0 || y == 255)
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(7));
							continue;
						}

						if (y < bottomHeight)
						{
							chunk.SetBlock(x, y, z, new BlockNetherrack());
						}

						if (y < topHeight)
						{
							chunk.SetBlock(x, 256 - y, z, new BlockNetherrack());
							if (GetRandomNumber(1, 50) == 25)
							{
								chunk.SetBlock(x, 256 - (y + 1), z, new Block(89));
							}
						}
					}

					//Turn the blocks ontop into the correct material
					for (var y = bottomHeight; y < 254; y++)
					{
						if (chunk.GetBlock(x, y + 1, z) == 0 && chunk.GetBlock(x, y, z) == 1)
						{
							chunk.SetBlock(x, y, z, new BlockNetherrack());

							chunk.SetBlock(x, y - 1, z, new BlockNetherrack());
							chunk.SetBlock(x, y - 2, z, new BlockNetherrack());
						}
					}
				}
			}

			new NetherLavaDecorator().Decorate(chunk, new PlainsBiome());
		}

		/*public override void SetBlock(Block block, Level level, bool broadcast)
		{
			ChunkColumn c;
			lock (ChunkCache)
			{
				if (
					!ChunkCache.TryGetValue(new Tuple<int, int>((int) block.Coordinates.X >> 4, (int) block.Coordinates.Z >> 4), out c))
					throw new Exception("No chunk found!");
			}

			c.SetBlock(((int) block.Coordinates.X & 0x0f), ((int) block.Coordinates.Y & 0x7f), ((int) block.Coordinates.Z & 0x0f),
				block);
			if (!broadcast) return;

			BlockChange.Broadcast(block, level);
		}*/

		public override Vector3 GetSpawnPoint()
		{
			return new Vector3(0, 82, 0);
		}

		public override void ClearCache()
		{
			lock (ChunkCache)
			{
				ChunkCache.Clear();
			}
		}
	}
}