using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using SharpMC.Entities;
using SharpMC.Log;
using SharpMC.Network.Packets;
using SharpMC.Util;
using SharpMC.World.Generators;

namespace SharpMC.World
{
	public class Level : IDisposable
	{
		private static readonly ILogger Log= LogManager.GetLogger(typeof (Level));

		private EntityManager EntityManager { get; }
		private ConcurrentDictionary<int, Player> Players { get; } 
		private ConcurrentDictionary<int, Entity> Entities { get; } 

		public string Name { get; }
		public PlayerLocation SpawnPoint { get; set; }
		public Gamemode DefaultGamemode { get; set; } = Gamemode.Creative;

		public int PlayerCount => Players.Count;

		public IWorldGenerator WorldGenerator { get; }

		public Level(string name, IWorldGenerator worldGenerator)
		{
            Name = name;
			WorldGenerator = worldGenerator;
			EntityManager = new EntityManager();
			Players = new ConcurrentDictionary<int, Player>();
			Entities = new ConcurrentDictionary<int, Entity>();

			SpawnPoint = worldGenerator.GetSpawnPoint();
		}

		private Timer TickTimer { get; set; }
		private object _tickLock = new object();
		public long GameTick = 0;
		private void OnTick(object state)
		{
			if (!Monitor.TryEnter(_tickLock))
			{
				return;
			}

			try
			{
				foreach (var player in Players.Values.ToArray())
				{
					player.OnTick();
				}
			}
			finally
			{
				Monitor.Exit(_tickLock);
				GameTick++;
			}
		}

		public void Initialize()
		{
			Stopwatch chunkLoading = Stopwatch.StartNew();

			int count = 0;
			foreach (var i in GenerateChunks(null, new ChunkCoordinates(SpawnPoint), new Dictionary<Tuple<int, int>, byte[]>(), 8))
			{
				count++;
			}
			chunkLoading.Stop();
			Log.LogInformation("World pre-cache {0} chunks completed in {1}ms", count, chunkLoading.ElapsedMilliseconds);

			TickTimer = new Timer(OnTick, null, 50, 50);
		}

		public void AddEntity(Entity entity)
		{
			
		}

		public void RemoveEntity(Entity entity)
		{
			
		}

		public virtual void AddPlayer(Player newPlayer, bool spawn)
		{
			EntityManager.AddEntity(newPlayer);

			if (Players.TryAdd(newPlayer.EntityId, newPlayer))
			{
				SpawnToAll(newPlayer);

				foreach (Entity entity in Entities.Values.ToArray())
				{
					entity.SpawnToPlayers(new[] { newPlayer });
				}
			}

			newPlayer.IsSpawned = spawn;
		}

		public void SpawnToAll(Player newPlayer)
		{
			var players = Players.Values.ToArray();
			newPlayer.SpawnToPlayers(players);

			foreach (var player in Players.Values.ToArray())
			{
				player.SpawnToPlayers(new Player[] { newPlayer });
			}
		}

		public virtual void RemovePlayer(Player player, bool despawn = true)
		{
			Player p;
			if (Players.TryRemove(player.EntityId, out p))
			{
				DespawnFromAll(player);

				foreach (Entity entity in Entities.Values.ToArray())
				{
					entity.DespawnFromPlayers(new [] { player });
				}

				EntityManager.RemoveEntity(null, player);
			}
			player.IsSpawned = !despawn;
		}

		public void DespawnFromAll(Player newPlayer)
		{
			var players = Players.Values.ToArray();
			newPlayer.DespawnFromPlayers(players);

			foreach (var player in Players.Values.ToArray())
			{
				player.DespawnFromPlayers(new Player[] { newPlayer });
			}
		}

		public IEnumerable<byte[]> GenerateChunks(Player player, ChunkCoordinates chunkPosition, Dictionary<Tuple<int, int>, byte[]> chunksUsed, double radius)
		{
			lock (chunksUsed)
			{
				Dictionary<Tuple<int, int>, double> newOrders = new Dictionary<Tuple<int, int>, double>();

				double radiusSquared = Math.Pow(radius, 2);

				int centerX = chunkPosition.X;
				int centerZ = chunkPosition.Z;

				for (double x = -radius; x <= radius; ++x)
				{
					for (double z = -radius; z <= radius; ++z)
					{
						var distance = (x*x) + (z*z);
						if (distance > radiusSquared)
						{
							//continue;
						}
						int chunkX = (int) (x + centerX);
						int chunkZ = (int) (z + centerZ);
						Tuple<int, int> index = new Tuple<int, int>(chunkX, chunkZ);
						newOrders[index] = distance;
					}
				}

				foreach (var chunkKey in chunksUsed.Keys.ToArray())
				{
					if (!newOrders.ContainsKey(chunkKey))
					{
						if (player != null)
						{
							player.UnloadChunk(new ChunkCoordinates(chunkKey.Item1, chunkKey.Item2));
						}
						chunksUsed.Remove(chunkKey);
					}
				}

				foreach (var pair in newOrders.OrderBy(pair => pair.Value))
				{
					if (chunksUsed.ContainsKey(pair.Key)) continue;

					if (WorldGenerator == null) continue;

					ChunkColumn chunkColumn = WorldGenerator.GenerateChunkColumn(new ChunkCoordinates(pair.Key.Item1, pair.Key.Item2));
					byte[] chunk = null;
					if (chunkColumn != null)
					{
						chunk = chunkColumn.ToArray();
					}

					chunksUsed.Add(pair.Key, chunk);

					yield return chunk;
				}
			}
		}

		public void RelayBroadcast(Packet packet)
		{
			Player[] players = Players.Values.ToArray();
			RelayBroadcast(players, packet);
		}

		public void RelayBroadcast(Player[] players, Packet packet)
		{
			foreach (var i in players)
			{
				i.Connection.SendPacket(packet);
			}
		}

		private bool _disposed = false;
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_disposed) return;
				_disposed = true;

				TickTimer.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Level()
		{
			Dispose(false);
		}
	}
}
