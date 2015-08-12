// Distrubuted under the MIT license
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
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using SharpMC.Core.Blocks;
using SharpMC.Core.Entity;
using SharpMC.Core.Enums;
using SharpMC.Core.Networking;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;
using Ent = SharpMC.Core.Entity.Entity;

namespace SharpMC.Core.Worlds
{
	public class Level
	{
		public Level()
		{
			CurrentWorldTime = 1200;
			Day = 1;
			OnlinePlayers = new Dictionary<int, Player>();
			DefaultGamemode = Config.GetProperty("Gamemode", Gamemode.Survival);
			BlockWithTicks = new Dictionary<Vector3, int>();
			StartTimeOfDayTimer();
			Entities = new List<Ent>();
			Dimension = 0;
            Timetorain = Globals.Rand.Next(24000, 96000);
		}

		internal int Dimension { get; set; }
		internal string LvlName { get; set; }
		internal int Difficulty { get; set; }
		internal Gamemode DefaultGamemode { get; set; }
		internal LvlType LevelType { get; set; }
		internal Dictionary<int, Player> OnlinePlayers { get; private set; }
		internal int CurrentWorldTime { get; set; }
		internal int Day { get; set; }
		public WorldProvider Generator { get; set; }
		internal List<Ent> Entities { get; private set; }
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
			return (int)(((val%16) + 16)%16);
		}

		public Block GetBlock(Vector3 blockCoordinates)
		{
			Vector2 chunkcoords = new Vector2((int) blockCoordinates.X >> 4, (int) blockCoordinates.Z >> 4);
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
		public void AddEntity(Ent entity)
		{
			Entities.Add(entity);
		}

		public void RemoveEntity(Ent entity)
		{
			if (Entities.Contains(entity)) Entities.Remove(entity);
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


		private static readonly Timer KtTimer = new Timer();

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

			foreach (var entity in Entities)
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