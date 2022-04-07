using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using SharpMC.Blocks;
using SharpMC.Core;
using SharpMC.Core.Networking;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;
using SharpMC.Entities;
using SharpMC.Enums;
using SharpMC.Log;
using SharpMC.Network.Packets;
using SharpMC.Util;
using SharpMC.World.Generators;
using SharpMC.Worlds;
using Timer = System.Timers.Timer;

namespace SharpMC.World
{
	public class Level : IDisposable
	{
		private static readonly ILogger Log= LogManager.GetLogger(typeof (Level));

		private EntityManager EntityManager { get; }
		private ConcurrentDictionary<int, Player> Players { get; } 
		internal ConcurrentDictionary<int, Entity> Entities { get; } 

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
		public long GameTick3 = 0;
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
				GameTick3++;
			}
		}

		public void Initialize()
		{
			var chunkLoading = Stopwatch.StartNew();

			var count = 0;
			foreach (var i in GenerateChunks(null, new ChunkCoordinates(SpawnPoint), new Dictionary<Tuple<int, int>, byte[]>(), 8))
			{
				count++;
			}
			chunkLoading.Stop();
			Log.LogInformation("World pre-cache {0} chunks completed in {1}ms", count, chunkLoading.ElapsedMilliseconds);

            TickTimer = null;  // TODO new Timer(OnTick, null, 50, 50);
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

				foreach (var entity in Entities.Values.ToArray())
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
				player.SpawnToPlayers(new[] { newPlayer });
			}
		}

		public virtual void RemovePlayer(Player player, bool despawn = true)
		{
			Player p;
			if (Players.TryRemove(player.EntityId, out p))
			{
				DespawnFromAll(player);

				foreach (var entity in Entities.Values.ToArray())
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
				player.DespawnFromPlayers(new[] { newPlayer });
			}
		}

		public IEnumerable<byte[]> GenerateChunks(Player player, ChunkCoordinates chunkPosition, Dictionary<Tuple<int, int>, byte[]> chunksUsed, double radius)
		{
			lock (chunksUsed)
			{
				var newOrders = new Dictionary<Tuple<int, int>, double>();

				var radiusSquared = Math.Pow(radius, 2);

				var centerX = chunkPosition.X;
				var centerZ = chunkPosition.Z;

				for (var x = -radius; x <= radius; ++x)
				{
					for (var z = -radius; z <= radius; ++z)
					{
						var distance = x*x + z*z;
						if (distance > radiusSquared)
						{
							//continue;
						}
						var chunkX = (int) (x + centerX);
						var chunkZ = (int) (z + centerZ);
						var index = new Tuple<int, int>(chunkX, chunkZ);
						newOrders[index] = distance;
					}
				}

				foreach (var chunkKey in chunksUsed.Keys.ToArray())
				{
					if (!newOrders.ContainsKey(chunkKey))
					{
						if (player != null)
                        {
                            var c = new ChunkCoordinates(chunkKey.Item1, chunkKey.Item2);
							player.UnloadChunk(c);
						}
						chunksUsed.Remove(chunkKey);
					}
				}

				foreach (var pair in newOrders.OrderBy(pair => pair.Value))
				{
					if (chunksUsed.ContainsKey(pair.Key)) continue;

					if (WorldGenerator == null) continue;

                    var c = new ChunkCoordinates(pair.Key.Item1, pair.Key.Item2);
					var chunkColumn = WorldGenerator.GenerateChunkColumn(c);
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
			var players = Players.Values.ToArray();
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

		public Level()
		{
			CurrentWorldTime = 1200;
			Day = 1;
			OnlinePlayers = new Dictionary<int, Player>();
			DefaultGamemode = Config.GetProperty("Gamemode", Gamemode.Survival);
			BlockWithTicks = new Dictionary<Vector3, int>();
			StartTimeOfDayTimer();
			Entities2 = new List<Entity>();
			Dimension = 0;
            Timetorain = Globals.Rand.Next(24000, 96000);
		}

		internal int Dimension { get; set; }
		internal string LvlName { get; set; }
		internal int Difficulty { get; set; }
		internal Gamemode DefaultGamemode3 { get; set; }
		internal LvlType LevelType { get; set; }
		internal Dictionary<int, Player> OnlinePlayers { get; private set; }
		internal int CurrentWorldTime { get; set; }
		internal int Day { get; set; }
		public WorldProvider Generator { get; set; }
		internal List<Entity> Entities2 { get; private set; }
		internal Dictionary<Vector3, int> BlockWithTicks { get; private set; }
        public int Timetorain { get; set; }
        internal bool Raining { get; set; }

		#region APISpecific

		public int GetWorldTime()
		{
			return CurrentWorldTime;
		}

		public void SetWorldTime(int time)
		{
			CurrentWorldTime = time;
		}

		public Player[] GetOnlinePlayers
		{
			get { return OnlinePlayers.Values.ToArray(); }
		}

		#endregion

		public void RemovePlayer(Player player)
		{
			RemovePlayer(player.EntityId);
		}

		public void RemovePlayer(int entityId)
		{
			lock (OnlinePlayers)
			{
				if (OnlinePlayers.ContainsKey(entityId))
				{
					OnlinePlayers.Remove(entityId);
				}
			}
		}

		public Player GetPlayer(int entityId)
		{
			if (OnlinePlayers.ContainsKey(entityId))
			{
				return OnlinePlayers[entityId];
			}

			return null;
		}

		public void AddPlayer(Player player)
		{
			OnlinePlayers.Add(player.EntityId, player);

			new PlayerListItem(player.Wrapper)
			{
				Action = 0,
				Gamemode = player.Gamemode,
				Username = player.Username,
				Uuid = player.Uuid
			}.Broadcast(this); //Send playerlist item to old players & player him self

			IntroduceNewPlayer(player.Wrapper);
		}

		public void BroadcastChat(McChatMessage message)
		{
			//foreach (var i in OnlinePlayers)
			//{
			//	new ChatMessage(i.Wrapper) {Message = @message}.Write();
			//}
			BroadcastChat(message, ChatMessageType.ChatBox, null);
		}

		public void BroadcastChat(McChatMessage message, Player sender)
        {
            /*foreach (var i in OnlinePlayers)
            {
                if (i == sender)
                {
                    continue;
                }
                new ChatMessage(i.Wrapper) { Message = @message }.Write();
            }*/
			BroadcastChat(message, ChatMessageType.ChatBox, sender);
        }

		public void BroadcastChat(McChatMessage message, ChatMessageType messagetype, Player sender)
		{
			foreach (var i in OnlinePlayers.Values)
			{
				if (i == sender)
				{
					continue;
				}
				new ChatMessage(i.Wrapper) { Message = @message }.Write();
			}
		}

		internal void IntroduceNewPlayer(ClientWrapper caller)
		{
			foreach (var i in OnlinePlayers.Values)
			{
				if (i.Wrapper != caller)
				{
					new PlayerListItem(caller)
					{
						Action = 0,
						Gamemode = i.Gamemode,
						Username = i.Username,
						Uuid = i.Uuid
					}.Write(); //Send TAB Item
					new SpawnPlayer(caller) {Player = i}.Write(); //Spawn the old player to new player
					new SpawnPlayer(i.Wrapper) {Player = caller.Player}.Write(); //Spawn the new player to old player
					i.BroadcastEquipment();
				}
			}
		}

		internal void BroadcastPlayerRemoval(ClientWrapper caller)
		{
			new PlayerListItem(caller)
			{
				Action = 4,
				Uuid = caller.Player.Uuid
			}.Broadcast(this);
		}

		public void SaveChunks()
		{
			Generator.SaveChunks(LvlName);
		}

		private int Mod(double val)
		{
			return (int)((val%16 + 16)%16);
		}

		public Block GetBlock(Vector3 blockCoordinates)
		{
			var chunkcoords = new Vector2((int) blockCoordinates.X >> 4, (int) blockCoordinates.Z >> 4);
			var chunk = Generator.GenerateChunkColumn(chunkcoords);

			var bid = chunk.GetBlock(Mod(blockCoordinates.X), (int) blockCoordinates.Y,
				Mod(blockCoordinates.Z));

			var metadata = chunk.GetMetadata(Mod(blockCoordinates.X), (int)blockCoordinates.Y,
				Mod(blockCoordinates.Z));

			var block = BlockFactory.GetBlockById(bid, metadata);
			block.Coordinates = blockCoordinates;
			block.Metadata = metadata;

			return block;
		}

		public void UpdateSign(Vector3 coordinates, string[] lines)
		{
			if (lines.Length >= 4)
			{
				var signUpdate = new UpdateSign(null)
				{
					SignCoordinates = coordinates,
					Line1 = lines[0],
					Line2 = lines[1],
					Line3 = lines[2],
					Line4 = lines[4],
				};
				BroadcastPacket(signUpdate);
			}
		}

		public void SetBlock(Vector3 coordinates, Block block)
		{
			block.Coordinates = coordinates;
			SetBlock(block);
		}

		public void SetBlock(Block block, bool broadcast = true, bool applyPhysics = true)
		{
			var chunk =
				Generator.GenerateChunkColumn(new Vector2((int) block.Coordinates.X >> 4, (int) block.Coordinates.Z >> 4));
			
			chunk.SetBlock(Mod(block.Coordinates.X), (int)block.Coordinates.Y,
				Mod(block.Coordinates.Z),
				block);
			chunk.IsDirty = true;

			if (applyPhysics) ApplyPhysics((int) block.Coordinates.X, (int) block.Coordinates.Y, (int) block.Coordinates.Z);

			if (!broadcast) return;
			
			var packet = new BlockChange(null)
			{
				BlockId = block.Id,
				MetaData = block.Metadata,
				Location = block.Coordinates
			};
			BroadcastPacket(packet);
		}

		internal void ApplyPhysics(int x, int y, int z)
		{
			DoPhysics(x - 1, y, z);
			DoPhysics(x + 1, y, z);
			DoPhysics(x, y - 1, z);
			DoPhysics(x, y + 1, z);
			DoPhysics(x, y, z - 1);
			DoPhysics(x, y, z + 1);
			DoPhysics(x, y, z);
		}

		private void DoPhysics(int x, int y, int z)
		{
			var block = GetBlock(new Vector3(x, y, z));
			if (block is BlockAir) return;
			block.DoPhysics(this);
		}

		public void ScheduleBlockTick(Block block, int tickRate)
		{
			//if (!BlockWithTicks.ContainsKey(block.Coordinates))
			//{
				BlockWithTicks[block.Coordinates] = CurrentWorldTime + tickRate;
			//}
		}
		public void AddEntity3(Entity entity)
		{
			Entities2.Add(entity);
		}

		public void RemoveEntity4(Entity entity)
		{
			if (Entities2.Contains(entity)) Entities2.Remove(entity);
		}

		public void BroadcastPacket(Package package)
		{
			BroadcastPacket(package, true);
		}

		public void BroadcastPacket(Package package, bool self)
		{
			foreach (var i in OnlinePlayers.Values)
			{
				if (i == null) continue;
				if (!self && package.Client != null && i.Wrapper.ClientIdentifier == package.Client.ClientIdentifier) continue;
				package.SetTarget(i.Wrapper);
				package.Write();
			}
		}

		public PlayerLocation GetSpawnPoint()
		{
			var spawn = Generator.GetSpawnPoint();
			return new PlayerLocation(spawn.X, spawn.Y, spawn.Z);
		}

		#region TickTimer

		private Task _gameTickThread;

		internal void StartTimeOfDayTimer()
		{
			_gameTickThread = new Task(StartTickTimer);
			_gameTickThread.Start();
		}


		private static readonly Timer KtTimer = new System.Timers.Timer();

		private void StartTickTimer()
		{
			KtTimer.Elapsed += GameTick;
			KtTimer.Interval = 50;
			KtTimer.Start();
		}

		private void DayTick()
		{
			if (CurrentWorldTime < 24000)
			{
				CurrentWorldTime += 1;
			}
			else
			{
				CurrentWorldTime = 0;
				Day++;
			}

			lock (OnlinePlayers)
			{
				foreach (var i in OnlinePlayers.Values)
				{
					new TimeUpdate(i.Wrapper) {Time = CurrentWorldTime, Day = Day}.Write();
				}
			}
		}

		private readonly Stopwatch _sw = new Stopwatch();
		private long _lastCalc;

		public int CalculateTps(Player player = null)
		{
			var average = _lastCalc;

			var d = 1000 - _lastCalc;
			d = d/50;
			var exact = d;

			var color = "a";
			if (exact <= 10) color = "c";
			if (exact <= 15 && exact > 10) color = "e";


			if (player != null)
			{
				player.SendChat("TPS: §" + color + exact);
				player.SendChat("Miliseconds in Tick: " + average + "ms");
			}

			return (int) exact;
		}

        private void WeatherTick()
        {
            if(Timetorain == 0 && !Raining)
            {
                Raining = true;

				var packet = new ChangeGameState(null)
				{
					Reason = GameStateReason.BeginRaining,
					Value = (float)1
				};
				BroadcastPacket(packet);

                Timetorain = Globals.Rand.Next(12000, 36000);
            }
            else if(!Raining)
            {
                --Timetorain;
            }
            else if(Raining && Timetorain == 0)
            {
                Raining = false;

	            var packet = new ChangeGameState(null)
	            {
		            Reason = GameStateReason.EndRaining,
		            Value = (float) 1
	            };
                BroadcastPacket(packet);

                Timetorain = Globals.Rand.Next(24000, 96000);
            }
            else if(Raining)
            {
                --Timetorain;
            }
        }

		private int _saveTick;

		private void GameTick(object source, ElapsedEventArgs e)
		{
			_sw.Start();

			DayTick();

            WeatherTick();

			foreach (var blockEvent in BlockWithTicks.ToArray())
			{
				if (blockEvent.Value <= CurrentWorldTime)
				{
					var d = GetBlock(blockEvent.Key);
					d.OnTick(this);
					BlockWithTicks.Remove(blockEvent.Key);
				}
			}

			foreach (var player in OnlinePlayers.Values.ToArray())
			{
				player.OnTick();
			}

			foreach (var entity in Entities2)
			{
				entity.OnTick();
			}

			/*if (_saveTick == 3000)
			{
				_saveTick = 0;
				ConsoleFunctions.WriteInfoLine("Saving chunks");
				var sw = new Stopwatch();
				sw.Start();
				SaveChunks();
				sw.Stop();
				ConsoleFunctions.WriteInfoLine("Saving chunks took: " + sw.ElapsedMilliseconds + "MS");

				GC.Collect(); //Collect garbage
			}
			*/

			if (_saveTick == 750)
			{
				GC.Collect();
				_saveTick = 0;
			}
			else
			{
				_saveTick++;
			}

			_sw.Stop();
			_lastCalc = _sw.ElapsedMilliseconds;
			_sw.Reset();
		}

		#endregion
	}
}