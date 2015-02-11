using System.Collections.Generic;
using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	public class PackageFactory
	{
		private ClientWrapper _client;
		private MSGBuffer _buffer;
		public List<Package> PlayPackages = new List<Package>();
		public List<Package> PingPackages = new List<Package>();

		public PackageFactory(ClientWrapper client, MSGBuffer buffer)
		{
			#region Ping

			PingPackages.Add(new Handshake(client, buffer));

			#endregion

			#region Play

			PlayPackages.Add(new ChatMessage(client, buffer));
			PlayPackages.Add(new Animation(client, buffer));

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
