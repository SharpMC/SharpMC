using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using SharpMC.Core.Utils;
using SharpMC.Util;
using SharpMC.Worlds;
using ChunkColumn = SharpMC.World.ChunkColumn;

namespace SharpMC.Core.Worlds.Better
{
	internal class BetterWorldProvider : WorldProvider
	{
		private static readonly Random Getrandom = new Random();
		private static readonly object SyncLock = new object();
		private readonly string _folder;
		public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();

		public BetterWorldProvider(string folder)
		{
			_folder = folder;
			IsCaching = true;
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
			if (ChunkCache.TryGetValue(chunkCoordinates.ToTuple(), out c)) return c;

			if (File.Exists(_folder + "/" + chunkCoordinates.X + "." + chunkCoordinates.Y + ".cfile"))
			{
				var cd = LoadChunk((int) chunkCoordinates.X, (int) chunkCoordinates.Y);
				lock (ChunkCache)
				{
					if (!ChunkCache.ContainsKey(new Tuple<int, int>(cd.X, cd.Z)))
						ChunkCache.Add(new Tuple<int, int>(cd.X, cd.Z), cd);
				}
				return cd;
			}

			var chunk = new ChunkColumn
            {
                X = (int) chunkCoordinates.X, Z = (int) chunkCoordinates.Y
            };
			PopulateChunk(chunk);

			if (!ChunkCache.ContainsKey(chunkCoordinates.ToTuple()))
				ChunkCache.Add(chunkCoordinates.ToTuple(), chunk);

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