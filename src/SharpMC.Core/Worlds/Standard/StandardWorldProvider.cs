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
using SharpMC.Core.Entity;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds.Standard.BiomeSystem;
using SharpMC.Core.Worlds.Standard.Decorators;

namespace SharpMC.Core.Worlds.Standard
{
	internal class StandardWorldProvider : WorldProvider
	{
		//World Tweaking settings
		//These settings can be tweaked to changed the look of the terrain.

		private const double OverhangOffset = 32.0; //Old value: 32.0 || Changes the offset from the bottom ground.
		private const double BottomOffset = 42.0; //Old value: 96.0  || Changes the offset from y level 0
		private const double OverhangsMagnitude = 16.0; //Old value: 16.0
		private const double BottomsMagnitude = 32.0; //Old value: 32.0
		private const double OverhangScale = 128.0; //Old value: 128.0 || Changes the scale of the overhang.
		private const double Groundscale = 256.0; //Old value: 256.0   || Changes the scale of the ground.
		private const double Threshold = 0.1; //Old value: 0.0 || Cool value: -0.3 hehehe
		private const double BottomsFrequency = 0.5; //Original 0.5
		private const double BottomsAmplitude = 0.5; //Original 0.5
		private const double OverhangFrequency = 0.5; //Original 0.5
		private const double OverhangAmplitude = 0.5; //Original 0.5
		private const bool EnableOverhang = true; //Enable overhang?
		private static readonly Random Getrandom = new Random();
		private static readonly object SyncLock = new object();
		public static int WaterLevel = 72;
		private readonly BiomeManager _biomeManager;
		private readonly CaveGenerator _cavegen = new CaveGenerator(ServerSettings.Seed.GetHashCode());
		private readonly string _folder;
		public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();

		public StandardWorldProvider(string folder)
		{
			_folder = folder;
			IsCaching = true;
			_biomeManager = new BiomeManager(ServerSettings.Seed.GetHashCode());
			//	_biomeManager.AddBiomeType(new OceanBiome()); //Not adding until we fixed the transitions :(
			_biomeManager.AddBiomeType(new FlowerForestBiome());
			_biomeManager.AddBiomeType(new ForestBiome());
			_biomeManager.AddBiomeType(new BirchForestBiome());
			_biomeManager.AddBiomeType(new PlainsBiome());
			_biomeManager.AddBiomeType(new DesertBiome());
			_biomeManager.AddBiomeType(new SunFlowerPlainsBiome());
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
					if (i.IsDirty)
					{
						SaveChunk(i);
					}
				}
			}
		}

		private bool SaveChunk(ChunkColumn chunk)
		{
			File.WriteAllBytes(_folder + "/" + chunk.X + "." + chunk.Z + ".cfile", Globals.Compress(chunk.Export()));
			return true;
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
			var overhang = new SimplexOctaveGenerator(ServerSettings.Seed.GetHashCode(), 8);
			overhang.SetScale(1/OverhangScale);
			bottom.SetScale(1/Groundscale);


			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					float ox = x + chunk.X*16;
					float oz = z + chunk.Z*16;

					var cBiome = _biomeManager.GetBiome((int) ox, (int) oz);
					chunk.BiomeId[x*16 + z] = cBiome.MinecraftBiomeId;

					var bottomHeight =
						(int)
							((bottom.Noise(ox, oz, BottomsFrequency, BottomsAmplitude)*BottomsMagnitude) + BottomOffset + cBiome.BaseHeight);

					var maxHeight =
						(int)
							((overhang.Noise(ox, oz, OverhangFrequency, OverhangAmplitude)*OverhangsMagnitude) + bottomHeight +
							 OverhangOffset);
					maxHeight = Math.Max(1, maxHeight);

					for (var y = 0; y < maxHeight && y < 256; y++)
					{
						if (y == 0)
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(7));
							continue;
						}

						if (y > bottomHeight)
						{
							//part where we do the overhangs
							if (EnableOverhang)
							{
								var density = overhang.Noise(ox, y, oz, OverhangFrequency, OverhangAmplitude);
								if (density > Threshold) chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(1));
							}
						}
						else
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(1));
						}
					}

					//Turn the blocks ontop into the correct material
					for (var y = 0; y < 256; y++)
					{
						if (chunk.GetBlock(x, y + 1, z) == 0 && chunk.GetBlock(x, y, z) == 1)
						{
							chunk.SetBlock(x, y, z, cBiome.TopBlock);

							chunk.SetBlock(x, y - 1, z, cBiome.Filling);
							chunk.SetBlock(x, y - 2, z, cBiome.Filling);
						}
					}

					foreach (var decorator in cBiome.Decorators)
					{
						decorator.Decorate(chunk, cBiome, x, z);
					}
					new OreDecorator().Decorate(chunk, cBiome, x, z); //Ores :)
					new BedrockDecorator().Decorate(chunk, cBiome, x, z); //Random bedrock :)
				}
			}

			new WaterDecorator().Decorate(chunk, new PlainsBiome()); //For now, ALWAYS use the water decorator on all chunks...
			_cavegen.GenerateCave(chunk);
			new LavaDecorator().Decorate(chunk, new PlainsBiome());
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