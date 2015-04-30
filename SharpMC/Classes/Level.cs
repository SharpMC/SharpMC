using System.Collections.Generic;
using System.Threading;
using System.Timers;
using SharpMC.Blocks;
using SharpMC.Enums;
using SharpMC.Interfaces;
using SharpMC.Networking.Packages;
using Timer = System.Timers.Timer;

namespace SharpMC.Classes
{
	public class Level
	{
		public Level()
		{
			Tick = 1200;
			Day = 0;
			OnlinePlayers = new List<Player>();
			DefaultGamemode = Gamemode.Creative;
			StartTimeOfDayTimer();
		}

		public string LVLName { get; set; }
		public int Difficulty { get; set; }
		public Gamemode DefaultGamemode { get; set; }
		public LVLType LevelType { get; set; }
		public List<Player> OnlinePlayers { get; set; }
		public int Tick { get; set; }
		public int Day { get; set; }
		public IWorldProvider Generator { get; set; }

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

			bid = (ushort) (bid >> 4);

			var block = BlockFactory.GetBlockById(bid);
			block.Coordinates = blockCoordinates;
			block.Metadata = metadata;

			return block;
		}

		public void SetBlock(Block block, bool broadcast = true)
		{
			var chunk =
				Generator.GenerateChunkColumn(new ChunkCoordinates((int) block.Coordinates.X >> 4, (int) block.Coordinates.Z >> 4));
			chunk.SetBlock((int) block.Coordinates.X & 0x0f, (int) block.Coordinates.Y & 0x7f, (int) block.Coordinates.Z & 0x0f,
				block);
			//chunk.SetMetadata(block.Coordinates.X & 0x0f, block.Coordinates.Y & 0x7f, block.Coordinates.Z & 0x0f, block.Metadata);
			chunk.IsDirty = true;
			//if (applyPhysics) ApplyPhysics(block.Coordinates.X, block.Coordinates.Y, block.Coordinates.Z);
			Generator.OverWriteCache(chunk);

			if (!broadcast) return;
			BlockChange.Broadcast(block);
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
		}

		#endregion
	}
}