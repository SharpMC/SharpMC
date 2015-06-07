#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Worlds
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading.Tasks;
	using System.Timers;

	using SharpMC.Blocks;
	using SharpMC.Entity;
	using SharpMC.Enums;
	using SharpMC.Interfaces;
	using SharpMC.Networking.Packages;
	using SharpMC.Utils;

	public class Level
	{
		public Level()
		{
			this.CurrentWorldTime = 1200;
			this.Day = 1;
			this.OnlinePlayers = new List<Player>();
			this.DefaultGamemode = Gamemode.Creative;
			this.BlockWithTicks = new ConcurrentDictionary<Vector3, int>();
			this.StartTimeOfDayTimer();
			this.Entities = new List<Entity>();
			this.Dimension = 0;
			this.timetorain = Globals.Rand.Next(24000, 96000);
		}

		internal int Dimension { get; set; }

		internal string LvlName { get; set; }

		internal int Difficulty { get; set; }

		internal Gamemode DefaultGamemode { get; set; }

		internal LVLType LevelType { get; set; }

		internal List<Player> OnlinePlayers { get; private set; }

		internal int CurrentWorldTime { get; set; }

		internal int Day { get; set; }

		public IWorldProvider Generator { get; set; }

		internal List<Entity> Entities { get; private set; }

		internal ConcurrentDictionary<Vector3, int> BlockWithTicks { get; private set; }

		public int timetorain { get; set; }

		public bool Raining { get; set; }

		#region APISpecific

		public Player[] GetOnlinePlayers
		{
			get
			{
				return this.OnlinePlayers.ToArray();
			}
		}

		#endregion

		public void RemovePlayer(Player player)
		{
			lock (this.OnlinePlayers)
			{
				this.OnlinePlayers.Remove(player);
			}
		}

		public Player GetPlayer(int entityId)
		{
			foreach (var p in this.OnlinePlayers)
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
			this.OnlinePlayers.Add(player);

			new PlayerListItem(player.Wrapper)
				{
					Action = 0, 
					Gamemode = player.Gamemode, 
					Username = player.Username, 
					UUID = player.Uuid
				}.Broadcast(this);

				// Send playerlist item to old players & player him self
			this.BroadcastExistingPlayers(player.Wrapper);
		}

		public void BroadcastChat(string message)
		{
			foreach (var i in this.OnlinePlayers)
			{
				new ChatMessage(i.Wrapper) { Message = @message }.Write();
			}
		}

		public void BroadcastChat(string message, Player sender)
		{
			foreach (var i in this.OnlinePlayers)
			{
				if (i == sender)
				{
					continue;
				}

				new ChatMessage(i.Wrapper) { Message = @message }.Write();
			}
		}

		internal void BroadcastExistingPlayers(ClientWrapper caller)
		{
			foreach (var i in this.OnlinePlayers)
			{
				if (i.Wrapper != caller)
				{
					new PlayerListItem(caller) { Action = 0, Gamemode = i.Gamemode, Username = i.Username, UUID = i.Uuid }.Write();

						// Send TAB Item
					new SpawnPlayer(caller) { Player = i }.Write(); // Spawn the old player to new player
					new SpawnPlayer(i.Wrapper) { Player = caller.Player }.Write(); // Spawn the new player to old player
					i.BroadcastEquipment();
				}
			}
		}

		internal void BroadcastPlayerRemoval(ClientWrapper caller)
		{
			new PlayerListItem(caller)
				{
					Action = 0, 
					Gamemode = caller.Player.Gamemode, 
					Username = caller.Player.Username, 
					UUID = caller.Player.Uuid
				}.Broadcast(this);
		}

		public void SaveChunks()
		{
			this.Generator.SaveChunks(this.LvlName);
		}

		public Block GetBlock(Vector3 blockCoordinates)
		{
			var chunk =
				this.Generator.GenerateChunkColumn(new Vector2((int)blockCoordinates.X >> 4, (int)blockCoordinates.Z >> 4));

			var bid = chunk.GetBlock(
				(int)blockCoordinates.X & 0x0f, 
				(int)blockCoordinates.Y & 0x7f, 
				(int)blockCoordinates.Z & 0x0f);

			var metadata = chunk.GetMetadata(
				(int)blockCoordinates.X & 0x0f, 
				(int)blockCoordinates.Y & 0x7f, 
				(int)blockCoordinates.Z & 0x0f);

			// bid = (ushort) (bid >> 4);
			var block = BlockFactory.GetBlockById(bid, metadata);
			block.Coordinates = blockCoordinates;
			block.Metadata = metadata;

			return block;
		}

		public void SetBlock(Vector3 coordinates, Block block)
		{
			block.Coordinates = coordinates;
			this.SetBlock(block);
		}

		public void SetBlock(Block block, bool broadcast = true, bool applyPhysics = true)
		{
			var chunk =
				this.Generator.GenerateChunkColumn(
					new ChunkCoordinates((int)block.Coordinates.X >> 4, (int)block.Coordinates.Z >> 4));
			chunk.SetBlock(
				(int)block.Coordinates.X & 0x0f, 
				(int)block.Coordinates.Y & 0x7f, 
				(int)block.Coordinates.Z & 0x0f, 
				block);
			chunk.IsDirty = true;

			if (applyPhysics)
			{
				this.ApplyPhysics((int)block.Coordinates.X, (int)block.Coordinates.Y, (int)block.Coordinates.Z);
			}

			if (!broadcast)
			{
				return;
			}

			BlockChange.Broadcast(block, this);
		}

		internal void ApplyPhysics(int x, int y, int z)
		{
			this.DoPhysics(x - 1, y, z);
			this.DoPhysics(x + 1, y, z);
			this.DoPhysics(x, y - 1, z);
			this.DoPhysics(x, y + 1, z);
			this.DoPhysics(x, y, z - 1);
			this.DoPhysics(x, y, z + 1);
		}

		private void DoPhysics(int x, int y, int z)
		{
			var block = this.GetBlock(new Vector3(x, y, z));
			if (block is BlockAir)
			{
				return;
			}

			block.DoPhysics(this);
		}

		public void ScheduleBlockTick(Block block, int tickRate)
		{
			this.BlockWithTicks[block.Coordinates] = this.CurrentWorldTime + tickRate;
		}

		public void AddEntity(Entity entity)
		{
			this.Entities.Add(entity);
		}

		public void RemoveEntity(Entity entity)
		{
			if (this.Entities.Contains(entity))
			{
				this.Entities.Remove(entity);
			}
		}

		public PlayerLocation GetSpawnPoint()
		{
			var spawn = this.Generator.GetSpawnPoint();
			return new PlayerLocation(spawn.X, spawn.Y, spawn.Z);
		}

		public int GetWorldTime()
		{
			return this.CurrentWorldTime;
		}

		public void SetWorldTime(int time)
		{
			this.CurrentWorldTime = time;
		}

		#region TickTimer

		private Task _gameTickThread;

		internal void StartTimeOfDayTimer()
		{
			this._gameTickThread = new Task(this.StartTickTimer);
			this._gameTickThread.Start();
		}

		private static readonly Timer KtTimer = new Timer();

		private void StartTickTimer()
		{
			KtTimer.Elapsed += this.GameTick;
			KtTimer.Interval = 50;
			KtTimer.Start();
		}

		private void DayTick()
		{
			if (this.CurrentWorldTime < 24000)
			{
				this.CurrentWorldTime += 1;
			}
			else
			{
				this.CurrentWorldTime = 0;
				this.Day++;
			}

			lock (this.OnlinePlayers)
			{
				foreach (var i in this.OnlinePlayers)
				{
					new TimeUpdate(i.Wrapper) { Time = this.CurrentWorldTime, Day = this.Day }.Write();
				}
			}
		}

		private readonly Stopwatch _sw = new Stopwatch();

		private long _lastCalc;

		public int CalculateTps(Player player = null)
		{
			var average = this._lastCalc;

			var d = 1000 - this._lastCalc;
			d = d / 50;
			var exact = d;

			var color = "a";
			if (exact <= 10)
			{
				color = "c";
			}

			if (exact <= 15 && exact > 10)
			{
				color = "e";
			}

			if (player != null)
			{
				player.SendChat("TPS: §" + color + exact);
				player.SendChat("Miliseconds in Tick: " + average + "ms");
			}

			return (int)exact;
		}

		private void WeatherTick()
		{
			if (this.timetorain == 0 && !this.Raining)
			{
				this.Raining = true;
				foreach (var player in this.OnlinePlayers.ToArray())
				{
					new ChangeGameState(player.Wrapper) { Reason = GameStateReason.BeginRaining, Value = 1 }.Write();
				}

				this.timetorain = Globals.Rand.Next(12000, 36000);
			}
			else if (!this.Raining)
			{
				--this.timetorain;
			}
			else if (this.Raining && this.timetorain == 0)
			{
				this.Raining = false;
				foreach (var player in this.OnlinePlayers.ToArray())
				{
					new ChangeGameState(player.Wrapper) { Reason = GameStateReason.EndRaining, Value = 1 }.Write();
				}

				this.timetorain = Globals.Rand.Next(24000, 96000);
			}
			else if (this.Raining)
			{
				--this.timetorain;
			}
		}

		private int _clockTick = 0;

		private int _saveTick;

		private void GameTick(object source, ElapsedEventArgs e)
		{
			this._sw.Start();

			this.DayTick();

			this.WeatherTick();

			foreach (var blockEvent in this.BlockWithTicks.ToArray())
			{
				if (blockEvent.Value <= this.CurrentWorldTime)
				{
					this.GetBlock(blockEvent.Key).OnTick(this);
					int value;
					this.BlockWithTicks.TryRemove(blockEvent.Key, out value);
				}
			}

			foreach (var player in this.OnlinePlayers.ToArray())
			{
				player.OnTick();
			}

			foreach (var entity in this.Entities.ToArray())
			{
				entity.OnTick();
			}

			if (this._saveTick == 3000)
			{
				this._saveTick = 0;
				ConsoleFunctions.WriteInfoLine("Saving chunks");
				var sw = new Stopwatch();
				sw.Start();
				this.SaveChunks();
				sw.Stop();
				ConsoleFunctions.WriteInfoLine("Saving chunks took: " + sw.ElapsedMilliseconds + "MS");

				// ConsoleFunctions.WriteInfoLine("Clearing chunk cache...");
				// Generator.ClearCache(); //Clear chunk cache
				GC.Collect(); // Collect garbage
			}
			else
			{
				this._saveTick++;
			}

			if (this._saveTick == 750)
			{
				GC.Collect();
			}

			this._sw.Stop();
			this._lastCalc = this._sw.ElapsedMilliseconds;
			this._sw.Reset();
		}

		#endregion
	}
}