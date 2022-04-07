using System.Collections.Generic;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class PackageFactory
	{
		private readonly ClientWrapper _client;
		private DataBuffer _buffer;
		public List<Package> LoginPackages = new List<Package>();
		public List<Package> PingPackages = new List<Package>();
		public List<Package> PlayPackages = new List<Package>();
		public List<Package> StatusPackages = new List<Package>();
 
		public PackageFactory(ClientWrapper client, DataBuffer buffer)
		{
			#region Ping

			PingPackages.Add(new Handshake(client, buffer));

			#endregion

			#region Login

			LoginPackages.Add(new EncryptionResponse(client, buffer));
			LoginPackages.Add(new LoginStart(client, buffer));

			#endregion

			#region Status

			StatusPackages.Add(new Request(client, buffer));
			StatusPackages.Add(new Ping(client, buffer));

			#endregion

			#region Play

			PlayPackages.Add(new ChatMessage(client, buffer));
			PlayPackages.Add(new Animation(client, buffer));
			PlayPackages.Add(new PlayerBlockPlacement(client, buffer));
			PlayPackages.Add(new HeldItemChange(client, buffer));
			PlayPackages.Add(new EntityAction(client, buffer));
			PlayPackages.Add(new PlayerAbilities(client, buffer));
			PlayPackages.Add(new PluginMessage(client, buffer));
			PlayPackages.Add(new KeepAlive(client, buffer));
			PlayPackages.Add(new PlayerPositionAndLook(client, buffer));
			PlayPackages.Add(new PlayerPosition(client, buffer));
			PlayPackages.Add(new PlayerLook(client, buffer));
			PlayPackages.Add(new OnGround(client, buffer));
			PlayPackages.Add(new ClientSettings(client, buffer));
			PlayPackages.Add(new PlayerDigging(client, buffer));
			PlayPackages.Add(new ClientStatus(client, buffer));
			PlayPackages.Add(new ClickWindow(client, buffer));
			PlayPackages.Add(new UseEntity(client, buffer));
			PlayPackages.Add(new CloseWindow(client, buffer));
			PlayPackages.Add(new UseItem(client, buffer));
			PlayPackages.Add(new CreativeInventoryAction(client, buffer));
			PlayPackages.Add(new UpdateSign(client, buffer));

			#endregion

			_client = client;
			_buffer = buffer;
		}

		public bool Handle(int packetId)
		{
			switch (_client.PacketMode)
			{
				case PacketMode.Ping:
					return HPing(packetId);
				case PacketMode.Play:
					return HPlay(packetId);
				case PacketMode.Login:
					return HLogin(packetId);
				case PacketMode.Status:
					return HStatus(packetId);
			}
			return false;
		}

		private bool HStatus(int packetid)
		{
			foreach (var package in StatusPackages)
			{
				if (package.ReadId == packetid)
				{
					package.Read();
					return true;
				}
			}
			return false;
		}

		private bool HPing(int packetid)
		{
			foreach (var package in PingPackages)
			{
				if (package.ReadId == packetid)
				{
					package.Read();
					return true;
				}
			}
			return false;
		}

		private bool HLogin(int packetid)
		{
			foreach (var package in LoginPackages)
			{
				if (package.ReadId == packetid)
				{
					package.Read();
					return true;
				}
			}
			return false;
		}

		private bool HPlay(int packetid)
		{
			foreach (var package in PlayPackages)
			{
				if (package.ReadId == packetid)
				{
					package.Read();
					return true;
				}
			}
			return false;
		}
	}
}