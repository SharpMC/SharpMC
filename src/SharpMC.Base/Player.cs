using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SharpMC.Entities;
using SharpMC.Log;
using SharpMC.Network.Packets.Play;
using SharpMC.Util;

namespace SharpMC
{
	public class Player : Entity
	{
		private static readonly ILogger Log = LogManager.GetLogger(typeof(Player));

		private Dictionary<Tuple<int,int>, byte[]> ChunksUsed = new Dictionary<Tuple<int, int>, byte[]>(); 

		public MCNetConnection Connection { get; }
		private MinecraftServer Server { get; }
		public string Username { get; set; }
		public Guid UUID { get; set; }

		public string DisplayName { get; set; }
		public MCNetConnection.AuthResponse AuthResponse { get; set; } = null;

		public Gamemode Gamemode { get; private set; }

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
			ChunkCoordinates cur = new ChunkCoordinates(KnownPosition);
			if (cur.DistanceTo(_prevChunkCoordinates) >= 2)
			{
				_prevChunkCoordinates = cur;
				SendChunksForKnownPosition(cur);
			}

			_timeSinceLastKeepAlive++;

			if (Level.GameTick%20 == 0)
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
	}
}
