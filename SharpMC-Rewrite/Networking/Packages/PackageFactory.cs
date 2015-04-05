using System.Collections.Generic;
using SharpMCRewrite.Classes;

namespace SharpMCRewrite.Networking.Packages
{
	public class PackageFactory
	{
		private readonly ClientWrapper _client;
		private MSGBuffer _buffer;
		public List<Package> PingPackages = new List<Package>();
		public List<Package> PlayPackages = new List<Package>();

		public PackageFactory(ClientWrapper client, MSGBuffer buffer)
		{
			#region Ping

			PingPackages.Add(new Handshake(client, buffer));
			PingPackages.Add(new Ping(client, buffer));

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

			#endregion

			_client = client;
			_buffer = buffer;
		}

		public bool Handle(int packetId)
		{
			return _client.PlayMode ? HPlay(packetId) : HPing(packetId);
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