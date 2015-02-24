using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using MiNET;
using SharpMCRewrite.Networking.Packages;
using SharpMCRewrite.Worlds;

namespace SharpMCRewrite
{
	public class Player
	{
		private readonly Dictionary<Tuple<int, int>, ChunkColumn> _chunksUsed;
		//Map stuff
		private readonly Vector2 CurrentChunkPosition = new Vector2(0, 0);
		//Inventory stuff
		public byte CurrentSlot = 0;
		public PlayerInventoryManager Inventory;

		public Player()
		{
			_chunksUsed = new Dictionary<Tuple<int, int>, ChunkColumn>();
			HealthManager = new HealthManager(this);
			Inventory = new PlayerInventoryManager(this);
		}

		public string Username { get; set; }
		public string UUID { get; set; }
		public ClientWrapper Wrapper { get; set; }
		public int UniqueServerID { get; set; }
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

		public void AddToList()
		{
			Globals.Level.AddPlayer(this);
		}

		public void BroadcastMovement()
		{
		}

		public void Tick()
		{
			if (IsSpawned)
			{
				if (Gamemode == Gamemode.Surival)
				{
					HealthManager.OnTick();
				}
				/* if (t == 100)
		    {
			    HealthManager.Health--;
				SendHealth();
			    t = 0;
		    }
		    else t += 1;*/
			}
		}

		public void SendHealth()
		{
			new UpdateHealth(Wrapper).Write();
			Console.WriteLine("Sending health...");
		}

		public static Player GetPlayer(ClientWrapper wrapper)
		{
			foreach (var  i in Globals.Level.OnlinePlayers)
			{
				if (i.Wrapper == wrapper)
				{
					return i;
				}
			}
			throw new ArgumentOutOfRangeException("The specified player could not be found ;(");
		}

		public void SendChat(string message)
		{
			new ChatMessage(Wrapper, new MSGBuffer(Wrapper)) {Message = message}.Write();
		}

		public void SendChunksFromPosition()
		{
			if (Coordinates == null)
			{
				Coordinates = Globals.Level.Generator.GetSpawnPoint();
				ViewDistance = 32;
			}
			SendChunksForKnownPosition(false);
		}

		public void SendChunksForKnownPosition(bool force = false)
		{
			const int multiplier = 12; //Viewdistance multiplier
			var centerX = (int) Coordinates.X/16;
			var centerZ = (int) Coordinates.Z/16;

			if (!force && IsSpawned && CurrentChunkPosition == new Vector2(centerX, centerZ)) return;

			CurrentChunkPosition.X = centerX;
			CurrentChunkPosition.Z = centerZ;

			var _worker = new BackgroundWorker {WorkerSupportsCancellation = true};
			_worker.DoWork += delegate(object sender, DoWorkEventArgs args)
			{
				var worker = sender as BackgroundWorker;
				var Counted = 0;

				foreach (
					var chunk in
						Globals.Level.Generator.GenerateChunks(ViewDistance*multiplier, Coordinates.X, Coordinates.Z,
							force ? new Dictionary<Tuple<int, int>, ChunkColumn>() : _chunksUsed))
				{
					if (worker.CancellationPending)
					{
						args.Cancel = true;
						break;
					}
					new ChunkData(Wrapper, new MSGBuffer(Wrapper)) {Chunk = chunk}.Write();
					Thread.Yield();

					if (Counted >= (ViewDistance*multiplier) && !IsSpawned)
					{
						new PlayerPositionAndLook(Wrapper).Write();

						IsSpawned = true;
						Globals.Level.AddPlayer(this);

						new PlayerListItem(Wrapper)
						{
							Action = 0,
							Gamemode = Wrapper.Player.Gamemode,
							Username = Wrapper.Player.Username,
							UUID = Wrapper.Player.UUID
						}.Broadcast();

						Globals.Level.BroadcastExistingPlayers(Wrapper);
						Wrapper.Player.Inventory.SendToPlayer();
					}
					Counted++;
				}
			};
			_worker.RunWorkerAsync();
		}
	}

	public enum EntityAction
	{
		Crouch = 0,
		UnCrouch = 1,
		LeaveBed = 2,
		StartSprinting = 3,
		StopSprinting = 4,
		JumpWithHorse = 5,
		OpenInventory = 6
	}
}