// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

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
			//PingPackages.Add(new Ping(client, buffer));
			PingPackages.Add(new StevenBug(client, buffer));

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