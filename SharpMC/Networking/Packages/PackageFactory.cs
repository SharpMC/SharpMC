#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Networking.Packages
{
	using System.Collections.Generic;

	using SharpMC.Enums;
	using SharpMC.Utils;

	public class PackageFactory
	{
		private readonly ClientWrapper _client;

		private DataBuffer _buffer;

		public List<Package> LoginPackages = new List<Package>();

		public List<Package> PingPackages = new List<Package>();

		public List<Package> PlayPackages = new List<Package>();

		public PackageFactory(ClientWrapper client, DataBuffer buffer)
		{
			

			this.PingPackages.Add(new Handshake(client, buffer));
			this.PingPackages.Add(new Ping(client, buffer));
			this.PingPackages.Add(new StevenBug(client, buffer));

			

			this.LoginPackages.Add(new EncryptionResponse(client, buffer));

			#region Play

			this.PlayPackages.Add(new ChatMessage(client, buffer));
			this.PlayPackages.Add(new Animation(client, buffer));
			this.PlayPackages.Add(new PlayerBlockPlacement(client, buffer));
			this.PlayPackages.Add(new HeldItemChange(client, buffer));
			this.PlayPackages.Add(new EntityAction(client, buffer));
			this.PlayPackages.Add(new PlayerAbilities(client, buffer));
			this.PlayPackages.Add(new PluginMessage(client, buffer));
			this.PlayPackages.Add(new KeepAlive(client, buffer));
			this.PlayPackages.Add(new PlayerPositionAndLook(client, buffer));
			this.PlayPackages.Add(new PlayerPosition(client, buffer));
			this.PlayPackages.Add(new PlayerLook(client, buffer));
			this.PlayPackages.Add(new OnGround(client, buffer));
			this.PlayPackages.Add(new ClientSettings(client, buffer));
			this.PlayPackages.Add(new PlayerDigging(client, buffer));
			this.PlayPackages.Add(new ClientStatus(client, buffer));
			this.PlayPackages.Add(new ClickWindow(client, buffer));
			this.PlayPackages.Add(new UseEntity(client, buffer));
			this.PlayPackages.Add(new CloseWindow(client, buffer));

			#endregion

			this._client = client;
			this._buffer = buffer;
		}

		public bool Handle(int packetId)
		{
			switch (this._client.PacketMode)
			{
				case PacketMode.Ping:
					return this.HPing(packetId);
				case PacketMode.Play:
					return this.HPlay(packetId);
				case PacketMode.Login:
					return this.HLogin(packetId);
			}

			return false;
		}

		private bool HPing(int packetid)
		{
			foreach (var package in this.PingPackages)
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
			foreach (var package in this.LoginPackages)
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
			foreach (var package in this.PlayPackages)
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