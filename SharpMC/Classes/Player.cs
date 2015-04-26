using System;
using System.Collections.Generic;
using System.Threading;
using SharpMC.Enums;
using SharpMC.Networking.Packages;
using SharpMC.Worlds;

namespace SharpMC.Classes
{
	public class Player
	{
		private readonly Dictionary<Tuple<int, int>, ChunkColumn> _chunksUsed;
		//Map stuff
		private readonly Vector2 _currentChunkPosition = new Vector2(0, 0);
		//Inventory stuff
		public byte CurrentSlot = 0;
		public PlayerInventoryManager Inventory;

		public Player(Level level)
		{
			_chunksUsed = new Dictionary<Tuple<int, int>, ChunkColumn>();
			HealthManager = new HealthManager(this);
			Inventory = new PlayerInventoryManager(this);
			CurrentLevel = level;
		}

		public string Username { get; set; }
		public string Uuid { get; set; }
		public ClientWrapper Wrapper { get; set; }
		public int UniqueServerId { get; set; }
		public Gamemode Gamemode { get; set; }
		public bool IsSpawned { get; set; }
		public bool Digging { get; set; }
		//Location stuff
		public byte Dimension { get; set; }
		public Vector3 Coordinates { get; set; }
		public float Yaw { get; set; }
		public float Pitch { get; set; }
		public bool OnGround { get; set; }
		//Client settings
		public string Locale { get; set; }
		public byte ViewDistance { get; set; }
		public byte ChatFlags { get; set; }
		public bool ChatColours { get; set; }
		public byte SkinParts { get; set; }
		public bool ForceChunkReload { get; set; }
		//Healh
		public HealthManager HealthManager { get; set; }
		//Not Sure Why Stuff
		public EntityAction LastEntityAction { get; set; }
		public Level CurrentLevel { get; set; }

		public void AddToList()
		{
			CurrentLevel.AddPlayer(this);
		}

		public void Tick()
		{
			if (IsSpawned)
			{
				if (Gamemode == Gamemode.Surival)
				{
					HealthManager.OnTick();
				}
			}
		}

		public void SendHealth()
		{
			new UpdateHealth(Wrapper).Write();
		}

		public void SendChunksFromPosition()
		{
			if (Coordinates == null)
			{
				Coordinates = CurrentLevel.Generator.GetSpawnPoint();
				ViewDistance = 8;
			}
			SendChunksForKnownPosition();
		}

		private void InitializePlayer()
		{
			new PlayerPositionAndLook(Wrapper).Write();

			IsSpawned = true;
			CurrentLevel.AddPlayer(this);
			Wrapper.Player.Inventory.SendToPlayer();
			if (Globals.SupportSharpMC)
			{
				new PlayerListHeaderFooter(Wrapper) {Header = "§6§l" + Globals.ProtocolName, Footer = "§eC# Powered!"}.Write();
			}
		}

		public void SendChunksForKnownPosition(bool force = false)
		{
			var centerX = (int) Coordinates.X >> 4;
			var centerZ = (int) Coordinates.Z >> 4;

			if (!force && IsSpawned && _currentChunkPosition == new Vector2(centerX, centerZ)) return;

			_currentChunkPosition.X = centerX;
			_currentChunkPosition.Z = centerZ;

			new Thread(() =>
			{
				var counted = 0;

				foreach (
					var chunk in
						CurrentLevel.Generator.GenerateChunks((ViewDistance*16), Coordinates.X, Coordinates.Z,
							_chunksUsed, this))
				{
					if (Wrapper != null && Wrapper.TcpClient.Client.Connected)
						new ChunkData(Wrapper, new MSGBuffer(Wrapper)) {Chunk = chunk}.Write();
					Thread.Yield();

					if (counted >= 5 && !IsSpawned)
					{
						InitializePlayer();
					}
					counted++;
				}
			}).Start();
		}
	}
}