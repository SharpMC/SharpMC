using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using SharpMC.Blocks;
using SharpMC.Entity;
using SharpMC.Enums;
using SharpMC.Interfaces;
using SharpMC.Networking.Packages;
using SharpMC.Utils;
using Timer = System.Timers.Timer;

namespace SharpMC.Worlds
{
	public class Level
	{
		public Level()
		{
			Tick = 1200;
			Day = 0;
			OnlinePlayers = new List<Player>();
			DefaultGamemode = Gamemode.Creative;
			BlockWithTicks = new ConcurrentDictionary<Vector3, int>();
			StartTimeOfDayTimer();
		}

		public string LVLName { get; set; }
		public int Difficulty { get; set; }
		public Gamemode DefaultGamemode { get; set; }
		public LVLType LevelType { get; set; }
		public List<Player> OnlinePlayers { get; private set; }
		public int Tick { get; set; }
		public int Day { get; set; }
		public IWorldProvider Generator { get; set; }
		public List<Entity.Entity> Entities { get; private set; }
		public ConcurrentDictionary<Vector3, int> BlockWithTicks { get; private set; }
		public void RemovePlayer(Player player)
		{
			lock (OnlinePlayers)
			{
				OnlinePlayers.Remove(player);
			}
		}

		public Player GetPlayer(int entityId)
		{
			foreach (Player p in OnlinePlayers)
			{
				if (p.EntityId == entityId)
				{
					return p;
				}
			}
			return null;
		}

		public void AddPlayer(Player player)
		{
			OnlinePlayers.Add(player);

			new PlayerListItem(player.Wrapper)
			{
				Action = 0,
				Gamemode = player.Gamemode,
				Username = player.Username,
				UUID = player.Uuid
			}.Broadcast(); //Send playerlist item to old players & player him self

			BroadcastExistingPlayers(player.Wrapper);
		}

		public void BroadcastChat(string Message)
		{
			foreach (var i in OnlinePlayers)
			{
				new ChatMessage(i.Wrapper) {Message = @Message}.Write();
			}
		}

		public void BroadcastExistingPlayers(ClientWrapper caller)
		{
			foreach (var i in OnlinePlayers)
			{
				if (i.Wrapper != caller)
				{
					new PlayerListItem(caller)
					{
						Action = 0,
						Gamemode = i.Gamemode,
						Username = i.Username,
						UUID = i.Uuid
					}.Write(); //Send TAB Item
					new SpawnPlayer(caller) {Player = i}.Write(); //Spawn the old player to new player
					new SpawnPlayer(i.Wrapper) {Player = caller.Player}.Write(); //Spawn the new player to old player
					i.BroadcastEquipment();
				}
			}
		}

		public void BroadcastPlayerRemoval(ClientWrapper caller)
		{
			new PlayerListItem(caller)
			{
				Action = 0,
				Gamemode = caller.Player.Gamemode,
				Username = caller.Player.Username,
				UUID = caller.Player.Uuid
			}.Broadcast(false, caller.Player);
		}

		public void SaveChunks()
		{
			Generator.SaveChunks(LVLName);
		}

		public Block GetBlock(Vector3 blockCoordinates)
		{
			var chunk = Generator.GetChunk((int) blockCoordinates.X >> 4, (int) blockCoordinates.Z >> 4);

			var bid = chunk.GetBlock((int) blockCoordinates.X & 0x0f, (int) blockCoordinates.Y & 0x7f,
				(int) blockCoordinates.Z & 0x0f);

			var metadata = chunk.GetMetadata((int) blockCoordinates.X & 0x0f, (int) blockCoordinates.Y & 0x7f,
				(int) blockCoordinates.Z & 0x0f);

			//bid = (ushort) (bid >> 4);

			var block = BlockFactory.GetBlockById(bid);
			block.Coordinates = blockCoordinates;
			block.Metadata = metadata;

			return block;
		}

		public void SetBlock(Block block, bool broadcast = true, bool applyPhysics = true)
		{
			var chunk =
				Generator.GenerateChunkColumn(new ChunkCoordinates((int) block.Coordinates.X >> 4, (int) block.Coordinates.Z >> 4));
			chunk.SetBlock((int) block.Coordinates.X & 0x0f, (int) block.Coordinates.Y & 0x7f, (int) block.Coordinates.Z & 0x0f,
				block);
			//chunk.SetMetadata(block.Coordinates.X & 0x0f, block.Coordinates.Y & 0x7f, block.Coordinates.Z & 0x0f, block.Metadata);
			chunk.IsDirty = true;
			//if (applyPhysics) ApplyPhysics(block.Coordinates.X, block.Coordinates.Y, block.Coordinates.Z);
			Generator.OverWriteCache(chunk);
			if (applyPhysics) ApplyPhysics((int) block.Coordinates.X, (int) block.Coordinates.Y, (int) block.Coordinates.Z);

			if (!broadcast) return;
			BlockChange.Broadcast(block);
		}

		public void ApplyPhysics(int x, int y, int z)
		{
			DoPhysics(x - 1, y, z);
			DoPhysics(x + 1, y, z);
			DoPhysics(x, y - 1, z);
			DoPhysics(x, y + 1, z);
			DoPhysics(x, y, z - 1);
			DoPhysics(x, y, z + 1);
		}

		private void DoPhysics(int x, int y, int z)
		{
			Block block = GetBlock(new Vector3(x,y,z));
			if (block is BlockAir) return;
			block.DoPhysics(this);
		}

		public void ScheduleBlockTick(Block block, int tickRate)
		{
			BlockWithTicks[block.Coordinates] = Tick + tickRate;
		}

		#region TickTimer

		private Thread TimerThread;
		private Thread GameTickThread;

		public void StartTimeOfDayTimer()
		{
			TimerThread = new Thread(() => StartTimeTimer());
			TimerThread.Start();

			GameTickThread = new Thread(() => StartTickTimer());
			GameTickThread.Start();
		}

		public void StopTimeOfDayTimer()
		{
			TimerThread.Abort();
			TimerThread = new Thread(() => StartTimeTimer());
		}

		private static readonly Timer kTimer = new Timer();
		private static readonly Timer kTTimer = new Timer();

		private void StartTimeTimer()
		{
			kTimer.Elapsed += RunDayTick;
			kTimer.Interval = 1000;
			kTimer.Start();
		}

		private void StartTickTimer()
		{
			kTTimer.Elapsed += GameTick;
			kTTimer.Interval = 50;
			kTTimer.Start();
		}

		private void StopTimeTimer()
		{
			kTimer.Stop();
		}

		private void RunDayTick(object source, ElapsedEventArgs e)
		{
			if (Tick < 24000)
			{
				Tick += 20;
			}
			else
			{
				Tick = 0;
				Day++;
			}

			lock (OnlinePlayers)
			{
				foreach (var i in OnlinePlayers)
				{
					new TimeUpdate(i.Wrapper) {Time = Tick, Day = Day}.Write();
				}
			}
		}

		private void GameTick(object source, ElapsedEventArgs e)
		{
			foreach (KeyValuePair<Vector3, int> blockEvent in BlockWithTicks.ToArray())
			{
				if (blockEvent.Value <= Tick)
				{
					GetBlock(blockEvent.Key).OnTick(this);
					int value;
					BlockWithTicks.TryRemove(blockEvent.Key, out value);
				}
			}
		}

		#endregion
	}
}