using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			PlayPackages.Add(new ChatMessage(client, buffer));
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
