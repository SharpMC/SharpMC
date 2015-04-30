using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using SharpMC.Enums;
using SharpMC.Networking.Packages;
using SharpMC.Worlds;

namespace SharpMC.Classes
{
	public class Player : Entity.Entity
	{
		private readonly Dictionary<Tuple<int, int>, ChunkColumn> _chunksUsed;
		//Map stuff
		private readonly Vector2 _currentChunkPosition = new Vector2(0, 0);
		//Inventory stuff
		public byte CurrentSlot = 0;
		public PlayerInventoryManager Inventory;

		public Player(Level level) : base(-1 ,level)
		{
			_chunksUsed = new Dictionary<Tuple<int, int>, ChunkColumn>();
			Inventory = new PlayerInventoryManager(this);
			Level = level;
		}

		public string Username { get; set; }
		public string Uuid { get; set; }
		public ClientWrapper Wrapper { get; set; }
		public Gamemode Gamemode { get; set; }
		public bool IsSpawned { get; set; }
		public bool Digging { get; set; }

		//Client settings
		public string Locale { get; set; }
		public byte ViewDistance { get; set; }
		public byte ChatFlags { get; set; }
		public bool ChatColours { get; set; }
		public byte SkinParts { get; set; }
		public bool ForceChunkReload { get; set; }

		//Not Sure Why Stuff
		public EntityAction LastEntityAction { get; set; }

		public void AddToList()
		{
			Level.AddPlayer(this);
		}

		public override void OnTick(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			if (IsSpawned)
			{
				if (Gamemode == Gamemode.Surival)
				{
					HealthManager.OnTick();
				}
			}
		}

		public void Respawn()
		{
			HealthManager.ResetHealth();
			if (Wrapper != null && Wrapper.TcpClient.Connected) new Respawn(Wrapper) {GameMode = (byte)Gamemode}.Write();
		}

		public void SendHealth()
		{
			new UpdateHealth(Wrapper).Write();
		}

		public void BroadcastEntityAnimation(Animations action)
		{
			new Animation(Wrapper){AnimationId = (byte)action, TargetPlayer = this}.Broadcast();
		}

		public void SendChunksFromPosition()
		{
			if (KnownPosition == null)
			{
				var d = Level.Generator.GetSpawnPoint();
				KnownPosition = new PlayerLocation(d.X, d.Y, d.Z);
				ViewDistance = 8;
			}
			SendChunksForKnownPosition();
		}

		private void InitializePlayer()
		{
			new PlayerPositionAndLook(Wrapper).Write();

			IsSpawned = true;
			Level.AddPlayer(this);
			Wrapper.Player.Inventory.SendToPlayer();
			if (Globals.SupportSharpMC)
			{
				new PlayerListHeaderFooter(Wrapper) {Header = "§6§l" + Globals.ProtocolName, Footer = "§eC# Powered!"}.Write();
			}
		}

		public void SendChunksForKnownPosition(bool force = false)
		{
			var centerX = (int) KnownPosition.X >> 4;
			var centerZ = (int)KnownPosition.Z >> 4;

			if (!force && IsSpawned && _currentChunkPosition == new Vector2(centerX, centerZ)) return;

			_currentChunkPosition.X = centerX;
			_currentChunkPosition.Z = centerZ;

			new Thread(() =>
			{
				var counted = 0;

				foreach (
					var chunk in
						Level.Generator.GenerateChunks((ViewDistance * 16), KnownPosition.X, KnownPosition.Z,
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