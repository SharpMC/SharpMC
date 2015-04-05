using System.Collections.Generic;
using System.Threading;
using System.Timers;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Enums;
using SharpMCRewrite.Interfaces;
using SharpMCRewrite.Networking.Packages;
using Timer = System.Timers.Timer;

namespace SharpMCRewrite.Classes
{
	public enum LVLType
	{
		Default,
		flat,
		largeBiomes,
		amplified,
		default_1_1
	}

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
			OnlinePlayers.Remove(player);
		}

		public void AddPlayer(Player player)
		{
			OnlinePlayers.Add(player);
		}

		public void BroadcastChat(string Message)
		{
			foreach (var i in OnlinePlayers)
			{
				//new ChatMessage ().Write (i.Wrapper, new MSGBuffer (i.Wrapper), new object[] { Message });
				new ChatMessage(i.Wrapper) {Message = @Message}.Write();
			}
			ConsoleFunctions.WriteInfoLine("Chat: " + Message);
		}

		public void BroadcastNewPlayer(ClientWrapper newPlayer)
		{
			foreach (var i in OnlinePlayers)
			{
				if (i.Wrapper != newPlayer)
				{
					new SpawnPlayer(i.Wrapper) {Player = newPlayer.Player}.Write();
					new SpawnPlayer(newPlayer) {Player = i}.Write();
				}
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
					}.Write();
				}
			}
			BroadcastNewPlayer(caller);
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

		/*public Block GetBlock(Vector3 blockCoordinates)
		{
			return GetBlock(new Vector3( blockCoordinates.X, blockCoordinates.Y, blockCoordinates.Z));
		}*/

		public Block GetBlock(Vector3 blockCoordinates)
		{
			//ChunkColumn chunk = Generator.GenerateChunkColumn(new Vector2(blockCoordinates.X / 16, blockCoordinates.Z / 16));

			var chunk = Generator.GetChunk((int)blockCoordinates.X >> 4, (int)blockCoordinates.Z >> 4);

			var bid = chunk.GetBlock((int)blockCoordinates.X & 0x0f, (int)blockCoordinates.Y & 0x7f, (int)blockCoordinates.Z & 0x0f);

			var metadata = chunk.GetMetadata((int)blockCoordinates.X & 0x0f, (int)blockCoordinates.Y & 0x7f, (int)blockCoordinates.Z & 0x0f);

			bid = (ushort) (bid >> 4);

			var block = BlockFactory.GetBlockById(bid);
			block.Coordinates = blockCoordinates;
			block.Metadata = metadata;

			return block;
		}

		public void SetBlock(Block block, bool broadcast = true)
		{
			Generator.SetBlock(block, this, broadcast);
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

			foreach (var i in OnlinePlayers)
			{
				new TimeUpdate(i.Wrapper) {Time = Tick, Day = Day} .Write();
			}
		}

		private void GameTick(object source, ElapsedEventArgs e)
		{
		}

		#endregion
	}
}