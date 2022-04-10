using System;
using SharpMC.Network.Util;
using HS = SharpMC.Network.Packets.Handshake.ToServer;
using LS = SharpMC.Network.Packets.Login.ToServer;
using PB = SharpMC.Network.Packets.Play.ToBoth;
using PS = SharpMC.Network.Packets.Play.ToServer;
using XB = SharpMC.Network.Packets.Status.ToBoth;
using XS = SharpMC.Network.Packets.Status.ToServer;

namespace SharpMC.Network.Packets
{
	public static class MCPacketFactory
	{
		private static PacketFactory<int, IMinecraftStream, Packet> HandshakeFactory { get; }
		private static PacketFactory<int, IMinecraftStream, Packet> StatusFactory { get; }
		private static PacketFactory<int, IMinecraftStream, Packet> LoginFactory { get; }
		private static PacketFactory<int, IMinecraftStream, Packet> PlayFactory { get; }

		static MCPacketFactory()
		{
			HandshakeFactory = new PacketFactory<int, IMinecraftStream, Packet>();
			StatusFactory = new PacketFactory<int, IMinecraftStream, Packet>();
			LoginFactory = new PacketFactory<int, IMinecraftStream, Packet>();
			PlayFactory = new PacketFactory<int, IMinecraftStream, Packet>();
		}

		internal static void Register<TPacket>(ConnectionState state, int packetId, Func<TPacket> createFunc) where TPacket : Packet
		{			
			switch (state)
			{
				case ConnectionState.Handshake:
					HandshakeFactory.Register(packetId, createFunc);
					break;
				case ConnectionState.Status:
					StatusFactory.Register(packetId, createFunc);
					break;
				case ConnectionState.Login:
					LoginFactory.Register(packetId, createFunc);
					break;
				case ConnectionState.Play:
					PlayFactory.Register(packetId, createFunc);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}

		private static TPacket CreatePacket<TPacket>(Type type, PacketFactory<int, IMinecraftStream, Packet> factory)
			where TPacket : Packet
		{
			int packetId;
			if (factory.TryGetPacketId(type, out packetId))
			{
				Packet packet;
				if (factory.TryGetPacket(packetId, out packet))
				{
					packet.PacketId = packetId;
					return (TPacket)packet;
				}
			}

			return null;
		}

		public static TPacket CreatePacket<TPacket>(ConnectionState state) where TPacket : Packet
		{
			var type = typeof (TPacket);
			switch (state)
			{
				case ConnectionState.Handshake:
					return CreatePacket<TPacket>(type, HandshakeFactory);
				case ConnectionState.Status:
					return CreatePacket<TPacket>(type, StatusFactory);
				case ConnectionState.Login:
					return CreatePacket<TPacket>(type, LoginFactory);
				case ConnectionState.Play:
					return CreatePacket<TPacket>(type, PlayFactory);
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}

		public static Packet GetPacket(ConnectionState state, int packetId)
		{
			bool success;
			Packet outPacket;
			switch (state)
			{
				case ConnectionState.Handshake:
					success = HandshakeFactory.TryGetPacket(packetId, out outPacket);
					break;
				case ConnectionState.Status:
					success = StatusFactory.TryGetPacket(packetId, out outPacket);
					break;
				case ConnectionState.Login:
					success = LoginFactory.TryGetPacket(packetId, out outPacket);
					break;
				case ConnectionState.Play:
					success = PlayFactory.TryGetPacket(packetId, out outPacket);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}

			if (!success) return null;
			outPacket.PacketId = packetId;
			return outPacket;
		}

		public static void Load()
		{
			RegisterHandshake();
			RegisterStatus();
			RegisterLogin();
			RegisterPlay();
		}

		private static void RegisterHandshake()
		{
            Register<HS.SetProtocol>(ConnectionState.Handshake);
            Register<HS.LegacyServerListPing>(ConnectionState.Handshake);
        }

		private static void RegisterStatus()
		{
            Register<XB.Ping>(ConnectionState.Status);
            Register<XS.PingStart>(ConnectionState.Status);
        }

		private static void RegisterLogin()
		{
            Register<LS.EncryptionBegin>(ConnectionState.Login);
            Register<LS.LoginPluginResponse>(ConnectionState.Login);
            Register<LS.LoginStart>(ConnectionState.Login);
		}

		private static void RegisterPlay()
		{
            Register<PB.CloseWindow>(ConnectionState.Play);
            Register<PB.CustomPayload>(ConnectionState.Play);
            Register<PB.KeepAlive>(ConnectionState.Play);
            Register<PB.VehicleMove>(ConnectionState.Play);
            Register<PS.Abilities>(ConnectionState.Play);
			Register<PS.AdvancementTab>(ConnectionState.Play);
			Register<PS.ArmAnimation>(ConnectionState.Play);
			Register<PS.BlockDig>(ConnectionState.Play);
			Register<PS.BlockPlace>(ConnectionState.Play);
			Register<PS.Chat>(ConnectionState.Play);
			Register<PS.ClientCommand>(ConnectionState.Play);
			Register<PS.CraftRecipeRequest>(ConnectionState.Play);
			Register<PS.DisplayedRecipe>(ConnectionState.Play);
			Register<PS.EditBook>(ConnectionState.Play);
			Register<PS.EnchantItem>(ConnectionState.Play);
			Register<PS.EntityAction>(ConnectionState.Play);
			Register<PS.Flying>(ConnectionState.Play);
			Register<PS.GenerateStructure>(ConnectionState.Play);
			Register<PS.HeldItemSlot>(ConnectionState.Play);
			Register<PS.LockDifficulty>(ConnectionState.Play);
			Register<PS.Look>(ConnectionState.Play);
			Register<PS.NameItem>(ConnectionState.Play);
			Register<PS.PickItem>(ConnectionState.Play);
			Register<PS.Pong>(ConnectionState.Play);
			Register<PS.Position>(ConnectionState.Play);
			Register<PS.PositionLook>(ConnectionState.Play);
			Register<PS.QueryBlockNbt>(ConnectionState.Play);
			Register<PS.QueryEntityNbt>(ConnectionState.Play);
			Register<PS.RecipeBook>(ConnectionState.Play);
			Register<PS.ResourcePackReceive>(ConnectionState.Play);
			Register<PS.SelectTrade>(ConnectionState.Play);
			Register<PS.SetBeaconEffect>(ConnectionState.Play);
			Register<PS.SetCreativeSlot>(ConnectionState.Play);
			Register<PS.SetDifficulty>(ConnectionState.Play);
			Register<PS.Settings>(ConnectionState.Play);
			Register<PS.Spectate>(ConnectionState.Play);
			Register<PS.SteerBoat>(ConnectionState.Play);
			Register<PS.SteerVehicle>(ConnectionState.Play);
			Register<PS.TabComplete>(ConnectionState.Play);
			Register<PS.TeleportConfirm>(ConnectionState.Play);
			Register<PS.UpdateCommandBlock>(ConnectionState.Play);
			Register<PS.UpdateCommandBlockMinecart>(ConnectionState.Play);
			Register<PS.UpdateJigsawBlock>(ConnectionState.Play);
			Register<PS.UpdateSign>(ConnectionState.Play);
			Register<PS.UpdateStructureBlock>(ConnectionState.Play);
			Register<PS.UseEntity>(ConnectionState.Play);
			Register<PS.UseItem>(ConnectionState.Play);
			Register<PS.WindowClick>(ConnectionState.Play);
        }

        private static void Register<T>(ConnectionState state) where T : Packet, IToServer, new()
        {
            var serverId = new T().ServerId;
            Register(state, serverId, () => new T());
        }
    }
}