using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using Microsoft.Extensions.Logging;
using SharpMC.Core;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;
using SharpMC.Entities;
using SharpMC.Enums;
using SharpMC.Log;
using SharpMC.Network.Packets.Play;
using SharpMC.Networking.Packages;
using SharpMC.TileEntities;
using SharpMC.Util;
using SharpMC.World;
using ChunkCoordinates = SharpMC.Util.ChunkCoordinates;
using EntityAction = SharpMC.Enums.EntityAction;
using PlayerLocation = SharpMC.Util.PlayerLocation;

namespace SharpMC
{
	public class Player : Entity
	{
		private static readonly ILogger Log = LogManager.GetLogger(typeof(Player));

		private Dictionary<Tuple<int,int>, byte[]> ChunksUsed = new Dictionary<Tuple<int, int>, byte[]>(); 

		public MCNetConnection Connection { get; }
		private MinecraftServer Server { get; }
		public Guid UUID { get; set; }

		public string DisplayName { get; set; }
		public MCNetConnection.AuthResponse AuthResponse { get; set; } = null;

		public bool IsConnected => Connection.IsConnected;
		public int ViewDistance { get; set; } = 8;

		public Player(MCNetConnection connection, MinecraftServer server, string username) : base(null)
        {
            Server = server;

			Connection = connection;
			Username = username;

			UUID = Guid.NewGuid();
		}

		public void InitiateGame()
		{
			Level = Server.LevelManager.GetLevel(this, "default");
			if (Level == null)
			{
				Disconnect("No level assigned to player!");
				return;
			}

			KnownPosition = Level.SpawnPoint;
			Gamemode = Level.DefaultGamemode;

			SendJoinGame();

			_prevChunkCoordinates = new ChunkCoordinates(KnownPosition);
			SendChunksForKnownPosition(_prevChunkCoordinates);

			SendPlayerPositionAndLook();

			Level.AddPlayer(this, true);
		}

		private long _timeSinceLastKeepAlive = 0;
		private ChunkCoordinates _prevChunkCoordinates = new ChunkCoordinates();

		public override void OnTick()
		{
            if (IsSpawned)
            {
                if (Gamemode == Enums.Gamemode.Survival)
                {
                    HealthManager.OnTick();
                }
            }

			ChunkCoordinates cur = new ChunkCoordinates(KnownPosition);
			if (cur.DistanceTo(_prevChunkCoordinates) >= 2)
			{
				_prevChunkCoordinates = cur;
				SendChunksForKnownPosition(cur);
			}

			_timeSinceLastKeepAlive++;

			if (Level.GameTick3 % 20 == 0)
			{
				if (Connection.KeepAliveReady || _timeSinceLastKeepAlive >= 100)
				{
					Connection.SendKeepAlive();
					_timeSinceLastKeepAlive = 0;
				}
			}
		}

		private void SendChunksForKnownPosition(ChunkCoordinates coords)
		{
			foreach (var i in Level.GenerateChunks(this, coords, ChunksUsed, ViewDistance))
			{
				Connection.SendPacket(new ChunkDataPacket()
				{
					Data = i
				});
			}
		}

		private void SendJoinGame()
		{
			JoinGamePacket joinGame = new JoinGamePacket
			{
				EntityId = EntityId,
				Gamemode = (byte) Gamemode,
				Difficulty = 0,
				Dimension = 0,
				LevelType = "flat",
				ReducedDebugInfo = false
			};

			Connection.SendPacket(joinGame);
		}

		public void SendPlayerPositionAndLook()
		{
			PlayerLocation loc = (PlayerLocation) KnownPosition.Clone();

			PlayerPositionAndLookPacket packet = new PlayerPositionAndLookPacket()
			{
				Flags = 0,
				TeleportId = 0,
				X = loc.X,
				Y = loc.Y,
				Z = loc.Z,
				Yaw = loc.Yaw,
				Pitch = loc.Pitch
			};

			Connection.SendPacket(packet);
		}

		public void UnloadChunk(ChunkCoordinates coordinates)
		{
			Connection.SendPacket(new UnloadChunk()
			{
				X = coordinates.X,
				Z = coordinates.Z
			});
		}

		public void Disconnect(string reason)
		{
			Log.LogWarning("Kicking player {0} with reason: {1}", Username, reason);
		}

		public override void SpawnToPlayers(Player[] players)
		{
			PlayerListProperty p = null;
			if (AuthResponse != null)
			{
				foreach (var i in AuthResponse.Properties)
				{
					if (i.Name.Equals("textures", StringComparison.InvariantCultureIgnoreCase))
					{
						p = new PlayerListProperty
						{
							Name = i.Name,
							Value = i.Value,
							IsSigned = true,
							Signature = i.Signature
						};
						break;
					}
				}
			}

			PlayerListItemPacket packet = new PlayerListItemPacket
			{
				Action = PlayerListAction.AddPlayer,
				Ping = 0,
				Gamemode = (int)Gamemode,
				Name = Username,
				//Displayname = DisplayName,
				UUID = UUID
			};

			if (p != null)
			{
				packet.Properties = new PlayerListProperty[]
				{
					p
				};
			}

			Level.RelayBroadcast(players, packet);

			SpawnPlayerPacket spp = new SpawnPlayerPacket
			{
				EntityId = EntityId,
				Pitch = (byte) (KnownPosition.Pitch.ToRadians()),
				Yaw = (byte) (KnownPosition.Yaw.ToRadians()),
				X = KnownPosition.X,
				Y = KnownPosition.Y,
				Z = KnownPosition.Z,
				Uuid = UUID
			};

			Level.RelayBroadcast(players, spp);
		}

		public override void DespawnFromPlayers(Player[] players)
		{
			PlayerListItemPacket packet = new PlayerListItemPacket
			{
				Action = PlayerListAction.RemovePlayer,
				UUID = UUID
			};

			Level.RelayBroadcast(players, packet);
		}

		public override void DespawnEntity()
		{
			IsSpawned = false;
			Level.DespawnFromAll(this);
		}

		private readonly List<Tuple<int, int>> _chunksUsed;
		private Vector2 _currentChunkPosition = new Vector2(0, 0);
		public PlayerInventoryManager Inventory; //The player's Inventory
		public string SkinBlob = "";
		
		public Player(Level level) : base(-1, level)
		{
			_chunksUsed = new List<Tuple<int, int>>();
			Inventory = new PlayerInventoryManager(this);
			Level = level;

			Width = 0.6;
			Height = 1.62;
			Length = 0.6;
			IsOperator = false;
			Loaded = false;
		}

		public string Username { get; set; } //The player's username
		public string Uuid { get; set; } // The player's UUID
		public ClientWrapper Wrapper { get; set; } //The player's associated ClientWrapper
		public Gamemode Gamemode { get; set; } //The player's gamemode
		public bool IsSpawned { get; set; } //Is the player spawned?
		public bool Digging { get; set; } // Is the player digging?
		private bool CanFly { get; set; } //Can the player fly?
		//Client settings
		public string Locale { get; set; }
		
		public int ChatFlags { get; set; }
		public bool ChatColours { get; set; }
		public byte SkinParts { get; set; }
		public int MainHand { get; set; }
		public bool ForceChunkReload { get; set; }
		//Not Sure Why Stuff
		public EntityAction LastEntityAction { get; set; }
		public bool IsOperator { get; internal set; }
		private bool Loaded { get; set; }
		public bool IsCrouching { get; set; }
		public bool IsAuthenticated()
		{
			if (ServerSettings.OnlineMode)
			{
				try
				{
					var uri = new Uri(
						string.Format(
							"http://session.minecraft.net/game/checkserver.jsp?user={0}&serverId={1}",
							Username,
							PacketCryptography.JavaHexDigest(Encoding.UTF8.GetBytes("")
								.Concat(Wrapper.SharedKey)
								.Concat(PacketCryptography.PublicKeyToAsn1(Globals.ServerKey))
								.ToArray())
							));

					var authenticated = new WebClient().DownloadString(uri);
					if (authenticated.Contains("NO"))
					{
						ConsoleFunctions.WriteInfoLine("Response: " + authenticated);
						return false;
					}
				}
				catch
				{
					return false;
				}

				return true;
			}

			return true;
		}

		#region PacketHandling

		public void PositionChanged(Vector3 location, float yaw = 0.0f, float pitch = 0.0f, bool onGround = false)
		{
			var originalchunkcoords = new Vector2(_currentChunkPosition.X, _currentChunkPosition.Y);
			var originalcoordinates = KnownPosition;
			if (yaw != 0.0f && pitch != 0.0f)
			{
				KnownPosition.Yaw = yaw;
				KnownPosition.Pitch = pitch;
			}
			KnownPosition.Y = location.Y;
			KnownPosition.X = location.X;
			KnownPosition.Z = location.Z;
			KnownPosition.OnGround = onGround;

			_currentChunkPosition.X = (int) location.X >> 4;
			_currentChunkPosition.Y = (int) location.Z >> 4;

			if (originalchunkcoords != _currentChunkPosition) SendChunksForKnownPosition();

			new EntityTeleport(Wrapper)
			{
				UniqueServerId = EntityId,
				Coordinates = location,
				OnGround = onGround,
				Pitch = (byte) pitch,
				Yaw = (byte) yaw,
			}.Broadcast(Level, false, this);

			LookChanged();

			/*new EntityRelativeMove(Wrapper)
			{
				Player = this,
				Movement = originalcoordinates.ToVector3() - location
			}.Broadcast(Level, false, this);*/
		}

		public void LookChanged()
		{
			new EntityLook(Wrapper)
			{
				EntityId = this.EntityId,
				Pitch =KnownPosition.Pitch,
				Yaw = KnownPosition.Yaw,
				OnGround = KnownPosition.OnGround
			}.Broadcast(Level, false, this);
			
			new EntityHeadLook(Wrapper)
			{
				EntityId = this.EntityId,
				HeadYaw = KnownPosition.Yaw,
			}.Broadcast(Level, false, this);
		}

		public void HeldItemChanged(int slot)
		{
			//Inventory.CurrentSlot = slot;
			Inventory.HeldItemChanged(slot);
			BroadcastEquipment();
		}

		public void PlayerHandSwing(byte hand)
		{
			PlayerAnimation(Animations.SwingArm, hand);
		}

		public void PlayerAnimation(Animations animation, byte hand = 0)
		{
			var packet = new Animation(Wrapper) {EntityId = EntityId, AnimationId = (byte)animation, Hand = hand};
			Level.BroadcastPacket(packet, false);
		}

		#endregion

		public void BroadcastEquipment()
		{
			//Main Hand
			var slotdata = Inventory.GetSlot(36 + Inventory.CurrentSlot);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Hand0,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(Level, false, this);

			//Second hand
			slotdata = Inventory.GetSlot(45);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Hand1,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(Level, false, this);

			//Helmet
			slotdata = Inventory.GetSlot(5);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Helmet,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(Level, false, this);

			//Chestplate
			slotdata = Inventory.GetSlot(6);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Chestplate,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(Level, false, this);

			//Leggings
			slotdata = Inventory.GetSlot(7);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Leggings,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(Level, false, this);

			//Boots
			slotdata = Inventory.GetSlot(8);
			new EntityEquipment(Wrapper)
			{
				Slot = EquipmentSlot.Boots,
				Item = slotdata,
				EntityId = EntityId
			}.Broadcast(Level, false, this);
		}

        public void SetGamemode(Gamemode target, bool silent)
		{
			Gamemode = target;
			new PlayerListItem(Wrapper)
			{
				Action = 1,
				Gamemode = Gamemode,
				Uuid = Uuid
			}.Broadcast(Level);

			new ChangeGameState(Wrapper)
			{
				Reason = GameStateReason.ChangeGameMode,
				Value = (float) target
			}.Write();

			if (!silent)
			{
				ConsoleFunctions.WriteInfoLine(Username + "'s gamemode was changed to " + target.ToString("D"));
				SendChat("Your gamemode was changed to " + target.ToString(), ChatColor.Yellow);
			}
		}

		public void SetGamemode(Gamemode target)
		{
			SetGamemode(target, false);
		}
	

		public void Teleport(PlayerLocation newPosition)
		{
			new EntityTeleport(Wrapper)
			{
				UniqueServerId = EntityId,
				Coordinates = newPosition.ToVector3(),
				OnGround = newPosition.OnGround,
				Pitch = newPosition.Pitch,
				Yaw = newPosition.Yaw
			}.Broadcast(Level, true, this);
		}

		public void Respawn()
		{
			HealthManager.ResetHealth();
			if (Wrapper != null && Wrapper.TcpClient.Connected)
			{
				new Respawn(Wrapper) {GameMode = (byte) Gamemode}.Write();
				Teleport(Level.GetSpawnPoint());
			}
		}

		public void SendHealth()
		{
			new UpdateHealth(Wrapper).Write();
		}

		internal void InitializePlayer()
		{
			if (!Loaded)
			{
				LoadPlayer();
				string savename = ServerSettings.OnlineMode ? Uuid : Username;
				IsOperator = OperatorLoader.IsOperator(savename);
			}

			var chunks = Level.Generator.GenerateChunks((ViewDistance * 21), _chunksUsed, this);
			new MapChunkBulk(Wrapper) {Chunks = chunks.ToArray()}.Write();

			new PlayerPositionAndLook(Wrapper) {X = KnownPosition.X, Y = KnownPosition.Y, Z = KnownPosition.Z, Yaw = KnownPosition.Yaw, Pitch = KnownPosition.Pitch}.Write();

			IsSpawned = true;
			Level.AddPlayer(this);
			Wrapper.Player.Inventory.SendToPlayer();
			BroadcastEquipment();
			SetGamemode(Gamemode, true);
			Globals.PluginManager.HandlePlayerJoin(this);
		}

		public void SendChunksForKnownPosition(bool force = false)
		{
			var centerX = (int) KnownPosition.X >> 4;
			var centerZ = (int) KnownPosition.Z >> 4;

			if (!force && IsSpawned && _currentChunkPosition == new Vector2(centerX, centerZ)) return;

			_currentChunkPosition.X = centerX;
			_currentChunkPosition.Y = centerZ;

			Wrapper.ThreadPool.LaunchThread(() =>
			{
				foreach (
					var chunk in
						Level.Generator.GenerateChunks((ViewDistance * 21),
							force ? new List<Tuple<int, int>>() : _chunksUsed, this))
				{
					if (Wrapper != null && Wrapper.TcpClient.Client.Connected)
					{
						new ChunkData(Wrapper) {Chunk = chunk, Queee = false}.Write();
						GetEntitysInChunk(chunk.X, chunk.Z);
					}

				}
			});
		}

		public void GetEntitysInChunk(int chunkX, int chunkZ)
		{
			foreach (var player in Level.GetOnlinePlayers)
			{
				if (player == this) continue;

				var x = (int)player.KnownPosition.X >> 4;
				var z = (int)player.KnownPosition.Z >> 4;
				if (chunkX == x && chunkZ == z)
				{
					new SpawnPlayer(Wrapper){Player = player}.Write();
				}
			}

			foreach (var entity in Level.Entities2)
			{
				var x = (int)entity.KnownPosition.X >> 4;
				var z = (int)entity.KnownPosition.Z >> 4;
				if (chunkX == x && chunkZ == z)
				{
					new SpawnObject(Wrapper){X = entity.KnownPosition.X, Y = entity.KnownPosition.Y, Z = entity.KnownPosition.Z, EntityId = entity.EntityId, Type = (ObjectType)entity.EntityTypeId, Yaw = entity.KnownPosition.Yaw, Pitch = entity.KnownPosition.Pitch}.Write();
				}
			}

			ChunkColumn chunk = Level.Generator.GenerateChunkColumn(new Vector2(chunkX, chunkZ));
			foreach (var raw in chunk.TileEntities)
			{
				var nbt = raw.Value;
				if (nbt == null) continue;

				string id = null;
				var idTag = nbt.Get("id");
				if (idTag != null)
				{
					id = idTag.StringValue;
				}

				if (string.IsNullOrEmpty(id)) continue;

				var tileEntity = TileEntityFactory.GetBlockEntityById(id);
				tileEntity.Coordinates = raw.Key;
				tileEntity.SetCompound(nbt);

				if (tileEntity.Id == "Sign")
				{
					var sign = (SignTileEntity) tileEntity;
					new UpdateSign(Wrapper)
					{
						SignCoordinates = sign.Coordinates,
						Line1 = sign.Line1,
						Line2 = sign.Line2,
						Line3 = sign.Line3,
						Line4 = sign.Line4,
					}.Write();
				}
			}
		}

		internal void UnloadChunk(int x, int y)
		{
			new ChunkData(Wrapper)
			{
				Queee = false,
				Unloader = true,
				Chunk = new ChunkColumn { X = x, Z = y }
			}.Write();
		}

		public void SendChat(McChatMessage message)
		{
			if (Wrapper.TcpClient == null)
			{
				ConsoleFunctions.WriteInfoLine(message.text);
				return;
			}

			new ChatMessage(Wrapper) {Message = message}.Write();
		}

		public void SendChat(string message)
		{
			SendChat(new McChatMessage(message));
		}

		public void SendChat(string message, ChatColor color)
		{
			SendChat("§" + color.Value + message);
		}

		public void Kick(McChatMessage reason)
		{
			new Disconnect(Wrapper) {Reason = reason}.Write();
			SavePlayer();
		}

		public void Kick()
		{
			Kick(new McChatMessage("Unknown reason."));
		}

		/// <summary>
		/// Returns true if player became Operator.
		/// Returns false if player's Operator status was removed.
		/// </summary>
		/// <returns></returns>
		public bool ToggleOperatorStatus()
		{
			string savename = ServerSettings.OnlineMode ? Uuid : Username;
			IsOperator = OperatorLoader.Toggle(savename.ToLower());
			return IsOperator;
		}

		public void SavePlayer()
		{
			byte[] health = HealthManager.Export();
			byte[] inv = Inventory.GetBytes();
			DataBuffer buffer = new DataBuffer(new byte[0]);
			buffer.WriteDouble(KnownPosition.X);
			buffer.WriteDouble(KnownPosition.Y);
			buffer.WriteDouble(KnownPosition.Z);
			buffer.WriteFloat(KnownPosition.Yaw);
			buffer.WriteFloat(KnownPosition.Pitch);
			buffer.WriteBool(KnownPosition.OnGround);
			buffer.WriteVarInt((int)Gamemode);
			buffer.WriteVarInt(health.Length);
			foreach (byte b in health)
			{
				buffer.WriteByte(b);
			}
			buffer.WriteVarInt(inv.Length);
			foreach (byte b in inv)
			{
				buffer.WriteByte(b);
			}
			byte[] data = buffer.ExportWriter;
			data = Globals.Compress(data);
			string savename = ServerSettings.OnlineMode ? Uuid : Username;
			File.WriteAllBytes("Players/" + savename + ".pdata", data);
		}

		public void LoadPlayer()
		{
			string savename = ServerSettings.OnlineMode ? Uuid : Username;
			if (File.Exists("Players/" + savename + ".pdata"))
			{
				byte[] data = File.ReadAllBytes("Players/" + savename + ".pdata");
				data = Globals.Decompress(data);
				DataBuffer reader = new DataBuffer(data);
				double x = reader.ReadDouble();
				double y = reader.ReadDouble();
				double z = reader.ReadDouble();
				float yaw = reader.ReadFloat();
				float pitch = reader.ReadFloat();
				bool onGround = reader.ReadBool();
				KnownPosition = new PlayerLocation(x, y, z) {Yaw = yaw, Pitch = pitch, OnGround = onGround};
				Gamemode = (Gamemode) reader.ReadVarInt();
				int healthLength = reader.ReadVarInt();
				byte[] healthData = reader.Read(healthLength);
				int inventoryLength = reader.ReadVarInt();
				byte[] inventoryData = reader.Read(inventoryLength);
				HealthManager.Import(healthData);
				Inventory.Import(inventoryData);
			}
			else
			{
				KnownPosition = Level.GetSpawnPoint();
			}
			Loaded = true;
		}
	}
}