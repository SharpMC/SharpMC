using System;
using SharpMC.Network.Packets.Handshake;
using SharpMC.Network.Packets.Login;
using SharpMC.Network.Packets.Play;
using SharpMC.Network.Packets.Status;
using SharpMC.Network.Util;

namespace SharpMC.Network.Packets
{
	public static class MCPacketFactory
	{
		private static PacketFactory<int, MinecraftStream, Packet> HandshakeFactory { get; }
		private static PacketFactory<int, MinecraftStream, Packet> StatusFactory { get; }
		private static PacketFactory<int, MinecraftStream, Packet> LoginFactory { get; }
		private static PacketFactory<int, MinecraftStream, Packet> PlayFactory { get; }

		static MCPacketFactory()
		{
			HandshakeFactory = new PacketFactory<int, MinecraftStream, Packet>();
			StatusFactory = new PacketFactory<int, MinecraftStream, Packet>();
			LoginFactory = new PacketFactory<int, MinecraftStream, Packet>();
			PlayFactory = new PacketFactory<int, MinecraftStream, Packet>();
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

		private static TPacket CreatePacket<TPacket>(Type type, PacketFactory<int, MinecraftStream, Packet> factory)
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
			Register(ConnectionState.Handshake, 0x00, () => new HandshakePacket());
		}

		private static void RegisterStatus()
		{
			Register(ConnectionState.Status, 0x00, () => new RequestPacket());
			Register(ConnectionState.Status, 0x01, () => new PingPacket());

		}

		private static void RegisterLogin()
		{
			Register(ConnectionState.Login, 0x00, () => new LoginStartPacket());
			Register(ConnectionState.Login, 0x01, () => new EncryptionResponsePacket());
		}

		private static void RegisterPlay()
		{
			Register(ConnectionState.Play, 0x0B, () => new KeepAlivePacket
            {
				PacketId = 0x0B
			});
			Register(ConnectionState.Play, 0x0c, () => new PlayerPosition());
			Register(ConnectionState.Play, 0x0D, () => new PlayerPositionAndLookPacket());
			Register(ConnectionState.Play, 0x0E, () => new PlayerLookPacket());
			Register(ConnectionState.Play, 0x04, () => new ClientSettingsPacket());
			Register(ConnectionState.Play, 0x02, () => new ChatMessagePacket());
		}
	}
}